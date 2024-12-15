using System.Collections.Generic;
using Code.Progress.SaveLoadService;
using Entitas;

namespace Code.Gameplay.Features.Currency.Systems
{
    public class TransferGoldToMetaStorageOnEndDay : ReactiveSystem<GameEntity>
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<MetaEntity> _metaGold;
        private readonly IGroup<GameEntity> _gold;

        public TransferGoldToMetaStorageOnEndDay(MetaContext context, GameContext gameContext, ISaveLoadService saveLoadService) : base(gameContext)
        {
            _saveLoadService = saveLoadService;
            _metaGold = context.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage,
                    MetaMatcher.Gold
                ));

            _gold = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.CurrencyStorage,
                    GameMatcher.Gold
                ));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(
                GameMatcher.GameState,
                GameMatcher.EndDay).Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isGameState && entity.isEndDay;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var metaStorage in _metaGold)
            foreach (var gold in _gold)
            {
                metaStorage.ReplaceGold(gold.Gold);
                _saveLoadService.SaveProgress();
            }
        }
    }
}