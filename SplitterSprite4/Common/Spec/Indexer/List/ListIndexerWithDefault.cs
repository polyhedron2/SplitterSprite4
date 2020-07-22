// -----------------------------------------------------------------------
// <copyright file="ListIndexerWithDefault.cs" company="MagicKitchen">
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
    /// <typeparam name="T_Default">Type of default value.</typeparam>
    public class ListIndexerWithDefault<T_Value, T_Default>
        : ListIndexerGetSet<T_Value>
    {
        internal ListIndexerWithDefault(
            DictIndexerWithDefault<decimal, T_Value, T_Default> dictIndexerWithDefault)
            : base(dictIndexerWithDefault)
        {
            this.AsDict = dictIndexerWithDefault;
        }

        /// <summary>
        /// Gets DictIndexer with decimal indexer for key definition.
        /// </summary>
        public new DictIndexerWithDefault<decimal, T_Value, T_Default> AsDict { get; }

        /// <summary>
        /// Indexer for list.
        /// </summary>
        /// <param name="indexKey">The string key for the list.</param>
        /// <param name="defaultVal">The default value for the list.</param>
        /// <returns>The translated list.</returns>
        public List<T_Value> this[string indexKey, T_Default defaultVal]
        {
            get => this.AsDict[indexKey, defaultVal].OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }
    }
}
