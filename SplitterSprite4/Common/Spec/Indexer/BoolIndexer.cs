﻿// -----------------------------------------------------------------------
// <copyright file="BoolIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for boolean accessor.
    /// </summary>
    public class BoolIndexer : LiteralIndexer<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal BoolIndexer(Spec parent, bool dictMode)
            : base(
                parent,
                () => "真偽値",
                (value) => bool.Parse(value),
                (value) => value.ToString(),
                () => "Bool",
                false,
                dictMode)
        {
        }
    }
}
