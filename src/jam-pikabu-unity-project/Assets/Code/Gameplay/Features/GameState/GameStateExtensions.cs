namespace Code.Gameplay.Features.GameState
{
    public static class GameStateExtensions
    {
        public static void ResetGameStates(this GameEntity entity)
        {
            entity.isEndDay = false;
            entity.isBeginDay = false;
            entity.isRoundPreparation = false;
            entity.isRoundLoop = false;
            entity.isRoundCompletion = false;
        }
    }
}