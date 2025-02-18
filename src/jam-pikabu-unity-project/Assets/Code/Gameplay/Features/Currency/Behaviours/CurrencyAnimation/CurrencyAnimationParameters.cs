using System;
using Code.Gameplay.Sound;
using UnityEngine;

namespace Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation
{
    public struct CurrencyAnimationParameters
    {
        public int Count;
        public string TextPrefix;
        public string AnimationName;
        public bool OverrideText;
        public CurrencyTypeId Type;
        public Sprite Sprite;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public GameObject LinkObject;
        public SoundTypeId BeginAnimationSound;
        public SoundTypeId StartReplenishSound;
        public Action StartReplenishCallback;
        public Action EachReplenishCallback;
    }
}