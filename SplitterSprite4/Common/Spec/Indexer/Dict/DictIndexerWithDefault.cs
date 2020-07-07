// -----------------------------------------------------------------------
// <copyright file="DictIndexerWithDefault.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Indexer class for dictionary type value in spec file.
    /// </summary>
    /// <typeparam name="T_Key">Type of dictionary key.</typeparam>
    /// <typeparam name="T_Value">Type of dictionary value.</typeparam>
    /// <typeparam name="T_Default">Type of dictionary default value.</typeparam>
    public class DictIndexerWithDefault<T_Key, T_Value, T_Default>
        : DictIndexerGetSet<T_Key, T_Value>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictIndexerWithDefault{T_Key, T_Value, T_Default}"/> class.
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
        internal DictIndexerWithDefault(
            Spec parent,
            Func<T_Key, IComparable> keyOrder,
            ScalarIndexer<T_Key> keyScalarIndexer,
            Func<Spec, IIndexerWithDefault<T_Value, T_Default>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
            : base(
                  parent,
                  keyOrder,
                  keyScalarIndexer,
                  valueIndexerGenerator,
                  referredSpecs)
        {
            this.ValueIndexerDefaultGenerator = valueIndexerGenerator;
        }

        protected Func<Spec, IIndexerWithDefault<T_Value, T_Default>>
            ValueIndexerDefaultGenerator { get; }

        public Dictionary<T_Key, T_Value> this[string indexKey, T_Default defualtVal]
        {
            get
            {
                lock (this.Parent.Properties)
                {
                    try
                    {
                        if (this.Parent.IsMolding)
                        {
                            this.Parent[indexKey].Mold[this.MoldingTypeIndex] =
                                new ScalarYAML(this.MoldingAccessCode);
                        }

                        var ret = new Dictionary<T_Key, T_Value>();

                        foreach (var key in this.SortedKeys(indexKey))
                        {
                            try
                            {
                                var indexer =
                                    this.ValueIndexerDefaultGenerator(
                                        this.Parent[indexKey][this.DictBodyIndex]);
                                ret[this.KeyGetter(this.Parent.Path, key)] =
                                    indexer[key, defualtVal];
                            }
                            catch (Spec.HiddenKeyException)
                            {
                                continue;
                            }
                        }

                        return ret;
                    }
                    catch (Exception ex)
                    {
                        if (this.Parent.IsMolding)
                        {
                            return this.MoldingDefault;
                        }
                        else
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
}
