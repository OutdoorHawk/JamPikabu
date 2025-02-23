using System.Collections.Generic;
using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Tutorial.Config;
using Code.Gameplay.Tutorial.Service;
using Code.Infrastructure.States.GameStateHandler;
using Code.Progress.Data.Tutorial;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    public class SetTutorialStepCheat : BaseCheat, ICheatActionInputString
    {
        private ITutorialService _tutorialService;
        public string CheatLabel => "Сбросить шаг туториала";

        public OrderType Order => OrderType.Penultimate;

        [Inject]
        private void Construct(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        public void Execute(string input)
        {
            int step = int.Parse(input);

            List<TutorialUserData> userData = _progressProvider.Progress.Tutorial.TutorialUserDatas;
            TutorialConfig config = _staticDataService.Get<TutorialStaticData>().Configs.Find(data => data.Order == step);

            if (config == null)
                return;

            TutorialUserData find = userData.Find(data => data.Type == config.Type);

            if (find == null)
            {
                userData.Add(new TutorialUserData
                {
                    TypeInt = (int)config.Type,
                    Completed = false,
                });
            }
            else
            {
                find.Completed = false;
            }

            _saveLoadService.SaveProgress();
            _tutorialService.Initialize();
        }

        /*public void Execute(string input)
        {
            int step = int.Parse(input);

            _progressProvider.Progress.Tutorial.TutorialUserDatas.Clear();

            foreach (TutorialConfig config in _staticDataService.Get<TutorialStaticData>().Configs)
            {
                if (step < config.Order)
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
        }*/
    }
}