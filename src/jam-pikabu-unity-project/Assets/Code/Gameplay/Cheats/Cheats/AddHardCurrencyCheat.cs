using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class AddHardCurrencyCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "ADD HARD";
        public OrderType Order { get; }

        public void Execute(string input)
        {
            IGroup<MetaEntity> group = _metaContext.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Hard,
                    MetaMatcher.Storage));

            foreach (var storage in group.GetEntities())
            {
                storage.ReplaceHard(storage.Hard + int.Parse(input));
            }
            
            _saveLoadService.SaveProgress();
        }
    }
}