using System.Collections.Generic;
using Code.Common;
using Code.Gameplay.Features.CharacterStats;
using Code.Gameplay.Features.CharacterStats.Indexing;
using Entitas;
using Zenject;

namespace Code.Gameplay.Common.EntityIndices
{
    public class GameEntityIndices : IInitializable
    {
        private readonly GameContext _game;

        public const string StatusesOfType = "StatusesOfType";
        public const string StatChanges = "StatChanges";
        public const string ArmamentsOfProducer = "ArmamentsOfProducer";

        public GameEntityIndices(GameContext game)
        {
            _game = game;
        }

        public void Initialize()
        {
            _game.AddEntityIndex(new EntityIndex<GameEntity, StatKey>(
                name: StatChanges,
                _game.GetGroup(GameMatcher.AllOf(
                    GameMatcher.StatChange,
                    GameMatcher.Target)),
                getKey: GetTargetStatKey,
                new StatKeyEqualityComparer()));
        }

        private StatKey GetTargetStatKey(GameEntity entity, IComponent component)
        {
            return new StatKey(
                (component as Target)?.Value ?? entity.Target,
                (component as StatChange)?.Value ?? entity.StatChange);
        }
    }

    public static class ContextIndicesExtensions
    {
        public static HashSet<GameEntity> TargetArmamentsOfProducer(this GameContext context, int producerId)
        {
            return ((EntityIndex<GameEntity, int>)context.GetEntityIndex(GameEntityIndices.ArmamentsOfProducer))
                .GetEntities(producerId);
        }

        public static HashSet<GameEntity> TargetStatChanges(this GameContext context, Stats stat, int targetId)
        {
            return ((EntityIndex<GameEntity, StatKey>)context.GetEntityIndex(GameEntityIndices.StatChanges))
                .GetEntities(new StatKey(targetId, stat));
        }
    }
}