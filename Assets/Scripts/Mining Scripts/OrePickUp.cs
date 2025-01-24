using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class OrePickUp : MonoBehaviour
{
    private OreMining ore;
    private GameObject miner;
    // Start is called before the first frame update
    void Start()
    {
        miner = GameObject.FindGameObjectWithTag("Miner");
        ore = miner.GetComponent<OreMining>();
    }

    // Update is called once per frame
    void Update()
    {
        Hand hand = this.gameObject.GetComponent<Interactable>().attachedToHand;
        if (hand != null )
        {
            hand.DetachObject(this.gameObject);
            ore.incOre();
            Destroy(this.gameObject);
        }
    }
}
