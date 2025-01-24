using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class TongsMover : MonoBehaviour
    {
        /*Tongs Variables for Setting Shapekeys*/
        public Interactable interactable;
        public SkinnedMeshRenderer[] renderers;

        public int relaxedShape = 100;
        public int gripShape = 50;

        public SteamVR_Action_Single gripSqueeze = SteamVR_Input.GetAction<SteamVR_Action_Single>("Squeeze");
        private new Rigidbody rigidbody;
        /*Set Components of Tongs*/
        private void Start()
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();

            if (interactable == null)
                interactable = GetComponent<Interactable>();
            
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        /*Updates ShapeKey animation and Grip*/
        private void Update()
        {
            float grip = 0;

            if (interactable.attachedToHand)
            {
                grip = gripSqueeze.GetAxis(interactable.attachedToHand.handType);
            }

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.SetBlendShapeWeight(0,
                    Mathf.Lerp(renderer.GetBlendShapeWeight(0), Mathf.Lerp(relaxedShape, gripShape, grip),
                        Time.deltaTime * 10));
            }
        }
    }
}