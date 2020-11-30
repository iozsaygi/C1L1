using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Core.Systems.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    internal sealed class Panther : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float leftChargeDuration = 0.1f;
        [SerializeField, Min(0.1f)] private float destroyDuration = 0.1f;

        private Animator animator = null;
        private SpriteRenderer spriteRenderer = null;
        private new Rigidbody2D rigidbody2D = null;
        private bool isChargeEnabled = false;
        private bool interacted = false;

        private CombatController combatController = null;
        private float leftChargeTimer = 0.0f;
        private float destroyTimer = 0.0f;

        private void Start()
        {
            TryGetComponent(out animator);
            TryGetComponent(out spriteRenderer);
            TryGetComponent(out rigidbody2D);
            combatController = FindObjectOfType<CombatController>();
        }

        private void Update()
        {
            if (isChargeEnabled)
            {
                leftChargeTimer += Time.deltaTime;
                rigidbody2D.velocity = new Vector2(-5.0f, rigidbody2D.velocity.y);
                if (leftChargeTimer > leftChargeDuration)
                {
                    // leftChargeTimer = 0.0f;
                    rigidbody2D.velocity = new Vector2(5.0f, rigidbody2D.velocity.y);
                    spriteRenderer.flipX = false;

                    destroyTimer += Time.deltaTime;
                    if (destroyTimer > destroyDuration)
                        Destroy(this.gameObject);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (interacted && collision.transform.CompareTag("Player"))
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            }

            if (collision.transform.CompareTag("Player") && !interacted)
            {
                collision.transform.GetComponent<CombatController>().TakeDamage(GetComponent<Combatable>());
                interacted = true;
            }
        }

        public void Charge()
        {
            animator.SetBool("Run", true);
            isChargeEnabled = true;
        }
    }
}