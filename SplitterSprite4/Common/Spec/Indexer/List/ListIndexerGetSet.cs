// -----------------------------------------------------------------------
// <copyright file="ListIndexerGetSet.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.List
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict;

    /// <summary>
    /// Indexer class for list type value in spec file.
    /// </summary>
    /// <typeparam name="T_Value">Type of list value.</typeparam>
    public class ListIndexerGetSet<T_Value> : ListIndexerGet<T_Value>
    {
        internal ListIndexerGetSet(
            DictIndexerGetSet<decimal, T_Value> dictIndexerGetSet)
            : base(dictIndexerGetSet)
        {
            this.AsDict = dictIndexerGetSet;
        }

        /// <summary>
        /// Gets DictIndexer with decimal indexer for key definition.
        /// </summary>
        public new DictIndexerGetSet<decimal, T_Value> AsDict { get; }

        /// <summary>
        /// Indexer for list.
        /// </summary>
        /// <param name="indexKey">The string key for the list.</param>
        /// <returns>The translated list.</returns>
        public new List<T_Value> this[string indexKey]
        {
            get => base[indexKey];

            set
            {
                this.AsDict[indexKey] =
                    value.Select((x, i) => (x, i)).ToDictionary(xi => (decimal)xi.i, xi => xi.x);
            }
        }
    }
}
