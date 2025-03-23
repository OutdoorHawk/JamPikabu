using Code.Gameplay.Features.GrapplingHook.Configs;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class ApplyGrapplingHookScaleSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _statOwner;
        private readonly GrapplingHookStaticData _config;

        public ApplyGrapplingHookScaleSystem(GameContext context, IStaticDataService staticDataService)
        {
            _config = staticDataService.Get<GrapplingHookStaticData>();
            _statOwner = context.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.BaseStats,
                    GameMatcher.StatModifiers,
                    GameMatcher.Scale,
                    GameMatcher.GrapplingHookBehaviour));
        }

        public void Execute()
        {
            foreach (var entity in _statOwner)
            {
                entity.ReplaceCollectLootRaycastRadius(_config.CollectLootRaycastRadius * entity.Scale);
                entity.GrapplingHookBehaviour.ApplyScaleVisual(entity.Scale);
            }
        }
    }
}