using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class DragObject : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float Pull = 1.0f;
    
        public void Start() 
        {
            var typedCollider = this.GetComponent<Collider>() as BoxCollider;
            if (typedCollider != null)
            {
                Camera.main.GetComponent<StageManager>().DragEntries.Add(this.gameObject);
            }
        }
    }
}
