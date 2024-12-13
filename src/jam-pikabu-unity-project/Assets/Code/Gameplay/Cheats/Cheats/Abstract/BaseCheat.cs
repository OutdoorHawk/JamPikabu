using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Service;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats.Abstract
{
    public abstract class BaseCheat
    {
        protected GameContext _gameContext;
        protected ISaveLoadService _saveLoadService;
        protected IProgressProvider _progressProvider;
        protected MetaContext _metaContext;
        protected IStaticDataService _staticDataService;
        protected IWindowService _windowService;

        [Inject]
        private void Construct(GameContext gameContext, MetaContext metaContext, ISaveLoadService saveLoadService, 
            IProgressProvider progressProvider, IStaticDataService staticDataService, IWindowService windowService)
        {
            this._windowService = windowService;
            _staticDataService = staticDataService;
            _metaContext = metaContext;
            _progressProvider = progressProvider;
            _saveLoadService = saveLoadService;
            _gameContext = gameContext;
        }
    }
}