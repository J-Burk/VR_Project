using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    /*Steam VR-Class*/
    [RequireComponent(typeof(Interactable))]
    public class ItemSpawner : MonoBehaviour
    {
        public ItemPackage itemPackage
        {
            get { return _itemPackage; }
        }

        public ItemPackage _itemPackage;

        public bool requireGrabActionToTake = false;
        public bool showTriggerHint = false;

        [EnumFlags]
        public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;

        private GameObject spawnedItem;
        private bool itemIsSpawned = false;
        private bool allowMultipleSpawns = true;

        public bool justPickedUpItem = false;

        //-------------------------------------------------
        void Update()
        {
            if ((!allowMultipleSpawns) && (itemIsSpawned == true) && (spawnedItem == null))
            {
                itemIsSpawned = false;
            }
        }

        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
           

            if (requireGrabActionToTake)
            {
                GrabTypes startingGrab = hand.GetGrabStarting();

                if (startingGrab != GrabTypes.None)
                {
                    SpawnAndAttachObject(hand, startingGrab);
                }
            }
        }

        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
            if (!justPickedUpItem && requireGrabActionToTake && showTriggerHint)
            {
                hand.HideGrabHint();
            }

            justPickedUpItem = false;
        }

        //-------------------------------------------------
        private void SpawnAndAttachObject(Hand hand, GrabTypes grabType)
        {
           
            if (showTriggerHint)
            {
                hand.HideGrabHint();
            }

            spawnedItem = GameObject.Instantiate(itemPackage.itemPrefab);
            spawnedItem.SetActive(true);
            hand.AttachObject(spawnedItem, grabType, attachmentFlags);

            itemIsSpawned = true;

            justPickedUpItem = true;
        }
    }
}
