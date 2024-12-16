using Code.Meta.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Meta.UI.Shop.Templates.UpgradeLoot
{
    public class UpgradeLootShopItem : MonoBehaviour
    {
        public Image Icon;
        public TMP_Text Name;
        public Button UpgradeButton;
        public PriceInfo UpgradePrice;
        public PriceInfo RatingFrom;
        public PriceInfo RatingTo;
    }
}