#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Common.Time.Editor
{
    public class TimeMenuEditor : EditorWindow
    {
        private float _timeScale;
        private ITimeService _timeService;

        /*
        [MenuItem("Tools/TimeMenu")]
        public static void ShowWindow()
        {
            var window = GetWindow<TimeMenuEditor>(false, "Time Menu", true);
            window.minSize = new Vector2(300, 100);
        }*/

        private void OnEnable()
        {
            // Устанавливаем текущее значение Time.timeScale при открытии окна
            _timeScale = UnityEngine.Time.timeScale;
            if (Application.isPlaying) 
                _timeService = FindFirstObjectByType<ProjectContext>().Container.Resolve<ITimeService>();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Adjust Time Scale", EditorStyles.boldLabel);

            _timeScale = EditorGUILayout.Slider("Time Scale", _timeScale, 0f, 10f);

            if (!Mathf.Approximately(UnityEngine.Time.timeScale, _timeScale))
            {
                UnityEngine.Time.timeScale = _timeScale;
            }

            // Кнопка сброса
            if (GUILayout.Button("Reset Time Scale"))
            {
                _timeScale = 1f;
                UnityEngine.Time.timeScale = 1f;
            }
        }

        private void OnDisable()
        {
            UnityEngine.Time.timeScale = Mathf.Clamp(_timeScale, 0f, 10f);
        }
    }
}
#endif