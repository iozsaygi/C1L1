using UnityEngine;

namespace C1L1.Core.Systems.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Controller2D))]
    internal sealed class PlayerMotor : MonoBehaviour
    {
        private Animator animator = null;

        private void Start()
        {
            TryGetComponent(out animator);
        }

        public void UpdateFloatParameter(string parameterName, float value)
        {
            animator.SetFloat(parameterName, value);
        }

        public void UpdateBoolParameter(string parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }

        public void UpdateTriggerParameter(string parameterName)
        {
            animator.SetTrigger(parameterName);
        }
    }
}