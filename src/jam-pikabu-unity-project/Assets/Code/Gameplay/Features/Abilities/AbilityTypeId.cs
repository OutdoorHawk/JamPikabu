namespace Code.Gameplay.Features.Abilities
{
    public enum AbilityTypeId
    {
        None = 0,
        /// <summary>
        /// Повышенная прыгучесть
        /// </summary>
        Bouncy = 1,
        /// <summary>
        /// Смена позиций
        /// </summary>
        SwapPositions = 2,
        /// <summary>
        /// Изменение размеров ингредиентов
        /// </summary>
        ChangeSizesIngredient = 3,
        /// <summary>
        /// Прилипание к крюку
        /// </summary>
        StickyToHook = 4,
        /// <summary>
        /// Изменение скорости крюка
        /// </summary>
        HookSpeedChange = 5,
        /// <summary>
        /// Подбор случайного лута
        /// </summary>
        PickupRandomLoot = 6,
        /// <summary>
        /// Можно подобрать только один лут такого типа
        /// </summary>
        SinglePickup = 7, 
        /// <summary>
        /// Можно подобрать только вместе с чем-то еще
        /// </summary>
        MultiPickup = 8,
        /// <summary>
        /// Очень тяжелый, замедляет крюк
        /// </summary>
        HeavyObject = 9,
        /// <summary>
        /// Увеличение размера крюка
        /// </summary>
        IncreaseHookSize = 10,
        /// <summary>
        /// Уменьшение размера крюка
        /// </summary>
        DecreaseHookSize = 11,
    }
}