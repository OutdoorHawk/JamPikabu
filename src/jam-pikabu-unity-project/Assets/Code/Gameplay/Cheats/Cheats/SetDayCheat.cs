using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Entitas;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class SetDayCheat : BaseCheat, ICheatActionInputString
    {
        public string CheatLabel => "Пройти дней";
        public OrderType Order => OrderType.Third;

        public void Execute(string input)
        {
            IGroup<MetaEntity> days = _metaContext.GetGroup(MetaMatcher.AllOf(
                MetaMatcher.Day));

            for (int i = 1; i <= int.Parse(input); i++)
            {
                MetaEntity day = TryFindExistingDay(days, i) ?? CreateNewDayProgressEntity(i);
                day.ReplaceStarsAmount(3);
            }

            _saveLoadService.SaveProgress();
        }

        private MetaEntity TryFindExistingDay(IGroup<MetaEntity> days, int i)
        {
            foreach (MetaEntity day in days)
            {
                if (day.Day != i)
                    continue;

                return day;
            }

            return null;
        }

        private MetaEntity CreateNewDayProgressEntity(int i)
        {
            return CreateMetaEntity.Empty()
                .With(x => x.AddDay(i))
                .AddStarsAmount(3);
        }
    }
}