using UnityEngine;

public class Basketball : MonoBehaviour
{
    //Gameobjects for the Animation and Sound
    float timer = 2;
    public GameObject winParticle;
    bool timerstart = false;
    private GameObject active = null;
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Animation Method And Destruction Timer
        if (active != null && timerstart) { 
            timer -= Time.deltaTime;
            if(timer < 0 )
            {
                timerstart = false;
                timer = 2;
                Destroy(active);
                active = null;
            }
        }
    }
    /*Destroys Object and activates the Sound and Animation
      @param other used for Destruction*/
    private void OnTriggerEnter(Collider other)
    {
        GameObject temp = other.gameObject;
        while (temp.transform.parent != null && temp.tag == "Untagged") {
            temp = temp.transform.parent.gameObject;
        }
        Vector3 pos = other.gameObject.transform.position;
        AudioSource.PlayClipAtPoint(clip, pos, 0.5f);
        Destroy(temp);
        active = Instantiate(winParticle);
        active.transform.position = pos;
        active.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
        timerstart = true;
    }
}
