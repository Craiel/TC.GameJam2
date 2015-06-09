using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class StageEntity : MonoBehaviour 
    {
        private float health;
        private float maxHealth;
    
        private bool isDead;
    
        // ---------------------------------------------
        // Public
        // ---------------------------------------------
        public float CollisionDamage;
    
        public bool CollisionEnabled;
    
        public bool IsDead
        {
            get
            {
                return this.isDead;
            }
        }
    
        public float MaxHealth
        {
            get
            {
                return this.maxHealth;
            }
        }
    
        public float Health
        {
            get
            {
                return this.health;
            }
        
            set
            {
                this.health = value;
                this.maxHealth = value;
                this.isDead = this.health <= 0.0f;
            }
        }
    
        public void TakeDamage(float damage, GameObject source = null)
        {
            if (this.isDead)
            {
                return;
            }
                
            // Todo: Add fancy damage calculations here
            this.health -= damage;
            if (this.health <= 0)
            {
                this.Die();
            }
        }
        
        // Update is called once per frame
        public virtual void Update() 
        {
        }
    
        public void SetCollision(float damage)
        {
            this.CollisionDamage = damage;
            this.CollisionEnabled = damage > 0.0f;
        }
        
        public virtual void OnTriggerEnter(Collider other)
        {
            this.HandleCollision(other);
        }
    
        public virtual void OnTriggerStay(Collider other)
        {
            this.HandleCollision(other);
        }
    
        public virtual void OnTriggerExit(Collider other)
        {
        }

        // ---------------------------------------------
        // Protected
        // ---------------------------------------------
        protected virtual void Die()
        {
            this.isDead = true;
        }
    
        protected virtual void HandleCollision(Collider other)
        {
            var component = GetComponent<Collider>().gameObject.GetComponent<StageEntity>();
            if (component != null && component.GetType() != this.GetType())
            {
                if (!component.CollisionEnabled)
                {
                    return;
                }
            
                this.TakeDamage(component.CollisionDamage, GetComponent<Collider>().gameObject);
            }
        }
    }
}
