#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public abstract partial class BaseStaticData
    {
        /*[Button(nameof(SendValuesToProd))]
        private void SendValuesToProd()
        {
            SendTo("Prod");
        }
        
        [Button(nameof(GetValuesFromProd))]
        private void GetValuesFromProd()
        {
            GetFrom("Prod");
        }

        [Button(nameof(SendValuesToDev))]
        private void SendValuesToDev()
        {
            SendTo("Dev");
        }

        [Button(nameof(GetValuesFromDev))]
        private void GetValuesFromDev()
        {
            GetFrom("Dev");
        }*/

        private void SendTo(string path)
        {
            string fileName = name;
            string resourcePath = $"Configs/{path}/{fileName}"; // Замените на актуальный путь

            var otherSO = Resources.Load<BaseStaticData>(resourcePath);
            if (otherSO != null)
            {
                CopyFieldsFrom(otherSO);
                Debug.Log($"Values copied from {otherSO.name} to {name}");
            }
            else
            {
                Debug.LogError($"Could not find ScriptableObject at path: {resourcePath}");
            }
        }

        private void GetFrom(string path)
        {
            string fileName = name;
            string resourcePath = $"Configs/{path}/{fileName}"; // Замените на актуальный путь

            var targetSO = Resources.Load<BaseStaticData>(resourcePath);
            if (targetSO != null)
            {
                targetSO.CopyFieldsFrom(this);
                Debug.Log($"Values pasted into {targetSO.name}");
            }
            else
            {
                Debug.LogError($"Could not find ScriptableObject at path: {resourcePath}");
            }
        }

        private void CopyFieldsFrom(BaseStaticData source)
    {
        if (source == null) return;

        Type type = GetType();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            // Копируем только сериализуемые поля
            if (field.IsDefined(typeof(SerializeField), true) || field.IsPublic)
            {
                var value = field.GetValue(source);

                // Если поле - массив или список, пересоздаем его
                if (field.FieldType.IsArray)
                {
                    var elementType = field.FieldType.GetElementType();
                    if (elementType != null && IsSerializableType(elementType))
                    {
                        var array = (Array)value;
                        if (array != null)
                        {
                            var clonedArray = Array.CreateInstance(elementType, array.Length);
                            Array.Copy(array, clonedArray, array.Length);
                            field.SetValue(this, clonedArray);
                        }
                    }
                }
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var elementType = field.FieldType.GetGenericArguments()[0];
                    if (IsSerializableType(elementType))
                    {
                        var list = (IList)value;
                        if (list != null)
                        {
                            var clonedList = (IList)Activator.CreateInstance(field.FieldType);
                            foreach (var item in list)
                            {
                                clonedList.Add(item);
                            }
                            field.SetValue(this, clonedList);
                        }
                    }
                }
                else if (IsSerializableType(field.FieldType))
                {
                    // Простые типы или классы, которые могут быть сериализованы
                    field.SetValue(this, value);
                }
            }
        }
    }

    private bool IsSerializableType(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || typeof(UnityEngine.Object).IsAssignableFrom(type) || type.IsSerializable;
    }
    }
}
#endif