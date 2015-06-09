using System.Collections.Generic;

using Assets.Scripts.Logic;
using Assets.Scripts.UI;

using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class StageManager : MonoBehaviour 
    {
        private readonly Queue<GameObject> activeIndicators = new Queue<GameObject>();
        private readonly Queue<GameObject> activeSplats = new Queue<GameObject>(); 
        private readonly Queue<GameObject> activeDrops = new Queue<GameObject>(); 
        private readonly List<StageEntity> destroyableObjects = new List<StageEntity>();

        private Player player1;
        private Player player2;
    
        private int maxLives = 3;
    
        private GameObject player1Model;
        private GameObject player2Model;

        private GameObject generatedObjectsRoot;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector3 SpawnPositionPlayer1 = Vector3.zero;
        public Vector3 SpawnPositionPlayer2 = Vector3.zero;

        public Rect StageBounds;

        public float FallDamage = 10;
        public float ZRangeTollerance = 0.4f;

        public int IndicatorLimit = 10;
        public int SplatLimit = 10;
        public int DropLimit = 100;

        public List<GameObject> CollidingGeometry = new List<GameObject>();
        public List<GameObject> DragEntries = new List<GameObject>();
        public List<GameObject> Enemies = new List<GameObject>();

        public GameObject Player1HUD;
        public GameObject Player2HUD;

        public AudioClip Music;

        public GameObject Player1ModelOverride;
        public GameObject Player2ModelOverride;

        public void Start()
        {
            this.generatedObjectsRoot = new GameObject("__stage_generated");

            if (SceneState.Instance.PlayerSelected)
            {
                this.InitializeFromSelection();
            }
            else
            {
                this.InitializeFromManual();
            }
        
            if (this.Music != null)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.Music);
            }
        }

        public void Update()
        {
            // Check if we have active players
            if ((this.player1 != null && !this.player1.IsDead) || (this.player2 != null && !this.player2.IsDead))
            {
                this.UpdateEnemies(); 
            }
            
            this.HandleInput();
        }
    
        public GameObject FindClosestPlayer(Vector3 position)
        {
            GameObject result = this.player1.gameObject;
            if (result != null)
            {
                if (this.player2 == null)
                {
                    return result;
                }
            
                float a = Mathf.Abs(Vector3.Distance(result.transform.position, position));
                float b = Mathf.Abs(Vector3.Distance(this.player2.transform.position, position));
                if (b > a)
                {
                    return this.player2.gameObject;
                }
            }
            else
            {
                return this.player2.gameObject;
            }
        
            return result;
        }

        public GameObject SpawnHitIndicator(GameObject indicatorObject, Vector3 position)
        {
            var indicator = (GameObject)Instantiate(indicatorObject, position, Quaternion.identity);
            indicator.transform.parent = this.generatedObjectsRoot.transform;
            if (this.activeIndicators.Count > this.IndicatorLimit)
            {
                Destroy(this.activeIndicators.Dequeue());
            }

            this.activeIndicators.Enqueue(indicator);
            return indicator;
        }

        public GameObject SpawnDropSplat(GameObject splatObject, Vector3 position)
        {
            var splat = (GameObject)Instantiate(splatObject, position, Quaternion.identity);
            splat.transform.parent = this.generatedObjectsRoot.transform;
            if (this.activeSplats.Count > this.SplatLimit)
            {
                Destroy(this.activeSplats.Dequeue());
            }

            this.activeSplats.Enqueue(splat);
            return splat;
        }

        public GameObject SpawnDrop(GameObject dropObject, Vector3 position, Quaternion rotation)
        {
            var drop = (GameObject)Instantiate(dropObject, position, rotation);
            drop.transform.parent = this.generatedObjectsRoot.transform;
            if (this.activeDrops.Count > this.DropLimit)
            {
                Destroy(this.activeDrops.Dequeue());
            }

            this.activeDrops.Enqueue(drop);
            return drop;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void UpdateEnemies()
        {
            // Loop through all enemies to check for aggro radius
            for (int i = 0; i < this.Enemies.Count; i++)
            {
                var controller = this.Enemies[i].GetComponent<Enemy>();
                if (controller == null)
                {
                    continue;
                }

                if (this.player1 != null)
                {
                    // Todo: check if abs is needed
                    var distance = Mathf.Abs(Vector3.Distance(this.player1.transform.position, controller.transform.position));
                    if (distance < controller.AggroRange)
                    {
                        controller.IsActive = true;
                        continue;
                    }
                }

                if (this.player2 != null)
                {
                    var distance = Mathf.Abs(Vector3.Distance(this.player2.transform.position, controller.transform.position));
                    if (distance < controller.AggroRange)
                    {
                        controller.IsActive = true;
                    }
                }
            }
        }

        private void InitializeFromSelection()
        {
            if (SceneState.Instance.Player1 != null)
            {
                this.player1Model = Instantiate(Resources.Load(SceneState.Instance.Player1), this.SpawnPositionPlayer1, Quaternion.identity) as GameObject;
                this.player1 = this.player1Model.GetComponent<Player>();
                this.player1.CameraFollows = true;
                this.Player1HUD.GetComponent<PlayerHUD>().Player = this.player1.gameObject;
            }

            if (SceneState.Instance.Player2 != null)
            {
                this.player2Model = Instantiate(Resources.Load(SceneState.Instance.Player2), this.SpawnPositionPlayer2, Quaternion.identity) as GameObject;
                this.player2 = this.player2Model.GetComponent<Player>();
                this.player2.ControlPrefix = "Player2";
                this.Player2HUD.GetComponent<PlayerHUD>().Player = this.player2.gameObject;
            }
        }

        private void InitializeFromManual()
        {
            if (this.Player1ModelOverride != null)
            {
                this.player1Model = this.Player1ModelOverride;
                this.player1 = this.player1Model.GetComponent<Player>();
                this.Player1HUD.GetComponent<PlayerHUD>().Player = this.player1.gameObject;
            }

            if (this.Player2ModelOverride != null)
            {
                this.player2Model = this.Player2ModelOverride;
                this.player2 = this.player2Model.GetComponent<Player>();
                this.Player2HUD.GetComponent<PlayerHUD>().Player = this.player2.gameObject;
            }
        }
    }
}
