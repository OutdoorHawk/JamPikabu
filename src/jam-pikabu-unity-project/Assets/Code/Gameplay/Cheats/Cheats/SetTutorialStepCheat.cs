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
   
    public class SetTutorialStepCheat : BaseCheat, ICheatActionInputString
    {
        private ITutorialService _tutorialService;
        public string CheatLabel => "Установить шаг туториала";

        public OrderType Order => OrderType.Penultimate;

        [Inject]
        private void Construct(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        public void Execute(string input)
        {
            int step = int.Parse(input);
            
            _progressProvider.Progress.Tutorial.TutorialUserDatas.Clear();

            foreach (TutorialConfig config in _staticDataService.Get<TutorialStaticData>().Configs)
            {
                if (step < (int)config.Type)
                    continue;

                List<TutorialUserData> userData = _progressProvider.Progress.Tutorial.TutorialUserDatas;
                
                userData.Add(new TutorialUserData
                {
                    TypeInt = (int)config.Type,
                    Completed = true,
                });
            }

            _tutorialService.SkipCurrentTutorial();
            _saveLoadService.SaveProgress();
        }
    }
}