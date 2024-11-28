using System;
using System.Collections.Generic;
using Code.Gameplay.StaticData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.Windows.Configs
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(WindowsStaticData), fileName = "Windows")]
    public class WindowsStaticData : BaseStaticData
    {
        [TabGroup("Windows"), SerializeField] private WindowConfig[] _configs;
        [TabGroup("Windows"), SerializeField] private RectTransform _uiRoot;

        private readonly Dictionary<WindowTypeId, WindowConfig> _windows = new();
        
        public RectTransform UIRoot => _uiRoot;

        public override void OnConfigInit()
        {
            base.OnConfigInit();
            
            foreach (var config in _configs)
                _windows.Add(config.Type, config);
        }

        public WindowConfig GetWindow(WindowTypeId type)
        {
            return _windows[type];
        }
    }
}