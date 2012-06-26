using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public static class With
    {
        public static void A<T>(T item, Action<T> work)
        {
            work(item);
        }

        public static void Use<T>(this T item, Action<T> work)
        {
            work(item);
        }
    }
}
