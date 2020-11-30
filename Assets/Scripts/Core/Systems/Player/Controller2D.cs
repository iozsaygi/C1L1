using C1L1.Core.Systems.Interaction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace C1L1.Core.Systems.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Controller2D : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private string horizontalAxisName = string.Empty;

        [Header("Controller Settings")]
        [SerializeField, Min(0.1f)] private float movementSpeed = 0.1f;
        [SerializeField, Min(0.1f)] private float jumpForce = 0.1f;
        [SerializeField] private LayerMask groundLayer = 0;
        [SerializeField] private Vector2 crouchedColliderSize = Vector2.zero;
        [SerializeField] private Vector2 crouchedColliderOffset = Vector2.zero;
        [SerializeField, Min(0.1f)] private float stairUsageSpeed = 0.1f;
        [SerializeField, Min(0.1f)] private float stairDestinationReachThreshold = 0.1f;

        [Header("Audio References")]
        [SerializeField] private AudioSource attackAudio = null;

        [Header("Events")]
        [SerializeField] private UnityEvent onAttack = null;
        [SerializeField] private UnityEvent onStairUsageEnd = null;

        [Header("Readonly")]
        [SerializeField] private ControllerState controllerState = ControllerState.Grounded;
        [SerializeField] private bool canUseStairs = false;
        public StairUsageTrigger StairDestination = null;

        private BoxCollider2D boxCollider2D = null;
        private new Rigidbody2D rigidbody2D = null;
        private SpriteRenderer spriteRenderer = null;
        private PlayerMotor playerMotor = null;
        private float horizontalAxisValue = 0.0f;
        private bool canGroundCheck = true;
        private bool canJump = true;
        private bool canWalk = true;
        private bool isCrouched = false;
        private Vector2 defaultColliderSize = Vector2.zero;
        private Vector2 defaultColliderPosition = Vector2.zero;

        public bool CanUseStairs { get => canUseStairs; set => canUseStairs = value; }

        private StairUsageTrigger lastStairDestination = null;
        public ControllerState CurrentState => controllerState;

        public List<GameObject> ObjectsToDisable = new List<GameObject>();
        private bool isObjectsDisabled = false;

        private void Start()
        {
            TryGetComponent(out boxCollider2D);
            TryGetComponent(out rigidbody2D);
            TryGetComponent(out spriteRenderer);
            TryGetComponent(out playerMotor);

            defaultColliderSize = boxCollider2D.size;
            defaultColliderPosition = boxCollider2D.offset;
        }

        private void Update()
        {
            if (controllerState != ControllerState.Disabled)
            {
                if (controllerState != ControllerState.OnStairs)
                {
                    UpdateDirection();
                    FetchMovementInput();
                    Jump();
                    Crouch();
                    TriggerAttack();
                }
            }
        }

        private void FixedUpdate()
        {
            if (controllerState != ControllerState.Disabled)
            {
                if (controllerState != ControllerState.OnStairs)
                {
                    Movement();
                    UpdateGroundState();
                }
                else
                {
                    UseStairs();
                }
            }
        }

        private void UpdateDirection()
        {
            if (horizontalAxisValue > 0.0f && !spriteRenderer.flipX)
                spriteRenderer.flipX = true;

            if (horizontalAxisValue < 0.0f && spriteRenderer.flipX)
                spriteRenderer.flipX = false;
        }

        private void FetchMovementInput()
        {
            if (controllerState != ControllerState.OnAir)
            {
                horizontalAxisValue = Input.GetAxisRaw(horizontalAxisName);

                // Update the controller state.
                if (horizontalAxisValue != 0.0f)
                    controllerState = ControllerState.Walking;
                else
                    controllerState = ControllerState.Grounded;

                // Feed the player motor so correct animation will be played.
                playerMotor.UpdateFloatParameter("Walking", Mathf.Abs(horizontalAxisValue));

                // Check if we are trying to use stairs.
                float vInput = Input.GetAxisRaw("Vertical");
                if (vInput != 0.0f && canUseStairs)
                    controllerState = ControllerState.OnStairs;
            }
            else
            {
                playerMotor.UpdateFloatParameter("Walking", 0.0f);
            }
        }

        private void Movement()
        {
            // Only move if we are not on the air.
            if (controllerState != ControllerState.OnAir && canWalk)
            {
                rigidbody2D.velocity = new Vector2(horizontalAxisValue * movementSpeed, rigidbody2D.velocity.y);
            }
        }

        private void UseStairs()
        {
            if (StairDestination == null)
                return;

            float vInput = Input.GetAxisRaw("Vertical");

            playerMotor.UpdateFloatParameter("Walking", Mathf.Abs(vInput));
            TriggerAttack();

            // Disable the objects.
            if (!isObjectsDisabled)
            {
                foreach (var goToDisable in ObjectsToDisable)
                    goToDisable.SetActive(false);

                isObjectsDisabled = true;
            }

            // Check if the stair destination is below me or not.
            if (transform.position.y > StairDestination.VerticalCheckPoint.position.y)
            {
                // I am above the stair destination.
                if (vInput < 0.0f)
                {
                    rigidbody2D.velocity = Vector2.zero;
                    StairDestination.GetComponent<Collider2D>().enabled = false;
                    rigidbody2D.gravityScale = 0.0f;
                    transform.position = Vector3.MoveTowards(transform.position, StairDestination.transform.position, Time.fixedDeltaTime * stairUsageSpeed);
                }
            }
            else
            {
                // Stair above me.
                if (vInput > 0.0f)
                {
                    rigidbody2D.velocity = Vector2.zero;
                    StairDestination.GetComponent<Collider2D>().enabled = false;
                    rigidbody2D.gravityScale = 0.0f;
                    transform.position = Vector3.MoveTowards(transform.position, StairDestination.transform.position, Time.fixedDeltaTime * stairUsageSpeed);
                }
            }

            if (Vector3.Distance(transform.position, StairDestination.transform.position) <= stairDestinationReachThreshold)
            {
                onStairUsageEnd.Invoke();
                canUseStairs = false;

                rigidbody2D.gravityScale = 1.0f;
                lastStairDestination = StairDestination;
                StairDestination = null;
                controllerState = ControllerState.Grounded;
                StartCoroutine(ResetLastStairCollider());
                ObjectsToDisable.Clear();
                isObjectsDisabled = false;
            }
        }

        private System.Collections.IEnumerator ResetLastStairCollider()
        {
            yield return new WaitForSeconds(1.0f);
            lastStairDestination.GetComponent<Collider2D>().enabled = true;
        }

        private void UpdateGroundState()
        {
            if (!canGroundCheck)
                return;

            RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0.0f, Vector2.down, 0.1f, groundLayer);

            if (raycastHit2D.collider != null)
            {
                controllerState = ControllerState.Grounded;
                playerMotor.UpdateBoolParameter("Jumping", false);
            }
            else
            {
                controllerState = ControllerState.OnAir;
            }
        }

        public void Jump()
        {
            // TO DO: Fix the input style.
            if (Input.GetButtonDown("Jump") && canJump)
            {
                if (controllerState != ControllerState.OnAir && controllerState != ControllerState.OnStairs &&
                    controllerState != ControllerState.Crouch)
                {
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
                    controllerState = ControllerState.OnAir;
                    canGroundCheck = false;

                    playerMotor.UpdateBoolParameter("Jumping", true);
                    StartCoroutine(UpdateGroundCheck(0.25f, true));
                }
            }
        }

        private void Crouch()
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("Fire2"))
            {
                if (controllerState == ControllerState.Grounded)
                {
                    playerMotor.UpdateBoolParameter("Crouching", true);
                    boxCollider2D.offset = crouchedColliderOffset;
                    boxCollider2D.size = crouchedColliderSize;
                    controllerState = ControllerState.Crouch;
                    canJump = false;
                    canWalk = false;
                    isCrouched = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.S) || Input.GetButtonUp("Fire2"))
            {
                if (controllerState == ControllerState.Grounded)
                {
                    playerMotor.UpdateBoolParameter("Crouching", false);
                    boxCollider2D.offset = defaultColliderPosition;
                    boxCollider2D.size = defaultColliderSize;
                    canJump = true;
                    canWalk = true;
                    isCrouched = false;
                    controllerState = ControllerState.Grounded;
                }
            }
        }

        private void TriggerAttack()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (controllerState == ControllerState.Walking)
                    return;

                if (isCrouched)
                {
                    playerMotor.UpdateTriggerParameter("Crouched Attack");
                }
                else
                {
                    playerMotor.UpdateBoolParameter("Jumping", false);
                    playerMotor.UpdateTriggerParameter("Attacking");
                }

                onAttack.Invoke();
            }
        }

        private System.Collections.IEnumerator UpdateGroundCheck(float delay, bool value)
        {
            yield return new WaitForSeconds(delay);
            canGroundCheck = value;
        }

        public void PlayAttackSound()
        {
            attackAudio.Play();
        }

        public void DisableInput()
        {
            controllerState = ControllerState.Disabled;
        }

        public void EnableInput()
        {
            controllerState = ControllerState.Grounded;
        }
    }
}