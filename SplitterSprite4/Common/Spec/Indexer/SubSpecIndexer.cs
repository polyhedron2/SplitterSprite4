// -----------------------------------------------------------------------
// <copyright file="SubSpecIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for SubSpec.
    /// </summary>
    public class SubSpecIndexer
    {
        private Spec parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSpecIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        internal SubSpecIndexer(Spec parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Indexer for sub spec.
        /// </summary>
        /// <param name="key">The string key for the sub spec.</param>
        /// <returns>The sub spec.</returns>
        public SubSpec this[string key]
        {
            get => new SubSpec(this.parent, key);
        }
    }
}
