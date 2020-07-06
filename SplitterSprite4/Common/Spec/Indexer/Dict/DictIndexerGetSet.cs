// -----------------------------------------------------------------------
// <copyright file="DictIndexerGetSet.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Indexer class for dictionary type value in spec file.
    /// </summary>
    /// <typeparam name="T_Key">Type of dictionary key.</typeparam>
    /// <typeparam name="T_Value">Type of dictionary value.</typeparam>
    public class DictIndexerGetSet<T_Key, T_Value> : DictIndexerGet<T_Key, T_Value>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictIndexerGetSet{T_Key, T_Value}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="keyOrder">
        /// Translation function
        /// from key to IComparable for sorting keys.
        /// </param>
        /// <param name="keyScalarIndexer">
        /// ScalarIndexer object for key access.
        /// </param>
        /// <param name="valueIndexerGenerator">
        /// The value indexer generator.
        /// </param>
        /// <param name="referredSpecs">
        /// The spec IDs which are referred while base spec referring.
        /// </param>
        internal DictIndexerGetSet(
            Spec parent,
            Func<T_Key, IComparable> keyOrder,
            ScalarIndexer<T_Key> keyScalarIndexer,
            Func<Spec, IIndexerGetSet<T_Value>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
            : base(
                  parent,
                  keyOrder,
                  keyScalarIndexer,
                  valueIndexerGenerator,
                  referredSpecs)
        {
            this.ValueIndexerGetSetGenerator = valueIndexerGenerator;
        }

        protected Func<Spec, IIndexerGetSet<T_Value>>
            ValueIndexerGetSetGenerator { get; }

        /// <summary>
        /// Indexer for dictionary.
        /// </summary>
        /// <param name="indexKey">The string key for the dictionary.</param>
        /// <returns>The translated dictionary.</returns>
        public new Dictionary<T_Key, T_Value> this[string indexKey]
        {
            get => base[indexKey];

            set
            {
                lock (this.Parent.Properties)
                {
                    try
                    {
                        foreach (var kv in value)
                        {
                            var key = this.KeySetter(this.Parent.Path, kv.Key);
                            var indexer = this.ValueIndexerGetSetGenerator(
                                this.Parent[indexKey][this.DictBodyIndex]);
                            indexer[key] = kv.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.Parent.Properties.ID}[{indexKey}]",
                            this.Type,
                            ex);
                    }
                }
            }
        }
    }
}
