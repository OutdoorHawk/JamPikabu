using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using Code.Gameplay.Windows;
using Code.Progress.SaveLoadService;
using Zenject;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopWindow : BaseWindow
    {
        public ShopTabsContainer TabsContainer;
        
        private ISaveLoadService _saveLoadService;
        private ISoundService _soundService;

        [Inject]
        private void Construct(ISaveLoadService saveLoadService, ISoundService soundService)
        {
            _soundService = soundService;
            _saveLoadService = saveLoadService;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            TabsContainer.Init();
            _soundService.PlayOneShotSound(SoundTypeId.OpenShop);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _saveLoadService.SaveProgress();
        }
        
    }
}