using UnityEngine;
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
    private bool desktopMode = false;

    // Update is called once per frame
    void Update()
    {

        


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


        WeaponStats stats = weapon.GetComponent<WeaponStats>();

        
        //Only sharpened weapons can be accepted
        if(stats.sharpeningScore > 0)
        {
            //Asks the OrderSystem to accept the weapon
            if (orderSys.tryToAccept(stats.oreType, stats.weaponType, stats.hammerScore, stats.sharpeningScore))
            {
                Destroy(weapon);
            }
        }

        //Weapon either gets deleted or needs to be re-entered to attempt again
        inRange = false;
        weapon = null;
    }


    /*When an Object with an Collider Collides with the Field do CashIn Weapons,
     *Sets the collided Weapon and the InRange variable 
      @param other collided Collider*/

    private void OnTriggerEnter(Collider other)
    {
        weapon = other.gameObject;
        int counter = 0;
        int max = 10;
        //Tries to get the right Weapon with the right Classes because it might be the childObject
        while (weapon.transform.parent != null && weapon.tag != "Weapon" && counter < max)
        {
            weapon = weapon.transform.parent.gameObject;
            //Prevention of maybe endless loop
            counter++;
        }
        inRange = true;
        //Is collided but not a weapon 
        if (weapon.tag != "Weapon") {
            weapon = null;
            inRange = false;
        }
    }
    /*Deregister the Weapon
      @param other collided Collider*/
    private void OnTriggerExit(Collider other)
    {
        weapon = null;
        inRange = false;
    }
}
