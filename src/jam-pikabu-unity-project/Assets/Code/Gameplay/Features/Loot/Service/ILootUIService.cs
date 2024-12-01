using System;

namespace Code.Gameplay.Features.Loot.Service
{
    public interface ILootUIService
    {
        event Action OnLootUpdate;
    }
}