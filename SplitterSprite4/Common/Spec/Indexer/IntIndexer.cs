// -----------------------------------------------------------------------
// <copyright file="IntIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for integer accessor.
    /// </summary>
    public class IntIndexer : LiteralIndexer<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal IntIndexer(Spec parent)
            : base(
                parent,
                () => "整数",
                (value) => int.Parse(value),
                (value) => value.ToString(),
                () => "Int",
                0)
        {
        }
    }
}
