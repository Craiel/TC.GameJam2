using UnityEngine;

namespace Assets.Scripts
{
    using Assets.Scripts.Stage;

    public class HitIndicatorScript : MonoBehaviour
    {
        private StageManager stageManager;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Timer = 3;
        public GameObject IndicatorObject;

        public void Start()
        {
            this.stageManager = Camera.main.GetComponent<StageManager>();

            //int i;
            
            //for(i=0;i<7;i++)
            //{
            float randX = this.transform.position.x-0.01f+Random.Range (0,0.02f);
            float randY = this.transform.position.y-0.01f+Random.Range (0,0.02f);
            float randZ = this.transform.position.z-0.01f+Random.Range (0,0.02f);
            var randV = new Vector3(randX,randY,randZ);
            var indicator = this.stageManager.SpawnHitIndicator(this.IndicatorObject, randV);
            indicator.GetComponent<Rigidbody>().AddForce (-15+Random.Range(0,30),Random.Range(0,10),-15+Random.Range(0,30));

            Destroy(indicator.gameObject, 5);
            //}
        }
    
        // Update is called once per frame
        public void Update() 
        {
            this.Timer-=1;
        
            if(this.Timer<=0)
            {
                Destroy (this.gameObject);
            }
        }
    }
}
