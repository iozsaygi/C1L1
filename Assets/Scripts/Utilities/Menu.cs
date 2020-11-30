using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C1L1.Utilities
{
    [DisallowMultipleComponent]
    internal sealed class Menu : MonoBehaviour
    {
        public void RestartCurrentActiveScene()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene()
                .buildIndex);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
    }
}