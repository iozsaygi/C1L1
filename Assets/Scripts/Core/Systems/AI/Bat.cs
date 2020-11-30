using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Core.Systems.AI
{
    public class Bat : MonoBehaviour
    {
        public float MoveSpeed = 5.0f;
        public float Frequency = 20.0f;
        public float Magnitude = 0.5f;
        public float LifeTime = 0.1f;

        private Vector3 axis = Vector3.zero;
        private Vector3 position = Vector3.zero;
        public Controller2D controller2D = null;
        Vector3 direction = Vector3.zero;

        private void Start()
        {
            position = transform.position;
            axis = Vector3.up;
            controller2D = FindObjectOfType<Controller2D>();

            if (controller2D.transform.position.x > transform.position.x)
            {
                direction = transform.right;
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                direction = -transform.right;
                GetComponent<SpriteRenderer>().flipX = true;
            }

            Destroy(gameObject, LifeTime);
        }

        private void Update()
        {
            position += direction * Time.deltaTime * MoveSpeed;
            transform.position = position + axis * Mathf.Sin(Time.time * Frequency) * Magnitude;
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