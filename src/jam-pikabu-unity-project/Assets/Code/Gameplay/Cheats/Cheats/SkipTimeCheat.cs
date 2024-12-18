using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using UnityEngine;
using Zenject;
using static Code.Gameplay.Common.Time.UnityTimeService;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class SkipTimeCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "Промотать время (минут)";
        public OrderType Order => OrderType.Third;

        private ITimeService _timeService;

        [Inject]
        private void Construct(ITimeService timeService)
        {
            _timeService = timeService;
        }

        public void Execute(string input)
        {
            int secondsOffset = int.Parse(input) * 60;
            _timeService.TimeOffset += secondsOffset;
            PlayerPrefs.SetInt(CheatTimeOffsetKey, _timeService.TimeOffset);
        }
    }
}