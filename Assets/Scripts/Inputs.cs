using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/*Desktop Mode Hotkeys for Testing*/
public class Inputs : MonoBehaviour
{
    private void Update()
    {
        //Hit
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameEvents.instance.PressH();
        }

        //Sharpening
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameEvents.instance.PressJ();
        }

        //Start Orders
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameEvents.instance.PressO();
        }

        //Pause Orders
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameEvents.instance.PressP();
        }
    }

}
