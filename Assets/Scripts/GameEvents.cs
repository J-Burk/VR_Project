using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;

    private void Awake()
    {
        instance = this; 
    }

    //Button Short-Cuts for Desktop Mode

    //Hammer System
    public event Action hPressed;
    public void PressH()
    {
        if (hPressed != null)
        {
            hPressed();
        }
    }

    //Sharpening System
    public event Action jPressed;
    public void PressJ()
    {
        if (jPressed != null)
        {
            jPressed();
        }
    }

    //Order System
    public event Action oPressed;
    public void PressO()
    {
        if (oPressed != null)
        {
            oPressed();
        }
    }

    //Pause (orders)
    public event Action pPressed;
    public void PressP()
    {
        if (pPressed != null)
        {
            pPressed();
        }
    }

    //Hammer Game has started
    public event Action hammerStarted;
    public void StartHammer()
    {
        if (hammerStarted != null)
        {
            hammerStarted();
        }
    }

    //Progress has been made in the Hammer Game
    public event Action<float> progressMadeHammer;
    public void MakeProgressHammer(float progress)
    {
        if (progressMadeHammer != null)
        {
            progressMadeHammer(progress);
        }
    }

    //Progress has been made in the Sharpening Game
    public event Action<float> progressMadeSharpen;
    public void MakeProgressSharpen(float progress)
    {
        if (progressMadeHammer != null)
        {
            progressMadeSharpen(progress);
        }
    }

    //The hammer hit the anvil
    public event Action<GameObject> hammerHit;
    public void HitHammer(GameObject hitCircle)
    {
        if (hammerHit != null)
        {
            hammerHit(hitCircle);
        }
    }

    //Toggles sharpening state
    public event Action<GameObject, int, bool> sharpToggle;
    public void ToggleSharp(GameObject weapon, int type, bool enter)
    {
        if (sharpToggle != null)
        {
            sharpToggle(weapon, type, enter);
        }
    }

    //Plays a sound by name
    public event Action<string, Vector3> onPlaySound;
    public void PlaySound(string name, Vector3 pos)
    {
        if (onPlaySound != null)
        {
            onPlaySound(name,pos);
        }
    }

    //The hammer game was successful
    public event Action<int, int, float> hammerSuccess;
    public void HammerSuccess(int weaponType, int oreType, float score)
    {
        if (sharpToggle != null)
        {
            hammerSuccess(weaponType, oreType, score);
        }
    }

    //The hammer game was failed
    public event Action hammerFail;
    public void HammerFail()
    {
        if (sharpToggle != null)
        {
            hammerFail();
        }
    }
}
