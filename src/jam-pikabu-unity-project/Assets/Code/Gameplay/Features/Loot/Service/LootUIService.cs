using System;

namespace Code.Gameplay.Features.Loot.Service
{
    public class LootUIService : ILootUIService
    {
        public event Action OnLootUpdate;
    }
}