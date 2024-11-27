using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Common.Animations
{
    public class AnimationEventProvider : MonoBehaviour
    {
        private readonly Dictionary<string, Action> _animationEvents = new();

        public void RegisterToEvent(string key, Action eventCallback)
        {
            _animationEvents.Add(key, eventCallback);
        }

        public void OnAnimationEvent(string key)
        {
            if (_animationEvents.TryGetValue(key, out Action callback) == false)
            {
                Debug.LogWarning($"Animation event with key: {key} not found!");
                return;
            }

            callback?.Invoke();
        }
    }
}