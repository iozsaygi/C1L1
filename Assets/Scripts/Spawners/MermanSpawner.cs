using C1L1.Core.Systems.AI;
using UnityEngine;

namespace C1L1.Spawners
{
    [DisallowMultipleComponent]
    internal sealed class MermanSpawner : MonoBehaviour
    {
        [SerializeField] private Transform player = null;
        [SerializeField] private GameObject mermamanPrefab = null;
        [SerializeField, Min(0.1f)] private float minimumSpawnRate = 0.1f;
        [SerializeField, Min(0.1f)] private float maximumSpawnRate = 0.1f;
        [SerializeField, Min(1)] private int maximumSpawnCount = 1;

        private int currentSpawnCount = 0;

        private void OnEnable()
        {
            StartCoroutine(SpawnMermaman());
        }

        private System.Collections.IEnumerator SpawnMermaman()
        {
            while (this.gameObject.activeSelf)
            {
                if (currentSpawnCount >= maximumSpawnCount)
                    this.gameObject.SetActive(false);

                yield return new WaitForSeconds(Random.Range(minimumSpawnRate, maximumSpawnRate));
                float sValue = Random.Range(0.0f, 1.0f);
                Merman merman = null;
                if (sValue > 0.5f)
                {
                    merman = Instantiate(mermamanPrefab, new Vector2(player.transform.position.x + 4.0f, transform.position.y), Quaternion.identity)
                        .GetComponent<Merman>();
                }
                else
                {
                    merman = Instantiate(mermamanPrefab, new Vector2(player.transform.position.x - 4.0f, transform.position.y), Quaternion.identity)
                        .GetComponent<Merman>();
                }
                merman.Player = player;

                currentSpawnCount++;
            }
        }
    }
}