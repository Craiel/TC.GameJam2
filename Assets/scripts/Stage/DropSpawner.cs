using UnityEngine;

namespace Assets.Scripts
{
    using Assets.Scripts.Stage;

    public class DropSpawner : MonoBehaviour
    {
        private StageManager stageManager;

        private float lastSpawnTick;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameObject DropObject;

        public float Delay = 5;
        public float Count = 10;

        public void Start()
        {
            this.stageManager = Camera.main.GetComponent<StageManager>();
        }

        public void Update() 
        {
            float camerax = Camera.main.gameObject.transform.position.x;
            float border = 8;
        
            if (camerax + border > this.transform.position.x && camerax - border < this.transform.position.x)
            {
                this.lastSpawnTick += 0.1f;
            
                if (this.lastSpawnTick > this.Delay)
                {
                    int i;
            
                    for (i = 0; i < this.Count; i++)
                    {
                        float randX = this.transform.position.x-0.01f + Random.Range(0, 0.02f);
                        float randY = this.transform.position.y-0.01f + Random.Range(0, 0.02f);
                        float randZ = this.transform.position.z-0.01f + Random.Range(0, 0.02f);
                        var randV = new Vector3(randX, randY, randZ);
                        var drop = this.stageManager.SpawnDrop(this.DropObject, randV, this.transform.rotation);
                        drop.GetComponent<Rigidbody>().AddForce(-15 + Random.Range(0,30), Random.Range(0,10), -15 + Random.Range(0,30));
                    }

                    this.lastSpawnTick = 0;
                }
            }
        }
    }
}
