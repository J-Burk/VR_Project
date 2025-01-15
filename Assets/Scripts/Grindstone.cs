using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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

    private bool isValidName(string name) {
        return name == axeName || name == swordName || name == daggerName;
    }

    private void OnTriggerEnter(Collider collision)
    {
        bool isValid = true;
        GameObject weapon = collision.gameObject;
        if (isValidName(weapon.name))
        {
            while (weapon.transform.parent != null && weapon.tag != "Weapon")
            {
                weapon = weapon.transform.parent.gameObject;
            }
            Debug.Log("Weapon at Root:" + weapon);
            weaponStats = weapon.GetComponent<WeaponStats>();
            isValid = !weaponStats.getPolished();
        }
        if (isValid && isValidName(collision.gameObject.name))
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.name == daggerName)
            {
                Debug.Log("Dagger Enter");
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 0, true);

            }

            if (collision.gameObject.name == axeName)
            {
                Debug.Log("Axe Enter");
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 1, true);
            }

            if (collision.gameObject.name == swordName)
            {
                Debug.Log("Sword Enter");
                sparks.Play();
                GameEvents.instance.ToggleSharp(collision.gameObject, 2, true);
            }

            GameEvents.instance.PlaySound("Grindstone", this.gameObject.transform.position);
        }

        /**
        if (collision.gameObject.name == "Iron Variant(Clone)")
        {
            Debug.Log("Eisen");
        }
        if (collision.gameObject.name == "Fire Steel Variant(Clone)")
        {
            Debug.Log("Feuerstein");
        }
        if (collision.gameObject.name == "Gold Variant(Clone)")
        {
            Debug.Log("Gold Gold Gold");
        }
        if (collision.gameObject.name == "Ingot(Clone)")
        {
            Debug.Log("Irgendso en Barren");
        }
        if (collision.gameObject.name == "Steel Variant(Clone)")
        {
            Debug.Log("Stahlhart");
        }
        if (collision.gameObject.name == "Hammer(Clone)")
        {
            Debug.Log("Hammer Sache");
        }
        */
    }

    private void OnTriggerExit(Collider collision)
    {
        weaponStats = null;
        if (collision.gameObject.name == daggerName)
        {
            Debug.Log("Dagger Exit");
            sparks.Stop();
            //sparks.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            GameEvents.instance.ToggleSharp(collision.gameObject, 0, false);
        }

        else if (collision.gameObject.name == axeName)
        {
            Debug.Log("Axe Exit");
            sparks.Stop();
            //sparks.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            GameEvents.instance.ToggleSharp(collision.gameObject, 1, false);
        }

        else if (collision.gameObject.name == swordName)
        {
            Debug.Log("Sword Exit");
            sparks.Stop();
            //sparks.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            GameEvents.instance.ToggleSharp(collision.gameObject, 2, false);
        }
        GameEvents.instance.PlaySound("GrindstoneStop", this.gameObject.transform.position);
    }

    private void Stop() {
        weaponStats = null;
        sparks.Stop();
        //sparks.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        GameEvents.instance.ToggleSharp(null, 2, false);
        GameEvents.instance.PlaySound("GrindstoneStop", this.gameObject.transform.position);
    }
}
