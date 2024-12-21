using System;

namespace Code.Gameplay.Features.Orders.Config
{
    [Flags]
    public enum OrderTag
    {
        None = 0,
        Tutorial = 1 << 1,
        Common = 1 << 2,
        Boss = 1 << 4,
    }
}