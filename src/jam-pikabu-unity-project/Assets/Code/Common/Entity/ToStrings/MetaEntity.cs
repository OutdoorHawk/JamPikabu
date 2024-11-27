using System;
using System.Linq;
using System.Text;
using Code.Common.Entity.ToStrings;
using Code.Common.Extensions;
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
                    case nameof(Purchased):
                        return PrintPurchasedItem();
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        return components.First().GetType().Name;
    }

    private string PrintPurchasedItem()
    {
        return new StringBuilder($"Purchased ")
            .With(s => s.Append($" shopItem: {ShopItemId.ToString()} "), when: hasShopItemId)
            .ToString();
    }

    public string BaseToString() => base.ToString();
}