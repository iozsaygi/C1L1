using C1L1.Core.Systems.AI;
using C1L1.Core.Systems.Combat;
using C1L1.Core.Systems.Player;
using C1L1.Spawners;
using UnityEngine;
using UnityEngine.Events;
using Welog.Core;

namespace C1L1.Core.Systems.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    internal sealed class Interactable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioClip heartCollectAudioEffect = null;
        [SerializeField] private Transform targetCanvas = null;
        [SerializeField] private GameObject fadeInPrefab = null;
        [SerializeField] private GameObject fadeOutPrefab = null;
        [SerializeField] private Transform castleEntrancePoint = null;
        [SerializeField] private GameObject entrance = null;
        [SerializeField] private GameObject thirdRoom = null;
        [SerializeField] private GameObject fourthRoom = null;
        [SerializeField] private Transform thirdRoomSpawnPoint = null;
        [SerializeField] private GameObject lobby = null;
        [SerializeField] private CameraController cameraController = null;
        [SerializeField] private Transform thirdRoomCameraSnapPoint = null;
        [SerializeField] private Transform thirdRoomCameraSnapPoint2 = null;
        [SerializeField] private Transform fourthRoomCameraSnapPoint = null;
        [Space]
        [SerializeField] private Transform bossRoom = null;
        [SerializeField] private Transform bossRoomPlayerSpawnPoint = null;
        [SerializeField] private Transform bossRoomCameraSnapPoint = null;
        [SerializeField] private AudioSource bossMusic = null;
        public bool ForceSingleProcess = false;

        [Header("Events")]
        [SerializeField] private UnityEvent onInteract = null;

        public bool DontWorkIfPlayerIsInvulnerable = false;

        private CombatController combatController = null;
        private bool triggeredBefore = false;
        private bool isTransformedToFourthRoom = false;

        private void Start()
        {
            combatController = FindObjectOfType<CombatController>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (DontWorkIfPlayerIsInvulnerable)
                return;

            if (ForceSingleProcess && triggeredBefore)
                return;

            if (collision.CompareTag("Player"))
            {
                onInteract.Invoke();
                triggeredBefore = true;
            }
        }

        // I know this shouldn't be here but the deadline is close :(
        public void PlayHeartCollectSoundEffect()
        {
            GameObject gameObject = new GameObject("Heart Collect Audio Effect");
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = heartCollectAudioEffect;
            audioSource.Play();

            // WelogAPI.Log(1, "Collected heart");
            combatController.Heal(1);

            Destroy(gameObject, 1.0f);
            Destroy(this.gameObject);
        }

        public GameObject SpawnFadeInEffect()
        {
            GameObject fip = Instantiate(fadeInPrefab);
            fip.transform.SetParent(targetCanvas, false);
            return fip;
        }

        public void DealDamageToPlayer()
        {
            combatController.TakeDamage(GetComponent<Combatable>());
        }

        public void StartPlayerTransformationToCastle()
        {
            StartCoroutine(TranslateToCastleRoutine());
        }

        public void StartPlayerThirdRoomTransformation()
        {
            StartCoroutine(TransformToThirdRoom());
        }

        public void DestroyEntrance()
        {
            Destroy(entrance.gameObject);
        }

        public void StartTransformationToFourthRoom()
        {
            if (!isTransformedToFourthRoom)
                StartCoroutine(TransformationToFourthRoom());
        }

        public void SnapCameraToSecondThirdRoomPoint()
        {
            StartCoroutine(TranslateToSecondPartOfThirdRoom());
        }

        private System.Collections.IEnumerator TranslateToSecondPartOfThirdRoom()
        {
            var go = SpawnFadeInEffect();
            yield return new WaitForSeconds(2.0f);
            Destroy(go);

            cameraController.IsChaseEnabled = false;
            cameraController.transform.position = thirdRoomCameraSnapPoint2.position;
        }

        private System.Collections.IEnumerator TransformationToFourthRoom()
        {
            var go = SpawnFadeInEffect();
            yield return new WaitForSeconds(2.0f);
            Destroy(go);

            GameObject fip = Instantiate(fadeOutPrefab);
            fip.transform.SetParent(targetCanvas, false);
            Destroy(fip, 2.0f);

            cameraController.transform.position = fourthRoomCameraSnapPoint.position;
            // cameraController.IsChaseEnabled = true;

            isTransformedToFourthRoom = true;
        }

        public void SetFourthRoomTransformation(bool val)
        {
            isTransformedToFourthRoom = val;
        }

        public void SnapCameraToFirstPartOfThirdRoom()
        {
            cameraController.transform.position = thirdRoomCameraSnapPoint.position;
        }

        private System.Collections.IEnumerator TranslateToCastleRoutine()
        {
            // yield return new WaitForSeconds(1.0f);
            var go = SpawnFadeInEffect();

            yield return new WaitForSeconds(2.0f);

            Destroy(go);

            GameObject fip = Instantiate(fadeOutPrefab);
            fip.transform.SetParent(targetCanvas, false);
            Destroy(fip, 2.0f);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = castleEntrancePoint.position;

            Controller2D controller2D = player.GetComponent<Controller2D>();
            controller2D.enabled = true;
            // controller2D.Jump();
            player.GetComponent<CombatController>().enabled = true;

            Destroy(entrance);
        }

        private System.Collections.IEnumerator TransformToThirdRoom()
        {
            var go = SpawnFadeInEffect();
            yield return new WaitForSeconds(2.0f);
            Destroy(go);

            GameObject fip = Instantiate(fadeOutPrefab);
            fip.transform.SetParent(targetCanvas, false);
            Destroy(fip, 2.0f);

            thirdRoom.SetActive(true);
            fourthRoom.SetActive(true);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = thirdRoomSpawnPoint.position;
            player.GetComponent<Controller2D>().enabled = true;
            player.GetComponent<Animator>().enabled = true;

            cameraController.IsChaseEnabled = false;
            cameraController.transform.position = thirdRoomCameraSnapPoint.position;

            // Clean up the lobby.
            foreach (var zombieSpawner in FindObjectsOfType<ZombieSpawner>())
                Destroy(zombieSpawner.gameObject);

            foreach (var zombie in FindObjectsOfType<Zombie>())
                Destroy(zombie.gameObject);

            Destroy(lobby);
        }

        public void StartTransformationToBossRoom()
        {
            StartCoroutine(TranslateToBossRoom());
        }

        private System.Collections.IEnumerator TranslateToBossRoom()
        {
            var go = SpawnFadeInEffect();
            yield return new WaitForSeconds(2.0f);
            Destroy(go);

            bossRoom.gameObject.SetActive(true);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = bossRoomPlayerSpawnPoint.position;
            player.GetComponent<Controller2D>().enabled = true;
            player.GetComponent<Animator>().enabled = true;

            cameraController.IsChaseEnabled = false;
            cameraController.transform.position = bossRoomCameraSnapPoint.position;

            // Clean up some bats.
            foreach (var bat in FindObjectsOfType<Bat>())
                Destroy(bat.gameObject);

            KillMermans();

            Destroy(thirdRoom);
            Destroy(fourthRoom);
        }

        public void KillMermans()
        {
            foreach (var merman in FindObjectsOfType<Merman>())
                Destroy(merman.gameObject);
        }

        public void StartBossFight()
        {
            StartCoroutine(TriggerBossFight());
        }

        private System.Collections.IEnumerator TriggerBossFight()
        {
            yield return new WaitForSeconds(3.0f);
            bossMusic.Play();

            FindObjectOfType<Boss>().enabled = true;
        }

        public void KillAllZombies()
        {
            foreach (var zombie in FindObjectsOfType<Zombie>())
                Destroy(zombie.gameObject);
        }
    }
}