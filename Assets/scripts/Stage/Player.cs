using UnityEngine;

namespace Assets.Scripts.Stage
{
    public class Player : CharacterEntity 
    {
        private Camera mainCamera;
    
        private int currentChain;
        private bool comboDidHit;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string ControlPrefix = "Player1";
    
        public bool CameraFollows = false;
        public bool CameraFollowsY = false;
    
        public GameObject CurrentTarget { get; set; }

        public int Score { get; set; }

        public int Lives { get; set; }
    
        // Use this for initialization
        public override void Start() 
        {
            base.Start();
        
            this.mainCamera = Camera.main;
        }
    
        public override void Update()
        {
            base.Update();
        
            if (this.IsComboLocked)
            {
                return;
            }
        
            float newX = Input.GetAxis(this.ControlPrefix + " Horizontal");
            float newZ = Input.GetAxis(this.ControlPrefix + " Vertical");
            
            bool attackAction = Input.GetButtonDown(this.ControlPrefix + " Attack");
            bool jumpAction = Input.GetButtonDown(this.ControlPrefix + " Jump");

            if (attackAction && jumpAction)
            {
                // Todo: Special attack
            }
            else if (attackAction)
            {
                if (this.InCombat && this.comboDidHit)
                {
                    this.currentChain++;
                }

                if (this.currentChain >= this.ComboChain.Length)
                {
                    this.currentChain = 0;
                }
            
                this.comboDidHit = false;
                this.EnterCombat(this.currentChain);
            }
            else if (jumpAction)
            {
                this.StartJump();
            }
        
            // Update movement and cam
            this.MoveCharacter(newX, newZ);
        
            if (this.CameraFollows && this.MovementState != CharacterMovementState.Falling)
            {
                this.mainCamera.transform.position = new Vector3(this.transform.position.x, this.mainCamera.transform.position.y, this.mainCamera.transform.position.z);
                if (this.CameraFollowsY)
                {
                    this.mainCamera.transform.LookAt(this.transform);
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void ResolveCombat(GameObject target, CharacterEntity targetData)
        {
            base.ResolveCombat(target, targetData);
        
            this.CurrentTarget = target;
            this.comboDidHit = true;
        
            if (targetData.IsDead && targetData as Enemy != null)
            {
                this.Score += ((Enemy)targetData).ScoreReward;
            }
        }
    
        protected override void LeaveCombat()
        {
            base.LeaveCombat();
        
            this.comboDidHit = false;
            this.currentChain = 0;
            this.CurrentTarget = null;
        }
    }
}
