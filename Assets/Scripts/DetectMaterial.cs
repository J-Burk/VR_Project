using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMaterial : MonoBehaviour
{
    private Dictionary<GameObject, string> inside;
    public bool isInBox;
    void Start()
    {
        
    }


    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject temp = other.gameObject;
        Debug.Log("Enter");
        if (!inside.ContainsKey(temp))
        {
            isInBox = true;
            inside.Add(temp, temp.name);
        }
    }
    void OnTriggerExit(Collider other)
    {
        inside.Remove(other.gameObject);
        Debug.Log("Exit");
        if (inside.Count == 0) { 
            isInBox = false;
        }
    }
    public Dictionary<GameObject, string>  getDir() { 
        return inside;
    }

}
