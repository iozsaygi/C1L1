using C1L1.Core.Systems.AI;
using C1L1.Core.Systems.Player;
using UnityEngine;

namespace C1L1.Spawners
{
    public class BatSpawner : MonoBehaviour
    {
        [Header("References")]
        public Controller2D Controller2D = null;
        public GameObject BatPrefab = null;
        public Transform[] SpawnPoints = null;

        [Header("Settings")]
        public float MinimumSpawnRate = 0.1f;
        public float MaximumSpawnRate = 0.1f;

        private void OnEnable()
        {
            StartCoroutine(SpawnRoutine());
        }

        private System.Collections.IEnumerator SpawnRoutine()
        {
            while (this.gameObject.activeSelf)
            {
                yield return new WaitForSeconds(Random.Range(MinimumSpawnRate, MaximumSpawnRate));

                var rPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
                var cPosition = new Vector2(rPoint.position.x, Controller2D.transform.position.y);
                Bat bat = Instantiate(BatPrefab, cPosition, Quaternion.identity).GetComponent<Bat>();
                bat.controller2D = Controller2D;
            }
        }
    }
}