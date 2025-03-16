using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.ABTesting;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using UnityEngine;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class AbTestCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "Установить аб тест";
        public OrderType Order => OrderType.Penultimate;

        public void Execute(string input)
        {
            string[] inputs = input.Split(" ");

            var tagId = (ExperimentTagTypeId)int.Parse(inputs[0]);
            var tagValueId = (ExperimentValueTypeId)int.Parse(inputs[1]);

            PlayerPrefs.SetString(tagId.ToString(), tagValueId.ToString());
            PlayerPrefs.Save();
        }
    }
}