using System;
using Code.Gameplay.StaticData;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Gameplay.Features.Customers.Config
{
    [Serializable]
    public class CustomerSetup : BaseData
    {
        public bool IsBossCustomer;
        public Sprite Sprite;
    }
}