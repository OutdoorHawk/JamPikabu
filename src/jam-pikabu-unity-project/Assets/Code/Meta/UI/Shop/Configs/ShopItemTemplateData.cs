using System;
using Code.Gameplay.StaticData.Data;
using UnityEngine;

namespace Code.Meta.UI.Shop.Configs
{
    [Serializable]
    public class ShopItemTemplateData : BaseData
    {
        public ShopItemKind Kind;
        public GameObject Prefab;
    }
}