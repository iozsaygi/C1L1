using UnityEngine;
using UnityEngine.Events;

namespace C1L1.Utilities
{
    internal sealed class AnimationEventExposer : MonoBehaviour
    {
        [SerializeField] private UnityEvent onAnimationEnds = null;

        public void TriggerAnimationEndEvent()
        {
            onAnimationEnds.Invoke();
        }

        public void DestroyMe()
        {
            Destroy(this.gameObject);
        }
    }
}
