using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Core.Systems.Projectiles
{
    public class Fireball : MonoBehaviour
    {
        public float Speed = 0.0f;
        public float LifeTime = 0.1f;
        public bool Right = true;

        private Vector3 position = Vector3.zero;

        private void Start()
        {
            Destroy(this.gameObject, LifeTime);
            position = transform.position;
            // Direction = transform.right;
        }

        private void Update()
        {
            if (Right)
            {
                transform.Translate(Vector2.right * Time.deltaTime * Speed);
            }
            else
            {
                transform.Translate(Vector2.left * Time.deltaTime * Speed);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<CombatController>().TakeDamage(GetComponent<Combatable>());
                GetComponent<Combatable>().SpawnFlameEffect();
                Destroy(this.gameObject);
            }
        }
    }
}