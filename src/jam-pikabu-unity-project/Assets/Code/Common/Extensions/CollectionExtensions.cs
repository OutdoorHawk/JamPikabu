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
            int n = list.Count;

            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1); // случайный индекс от 0 до i
                (list[i], list[j]) = (list[j], list[i]); // обмен значений
            }
            return list;
        }
    }
}