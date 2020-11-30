using UnityEngine;
using UnityEngine.Events;

namespace C1L1.Core.Systems.AI
{
    [DisallowMultipleComponent]
    internal sealed class PantherChargeTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPlayerEntersRange = null;
        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !hasTriggered)
            {
                onPlayerEntersRange.Invoke();
                hasTriggered = true;
            }
        }
    }
}