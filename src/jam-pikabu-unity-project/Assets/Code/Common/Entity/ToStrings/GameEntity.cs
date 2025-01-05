using System;
using System.Linq;
using System.Text;
using Code.Common.Entity.ToStrings;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Currency;
using Code.Gameplay.Features.GameState;
using Code.Gameplay.Features.GrapplingHook;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.RoundState;
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
                    
                    case nameof(CurrencyStorage):
                        return new StringBuilder($"Gold Storage: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                            
                    case nameof(Loot):
                        return new StringBuilder($"Loot: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .With(s => s.Append($"Type:{LootTypeId} "), when: hasLootTypeId)
                            .ToString();
                    
                    case nameof(RoundStateController):
                        return new StringBuilder($"RoundStateController: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                    
                    case nameof(LootEffectsApplier):
                        return new StringBuilder($"LootEffectsApplier: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                    
                    case nameof(GameState):
                        return new StringBuilder($"GameState: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .ToString();
                    
                    case nameof(Ability):
                        return new StringBuilder($"Ability: ")
                            .With(s => s.Append($"Id:{Id} "), when: hasId)
                            .With(s => s.Append($"Type:{AbilityType} "), when: hasAbilityType)
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