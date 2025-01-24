using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class SceneLoader : MonoBehaviour
{
    public SteamVR_Action_Boolean menuPressAction;
    [SerializeField] private int actScene = 1;

    public void Update()
    {
        if (menuPressAction.stateDown | Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene((actScene + 1) % 2);
        }
    }
}
