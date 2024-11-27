using System;
using UnityEngine.Events;

namespace Code.Gameplay.Common.Animations
{
    [Serializable]
    public struct AnimationEventData
    {
        public string EventName;
        public UnityEvent Event;
    }
}