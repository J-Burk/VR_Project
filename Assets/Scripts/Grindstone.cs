using System;
using UnityEngine;

public class Grindstone : MonoBehaviour
{
    public String daggerName;
    public String swordName;
    public String axeName;
    public ParticleSystem sparks;
    WeaponStats weaponStats;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (weaponStats != null) {
            if (weaponStats.getPolished()) {
                Stop();
            }
        }
    }
    /*is the Name valid?
     @prarm name the given name
     @return if valid*/
    private bool isValidName(string name) {
        return name == axeName || name == swordName || name == daggerName;
    }

    /*Starts the Grinding animation and Sound if the Weapon is the collided Object*/
    private void OnTriggerEnter(Collider collision)
    {
        bool isValid = true;
        GameObject weapon = collision.gameObject;
        if (isValidName(weapon.name))
        {
            //Gets the right Weapon child
            while (weapon.transform.parent != null && weapon.tag != "Weapon")
            {
                weapon = weapon.transform.parent.gameObject;
            }
            weaponStats = weapon.GetComponent<WeaponStats>();
            isValid = !weaponStats.getPolished();
        }
        //Checks if valid Object
        if (isValid && isValidName(collision.gameObject.name))
        {
            if (collision.gameObject.name == daggerName)
            {
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 0, true);

            }

            if (collision.gameObject.name == axeName)
            {
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 1, true);
            }

            if (collision.gameObject.name == swordName)
            {
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 2, true);
            }

            GameEvents.instance.PlaySound("Grindstone", this.gameObject.transform.position);
        }

    }
    /*Deregisters the collided Weapon and stops the Animation and Sparks
      @param collision collided Collider*/
       
    private void OnTriggerExit(Collider collision)
    {
        weaponStats = null;
        if (collision.gameObject.name == daggerName)
        {
            GameEvents.instance.ToggleSharp(collision.gameObject, 0, false);
        }

        else if (collision.gameObject.name == axeName)
        {
            GameEvents.instance.ToggleSharp(collision.gameObject, 1, false);
        }

        else if (collision.gameObject.name == swordName)
        {
            GameEvents.instance.ToggleSharp(collision.gameObject, 2, false);
        }
        sparks.Stop();
        GameEvents.instance.PlaySound("GrindstoneStop", this.gameObject.transform.position);
    }
    /*Stops the Grinding Animation and Sound and Deregister Weapon*/
    private void Stop() {
        weaponStats = null;
        sparks.Stop();
        GameEvents.instance.ToggleSharp(null, 2, false);
        GameEvents.instance.PlaySound("GrindstoneStop", this.gameObject.transform.position);
    }
}
