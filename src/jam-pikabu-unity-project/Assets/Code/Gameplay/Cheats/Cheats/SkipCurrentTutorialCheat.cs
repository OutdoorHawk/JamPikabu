using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class SkipCurrentTutorialCheat : BaseCheat, ICheatActionBasic
    {
        private ITutorialService _tutorialService;
        public string CheatLabel => "Skip Current Tutorial";
        
        public OrderType Order => OrderType.Penultimate;

        public void Execute()
        {
            _tutorialService.SkipCurrentTutorial();
        }

        [Inject]
        private void Construct(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }
    }
}