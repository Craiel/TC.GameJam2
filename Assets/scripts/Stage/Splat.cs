using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class Splat : MonoBehaviour
    {
        private float currentScale;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float StartingScale = 0.005f;
    
        public void Start() 
        {
            float startScale = 0.01f + Random.Range(0f, 0.3f);
            this.transform.localScale = new Vector3(startScale, 0.1f, startScale);
            this.currentScale = this.StartingScale;
        }
    
        public void Update()
        {
            this.transform.localScale += new Vector3(this.currentScale, 0.0f, this.currentScale);
        
            if (this.currentScale > 0.0035f)
            {
                this.currentScale = this.currentScale * 0.85f;
            }
        
            if (this.transform.localScale.x > 1.2f)
            {
                this.transform.Translate(Vector3.down * Time.deltaTime * 0.125f, Space.World);
            
                if (this.transform.position.y < 0.4f)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
