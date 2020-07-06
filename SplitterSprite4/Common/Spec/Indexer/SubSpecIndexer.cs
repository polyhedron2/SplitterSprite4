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
    public class SubSpecIndexer : IIndexerGet<SubSpec>
    {
        private Spec parent;
        private bool dictMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSpecIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal SubSpecIndexer(Spec parent, bool dictMode)
        {
            this.parent = parent;
            this.dictMode = dictMode;
        }

        /// <summary>
        /// Indexer for sub spec.
        /// </summary>
        /// <param name="key">The string key for the sub spec.</param>
        /// <returns>The sub spec.</returns>
        public SubSpec this[string key]
        {
            get
            {
                if (this.parent.IsHidden(key) && this.dictMode)
                {
                    throw new Spec.HiddenKeyException(this.parent.ID, key);
                }

                return new SubSpec(this.parent, key);
            }
        }

        /// <summary>
        /// Remove this sub spec from parent.
        /// If base spec contains the sub spec, the base values will be referred.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        public void Remove(string key)
        {
            new SubSpec(this.parent, key).Remove();
        }

        /// <summary>
        /// Hide this sub spec from parent.
        /// If base spec contains the sub spec, the base values will be hidden.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        public void Hide(string key)
        {
            new SubSpec(this.parent, key).Hide();
        }

        /// <summary>
        /// Add special value into the key of the spec.
        /// Ensure that spawner spec exists, even if the spec is empty.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        public void Hold(string key)
        {
            new SubSpec(this.parent, key).Hold();
        }
    }
}
