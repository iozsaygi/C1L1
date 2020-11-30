using C1L1.Core.Systems.AI;
using UnityEngine;

namespace C1L1.Spawners
{
    internal sealed class ZombieSpawner : MonoBehaviour
    {
        public GameObject ZombiePrefab = null;
        [Min(0.1f)] public float MinimumSpawnInterval = 0.1f;
        [Min(0.1f)] public float MaximumSpawnInterval = 0.1f;
        public Transform FirstPoint = null;
        public Transform SecondPoint = null;
        [Min(0)] public int MaximumSpawnCount = 0;

        private int currentSpawnCount = 0;

        private void Start()
        {
            StartCoroutine(SpawnZombie());
        }

        private System.Collections.IEnumerator SpawnZombie()
        {
            while (this.gameObject.activeSelf)
            {
                // Stop the spawn process if i already spawned max. number of zombies.
                if (currentSpawnCount >= MaximumSpawnCount)
                    this.gameObject.SetActive(false);

                float randomInterval = Random.Range(MinimumSpawnInterval, MaximumSpawnInterval);
                yield return new WaitForSeconds(randomInterval);

                Zombie zombie = null;
                float rndPoint = Random.Range(0.0f, 1.0f);

                if (rndPoint > 0.5f)
                {
                    zombie = Instantiate(ZombiePrefab, FirstPoint.position, Quaternion.identity).GetComponent<Zombie>();
                    zombie.IsCurrentTargetFirstPoint = false;
                }
                else
                {
                    zombie = Instantiate(ZombiePrefab, SecondPoint.position, Quaternion.identity).GetComponent<Zombie>();
                    zombie.IsCurrentTargetFirstPoint = true;
                }

                zombie.FirstPoint = FirstPoint.position;
                zombie.SecondPoint = SecondPoint.position;

                currentSpawnCount++;
            }
        }
    }
}