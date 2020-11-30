using UnityEngine;

namespace C1L1.Core.Systems.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Zombie : MonoBehaviour
    {
        public Vector2 FirstPoint = Vector2.zero;
        public Vector2 SecondPoint = Vector2.zero;
        [Min(0.1f)] public float MovementSpeed = 0.1f;
        [Min(0.1f)] public float ReachThreshold = 0.1f;
        public bool IsCurrentTargetFirstPoint = true;

        private SpriteRenderer spriteRenderer = null;

        private void Start()
        {
            TryGetComponent(out spriteRenderer);
        }

        private void Update()
        {
            if (IsCurrentTargetFirstPoint)
            {
                transform.position = Vector2.MoveTowards(transform.position, FirstPoint, Time.deltaTime * MovementSpeed);

                if (Vector2.Distance(transform.position, FirstPoint) <= ReachThreshold)
                    IsCurrentTargetFirstPoint = false;

                if (FirstPoint.x < transform.position.x && !spriteRenderer.flipX)
                    spriteRenderer.flipX = true;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, SecondPoint, Time.deltaTime * MovementSpeed);

                if (Vector2.Distance(transform.position, SecondPoint) <= ReachThreshold)
                    IsCurrentTargetFirstPoint = true;

                if (FirstPoint.x < transform.position.x && spriteRenderer.flipX)
                    spriteRenderer.flipX = false;
            }
        }
    }
}