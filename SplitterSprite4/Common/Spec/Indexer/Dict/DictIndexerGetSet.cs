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
    using System.Linq;

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
        /// <param name="ensuredKeys">
        /// Keys which must be contained.
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
            ImmutableList<T_Key> ensuredKeys,
            Func<Spec, IIndexerGetSet<T_Value>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
            : base(
                  parent,
                  keyOrder,
                  keyScalarIndexer,
                  ensuredKeys,
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
                        this.Parent.SubSpec.Remove(indexKey);

                        var baseKeys = this[indexKey].Keys;
                        var hideKeys = baseKeys.Except(value.Keys);
                        var updateKeys = baseKeys.Union(value.Keys);

                        foreach (var key in updateKeys.OrderBy(this.KeyOrder))
                        {
                            string keyStr;
                            try
                            {
                                keyStr = this.KeySetter(this.Parent.Path, key);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidKeyException(key.ToString(), ex);
                            }

                            var indexer = this.ValueIndexerGetSetGenerator(
                                this.Parent[indexKey][this.DictBodyIndex]);

                            if (hideKeys.Contains(key))
                            {
                                indexer.Hide(keyStr);
                            }
                            else
                            {
                                indexer[keyStr] = value[key];
                            }
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
