using UnityEngine;
using UnityEngine.Events;

namespace C1L1.Core.Systems.Combat
{
    [DisallowMultipleComponent]
    internal sealed class Combatable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject flameEffect = null;
        [SerializeField] private GameObject heartPrefab = null;

        [Header("Settings")]
        [SerializeField, Min(1)] private int maximumHealth = 1;
        [SerializeField, Min(0)] private int scoreGain = 0;
        [SerializeField, Min(0)] private int damage = 0;

        [Header("Events")]
        [SerializeField] private UnityEvent onTookDamage = null;
        [SerializeField] private UnityEvent onDeath = null;

        private int currentHealth = 1;
        public int CurrentHealth => currentHealth;
        public int ScoreGain => scoreGain;
        public int Damage => damage;
        public int MaximumHealth => maximumHealth;

        private void Start()
        {
            currentHealth = maximumHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            onTookDamage.Invoke();

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                onDeath.Invoke();
                Destroy(this.gameObject);
            }
        }

        public void SpawnFlameEffect()
        {
            Instantiate(flameEffect, transform.position, Quaternion.identity);
        }

        public void SpawnHeart()
        {
            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue > 0.5f)
                Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
    }
}