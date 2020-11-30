using TMPro;
using UnityEngine;

namespace C1L1.UI
{
    public class FadingText : MonoBehaviour
    {
        public TextMeshProUGUI Text;

        public void Modify(string msg, float fontSize, Color color)
        {
            Text.text = msg;
            Text.fontSize = fontSize;
            Text.color = color;
        }
    }
}