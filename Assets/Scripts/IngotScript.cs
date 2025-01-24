using UnityEngine;
using Valve.VR.InteractionSystem;

public class IngotScript : MonoBehaviour
{
    // Sets the Ingot AttachementsFlag
    void Start()
    {
        GetComponent<Interactable>().useHandObjectAttachmentPoint = true;
    }
}
