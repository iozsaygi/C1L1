using C1L1.Core.Systems.Combat;
using C1L1.UI;
using UnityEngine;
using UnityEngine.Events;
using Welog.Core;

namespace C1L1.Core.Systems.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class CombatController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform attackPointRight = null;
        [SerializeField] private Transform attackPointLeft = null;
        [SerializeField] private AudioSource hitAudioEffect = null;
        [SerializeField] private AudioSource beingHitAudioEffect = null;
        [SerializeField] private Canvas targetCanvas = null;
        [SerializeField] private GameObject textPrefab = null;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float attackPointHorizontalOffset = 0.1f;
        [SerializeField, Min(0.1f)] private float colliderDetectionRadius = 0.1f;
        [SerializeField] private LayerMask combatableLayer = 0;

        [Header("Health Settings")]
        [SerializeField, Min(1)] private int maximumHealth = 1;
        [SerializeField] private int currentHealth = 0;

        [Header("Readonly")]
        [SerializeField] private bool isInvulnerable = false;

        [Header("Events")]
        [SerializeField] private UnityEvent onDeath = null;
        [SerializeField] private UnityEvent onTookDamage = null;
        [SerializeField] private UnityEvent onHeal = null;
        [SerializeField] private UnityEvent onScoreGain = null;

        private new Rigidbody2D rigidbody2D = null;
        private SpriteRenderer spriteRenderer = null;
        private Controller2D controller2D = null;
        private PlayerMotor playerMotor = null;
        private BoxCollider2D boxCollider2D = null;
        private int score = 0;

        public int MaximumHealth => maximumHealth;
        public int CurrentHealth => currentHealth;

        public int Score => score;

        private void Start()
        {
            TryGetComponent(out boxCollider2D);
            TryGetComponent(out rigidbody2D);
            TryGetComponent(out spriteRenderer);
            TryGetComponent(out controller2D);
            TryGetComponent(out playerMotor);

            currentHealth = maximumHealth;
        }

        private System.Collections.IEnumerator ResetInvulnerability()
        {
            yield return new WaitForSeconds(1.5f);
            isInvulnerable = false;
            controller2D.EnableInput();
            playerMotor.UpdateBoolParameter("Hurt", false);
        }

        public void TakeDamage(Combatable combatable)
        {
            if (isInvulnerable || currentHealth <= 0)
                return;

            Debug.Assert(rigidbody2D != null);
            Debug.Assert(combatable != null);

            currentHealth -= combatable.Damage;

            if (currentHealth <= 0)
            {
                playerMotor.UpdateTriggerParameter("Death");
                controller2D.DisableInput();
                onDeath.Invoke();
                boxCollider2D.size = new Vector2(1.0f, 0.1f);

                return;
            }

            //if (controller2D.CurrentState != ControllerState.OnStairs)
             //   controller2D.DisableInput();

            // Maybe disable adding force if player using stairs at the moment?
            //if (controller2D.CurrentState != ControllerState.OnStairs && controller2D.CurrentState != ControllerState.Disabled)
            //{
                Vector2 direction = (combatable.transform.position - transform.position).normalized;
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce(new Vector2((-direction.x) * 200.0f, 500.0f), ForceMode2D.Force);

            if (rigidbody2D.gravityScale == 0.0f)
                rigidbody2D.gravityScale = 1.0f;

                playerMotor.UpdateBoolParameter("Hurt", true);
            //}
            beingHitAudioEffect.Play();

            isInvulnerable = true;
            StartCoroutine(ResetInvulnerability());

            SpawnText("-" + combatable.Damage + " health", 25.0f, Color.red, transform.position, new Vector3(0.0f, 100.0f, 0.0f));
            onTookDamage.Invoke();
        }

        public void AddScore(int value)
        {
            score += value;
            onScoreGain.Invoke();
        }

        public void Attack()
        {
            Collider2D[] collider2Ds = null;

            // First figure out where the player is looking.
            if (spriteRenderer.flipX)
            {
                // Right.
                collider2Ds = Physics2D.OverlapCircleAll(attackPointRight.position, colliderDetectionRadius, combatableLayer);
            }
            else
            {
                // Left.
                collider2Ds = Physics2D.OverlapCircleAll(attackPointLeft.position, colliderDetectionRadius, combatableLayer);
            }

            foreach (Collider2D collider2D in collider2Ds)
            {
                Combatable combatable = collider2D.GetComponent<Combatable>();
                if (combatable != null)
                {
                    combatable.TakeDamage(1);
                    hitAudioEffect.Play();
                    WelogAPI.Log(0, "Dealt 1 damage to " + collider2D.gameObject.name);

                    if (combatable.ScoreGain != 0 && combatable.CurrentHealth <= 0)
                    {
                        AddScore(combatable.ScoreGain);
                        SpawnText("+" + combatable.ScoreGain.ToString() + " points", 25.0f, Color.white, combatable.transform.position, new Vector3(0.0f,
                            100.0f, 0.0f));
                    }
                }
            }
        }

        public void Heal(int health)
        {
            int healedValue = currentHealth + health;
            if (healedValue <= maximumHealth)
            {
                currentHealth += health;
                // WelogAPI.Log(1, "Restored " + health.ToString() + " health");
                SpawnText("+" + health.ToString() + " health", 25.0f, Color.green, transform.position, new Vector3(0.0f, 100.0f, 0.0f));
                onHeal.Invoke();
            }
            else
            {
                WelogAPI.Log(1, "Already at maximum health");
                // SpawnText("Already at maximum health!", 20.0f, Color.green, transform.position, new Vector3(0.0f, 100.0f, 0.0f));
            }
        }

        public void SpawnText(string msg, float fontSize, Color color, Vector3 at, Vector3 offset)
        {
            var fadingText = Instantiate(textPrefab, Vector2.zero, textPrefab.transform.rotation).GetComponent<FadingText>();

            var screenPos = Camera.main.WorldToScreenPoint(at);

            fadingText.gameObject.transform.SetParent(targetCanvas.transform, false);
            fadingText.gameObject.transform.position = screenPos + offset;

            fadingText.Modify(msg, fontSize, color);
        }
    }
}