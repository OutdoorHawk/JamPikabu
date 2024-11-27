using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Windows.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(WindowsStaticData), fileName = "Windows")]
    public class WindowsStaticData : ScriptableObject
    {
        [TabGroup("Windows"), SerializeField] private WindowConfig[] _configs;

        [TabGroup("Windows"), SerializeField] private RectTransform _uiRoot;

        [TabGroup("Data"), SerializeField] private ResultTexts _resultTexts;
        [TabGroup("Data"), SerializeField] private float _tutorialDuration = 12;
        
        public RectTransform UIRoot => _uiRoot;
        public float TutorialDuration => _tutorialDuration;
        public ResultTexts ResultTexts => _resultTexts;
        public WindowConfig[] Configs => _configs;
    }
}