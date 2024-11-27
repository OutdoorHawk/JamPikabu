using System;
using System.Collections.Generic;

namespace Code.Common.Extensions
{
    public static class FunctionalExtensions
    {
        public static T With<T>(this T self, Action<T> set)
        {
            set.Invoke(self);
            return self;
        }

        public static T With<T>(this T self, Action<T> apply, bool when)
        {
            if (when)
                apply?.Invoke(self);

            return self;
        }

        public static void RefreshList<T>(this List<T> list, IEnumerable<T> selected)
        {
            list.Clear();
            list.AddRange(selected);
        }
    }
}