using UnityEngine;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    //Sets Progressbar
    public float max = 1;
    public float current;
    public GameObject fill;
    public float fillMax = 1;
    //Sets current Fill
    void Update()
    {
        GetCurrentFill();
    }

    void Start()
    {
        GameEvents.instance.progressMadeHammer += setFill;
    }
    /*Set the Progress
      @progress the to set variable*/
    public void setFill(float progress)
    {
        current = progress;
    }
    /*Scales the Scaling and the filled of the fillbar*/
    void GetCurrentFill()
    {
        Vector3 scale = fill.transform.localScale;
        fill.transform.localScale = new Vector3(scale.x, fillMax * current / max, scale.z);
    }
    
}
