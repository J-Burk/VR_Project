using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class BellowHandler : MonoBehaviour
    {
        // LinearDrive Variable
        public bool isBeingUsed;

        // FlameVariable
        public GameObject flame;
        private float ereaseTemperatur;

        // Bellow Renderer Values
        public GameObject bellow;
        private Interactable interactable;
        private SkinnedMeshRenderer[] renderers;

        // Transform Points 
        public GameObject startPoint;
        public GameObject endPoint;
        public GameObject SoundPoint;
        private float speed;
        private float returnDistance;

        // Calculate Variables
        private float completeDistance;
        public float endDistance;
        public float startDistance;
        private bool moveBack;

        void Start()
        {
            // Temperatur Setup
            flame = GameObject.FindGameObjectWithTag("Flame");
            ereaseTemperatur = 10;

            // Movement
            if (interactable == null)
                interactable = GetComponent<Interactable>();
            renderers = bellow.GetComponentsInChildren<SkinnedMeshRenderer>();

            transform.position = startPoint.transform.position;
            speed = 1;
            moveBack = false;

            // Complete Distance for calculating
            completeDistance = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);
        }


        void Update()
        {
            // LinearDrive Variable
            isBeingUsed = transform.GetComponent<LinearDrive>().isBeingUsed;
            startDistance = Vector3.Distance(startPoint.transform.position, transform.position);
            endDistance = Vector3.Distance(endPoint.transform.position, transform.position);

            returnDistance = Vector3.Distance(startPoint.transform.position, transform.position);

            // Render for Bellowanimation
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.SetBlendShapeWeight(0, ((returnDistance / completeDistance) * 100));
            }

            // Set Bellow Back if Player leafs it
            if (!isBeingUsed && startDistance > 0.001)
            {
                transform.position -= (endPoint.transform.position - startPoint.transform.position) * speed * Time.deltaTime;
            }

            // Erease flmae
            if (endDistance <= 0 && !moveBack)
            {
                moveBack = true;
                flame.GetComponent<FlameHandler>().temperatur += ereaseTemperatur;
                GameEvents.instance.PlaySound("Bellows",SoundPoint.transform.position);
            }

            // Set Back Variable for Beware Flamespam
            if (startDistance <= 0.001)
            {
                moveBack = false;
            }

            // Set back the Linear Drive for new Using
            if(!isBeingUsed && !moveBack)
            {
                GetComponent<LinearDrive>().setbackHandPosition();
            }
        }
    }
}

