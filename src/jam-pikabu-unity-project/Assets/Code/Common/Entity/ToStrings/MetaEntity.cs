using System;
using System.Linq;
using System.Text;
using Code.Common.Entity.ToStrings;
using Code.Common.Extensions;
using Code.Meta.Features.LootCollection;
using Code.Meta.Features.MapBlocks;
using Code.Meta.UI.Shop;
using Entitas;
using UnityEngine;

// ReSharper disable once CheckNamespace
public sealed partial class MetaEntity : INamedEntity
{
    private EntityPrinter _printer;

    public override string ToString()
    {
        if (_printer == null)
            _printer = new EntityPrinter(this);

        _printer.InvalidateCache();

        return _printer.BuildToString();
    }

    public string EntityName(IComponent[] components)
    {
        try
        {
            if (components.Length == 1)
                return components[0].GetType().Name;

            foreach (IComponent component in components)
            {
                switch (component.GetType().Name)
                {
                    case nameof(Code.Meta.Features.Days.Day):
                        return PrintDay();
                    case nameof(Purchased):
                        return PrintPurchasedItem();
                    case nameof(LootFreeUpgradeTimer):
                        return PrintLootFreeUpgradeTimer();
                    case nameof(LootProgression):
                        return PrintLootProgression();
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        return components.First().GetType().Name;
    }

    private string PrintDay()
    {
        return new StringBuilder($"Completed Day")
            .With(s => s.Append($" : {Day.ToString()} "))
            .With(s => s.Append($" : {StarsAmount.ToString()} "))
            .ToString();
    }

    private string PrintPurchasedItem()
    {
        return new StringBuilder($"Purchased ")
            .With(s => s.Append($" shopItem: {ShopItemId.ToString()} "), when: hasShopItemId)
            .ToString();
    }

    private string PrintLootFreeUpgradeTimer()
    {
        return new StringBuilder($"FreeUpgradeTimer ")
            .With(s => s.Append($" Type: {LootTypeId.ToString()} "))
            .ToString();
    }

    private string PrintLootProgression()
    {
        return new StringBuilder($"LootProgression ")
            .With(s => s.Append($" Type: {LootTypeId.ToString()} "))
            .ToString();
    }

    public string BaseToString() => base.ToString();
}