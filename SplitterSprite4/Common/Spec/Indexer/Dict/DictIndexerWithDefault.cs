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
        /// <param name="ensuredKeys">
        /// Keys which must be contained.
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
            ImmutableList<T_Key> ensuredKeys,
            Func<Spec, IIndexerWithDefault<T_Value, T_Default>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
            : base(
                  parent,
                  keyOrder,
                  keyScalarIndexer,
                  ensuredKeys,
                  valueIndexerGenerator,
                  referredSpecs)
        {
            this.ValueIndexerDefaultGenerator = valueIndexerGenerator;
        }

        protected Func<Spec, IIndexerWithDefault<T_Value, T_Default>>
            ValueIndexerDefaultGenerator { get; }

        public Dictionary<T_Key, T_Value> this[string indexKey, T_Default defaultVal]
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

                        // If it is looped, stop to search keys in spec.
                        var isLooped = this.ReferredSpecs.Contains(this.Parent.ID);
                        if (isLooped)
                        {
                            return new Dictionary<T_Key, T_Value>();
                        }

                        Dictionary<T_Key, T_Value> ret;
                        if (this.Parent.Base != null)
                        {
                            ret = new DictIndexerWithDefault<T_Key, T_Value, T_Default>(
                                this.Parent.Base,
                                this.KeyOrder,
                                this.KeyScalarIndexer,
                                ImmutableList<T_Key>.Empty,
                                this.ValueIndexerDefaultGenerator,
                                this.ReferredSpecs.Add(this.Parent.ID))[indexKey, defaultVal];
                        }
                        else
                        {
                            ret = new Dictionary<T_Key, T_Value>();
                        }

                        if (this.Parent.Properties.ContainsKey(indexKey))
                        {
                            // Translate keys and values from spec.
                            var itsMap = this.Parent.Properties[indexKey].Mapping[this.DictBodyIndex];
                            foreach (var yamlKeyValue in itsMap)
                            {
                                var yamlKey = yamlKeyValue.Key;
                                T_Key translatedKey;
                                try
                                {
                                    translatedKey = this.KeyGetter(this.Parent.Path, yamlKey);
                                }
                                catch (Exception ex)
                                {
                                    throw new InvalidKeyException(yamlKey, ex);
                                }

                                try
                                {
                                    var indexer =
                                        this.ValueIndexerDefaultGenerator(
                                            this.Parent[indexKey][this.DictBodyIndex]);
                                    ret[translatedKey] = indexer[yamlKey, defaultVal];
                                }
                                catch (Spec.HiddenKeyException)
                                {
                                    ret.Remove(translatedKey);
                                    continue;
                                }
                            }
                        }

                        foreach (var ensuredKey in this.EnsuredKeys)
                        {
                            if (!ret.ContainsKey(ensuredKey))
                            {
                                var yamlKey = this.KeySetter(this.Parent.Path, ensuredKey);
                                var indexer =
                                    this.ValueIndexerDefaultGenerator(
                                        this.Parent[indexKey][this.DictBodyIndex]);
                                ret[ensuredKey] = indexer[yamlKey, defaultVal];
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
