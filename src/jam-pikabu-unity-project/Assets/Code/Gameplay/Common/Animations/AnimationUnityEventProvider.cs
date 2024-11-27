using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Gameplay.Common.Animations
{
    public class AnimationUnityEventProvider : MonoBehaviour
    {
        public AnimationEventData[] AnimationEvents;

        private readonly Dictionary<string, UnityEvent> _animationEvents = new();

        private void Awake()
        {
            foreach (var animationEvent in AnimationEvents)
            {
                _animationEvents[animationEvent.EventName] = animationEvent.Event;
            }
        }

        public void OnAnimationEvent(string key)
        {
            if (_animationEvents.TryGetValue(key, out UnityEvent callback) == false)
            {
                Debug.LogWarning($"Animation event with key: {key} not found!");
                return;
            }

            callback?.Invoke();
        }
    }
}