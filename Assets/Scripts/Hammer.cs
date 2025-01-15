using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    // Collisionpoints
    public float collisionX;
    public float collisionZ;
    public ParticleSystem sparks;
    void Start()
    {
        collisionX = 0;
        collisionZ = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Give the Postition where the GameObject hits an other Object
        // With Tag Anvil
        /**
        if (collision.gameObject.CompareTag("Anvil"))
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
            //GameEvents.instance.HitHammer(collisionX, collisionZ);
        }
        // With Tag Material
        if (collision.gameObject.CompareTag("Material"))
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
        }
        if(collision.gameObject.CompareTag("Hittrigger"))
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
            GameEvents.instance.HitHammer(collision.gameObject);
            Debug.Log("Hier k�nnte ihre Werbung stehen");
        }
        if (collision.gameObject.CompareTag("StartTrigger"))
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
            GameEvents.instance.StartHammer();
            Debug.Log("StartTrigger getroffen");
        }*/
        if(this.gameObject.tag == "Hittrigger" && collision.gameObject.tag == "Hammer")
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
            PlayHammerSparks();
            GameEvents.instance.HitHammer(this.gameObject);

        }
        if (this.gameObject.tag == "StartTrigger" && collision.gameObject.tag == "Hammer")
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            Debug.Log("Hi " + collision.GetContact(0).point);
            GameEvents.instance.StartHammer();
            Debug.Log("StartTrigger getroffen");

        }
    }

    private void PlayHammerSparks()
    {
        sparks.transform.SetParent(null);
        sparks.Play();
    }
}
