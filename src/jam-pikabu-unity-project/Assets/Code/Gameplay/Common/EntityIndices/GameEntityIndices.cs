using System.Collections.Generic;
using Entitas;
using Zenject;

namespace Code.Gameplay.Common.EntityIndices
{
    public class GameEntityIndices : IInitializable
    {
        private readonly GameContext _game;

        public const string StatusesOfType = "StatusesOfType";
        public const string StatChanges = "StatChanges";
        public const string InventorySlots = "InventorySlots";
        public const string ArmamentsOfProducer = "ArmamentsOfProducer";

        public GameEntityIndices(GameContext game)
        {
            _game = game;
        }

        public void Initialize()
        {
        }
        
    }

    public static class ContextIndicesExtensions
    {
        public static HashSet<GameEntity> TargetArmamentsOfProducer(this GameContext context, int producerId)
        {
            return ((EntityIndex<GameEntity, int>)context.GetEntityIndex(GameEntityIndices.ArmamentsOfProducer))
                .GetEntities(producerId);
        }
    }
}