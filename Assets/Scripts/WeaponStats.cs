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
    /*Sharpened Animation Reference */
    public GameObject ps;
    /*Sharpened Animation when spawned */
    private GameObject spawned = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    /*Sets the animation location to the weapon location */
    void Update()
    {
        if (spawned != null)
        {
            spawned.transform.position = this.gameObject.transform.position;
        }
    }
    /*Creates the sharpened Animation at the Object */
    public void polished() {
        spawned = Instantiate(ps);
        spawned.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }
    /*Destroys the Animation on Weapon Destruction */
    public void OnDestroy()
    {
        Destroy(spawned);
    }
    /*Returns, if the Weapon was sharpened
      @return bool is sharpened
     */
    public bool getPolished() {
        return spawned != null;
    }

}
