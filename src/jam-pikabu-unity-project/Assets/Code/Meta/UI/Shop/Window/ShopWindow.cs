using Code.Gameplay.Windows;
using Code.Progress.SaveLoadService;
using Zenject;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopWindow : BaseWindow
    {
        public ShopTabsContainer TabsContainer;
        
        private ISaveLoadService _saveLoadService;

        [Inject]
        private void Construct(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            TabsContainer.Init();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _saveLoadService.SaveProgress();
        }
        
    }
}