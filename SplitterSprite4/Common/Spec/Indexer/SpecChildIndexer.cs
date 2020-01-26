// -----------------------------------------------------------------------
// <copyright file="SpecChildIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;

    /// <summary>
    /// Indexer class for SpecChild.
    /// </summary>
    public class SpecChildIndexer
    {
        private Spec parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecChildIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        internal SpecChildIndexer(Spec parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Indexer for spec child.
        /// </summary>
        /// <param name="key">The string key for the spec child.</param>
        /// <param name="type">The SpawnerChild type which this spec child will define.</param>
        /// <returns>The spec child.</returns>
        public SpecChild this[string key, Type type]
        {
            get => new SpecChild(this.parent, key, type);
        }
    }
}
