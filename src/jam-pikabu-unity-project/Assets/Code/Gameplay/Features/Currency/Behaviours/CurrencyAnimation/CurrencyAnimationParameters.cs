using System;
using UnityEngine;

namespace Code.Gameplay.Features.Currency.Behaviours.CurrencyAnimation
{
    public struct CurrencyAnimationParameters
    {
        public int Count;
        public string TextPrefix;
        public CurrencyTypeId Type;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public GameObject LinkObject;
        public Action StartReplenishCallback;
    }
}