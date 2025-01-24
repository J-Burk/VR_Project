using UnityEngine;
using Valve.VR.InteractionSystem;

[ExecuteInEditMode()]
public class Material : MonoBehaviour
{
    bool desktopMode =  false;

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
    //Time and destruction animation
    public GameObject failureAnimation;
    private const float maxTime = 10;
    private float time = maxTime;
    //active animation
    private GameObject tempAnime = null;
    //Last Color entry for heat
    private float redS;
    //Fillbar
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
        redS = ingotMat.color.r;
        //SnapValues
        inRange = false;
        actState = 0;
        anvil = GameObject.FindGameObjectWithTag("Anvil");
        //Sets Positions for Ingots
        placeHolders[0] = new Vector3(0.451f, 0.86f, -0.814f);
        placeHolders[1] = new Vector3(0.280000001f, 0.860000014f, -0.81400001f);
        placeHolders[2] = new Vector3(0.638999999f, 0.860000014f, -0.81400001f);
        placeHolders[3] = new Vector3(0.112999998f, 0.860000014f, -0.81400001f);
        placeHolders[4] = new Vector3(0.786000013f, 0.860000014f, -0.81400001f);
        placeHolders[5] = new Vector3(-0.0209999997f, 0.860000014f, -0.81400001f);
        //Sets the Fillbar positions
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
        //Drops Ingot if too hot and not used tongs
        Hand hand = this.gameObject.GetComponent<Interactable>().attachedToHand;
        if (temperatur > 50 && hand != null && hand.name != "Tongs(Clone)" && !desktopMode) {
            hand.DetachObject(this.gameObject);
        }
        //Material is set on the position if Empty
        if (inRange && this.gameObject.GetComponent<Interactable>().attachedToHand == null && actState == 0) {
            if (anvil.GetComponent<Anvil>().snapedObjectCounter < 6)
            {

                setMaterialOnPlace();
                anvil.GetComponent<Anvil>().snapedObjectCounter++;
            }
        }
        //Mateial is picked up
        if (actState != 0 && this.gameObject.GetComponent<Interactable>().attachedToHand != null) { 
            deleteMaterialPlace();
        }
        //If temperature reaches max
        if (temperatur >= 100)
        {
            if (tempAnime == null)
            {
                //Sets the Animation with position and scaling and Sound
                tempAnime = Instantiate(failureAnimation);
                tempAnime.transform.position = this.gameObject.transform.position;
                GameEvents.instance.PlaySound("Steam",this.gameObject.transform.position);
                tempAnime.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            //Destroy the overheatet Ingot with Animation etc.
            time -= Time.deltaTime;
            if(time <= 0) {

                GameEvents.instance.PlaySound("SteamStop", this.gameObject.transform.position);
                Destroy(tempAnime);
                Destroy(this.gameObject);
            }
        }
        //Stops the destruction if taken outside
        else if(time < maxTime){

            GameEvents.instance.PlaySound("SteamStop", this.gameObject.transform.position);
            time = maxTime;
            Destroy(tempAnime);
            tempAnime=null;
        }
        //Changes Ingot Color depending on the temperature
        Color temp = ingotMat.color;
        temp.r = redS + (1 - redS) / 100 * temperatur;
        ingotMat.color = temp;
        ingotMat.EnableKeyword("_EMISSION");
        ingotMat.SetColor("_EmissionColor", Color.red / 100 * temperatur/1.5f);

    }
    /*Sets the in Range to true if collided with Anvil
      @param Collider other checks if Anvil*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Anvil"))
        {
            anvil = other.gameObject;
            inRange = true;
        }
    }
    /*Sets the in Range to false
      @param Collider other never used*/
    private void OnTriggerExit(Collider other)
    {
        inRange = false;

    }
    /*Deletes the Material from the Anvilspot*/
    private void deleteMaterialPlace() {
        anvil.GetComponent<Anvil>().states[actState-1] = null;

        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        actState = 0;
        anvil.GetComponent<Anvil>().snapedObjectCounter--;
    }
    /*Sets the material on the next empty Field*/
    private void setMaterialOnPlace()
    {
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
    /*Sets the current UI Fillbar*/
    void GetCurrentFill()
    {
        current = temperatur / maxTemperatur;
        Vector3 scale = fill.transform.localScale;
        fill.transform.localScale = new Vector3(fillMax * current / max, scale.y , scale.z);
    }
    /*is the Temperature ok
      @return returns if the Temperature is alright to smith*/
    public bool temperatureOk()
    {
        return temperatur >= lowBorderTemperature && temperatur <= highBorderTemperature;
    }
    /*is Temperature if temperature is too high/low
      @return the state of the temperatur*/
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
