using System;
using System.Linq;
using System.Text;
using Code.Common.Entity.ToStrings;
using Code.Common.Extensions;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.Loot;
using Entitas;
using UnityEngine;

// ReSharper disable once CheckNamespace
public sealed partial class GameEntity : INamedEntity
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
                    case nameof(GrapplingHook):
                        return new StringBuilder($"Hook ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                    
                    case nameof(Code.Gameplay.Features.Currency.Gold):
                        return new StringBuilder($"Gold Storage: ")
                            .With(s => s.Append($"Amount: {Gold} "), when: hasGold)
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                            
                    case nameof(Loot):
                        return new StringBuilder($"Loot: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
             
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        return components.First().GetType().Name;
    }

    public string BaseToString() => base.ToString();
}