using System;
using Code.Gameplay.StaticData.Data;
using Code.Meta.UI.Shop.Items;
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