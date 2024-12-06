using System;
using System.Collections.Generic;

namespace RoyalGold.Sources.Scripts.Game.MVC.Utils
{
    public static class CollectionExtensions
    {
        public static void AddSafe<T>(this ICollection<T> list, T element)
        {
            if (list.Contains(element) == false) 
                list.Add(element);
        }

        public static void RemoveSafe<T>(this ICollection<T> list, T element)
        {
            if (list.Contains(element)) 
                list.Remove(element);
        }
        
        public static List<T> ShuffleList<T>(this List<T> list)
        {
            Random rng = new Random();
            list.Sort((_, _) => rng.Next(-1, 2));
            return list;
        }
    }
}