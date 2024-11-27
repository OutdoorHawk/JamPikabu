using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Common.Extensions.Animations
{
    public static class AnimationExtensions
    {
        private static readonly Dictionary<AnimationParameter, int> ParametersHashes = new();

        static AnimationExtensions()
        {
            foreach (AnimationParameter trigger in Enum.GetValues(typeof(AnimationParameter)))
                ParametersHashes.Add(trigger, Animator.StringToHash(trigger.ToString()));
        }
        
        public static int AsHash(this AnimationParameter parameter)
        {
            return ParametersHashes[parameter];
        }
    }
}