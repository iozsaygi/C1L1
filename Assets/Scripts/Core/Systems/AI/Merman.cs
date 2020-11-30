using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using C1L1.Core.Systems.Projectiles;
using UnityEngine;

namespace C1L1.Core.Systems.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class Merman : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject FireballPrefab = null;
        [SerializeField] private Transform rightAttackPoint = null;
        [SerializeField] private Transform leftAttackPoint = null;
        [SerializeField] private Transform player = null;
        public Transform Player { set => player = value; }
        [SerializeField] private BoxCollider2D collisionCollider = null;
        [SerializeField] private BoxCollider2D playerDamageTrigger = null;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float movementSpeed = 0.1f;
        [SerializeField, Min(0.1f)] private float initialVerticalVelocity = 0.1f;
        [SerializeField, Min(0.1f)] private float normalStateChangeDelay = 0.1f;
        [SerializeField, Min(0.1f)] private float minimumThinkRate = 0.1f;
        [SerializeField, Min(0.1f)] private float maximumThinkRate = 0.1f;

        private Animator animator = null;
        private AudioSource audioSource = null;
        private new Rigidbody2D rigidbody2D = null;
        private SpriteRenderer spriteRenderer = null;

        private bool doIWantToMove = false;
        private bool isMoveDirectionRight = false;
        private float moveDuration = 0.0f;
        private float moveTimer = 0.0f;

        private void Start()
        {
            TryGetComponent(out animator);
            TryGetComponent(out audioSource);
            TryGetComponent(out rigidbody2D);
            TryGetComponent(out spriteRenderer);

            if (!collisionCollider.isTrigger)
                collisionCollider.isTrigger = true;

            rigidbody2D.gravityScale = 0.0f;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, initialVerticalVelocity);

            audioSource.Play();

            StartCoroutine(EnableNormalState());
            StartCoroutine(Think());
        }

        private void Update()
        {
            // Update the sprite renderer based on player position.
            if (player.position.x > transform.position.x && spriteRenderer.flipX)
                spriteRenderer.flipX = false;

            if (player.position.x < transform.position.x && !spriteRenderer.flipX)
                spriteRenderer.flipX = true;

            if (doIWantToMove)
            {
                animator.SetBool("WantsToMove", true);
                moveTimer += Time.deltaTime;

                if (moveTimer < moveDuration)
                {
                    if (isMoveDirectionRight)
                    {
                        transform.Translate(Vector2.right * Time.deltaTime * movementSpeed);
                    }
                    else
                    {
                        transform.Translate(Vector2.left * Time.deltaTime * movementSpeed);
                    }
                }
                else
                {
                    doIWantToMove = false;
                }
            }
            else
            {
                animator.SetBool("WantsToMove", false);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
                collision.GetComponent<CombatController>().TakeDamage(GetComponent<Combatable>());
        }

        private System.Collections.IEnumerator EnableNormalState()
        {
            yield return new WaitForSeconds(normalStateChangeDelay);
            collisionCollider.isTrigger = false;
            rigidbody2D.gravityScale = 1.0f;
            rigidbody2D.velocity = Vector2.zero;
        }

        private System.Collections.IEnumerator Think()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(Random.Range(minimumThinkRate, maximumThinkRate));

                float tValue = Random.Range(0.0f, 1.0f);
                if (tValue > 0.5f)
                    LaunchFireball();
                else
                    SetMovement();
            }
        }

        private void LaunchFireball()
        {
            if (player.transform.position.x > transform.position.x)
            {
                Fireball fireball = Instantiate(FireballPrefab, rightAttackPoint.position, Quaternion.identity).GetComponent<Fireball>();
                fireball.Right = true;
            }
            else
            {
                Fireball fireball = Instantiate(FireballPrefab, leftAttackPoint.position, Quaternion.identity).GetComponent<Fireball>();
                fireball.Right = false;

                fireball.GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        private void SetMovement()
        {
            moveTimer = 0.0f;
            moveDuration = Random.Range(1.0f, 2.0f);
            doIWantToMove = true;

            if (player.transform.position.x > transform.position.x)
                isMoveDirectionRight = true;
            else
                isMoveDirectionRight = false;
        }
    }
}