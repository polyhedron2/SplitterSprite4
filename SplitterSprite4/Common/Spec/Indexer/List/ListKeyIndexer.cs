// -----------------------------------------------------------------------
// <copyright file="ListKeyIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.List
{
    /// <summary>
    /// Indexer class for decimal floating point number accessor.
    /// This class is used for accessing key for ListIndexer.
    /// Because ListIndexer is implemented as DictIndexer with decimal.
    /// </summary>
    public class ListKeyIndexer : LiteralIndexer<decimal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListKeyIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal ListKeyIndexer(Spec parent)
            : base(
                parent,
                () => "リスト",
                (value) => decimal.Parse(value),
                (value) => value.ToString(),
                () => "ListKey",
                0.0m,
                dictMode: true)
        {
        }
    }
}
