using System;

namespace Code.Gameplay.Windows.Configs
{
    [Serializable]
    public class WindowConfig
    {
        public WindowTypeId Type;
        public string WindowName;
        public BaseWindow WindowPrefab;
    }
}