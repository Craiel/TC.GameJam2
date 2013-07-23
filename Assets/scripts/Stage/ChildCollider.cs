using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class ChildCollider : MonoBehaviour 
    {
        private CharacterEntity host;
    
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameObject Parent;
    
        public bool IndicatorOnly = false;
    
        public void Start()
        {
            this.host = this.Parent.GetComponent<CharacterEntity>();
        }
    
        public void OnTriggerEnter(Collider other)
        {
            if (this.host == null)
            {
                return;
            }
        
            this.host.OnChildCollision(this.collider, other, this.IndicatorOnly);
        }
    
        public void OnTriggerStay(Collider other)
        {
            if (this.host == null)
            {
                return;
            }
        
            this.host.OnChildCollisionStay(this.collider, other, this.IndicatorOnly);
        }
    }
}
