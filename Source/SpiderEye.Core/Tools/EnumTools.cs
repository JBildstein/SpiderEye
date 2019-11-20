using System;
using System.Collections.Generic;

namespace SpiderEye.Tools
{
    internal static class EnumTools
    {
        public static IEnumerable<T> GetFlags<T>(T input)
            where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (input.HasFlag(value)) { yield return value; }
            }
        }
    }
}
