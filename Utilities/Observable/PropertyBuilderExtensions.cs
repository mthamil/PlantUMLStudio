using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Observable
{
    /// <summary>
    /// Provides convenient extensions for <see cref="PropertyBuilder{T,V}"/>
    /// </summary>
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Specifies string comparison options.
        /// </summary>
        public static PropertyBuilder<T, string> EqualWhen<T>(this PropertyBuilder<T, string> builder, StringComparison comparisonOption)
        {
            builder.EqualWhen((x, y) => String.Equals(x, y, comparisonOption));
            return builder;
        }

        /// <summary>
        /// Specifies that a property's equality is determined using a sequence equality comparison.
        /// </summary>
        public static PropertyBuilder<T, TCollection> SequenceEqualWhen<T, TCollection, TItem>(this PropertyBuilder<T, TCollection> builder, IEqualityComparer<TItem> itemComparer = null) 
                where TCollection : IEnumerable<TItem>
        {
            if (itemComparer == null)
                itemComparer = EqualityComparer<TItem>.Default;

            builder.EqualWhen((x, y) => SequencesEqual(x, y, itemComparer));
            return builder;
        }

        private static bool SequencesEqual<TCollection, TItem>(TCollection x, TCollection y, IEqualityComparer<TItem> itemComparer) where TCollection : IEnumerable<TItem>
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.SequenceEqual(y, itemComparer);
        }
    }
}