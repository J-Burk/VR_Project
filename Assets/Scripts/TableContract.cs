using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Valve.VR.InteractionSystem;

/* Class handling handing in weapons to complete orders*/
public class TableContract : MonoBehaviour
{
    /* Communication with OrderSystem.cs easier without Game Events here*/
    public OrderSystem orderSys;

    /*Weapon currently tried to be handed in */
    private GameObject weapon = null;
    
    /* If a Game Object is currently in range of the acceptance box */
    private bool inRange = false;

    /* Handing in on Desktop requires a check to be skipped */
    private bool desktopMode = true;

    // Update is called once per frame
    void Update()
    {

        //TODO @Daven müssen die checks alle rein? besonders der weapon.tag wird schon beim setzen von inRange und weapon überprüft oder?


        //If a weapon is in range of the acceptance box
        if (inRange && weapon != null ) {
            //If a weapon is NOT attached to the hand and is a proper weapon
            if(weapon.GetComponent<Interactable>().attachedToHand == null && weapon.tag.Equals("Weapon"))
            {
                if( desktopMode || weapon.transform.parent == null)
                {
                    cashIn();
                }
                
            }
        }
    }

    /*
     * Attemps to hand in a weapon
     */
    private void cashIn() {

        Debug.Log("Trying Cash in");

        WeaponStats stats = weapon.GetComponent<WeaponStats>();

        Debug.Log("Weapon Type: " + stats.weaponType);
        Debug.Log("Ore Type: " + stats.oreType);
        Debug.Log("Hammer Score: " + stats.hammerScore);
        Debug.Log("Sharpening Score: " + stats.sharpeningScore);

        //Only sharpened weapons can be accepted
        if(stats.sharpeningScore > 0)
        {
            //Asks the OrderSystem to accept the weapon
            if (orderSys.tryToAccept(stats.oreType, stats.weaponType, stats.hammerScore, stats.sharpeningScore))
            {
                Debug.Log("Matching with Order");
                Destroy(weapon);
            }
            else
            {
                //TODO Sound effekt
                Debug.Log("Not matching with Order");
            }
        }

        //Weapon either gets deleted or needs to be re-entered to attempt again
        inRange = false;
        weapon = null;
    }


    //TODO @Daven Kommentieren

    private void OnTriggerEnter(Collider other)
    {
        weapon = other.gameObject;
        int counter = 0;
        int max = 10;
        Debug.Log(weapon.name);
        while (weapon.transform.parent != null && weapon.tag != "Weapon" && counter < max)
        {
            weapon = weapon.transform.parent.gameObject;
            counter++;
        }
        inRange = true;
        Debug.Log(weapon.name);
        if (weapon.tag != "Weapon") {
            weapon = null;
            inRange = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        weapon = null;
        inRange = false;
    }
}
