using System;

namespace Code.Gameplay.Features.Orders.Config
{
    [Flags]
    public enum OrderTag
    {
        None = 0,          // 00000000
        Tutorial = 1 << 0, // 00000001 (1)
        Common = 1 << 1,   // 00000010 (2)
        Boss = 1 << 2,     // 00000100 (4)
        Easy = 1 << 3,     // 00001000 (8)
        Medium = 1 << 4,   // 00010000 (16)
        Hard = 1 << 5,     // 00100000 (32)
    }
}