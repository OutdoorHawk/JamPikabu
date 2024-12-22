using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class AddGoldCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "Установить золото";
        public OrderType Order => OrderType.Third;

        public void Execute(string input)
        {
            IGroup<MetaEntity> storage = _metaContext.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Storage, MetaMatcher.Gold));

            foreach (MetaEntity metaEntity in storage) 
                metaEntity.ReplaceGold(int.Parse(input));

            _saveLoadService.SaveProgress();
        }
    }
}