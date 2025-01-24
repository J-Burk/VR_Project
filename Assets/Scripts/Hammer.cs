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
    /* Sets this collided Object if Collided with hammer
      @collision hammer*/
    private void OnCollisionEnter(Collision collision)
    {
        // Give the Postition where the GameObject hits an other Object
        // With Tag Anvil
        
        if(this.gameObject.tag == "Hittrigger" && collision.gameObject.tag == "Hammer")
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            GameEvents.instance.HitHammer(this.gameObject);

        }
        if (this.gameObject.tag == "StartTrigger" && collision.gameObject.tag == "Hammer")
        {
            collisionX = collision.GetContact(0).point.x;
            collisionZ = collision.GetContact(0).point.z;
            GameEvents.instance.StartHammer();

        }
    }
    /*Plays the hit Sparkanimation*/
    public void PlayHammerSparks()
    {
        sparks.transform.SetParent(null);
        sparks.Play();
    }
}
