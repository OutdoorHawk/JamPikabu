using Code.Gameplay.Features.Currency.Behaviours;
using Code.Gameplay.Features.Orders.Behaviours;
using Code.Gameplay.Windows;

namespace Code.Gameplay.Features.HUD
{
    public class PlayerHUDWindow : BaseWindow
    {
        public OrderViewBehaviour OrderViewBehaviour;
        public CurrencyHolder CurrencyHolder;
    }
}