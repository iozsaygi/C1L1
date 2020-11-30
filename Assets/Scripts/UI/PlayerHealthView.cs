using C1L1.Core.Systems.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace C1L1.UI
{
    internal sealed class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private CombatController combatController = null;
        [SerializeField] private Slider healthBar = null;
        [SerializeField] private TextMeshProUGUI scoreText = null;

        private void Start()
        {
            healthBar.minValue = 0;
            healthBar.maxValue = combatController.MaximumHealth;
        }

        public void UpdateHealthView()
        {
            healthBar.value = combatController.CurrentHealth;
        }

        public void UpdateScoreView()
        {
            scoreText.text = combatController.Score.ToString();
        }
    }
}