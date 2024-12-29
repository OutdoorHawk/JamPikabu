using System.Collections.Generic;
using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Tutorial.Config;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Code.Progress.Data.Tutorial;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class CompleteAllTutorialCheat : BaseCheat, ICheatActionBasic
    {
        private ITutorialService _tutorialService;
        public string CheatLabel => "Complete All Tutorial";
        
        public OrderType Order => OrderType.Penultimate;

        [Inject]
        private void Construct(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        public void Execute()
        {
            List<TutorialUserData> tutorialUserDatas = _progressProvider.Progress.Tutorial.TutorialUserDatas;
            tutorialUserDatas.Clear();
            
            foreach (var tutorials in _staticDataService.Get<TutorialStaticData>().Configs)
            {
                tutorialUserDatas.Add(new TutorialUserData
                {
                    TypeInt = (int)tutorials.Type,
                    Completed = true
                });
            }
          
            _saveLoadService.SaveProgress();
            _tutorialService.Initialize();
            _tutorialService.SkipCurrentTutorial();
        }
    }
}