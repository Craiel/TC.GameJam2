using Assets.Scripts.Logic;
using UnityEngine;

namespace Assets.Scripts.Stage
{
    using System.Collections.Generic;

    public enum CharacterMovementState
    {
        Idle,
        Walking,
        Jumping,
        Leveling,
        Falling,
        Landing
    }

    public class CharacterEntity : StageEntity 
    {
        private readonly List<GameObject> hitTest = new List<GameObject>();

        private StageManager gameManager;
        private CapsuleCollider movementCollider;

        private ComboChainElement[] comboChain;
    
        private Vector3? startPos;
            
        private int moveTimeoutTimer;
    
        private Vector3 lastGroundedPosition;
        private Vector2 moveDirection;
    
        private bool rotate;
    
        private float upVelocity;
        private float baseY;
        private float comboDelay;
    
        private float idleDelay;
        private float lastCombatTick;
        private float deathTick;
        private int comboStage;
        private bool allowHit;
        private bool allowAirCombatAnim ;
        private bool lockAirAnimation;
        private bool playComboAnimation;
        private bool forceComboAnimation;
    
        private string lastAnimationPlayed;
        private bool lastAnimationWasCombat;
        private bool queueNextCombat;
        private int queuedComboStage;
            
        // ---------------------------------------------
        // Public
        // ---------------------------------------------
        public int StartingHealth = 100;
    
        public float Speed = 1.0f;
        public float AirSpeed = 1.0f;
    
        public float JumpStrength = 3.0f;
        public float Gravity = 1.0f;
        public float TerminalVelocity = -0.5f;
    
        public float AnimationSpeed = 1.0f;
        public float AnimationFadeTime = 0.4f;
    
        public float IdleTime = 0.5f;
        
        public float DeathDepth = -3;
    
        public bool CanMoveInAir = false;
        
        public CharacterMovementState MovementState { get; set; }
    
        public string IdleAnimation;
        public string WalkAnimation;
        public string JumpAnimation;
        public string FallAnimation;
        public string DeathAnimation;
        public string LevelingAnimation;
        public string FallCombatAnimation;
        public string JumpCombatAnimation;
    
        public string Name = "No Name";
        public float CombatTimeout = 2;
        public float HitReach = 1.0f;
        public float DeathTimer = 3;

        public GameObject Combo;
    
        public Texture2D Portrait;
    
        public AudioClip SFXJump;
        public AudioClip SFXLand;
        public AudioClip SFXFalling;
        public AudioClip SFXDying;
    
        public GameObject HitIndicator;
        
        public bool IsAirborne
        {
            get
            {
                return this.MovementState == CharacterMovementState.Jumping || 
                       this.MovementState == CharacterMovementState.Falling || 
                       this.MovementState == CharacterMovementState.Landing || 
                       this.MovementState == CharacterMovementState.Leveling;
            }
        }
    
        public bool IsComboLocked
        {
            get
            {
                return this.comboDelay > 0;
            }
        }
    
        public virtual void Start()
        {
            this.gameManager = Camera.main.GetComponent<StageManager>();
            this.movementCollider = this.GetComponent<CapsuleCollider>();
        
            this.baseY = this.transform.position.y;
            this.Health = this.StartingHealth;

            if (this.Combo != null)
            {
                this.comboChain = this.Combo.GetComponents<ComboChainElement>();
            }
        }
    
        public override void Update()
        {
            base.Update();
        
            // when dead we only do the basics
            if (this.IsDead)
            {
                this.deathTick += 0.1f;
                if (this.deathTick > this.DeathTimer)
                {
                    Destroy(this.gameObject);
                }
            
                this.UpdateAnimationState();
                return;
            }
        
            // During combo lock nothing happens
            if (this.IsComboLocked)
            {
                this.comboDelay -= 0.1f;
                return;
            }
                
            // Initial Positioning
            if (this.startPos != null)
            {
                this.transform.position = (Vector3)this.startPos;
                this.startPos = null;
            }
                
            // Update the state
            if (this.IsAirborne)
            {
                this.UpdateAirState();
            }
            else if (this.MovementState != CharacterMovementState.Idle)
            {
                // Add and check until we timeout to idle state
                this.idleDelay += 0.1f;
                if (this.idleDelay > this.IdleTime)
                {
                    this.MovementState = CharacterMovementState.Idle;
                    this.idleDelay = 0;
                }
            }
        
            // Check the combo state
            if (this.InCombat && !this.IsAirborne)
            {
                // Queue the next combo if we are allowed to
                if (this.queueNextCombat)
                {
                    this.EnterCombat(this.queuedComboStage);
                } 
                else
                {
                    // Check if the combo times out
                    this.lastCombatTick += 0.1f;
                    if (this.lastCombatTick > this.CombatTimeout)
                    {
                        this.LeaveCombat();
                    }
                }
            }
        
            this.UpdateAnimationState();
        }
    
        public void Initialize(Vector3 position)
        {
            this.startPos = position;
        }

        public void OnChildCollision(Collider child, Collider other, bool indicatorOnly)
        {
            if (!this.InCombat)
            {
                return;
            }
            
            var target = other.GetComponent<CharacterEntity>();
            if (target != null && !target.IsDead && this.allowHit)
            {
                this.TestHitCollision(child.transform.position, target, indicatorOnly);
            }
        }
    
        public void OnChildCollisionStay(Collider child, Collider other, bool indicatorOnly)
        {
            if (!this.InCombat)
            {
                return;
            }

            var target = other.GetComponent<Enemy>();
            if (target != null && !target.IsDead && this.allowHit)
            {
                this.TestHitCollision(child.transform.position, target, indicatorOnly);
            }
        }
        
        // ---------------------------------------------
        // Protected
        // ---------------------------------------------
        protected StageManager GameManager
        {
            get
            {
                return this.gameManager;
            }
        }
    
        protected bool InCombat { get; private set; }

        protected ComboChainElement[] ComboChain
        {
            get
            {
                return this.comboChain;
            }
        }

        protected virtual void ResolveCombat(GameObject target, CharacterEntity targetData)
        {
            if (this.hitTest.Contains(target))
            {
                return;
            }

            this.hitTest.Add(target);
            target.GetComponent<StageEntity>().TakeDamage(this.comboChain[this.comboStage].Damage);

            // Combo knockback from force
            if (this.comboChain[this.comboStage].Force > 0)
            {
                Vector3 direction = (target.transform.position - this.transform.position).normalized;
                direction.y = 0;
                target.transform.Translate(direction * this.comboChain[this.comboStage].Force);
            }
        }
    
        protected void EnterCombat(int newStage)
        {
            this.hitTest.Clear();

            // Check if we are in air lock mode, no combat then
            if (this.lockAirAnimation || this.IsComboLocked)
            {
                return;
            }
        
            if (!this.IsAirborne && this.InCombat && this.lastAnimationWasCombat && this.GetComponent<Animation>().isPlaying)
            {
                this.queuedComboStage = newStage;
                this.queueNextCombat = true;
                return;
            }
        
            this.queueNextCombat = false;
            if (this.comboChain[newStage].SFX != null)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.comboChain[newStage].SFX);
            }
        
            // If we are executing the same stage again force the animation
            if (this.comboStage == newStage)
            {
                this.forceComboAnimation = true;
            }
        
            this.playComboAnimation = true;
            this.allowAirCombatAnim = true;
            this.InCombat = true;
            this.lastCombatTick = 0;
            this.comboStage = newStage;
            this.allowHit = true;
        }
    
        protected virtual void LeaveCombat()
        {
            this.InCombat = false;
            this.comboStage = 0;
            this.allowHit = false;
            this.allowAirCombatAnim = false;
        }
    
        protected void StartJump()
        {
            // No jumping if we already are or have no rigid body\\ t
            if (this.IsAirborne)
            {
                return;
            }
        
            // Jumping
            if (this.SFXJump != null)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.SFXJump);
            }
        
            this.lockAirAnimation = false;
            this.lastGroundedPosition = this.transform.position;
            this.upVelocity = this.JumpStrength;
            this.MovementState = CharacterMovementState.Jumping;
        }
        
        protected void MoveCharacter(float newX, float newZ)
        {
            // Lock movement while in combo delay
            if (this.IsComboLocked)
            {
                return;
            }
        
            // Skip out of 0 movement requests
            if (Mathf.Abs(newX) < Mathf.Epsilon && Mathf.Abs(newZ) < Mathf.Epsilon)
            {
                this.moveDirection = new Vector2(0, 0);
                return;
            }
        
            if (this.CanMoveInAir || this.MovementState == CharacterMovementState.Idle || this.MovementState == CharacterMovementState.Walking)
            {
                this.idleDelay = 0;
                if (this.MovementState == CharacterMovementState.Idle)
                {
                    this.MovementState = CharacterMovementState.Walking;
                }
            
                // Clear out depth movement if we are airborne
                if (this.IsAirborne)
                {
                    newZ = 0;
                }

                // Walking
                if (this.MovementState == CharacterMovementState.Idle)
                {
                    this.MovementState = CharacterMovementState.Walking;
                }

                float clampedX = this.CheckXMovementBounds(this.transform.position.x, newX * this.Speed);
                float clampedZ = this.CheckZMovementBounds(this.transform.position.z, newZ * this.Speed);
                var newVector = new Vector3(clampedX, 0, clampedZ);
            
                // Test the new vector against colliding geometry
                Vector3 target = this.movementCollider.transform.position + this.movementCollider.center + new Vector3(newX, 0, newZ);
                for (int i = 0; i < this.gameManager.CollidingGeometry.Count; i++)
                {
                    var test = this.gameManager.CollidingGeometry[i].GetComponent<Collider>() as CapsuleCollider;
                    if (test == null)
                    {
                        continue;
                    }

                    float distance = (target - (test.transform.position + test.center)).magnitude;
                    float requiredDistance = (this.movementCollider.radius * this.movementCollider.transform.localScale.x) + (test.radius * test.transform.localScale.x);
                    if (distance < requiredDistance)
                    {
                        // We are too close to a collider, bail out
                        return;
                    }
                }
            
                // Test if we are moving into a drag entry
                bool wasDragged = this.CheckForDrag(target);
            
                if (!wasDragged && (this.MovementState == CharacterMovementState.Walking || this.MovementState == CharacterMovementState.Idle))
                {
                    this.lastGroundedPosition = this.transform.position;
                }
            
                this.transform.Translate(newVector, null);
            
                this.moveDirection = new Vector2(newX, newZ);
                bool newState = newX < 0;
                if ((newX < 0 || newX > 0) && this.rotate != newState)
                {
                    this.rotate = newState;
                    this.transform.Rotate(Vector3.up, 180);
                }
            }
        }
    
        protected override void Die()
        {
            base.Die();
        
            if (this.SFXDying != null)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.SFXDying);
            }
        }
    
        // ---------------------------------------------
        // Private
        // ---------------------------------------------
        private void TestHitCollision(Vector3 position, CharacterEntity target, bool indicatorOnly)
        {
            if (this.HitIndicator != null)
            {
                this.gameManager.SpawnHitIndicator(this.HitIndicator, position);
            }

            if (indicatorOnly)
            {
                return;
            }

            if (this.comboChain[this.comboStage].SFXHit != null)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.comboChain[this.comboStage].SFXHit);
            }

            this.ResolveCombat(target.gameObject, target);
            this.allowHit = false;
        }

        private bool CheckForDrag(Vector3 target)
        {
            foreach (var drag in this.gameManager.DragEntries)
            {
                var typedCollider = drag.GetComponent<Collider>() as BoxCollider;
                if (typedCollider.bounds.Contains(target))
                {
                    this.upVelocity = -drag.GetComponent<DragObject>().Pull;
                    this.MovementState = CharacterMovementState.Falling;
                    this.moveDirection = new Vector2(0, 0);
                    this.LeaveCombat();
                    if (this.SFXFalling != null)
                    {
                        this.GetComponent<AudioSource>().PlayOneShot(this.SFXFalling);
                    }
                
                    return true;
                }
            }
        
            return false;
        }
    
        private void UpdateAirState()
        {
            this.upVelocity -= this.Gravity;
            if (this.upVelocity < this.TerminalVelocity)
            {
                this.upVelocity = this.TerminalVelocity;
            }
        
            this.transform.Translate(Mathf.Abs(this.moveDirection.x * this.AirSpeed), this.upVelocity, 0);
        
            // Stop air movement if we are hitting the boundaries
            if (this.transform.position.x < this.gameManager.StageBounds.x)
            {
                this.moveDirection = new Vector2(0, 0);
                this.transform.position = new Vector3(this.gameManager.StageBounds.x, this.transform.position.y, this.transform.position.z);
            }
            else if (this.transform.position.x > this.gameManager.StageBounds.width)
            {
                this.moveDirection = new Vector2(0, 0);
                this.transform.position = new Vector3(this.gameManager.StageBounds.width, this.transform.position.y, this.transform.position.z);
            }
        
            if (this.upVelocity < 0.1f && this.MovementState == CharacterMovementState.Jumping)
            {
                // Leveling
                this.MovementState = CharacterMovementState.Leveling;
                return;
            }
        
            if (this.upVelocity < -0.01f && this.MovementState == CharacterMovementState.Leveling)
            {
                // Landing
                this.MovementState = CharacterMovementState.Landing;
                return;
            }
        
            // Check if we are jumping into a drag area
            if (this.MovementState != CharacterMovementState.Falling)
            {
                bool wasDragged = this.CheckForDrag(this.transform.position);
                if (wasDragged)
                {
                    return;
                }
            }
        
            if (this.transform.position.y < this.baseY && this.MovementState == CharacterMovementState.Landing)
            {
                // Landed
                if (this.SFXLand != null)
                {
                    this.GetComponent<AudioSource>().PlayOneShot(this.SFXLand);
                }
            
                this.upVelocity = 0;
                this.lockAirAnimation = false;
                this.transform.position = new Vector3(this.transform.position.x, this.baseY, this.transform.position.z);
                this.MovementState = CharacterMovementState.Idle;
                this.LeaveCombat();
                return;
            }
        
            if (this.transform.position.y < this.DeathDepth)
            {
                // Death from falling
                this.TakeDamage(this.gameManager.FallDamage);
                this.upVelocity = 0;
                this.transform.position = new Vector3(this.lastGroundedPosition.x, this.baseY, this.lastGroundedPosition.z);
                this.MovementState = CharacterMovementState.Idle;
            }
        }
    
        private float CheckXMovementBounds(float origin, float translation)
        {
            if ((origin + translation) < this.gameManager.StageBounds.x && translation < 0)
            {
                this.transform.position = this.transform.position + new Vector3(Mathf.Abs(translation), 0, 0);
            }
        
            if ((origin + translation) > this.gameManager.StageBounds.width && translation > 0)
            {
                this.transform.position = this.transform.position - new Vector3(translation, 0, 0);
            }
        
            return translation;
        }
    
        private float CheckZMovementBounds(float origin, float translation)
        {
            if ((origin + translation) < this.gameManager.StageBounds.y && translation < 0)
            {
                this.transform.position = this.transform.position + new Vector3(0, 0, Mathf.Abs(translation));
            }
        
            if ((origin + translation) > this.gameManager.StageBounds.height && translation > 0)
            {
                this.transform.position = this.transform.position - new Vector3(0, 0, translation);
            }
        
            return translation;
        }
    
        private void UpdateAnimationState()
        {
            if (this.GetComponent<Animation>() == null)
            {
                return;
            }
        
            if (this.IsDead)
            {
                this.PlayAnimation(this.DeathAnimation, true, true, this.AnimationSpeed);
                return;
            }
        
            if (this.playComboAnimation)
            {
                if (this.forceComboAnimation)
                {
                    this.lastAnimationPlayed = null;
                }
            
                this.PlayAnimation(this.comboChain[this.comboStage].Animation, true, false, this.comboChain[this.comboStage].AnimationSpeed);
                this.playComboAnimation = false;
                this.lastAnimationWasCombat = true;
                this.comboDelay = this.comboChain[this.comboStage].Delay;
                return;
            }
        
            if (this.MovementState == CharacterMovementState.Idle && !this.InCombat)
            {
                this.PlayAnimation(this.IdleAnimation, false, true, this.AnimationSpeed);
                return;
            }
        
            if (this.MovementState == CharacterMovementState.Walking && !this.InCombat)
            {
                this.PlayAnimation(this.WalkAnimation, false, true, this.AnimationSpeed);
                return;
            }
        
            if (!this.lockAirAnimation && this.MovementState == CharacterMovementState.Jumping)
            {
                if (this.InCombat)
                {
                    if (this.allowAirCombatAnim)
                    {
                        this.PlayAnimation(this.JumpCombatAnimation, true, false, this.AnimationSpeed);
                        this.allowAirCombatAnim = false;
                        this.lockAirAnimation = true;
                    }
                
                    return;
                }
            
                this.PlayAnimation(this.JumpAnimation, true, true, this.AnimationSpeed);
                return;
            }
        
            if (!this.lockAirAnimation && this.MovementState == CharacterMovementState.Leveling)
            {
                this.PlayAnimation(this.LevelingAnimation, true, true, this.AnimationSpeed);
                return;
            }
        
            if (!this.lockAirAnimation && (this.MovementState == CharacterMovementState.Falling || this.MovementState == CharacterMovementState.Landing))
            {
                if (this.InCombat)
                {
                    if (this.allowAirCombatAnim)
                    {
                        this.PlayAnimation(this.FallCombatAnimation, true, false, this.AnimationSpeed);
                        this.allowAirCombatAnim = false;
                        this.lockAirAnimation = true;
                    }
                
                    return;
                }
            
                this.PlayAnimation(this.FallAnimation, true, true, this.AnimationSpeed);
            }
        }
    
        private void PlayAnimation(string animationName, bool onceOnly = true, bool transition = true, float speed = 1.0f)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                this.GetComponent<Animation>().Stop();
                return;
            }

            if (this.lastAnimationPlayed == animationName)
            {
                if (onceOnly || this.GetComponent<Animation>().isPlaying)
                {
                    return;
                }
            }
        
            // Turn off looping if we have to since we are changing animations
            if (this.lastAnimationPlayed != null)
            {
                if (this.GetComponent<Animation>()[this.lastAnimationPlayed].wrapMode == WrapMode.Loop)
                {
                    this.GetComponent<Animation>()[this.lastAnimationPlayed].wrapMode = WrapMode.Default;
                    this.GetComponent<Animation>().Stop();
                }
            }

            this.GetComponent<Animation>()[animationName].speed = speed;
            if (!onceOnly)
            {
                this.GetComponent<Animation>()[animationName].wrapMode = WrapMode.Loop;
            }
        
            if (transition && this.GetComponent<Animation>().isPlaying)
            {
                this.GetComponent<Animation>().CrossFade(animationName, this.AnimationFadeTime, PlayMode.StopAll);
            }
            else
            {
                this.GetComponent<Animation>().Play(animationName);
            }

            this.lastAnimationPlayed = animationName;
        }
    }
}