// -----------------------------------------------------------------------
// <copyright file="ListIndexerGet.cs" company="MagicKitchen">
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
    public class ListIndexerGet<T_Value>
    {
        internal ListIndexerGet(
            DictIndexerGet<decimal, T_Value> dictIndexerGet)
        {
            this.AsDict = dictIndexerGet;
        }

        /// <summary>
        /// Gets DictIndexer with decimal indexer for key definition.
        /// </summary>
        public DictIndexerGet<decimal, T_Value> AsDict { get; }

        /// <summary>
        /// Indexer for list.
        /// </summary>
        /// <param name="indexKey">The string key for the list.</param>
        /// <returns>The translated list.</returns>
        public List<T_Value> this[string indexKey]
        {
            get => this.AsDict[indexKey].OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }
    }
}
