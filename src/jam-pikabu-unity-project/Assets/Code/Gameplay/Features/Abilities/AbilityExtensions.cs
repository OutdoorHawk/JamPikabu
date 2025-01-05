using System;
using System.Collections.Generic;
using Code.Common;
using RoyalGold.Sources.Scripts.Game.MVC.Utils;

namespace Code.Gameplay.Features.Abilities
{
    public static class AbilityExtensions
    {
        private static GameContext GameContext => Contexts.sharedInstance.game;

        public static GameEntity Target(this GameEntity effect)
        {
            return effect.hasTarget
                ? GameContext.GetEntityWithId(effect.Target)
                : null;
        }

        public static GameEntity GetRandomLoot(GameEntity target, List<GameEntity> buffer)
        {
            buffer.ShuffleList();
            buffer.Remove(target);

            foreach (GameEntity loot in buffer)
            {
                if (loot.IsNullOrDestructed())
                    continue;
                
                return loot;
            }

            return null;
        }
        
        public static GameEntity GetRandomLoot(GameEntity target, List<GameEntity> buffer, Func<GameEntity, bool> predicate)
        {
            buffer.ShuffleList();  
            buffer.Remove(target);

            foreach (GameEntity loot in buffer)
            {
                if (loot.IsNullOrDestructed())
                    continue;
                
                if (predicate(loot))
                {
                    return loot;
                }
            }

            return null;
        }
    }
}