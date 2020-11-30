using UnityEngine;
using UnityEngine.SceneManagement;

namespace C1L1.Scripted
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class IntroBelmont : MonoBehaviour
    {
        [SerializeField] private Canvas targetCanvas = null;
        [SerializeField] private GameObject fadeInPrefab = null;
        [SerializeField] private Sprite belmontBehindSprite = null;

        private Animator animator = null;
        private SpriteRenderer spriteRenderer = null;

        private void Start()
        {
            TryGetComponent(out animator);
            TryGetComponent(out spriteRenderer);
        }

        public void TurnBehind()
        {
            animator.enabled = false;
            spriteRenderer.sprite = belmontBehindSprite;
            StartCoroutine(SpawnFadeInEffect());
        }

        public System.Collections.IEnumerator SpawnFadeInEffect()
        {
            yield return new WaitForSeconds(2.5f);
            GameObject fip = Instantiate(fadeInPrefab);
            fip.transform.SetParent(targetCanvas.transform, false);
            StartCoroutine(ChangeScene());
        }

        private System.Collections.IEnumerator ChangeScene()
        {
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadSceneAsync(1);
        }
    }
}