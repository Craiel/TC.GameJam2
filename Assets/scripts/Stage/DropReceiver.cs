using Assets.Scripts.Stage;

using UnityEngine;

namespace Assets.Scripts
{
    public class DropReceiver : MonoBehaviour
    {
        private StageManager stageManager;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Start()
        {
            this.stageManager = Camera.main.GetComponent<StageManager>();
        }

        public void OnTriggerEnter(Collider other)
        {
            var drop = other.GetComponent<Drop>();
            if (drop == null)
            {
                return;
            }

            if (drop.SplatObject != null)
            {
                var position = new Vector3(other.attachedRigidbody.transform.position.x, 0.5f, other.attachedRigidbody.transform.position.z);
                this.stageManager.SpawnDropSplat(drop.SplatObject, position);
            }

            Destroy(drop.gameObject);
        }
    }
}
