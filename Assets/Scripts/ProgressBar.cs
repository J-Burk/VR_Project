using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public float max = 1;
    public float current;
    public GameObject fill;
    public float fillMax = 1;

    void Update()
    {
        GetCurrentFill();
    }

    void Start()
    {
        if (GameEvents.instance != null)
        {
            GameEvents.instance.progressMadeHammer += setFill;
        }
        else
        {
            Debug.Log("GameEvents.instance ist null. Stelle sicher, dass es korrekt initialisiert wurde.");
        }
    }

    public void setFill(float progress)
    {
        Debug.Log("setFill called");
        current = progress;
    }

    void GetCurrentFill()
    {
        Vector3 scale = fill.transform.localScale;
        fill.transform.localScale = new Vector3(scale.x, fillMax * current / max, scale.z);
    }
    
}
