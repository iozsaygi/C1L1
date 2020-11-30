using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Core.Systems.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    internal sealed class Boss : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform target = null;
        [SerializeField] private Transform initialMovePosition = null;
        [SerializeField] private GameObject healthBar = null;

        [Header("Settings")]
        [SerializeField, Min(0.1f)] private float movementSpeed = 0.1f;
        [SerializeField, Min(0.1f)] private float chargedMovementSpeed = 0.1f;
        [SerializeField, Min(0.1f)] private float requiredDistanceForPointReach = 0.1f;
        [SerializeField, Min(0.1f)] private float minimumSleepTime = 0.1f;
        [SerializeField, Min(0.1f)] private float maximumSleepTime = 0.1f;

        private Animator animator = null;
        private Combatable combatable = null;
        private CombatController combatController = null;
        private bool hasReachedInitialPosition = false;
        private Vector2 chargePosition = Vector2.zero;
        private bool readyToCharge = false;
        private bool goBackToStartingPoint = false;
        private Vector2 chargeStartPosition = Vector2.zero;

        private void Start()
        {
            TryGetComponent(out animator);
            TryGetComponent(out combatable);

            animator.SetBool("Activated", true);
        }

        private void Update()
        {
            if (!hasReachedInitialPosition)
            {
                transform.position = Vector2.MoveTowards(transform.position, initialMovePosition.position, Time.deltaTime * movementSpeed);
                if (Vector2.Distance(transform.position, initialMovePosition.position) < requiredDistanceForPointReach)
                {
                    hasReachedInitialPosition = true;
                    healthBar.SetActive(true);
                    StartCoroutine(Sleep());
                }
            }

            if (readyToCharge)
            {
                transform.position = Vector2.MoveTowards(transform.position, chargePosition, Time.deltaTime * chargedMovementSpeed);
                if (Vector2.Distance(transform.position, chargePosition) < requiredDistanceForPointReach)
                {
                    goBackToStartingPoint = true;
                    readyToCharge = false;
                }
            }

            if (goBackToStartingPoint)
            {
                transform.position = Vector2.MoveTowards(transform.position, chargeStartPosition, Time.deltaTime * movementSpeed);
                float rDistanceRequired = Random.Range(0.75f, 1.5f);

                if (Vector2.Distance(transform.position, chargeStartPosition) < rDistanceRequired)
                {
                    goBackToStartingPoint = false;
                    StartCoroutine(Sleep());
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            combatController = collision.GetComponent<CombatController>();
            if (combatController != null)
                combatController.TakeDamage(combatable);
        }

        private System.Collections.IEnumerator Sleep()
        {
            yield return new WaitForSeconds(Random.Range(minimumSleepTime, maximumSleepTime));
            SetCharge();
        }

        private void SetCharge()
        {
            chargeStartPosition = transform.position;
            chargePosition = new Vector2(target.position.x + 0.5f, target.position.y + 0.5f);
            readyToCharge = true;
        }
    }
}