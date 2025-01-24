using UnityEngine;

[ExecuteInEditMode()]
public class ProgressBarSharpen : MonoBehaviour
{
    //Grind Variables
    public float max = 1;
    public float current;
    public GameObject fill;
    public float fillMax = 1;
    //Sets Bar
    void Update()
    {
        GetCurrentFill();
    }

    void Start()
    {
        GameEvents.instance.progressMadeSharpen += setFill;
    }
    /*Sets the fill bar Variable*/
    public void setFill(float progress)
    {
        current = progress;
    }
    /*Sets the FillBar UI*/
    void GetCurrentFill()
    {
        Vector3 scale = fill.transform.localScale;
        fill.transform.localScale = new Vector3(scale.x, fillMax * current / max, scale.z);
    }
    
}
