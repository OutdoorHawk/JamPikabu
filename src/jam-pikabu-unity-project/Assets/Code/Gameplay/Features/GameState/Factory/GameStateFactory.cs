using System;
using Code.Common.Entity;
using Code.Common.Extensions;

namespace Code.Gameplay.Features.GameState.Factory
{
    public class GameStateFactory : IGameStateFactory
    {
        public void CreateSwitchGameStateRequest(GameStateTypeId newState)
        {
            var request = CreateGameEntity
                    .Empty()
                    .With(x => x.isSwitchGameStateRequest = true)
                    .AddGameStateTypeId(newState)
                ;

            switch (newState)
            {
                case GameStateTypeId.Unknown:
                    break;
                case GameStateTypeId.BeginDay:
                    request.With(x => x.isBeginDay = true);
                    break;
                case GameStateTypeId.RoundPreparation:
                    request.With(x => x.isRoundPreparation = true);
                    break;
                case GameStateTypeId.RoundLoop:
                    request.With(x => x.isRoundLoop = true);
                    break;
                case GameStateTypeId.RoundCompletion:
                    request.With(x => x.isRoundCompletion = true);
                    break;
                case GameStateTypeId.EndDay:
                    request.With(x => x.isEndDay = true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }
}