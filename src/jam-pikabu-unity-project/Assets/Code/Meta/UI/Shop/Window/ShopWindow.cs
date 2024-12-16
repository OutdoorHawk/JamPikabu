using Code.Gameplay.Windows;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopWindow : BaseWindow
    {
        public ShopTabsContainer TabsContainer;


        protected override void Initialize()
        {
            base.Initialize();
            TabsContainer.Init();
        }
    }
}