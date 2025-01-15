using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Valve.VR.InteractionSystem;

public class Basketball : MonoBehaviour
{
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
