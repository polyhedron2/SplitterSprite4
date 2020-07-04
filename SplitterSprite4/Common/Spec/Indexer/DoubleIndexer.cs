﻿// -----------------------------------------------------------------------
// <copyright file="DoubleIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for double precision floating point number accessor.
    /// </summary>
    public class DoubleIndexer : LiteralIndexer<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        internal DoubleIndexer(Spec parent, bool allowHiddenValue)
            : base(
                parent,
                () => "実数",
                (value) => double.Parse(value),
                (value) => value.ToString(),
                () => "Double",
                0.0,
                allowHiddenValue)
        {
        }
    }
}
