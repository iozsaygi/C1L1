using UnityEngine;
using UnityEngine.Events;

namespace C1L1.Utilities
{
    [DisallowMultipleComponent]
    internal sealed class PauseManager : MonoBehaviour
    {
        [Header("General References")]
        [SerializeField] private GameObject pauseScreen = null;
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        [Header("Audio References")]
        [SerializeField] private AudioSource pauseAudio = null;
        [SerializeField] private AudioSource resumeAudio = null;

        [Header("Events")]
        [SerializeField] private UnityEvent onPause = null;
        [SerializeField] private UnityEvent onResume = null;

        private void Start()
        {
            if (Time.timeScale != 1.0f)
                Time.timeScale = 1.0f;
        }

        private void Update()
        {
            SwitchTimeState();
        }

        private void SwitchTimeState()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                switch (Time.timeScale)
                {
                    case 1.0f:
                        // Pause.
                        pauseAudio.Play();
                        pauseScreen.SetActive(true);
                        Time.timeScale = 0.0f;

                        onPause.Invoke();
                        break;

                    case 0.0f:
                        // Unpause.
                        Time.timeScale = 1.0f;
                        resumeAudio.Play();
                        pauseScreen.SetActive(false);

                        onResume.Invoke();
                        break;
                }
            }
        }
    }
}