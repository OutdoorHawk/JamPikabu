namespace Code.Meta.Features.Consumables.Data
{
    public readonly struct PurchasedConsumableData
    {
        public readonly ConsumableTypeId Type;
        public readonly int Amount;
        public readonly int ExpirationTime;

        public PurchasedConsumableData
        (
            ConsumableTypeId type,
            int amount = 0,
            int expirationTime = 0
        )
        {
            Type = type;
            Amount = amount;
            ExpirationTime = expirationTime;
        }
        
        public PurchasedConsumableData SetAmount(int newAmount)
        {
            return new PurchasedConsumableData(Type, newAmount, ExpirationTime);
        }
    }
}