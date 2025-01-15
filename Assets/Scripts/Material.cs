using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEditorInternal;
using UnityEngine;
using Valve.VR.InteractionSystem;

[ExecuteInEditMode()]
public class Material : MonoBehaviour
{
    //TODO f�r abgabe disablen
    bool desktopMode =  true;

    // Heatervariables
    public float temperatur;
    private float distance;
    private GameObject heater;
    public Renderer myRenderer;
    private Color color;
    private float maxRange;
    private float minTemperatur;
    private float maxTemperatur;
    public float lowBorderTemperature;
    public float highBorderTemperature;
    public GameObject failureAnimation;
    private const float maxTime = 10;
    private float time = maxTime;
    private GameObject tempAnime = null;
    private float redS;
    public GameObject lowBorder;
    public GameObject highBorder;

    // Snapvariables
    public bool inRange;
    public GameObject anvil;
    public int actState;
    private Vector3[] placeHolders = new Vector3[6];
    private UnityEngine.Material ingotMat;

    //Progress Bar
    public float max = 1;
    public float current;
    public GameObject fill;
    public float fillMax = 1;

    // Flamevalues
    public float flameTemperatur;

    void Start()
    {
        // Temperaturvalues
        temperatur = 0;
        minTemperatur = 20;
        maxTemperatur = 100;
        lowBorderTemperature = 30;
        highBorderTemperature = 90;
        maxRange = 0.5f;
        heater = GameObject.FindGameObjectWithTag("heat");
        myRenderer = GetComponent<Renderer>();
        //color = myRenderer.material.color;
        ingotMat = this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>().material;
        Debug.Log(this.gameObject.transform.GetChild(0).name);
        Debug.Log(this.gameObject.transform.GetChild(0).transform.GetChild(0).name);
        Debug.Log(this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>().material);
        redS = ingotMat.color.r;
        //SnapValues
        inRange = false;
        actState = 0;
        anvil = GameObject.FindGameObjectWithTag("Anvil");
        placeHolders[0] = new Vector3(0.451f, 0.86f, -0.814f);
        placeHolders[1] = new Vector3(0.280000001f, 0.860000014f, -0.81400001f);
        placeHolders[2] = new Vector3(0.638999999f, 0.860000014f, -0.81400001f);
        placeHolders[3] = new Vector3(0.112999998f, 0.860000014f, -0.81400001f);
        placeHolders[4] = new Vector3(0.786000013f, 0.860000014f, -0.81400001f);
        placeHolders[5] = new Vector3(-0.0209999997f, 0.860000014f, -0.81400001f);

        Vector3 position = lowBorder.transform.localPosition;
        lowBorder.transform.localPosition = new Vector3(lowBorderTemperature / maxTemperatur, position.y, position.z);
        position = highBorder.transform.localPosition;
        highBorder.transform.localPosition = new Vector3(highBorderTemperature / maxTemperatur, position.y, position.z);

        flameTemperatur = 100;
    }

    // Update is called once per frame
    void Update()
    {
        flameTemperatur = GameObject.FindGameObjectWithTag("Flame").GetComponent<FlameHandler>().temperatur;
        GetCurrentFill();
        // Update for the distance between oven and material
        distance = Vector3.Distance(transform.position, heater.transform.position);
        // Heat down the Material Temperature, if its to far away from oven
        if (distance >= maxRange && temperatur > minTemperatur)
        {
            temperatur -= Time.deltaTime;
        }
        // Heats up Material, if its near by oven
        else if (distance < maxRange && temperatur < maxTemperatur)
        {
            temperatur += (Time.deltaTime * (1 - distance)) * 5 * (flameTemperatur / 100);
        }

        // Colorchange based on Temperatur
        color.a = (temperatur - 20) / 100;
        // Overwrites the actual Collormaterial
        //myRenderer.material.color = color;

        Hand hand = this.gameObject.GetComponent<Interactable>().attachedToHand;
        if (temperatur > 50 && hand != null && hand.name != "Tongs(Clone)" && !desktopMode) {
            hand.DetachObject(this.gameObject);
        }

        if (inRange && this.gameObject.GetComponent<Interactable>().attachedToHand == null && actState == 0) {
            if (anvil.GetComponent<Anvil>().snapedObjectCounter < 6)
            {

                setMaterialOnPlace();
                anvil.GetComponent<Anvil>().snapedObjectCounter++;
            }
        }
        if (actState != 0 && this.gameObject.GetComponent<Interactable>().attachedToHand != null) { 
            deleteMaterialPlace();
        }
        if (temperatur >= 100)
        {
            if (tempAnime == null)
            {
                tempAnime = Instantiate(failureAnimation);
                tempAnime.transform.position = this.gameObject.transform.position;
                GameEvents.instance.PlaySound("Steam",this.gameObject.transform.position);
                tempAnime.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            time -= Time.deltaTime;
            if(time <= 0) {

                GameEvents.instance.PlaySound("SteamStop", this.gameObject.transform.position);
                Destroy(tempAnime);
                Destroy(this.gameObject);
            }
        }
        else if(time < maxTime){

            GameEvents.instance.PlaySound("SteamStop", this.gameObject.transform.position);
            time = maxTime;
            Destroy(tempAnime);
            tempAnime=null;
        }
        Color temp = ingotMat.color;
        temp.r = redS + (1 - redS) / 100 * temperatur;
        Debug.Log(temp.r + "; redleft: " + (1 - redS) / 100 * temperatur);
        ingotMat.color = temp;
        ingotMat.EnableKeyword("_EMISSION");
        ingotMat.SetColor("_EmissionColor", Color.red / 100 * temperatur/1.5f);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Anvil"))
        {
            anvil = other.gameObject;
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;

    }

    private void deleteMaterialPlace() {
        anvil.GetComponent<Anvil>().states[actState-1] = null;

        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        actState = 0;
        anvil.GetComponent<Anvil>().snapedObjectCounter--;
    }

    private void setMaterialOnPlace()
    {
        Debug.Log("Wo bin ich");
        bool stop = false;
        int length = anvil.GetComponent<Anvil>().states.Length;
        for (int i = 0; i < length && !stop; i++) { 
            if (!anvil.GetComponent<Anvil>().states[i])
            {
                anvil.GetComponent<Anvil>().states[i] = this.gameObject;
                this.transform.position = placeHolders[i];
                actState = i+1;
                stop = true;
            }

        }
        
        this.transform.rotation = Quaternion.Euler(0, 90, 90);
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    void GetCurrentFill()
    {
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        current = temperatur / maxTemperatur;
        Vector3 scale = fill.transform.localScale;
        fill.transform.localScale = new Vector3(fillMax * current / max, scale.y , scale.z);
    }

    public bool temperatureOk()
    {
        Debug.Log("Temperatur: " + temperatur);
        return temperatur >= lowBorderTemperature && temperatur <= highBorderTemperature;
    }

    public int hotOrCold()
    {
        if (temperatur > highBorderTemperature)
        {
            return 0;
        }

        if (temperatur < lowBorderTemperature)
        {
            return 1;
        }

        return 2;
    }
}
