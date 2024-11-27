using System;
using UnityEngine;

namespace Code.Progress.Data
{
    [Serializable]
    public class PlayerFireamsProgress
    {
        [SerializeField] public int SlotsAvailable;
        [SerializeField] public int SlotsPurchased;

        public void UnlockFirearmSlot()
            => SlotsAvailable++;
        
        public void PurchaseFirearmSlot()
            => SlotsPurchased++;
    }
}