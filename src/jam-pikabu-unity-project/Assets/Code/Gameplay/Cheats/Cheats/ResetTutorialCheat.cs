using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class ResetTutorialCheat : BaseCheat, ICheatActionBasic
    {
        public string CheatLabel => "Reset Tutorial";
        
        public OrderType Order => OrderType.Penultimate;

        public void Execute()
        {
            _progressProvider.Progress.Tutorial.TutorialUserDatas.Clear();
            _saveLoadService.SaveProgress();
        }
    }
}