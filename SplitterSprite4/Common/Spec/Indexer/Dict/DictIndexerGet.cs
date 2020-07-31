// -----------------------------------------------------------------------
// <copyright file="DictIndexerGet.cs" company="MagicKitchen">
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
    public class DictIndexerGet<T_Key, T_Value>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictIndexerGet{T_Key, T_Value}"/> class.
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
        internal DictIndexerGet(
            Spec parent,
            Func<T_Key, IComparable> keyOrder,
            ScalarIndexer<T_Key> keyScalarIndexer,
            ImmutableList<T_Key> ensuredKeys,
            Func<Spec, IIndexerGet<T_Value>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
        {
            this.Parent = parent;

            this.KeyOrder = keyOrder;
            this.KeyScalarIndexer = keyScalarIndexer;
            this.EnsuredKeys = ensuredKeys;
            this.KeyTypeGenerator = keyScalarIndexer.TypeGenerator;
            this.KeyGetter = keyScalarIndexer.Getter;
            this.KeySetter = keyScalarIndexer.Setter;
            this.KeyMoldingAccessCodeGenerator =
                keyScalarIndexer.MoldingAccessCodeGenerator;

            this.ValueIndexerGetGenerator = valueIndexerGenerator;

            this.Type = $"{this.KeyTypeGenerator()}キーマッピング";
            this.MoldingAccessCode =
                $"Dict, {Spec.EncodeCommas(this.KeyMoldingAccessCodeGenerator())}";
            this.MoldingDefault = new Dictionary<T_Key, T_Value>();
            this.ReferredSpecs = referredSpecs;
        }

        protected string MoldingTypeIndex { get => "MoldingType"; }

        protected string DictBodyIndex { get => "DictBody"; }

        protected Spec Parent { get; }

        protected Func<T_Key, IComparable> KeyOrder { get; }

        protected ScalarIndexer<T_Key> KeyScalarIndexer { get; }

        protected ImmutableList<T_Key> EnsuredKeys { get; }

        protected Func<string> KeyTypeGenerator { get; }

        protected Func<AgnosticPath, string, T_Key> KeyGetter { get; }

        protected Func<AgnosticPath, T_Key, string> KeySetter { get; }

        protected Func<string> KeyMoldingAccessCodeGenerator { get; }

        protected Func<Spec, IIndexerGet<T_Value>> ValueIndexerGetGenerator { get; }

        protected string Type { get; }

        protected string MoldingAccessCode { get; }

        protected Dictionary<T_Key, T_Value> MoldingDefault { get; }

        protected ImmutableList<string> ReferredSpecs { get; }

        /// <summary>
        /// Indexer for dictionary.
        /// </summary>
        /// <param name="indexKey">The string key for the dictionary.</param>
        /// <returns>The translated dictionary.</returns>
        public Dictionary<T_Key, T_Value> this[string indexKey]
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
                            ret = new DictIndexerGet<T_Key, T_Value>(
                                this.Parent.Base,
                                this.KeyOrder,
                                this.KeyScalarIndexer,
                                ImmutableList<T_Key>.Empty,
                                this.ValueIndexerGetGenerator,
                                this.ReferredSpecs.Add(this.Parent.ID))[indexKey];
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
                                        this.ValueIndexerGetGenerator(
                                            this.Parent[indexKey][this.DictBodyIndex]);
                                    ret[translatedKey] = indexer[yamlKey];
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
                                    this.ValueIndexerGetGenerator(
                                        this.Parent[indexKey][this.DictBodyIndex]);
                                ret[ensuredKey] = indexer[yamlKey];
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

        /// <summary>
        /// Remove key and value from the dictionary.
        /// </summary>
        /// <param name="dictKey">The target dictionary key.</param>
        /// <param name="removeKey">The key which will be removed.</param>
        public void Remove(string dictKey, T_Key removeKey)
        {
            this.ValueIndexerGetGenerator(
                this.Parent[dictKey][this.DictBodyIndex]).Remove(
                this.KeySetter(this.Parent.Path, removeKey));
        }

        /// <summary>
        /// Hide key and value from the dictionary.
        /// </summary>
        /// <param name="dictKey">The target dictionary key.</param>
        /// <param name="hideKey">The key which will be hidden.</param>
        public void Hide(string dictKey, T_Key hideKey)
        {
            this.ValueIndexerGetGenerator(
                this.Parent[dictKey][this.DictBodyIndex]).Hide(
                this.KeySetter(this.Parent.Path, hideKey));
        }

        /// <summary>
        /// Hold key with empty value on the dictionary.
        /// </summary>
        /// <param name="dictKey">The target dictionary key.</param>
        /// <param name="holdKey">The key which will be held.</param>
        public void Hold(string dictKey, T_Key holdKey)
        {
            this.ValueIndexerGetGenerator(
                this.Parent[dictKey][this.DictBodyIndex]).Hold(
                this.KeySetter(this.Parent.Path, holdKey));
        }

        public class InvalidKeyException : Exception
        {
            internal InvalidKeyException(string keyStr, Exception inner)
                : base($"{keyStr}はマッピングのキーとして不正です。", inner)
            {
            }
        }
    }
}
