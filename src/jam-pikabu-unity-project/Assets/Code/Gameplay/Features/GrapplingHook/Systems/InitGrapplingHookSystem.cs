using Code.Gameplay.Features.GrapplingHook.Factory;
using Code.Infrastructure.SceneContext;
using Entitas;

namespace Code.Gameplay.Features.GrapplingHook.Systems
{
    public class InitGrapplingHookSystem : IInitializeSystem
    {
        private readonly ISceneContextProvider _provider;
        private readonly IGrapplingHookFactory _grapplingHookFactory;

        public InitGrapplingHookSystem(ISceneContextProvider provider, IGrapplingHookFactory grapplingHookFactory)
        {
            _grapplingHookFactory = grapplingHookFactory;
            _provider = provider;
        }

        public void Initialize()
        { 
            _grapplingHookFactory.CreateGrapplingHook(_provider.Context.HookSpawnPoint);
        }
    }
}