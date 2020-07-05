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
    using System.Linq;
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
        /// <param name="keyTypeGenerator">
        /// The access type string generator for key.
        /// </param>
        /// <param name="keyGetter">
        /// Translation function
        /// from spec path and string key in spec
        /// to indexed key.
        /// </param>
        /// <param name="keySetter">
        /// Translation function
        /// from spec path and indexed key
        /// to string key in spec.
        /// </param>
        /// <param name="keyOrder">
        /// Translation function
        /// from key to IComparable for sorting keys.
        /// </param>
        /// <param name="keyMoldingAccessCodeGenerator">
        /// The key type and parameter information generator for molding.
        /// </param>
        /// <param name="valueIndexerGenerator">
        /// The value indexer generator.
        /// </param>
        /// <param name="referredSpecs">
        /// The spec IDs which are referred while base spec referring.
        /// </param>
        internal DictIndexerGet(
            Spec parent,
            Func<string> keyTypeGenerator,
            Func<AgnosticPath, string, T_Key> keyGetter,
            Func<AgnosticPath, T_Key, string> keySetter,
            Func<T_Key, IComparable> keyOrder,
            Func<string> keyMoldingAccessCodeGenerator,
            Func<Spec, IIndexerGet<T_Value>> valueIndexerGenerator,
            ImmutableList<string> referredSpecs)
        {
            this.Parent = parent;

            this.KeyTypeGenerator = Spec.FixCulture(keyTypeGenerator);
            this.KeyGetter = Spec.FixCulture(keyGetter);
            this.KeySetter = Spec.FixCulture(keySetter);
            this.KeyOrder = keyOrder;
            this.KeyMoldingAccessCodeGenerator =
                Spec.FixCulture(keyMoldingAccessCodeGenerator);

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

        protected Func<string> KeyTypeGenerator { get; }

        protected Func<AgnosticPath, string, T_Key> KeyGetter { get; }

        protected Func<AgnosticPath, T_Key, string> KeySetter { get; }

        protected Func<T_Key, IComparable> KeyOrder { get; }

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
                            this.Parent.Mold[indexKey][this.MoldingTypeIndex] =
                                new ScalarYAML(this.MoldingAccessCode);
                        }

                        var ret = new Dictionary<T_Key, T_Value>();

                        foreach (var key in this.SortedKeys(indexKey))
                        {
                            try
                            {
                                var indexer =
                                    this.ValueIndexerGetGenerator(
                                        this.Parent[indexKey][this.DictBodyIndex]);
                                ret[this.KeyGetter(this.Parent.Path, key)] =
                                    indexer[key];
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

        internal ImmutableList<string> Keys(string indexKey)
        {
            // If it is looped, stop to search keys in spec.
            var isLooped = this.ReferredSpecs.Contains(this.Parent.ID);
            if (isLooped)
            {
                return ImmutableList<string>.Empty;
            }

            ImmutableList<string> ret;
            if (this.Parent.Base != null)
            {
                ret = new DictIndexerGet<T_Key, T_Value>(
                    this.Parent.Base,
                    this.KeyTypeGenerator,
                    this.KeyGetter,
                    this.KeySetter,
                    this.KeyOrder,
                    this.KeyMoldingAccessCodeGenerator,
                    this.ValueIndexerGetGenerator,
                    this.ReferredSpecs.Add(this.Parent.ID))
                    .Keys(indexKey);
            }
            else
            {
                ret = ImmutableList<string>.Empty;
            }

            // Translate keys and values from spec.
            var itsMap = this.Parent.Properties.Mapping[indexKey];
            foreach (var yamlKeyValue in itsMap)
            {
                var yamlKey = yamlKeyValue.Key;
                ret.Add(yamlKey);
            }

            return ret;
        }

        internal IOrderedEnumerable<string> SortedKeys(string indexKey)
        {
            return this.Keys(indexKey).OrderBy(
                key => this.KeyOrder(this.KeyGetter(this.Parent.Path, key)));
        }
    }
}
