using System;
using UnityEngine;

namespace Code.Progress.Data
{
    [Serializable]
    public class PlayerConstructionsProgress
    {
        [SerializeField] public int SlotsAvailable;
        [SerializeField] public int SlotsPurchased;

        public void UnlockConstructionSlot()
            => SlotsAvailable++;
        
        public void PurchaseConstructionSlot()
            => SlotsPurchased++;
    }
}