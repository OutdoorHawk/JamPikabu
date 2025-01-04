namespace Code.Gameplay.Features.Abilities
{
    public static class AbilityExtensions
    {
        private static GameContext GameContext => Contexts.sharedInstance.game;
        
        public static GameEntity Target(this GameEntity effect)
        {
            return effect.hasTarget
                ? GameContext.GetEntityWithId(effect.Target)
                : null;
        }
    }
}