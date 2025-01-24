using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OreMining : MonoBehaviour
{
    private int hitter = 0;
    private int oreCount = 0;
    public AudioClip hit;
    public GameObject ore;
    public GameObject textBox;
    private TextMeshProUGUI text;
    bool selfDest = false;
    float time = 5;
    float coolDown = 0;
    // Start is called before the first frame update
    void Start()
    {
        text = textBox.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selfDest) {
            time -= Time.deltaTime;
            text.text = "Self Destruction in: " + time;
            if(time <= 0){
                Application.Quit();
            }
        }
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else {
            coolDown = 0;
        }
    }

    public void collide(Collision collision) {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Pickaxe" && coolDown == 0)
        {

            coolDown = 0.3f;
            if (hitter < 6)
            {
                hitter++;
                AudioSource.PlayClipAtPoint(hit, collision.gameObject.transform.position, 0.4f);
            }
            else
            {
                Instantiate(ore, collision.transform.position, new Quaternion(0, 0, 0, 0));
                AudioSource.PlayClipAtPoint(hit, collision.gameObject.transform.position, 0.9f);
                hitter = 0;
            }

        }
    }

    public void incOre()
    {
        oreCount++;
        text.text =  oreCount + "/100 Ores";
        if (oreCount == 100) {
            selfDest = true;
        }
    }

}
