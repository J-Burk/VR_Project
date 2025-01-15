using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Class saving the data of a weapon */
public class WeaponStats : MonoBehaviour
{
    /*Type of the weapon */
    public int weaponType = 0;

    /*Type of Ingot the weapon is made of */
    public int oreType = 0;

    /*Score from the Hammer Game */
    public float hammerScore = 0;

    /*Score from the Sharpening Game */
    public float sharpeningScore = 0;
    public GameObject ps;
    private GameObject spawned = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (spawned != null)
        {
            spawned.transform.position = this.gameObject.transform.position;
        }
    }

    public void polished() {
        spawned = Instantiate(ps);
        spawned.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    public void OnDestroy()
    {
        Destroy(spawned);
    }

    public bool getPolished() {
        return spawned != null;
    }

}
