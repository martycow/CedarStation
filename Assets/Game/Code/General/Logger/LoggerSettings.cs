using Cedar.Core;
using UnityEngine;

namespace Game.General
{
    [CreateAssetMenu(menuName = "Cedar Station/Tools/Create Logger Settings", fileName = "Settings_Logger")]
    public class LoggerSettings : ScriptableObject
    {
        [SerializeField] private Color[] primaryColors =
        {
            new Color(0.831f, 0.831f, 0.847f),
            new Color(0.376f, 0.647f, 0.980f),
            new Color(0.525f, 0.937f, 0.675f),
            new Color(0.753f, 0.518f, 0.988f),
            new Color(0.988f, 0.827f, 0.302f),
            new Color(0.984f, 0.573f, 0.275f),
            new Color(0.176f, 0.831f, 0.745f),
            new Color(0.957f, 0.447f, 0.714f),
            new Color(0.655f, 0.545f, 0.980f),
            new Color(0.404f, 0.906f, 0.976f),
            new Color(0.745f, 0.906f, 0.392f),
            new Color(0.992f, 0.729f, 0.455f),
            new Color(0.431f, 0.906f, 0.718f),
        };

        [SerializeField] private Color successColor = Color.lawnGreen;
        [SerializeField] private Color failColor = Color.softRed;
        [SerializeField] private Color warnColor = new Color(0.992f, 0.906f, 0.541f);
        [SerializeField] private Color errorColor = new Color(0.988f, 0.643f, 0.643f);

        public Color SuccessColor => successColor;
        public Color FailColor => failColor;
        public Color WarnColor => warnColor;
        public Color ErrorColor => errorColor;

        public Color GetPrimaryColor(SystemTag systemTag)
        {
            var index = (int)systemTag % primaryColors.Length;
            return primaryColors[index];
        }
    }
}