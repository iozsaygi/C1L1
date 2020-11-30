using C1L1.Core.Systems.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace C1L1.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Slider))]
    public class BossHealthView : MonoBehaviour
    {
        [SerializeField] private Combatable combatable = null;
        private Slider slider = null;

        private void Start()
        {
            TryGetComponent(out slider);

            slider.maxValue = combatable.MaximumHealth;
            slider.value = combatable.MaximumHealth;
            slider.minValue = 0;
        }

        public void UpdateHealthView()
        {
            slider.value = combatable.CurrentHealth;
        }
    }
}