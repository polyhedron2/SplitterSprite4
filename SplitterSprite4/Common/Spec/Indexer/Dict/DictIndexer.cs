// -----------------------------------------------------------------------
// <copyright file="DictIndexer.cs" company="MagicKitchen">
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
    public class DictIndexer<T_Key, T_Value>
    {
        private Spec parent;

        private string keyType;
        private Func<AgnosticPath, string, T_Key> keyGetter;
        private Func<AgnosticPath, T_Key, string> keySetter;
        private Func<T_Key, IComparable> keyOrder;
        private string keyMoldingAccessCode;

        private string valueType;
        private Func<AgnosticPath, string, T_Value> valueGetter;
        private Func<AgnosticPath, T_Value, string> valueSetter;
        private string valueMoldingAccessCode;

        private string type;
        private string moldingAccessCode;
        private Dictionary<T_Key, T_Value> moldingDefault;
        private ImmutableList<string> referredSpecs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictIndexer{T_Key, T_Value}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="keyType">The access type string for key.</param>
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
        /// <param name="keyMoldingAccessCode">
        /// The key type and parameter information for molding.
        /// </param>
        /// <param name="valueType">The access type string for value.</param>
        /// <param name="valueGetter">
        /// Translation function
        /// from spec path and string value in spec
        /// to indexed value.
        /// </param>
        /// <param name="valueSetter">
        /// Translation function
        /// from spec path and indexed value
        /// to string value in spec.
        /// </param>
        /// <param name="valueMoldingAccessCode">
        /// The value type and parameter information for molding.
        /// </param>
        /// <param name="referredSpecs">
        /// The spec IDs which are referred while base spec referring.
        /// </param>
        internal DictIndexer(
            Spec parent,
            string keyType,
            Func<AgnosticPath, string, T_Key> keyGetter,
            Func<AgnosticPath, T_Key, string> keySetter,
            Func<T_Key, IComparable> keyOrder,
            string keyMoldingAccessCode,
            string valueType,
            Func<AgnosticPath, string, T_Value> valueGetter,
            Func<AgnosticPath, T_Value, string> valueSetter,
            string valueMoldingAccessCode,
            ImmutableList<string> referredSpecs)
        {
            this.parent = parent;

            this.keyType = keyType;
            this.keyGetter = keyGetter;
            this.keySetter = keySetter;
            this.keyOrder = keyOrder;
            this.keyMoldingAccessCode = keyMoldingAccessCode;

            this.valueType = valueType;
            this.valueGetter = valueGetter;
            this.valueSetter = valueSetter;
            this.valueMoldingAccessCode = valueMoldingAccessCode;

            this.type = $"マッピング({keyType}=>{valueType})";
            this.moldingAccessCode = string.Join(
                ", ",
                Spec.EncodeCommas(keyMoldingAccessCode),
                Spec.EncodeCommas(valueMoldingAccessCode));
            this.moldingDefault = new Dictionary<T_Key, T_Value>();
            this.referredSpecs = referredSpecs;
        }

        /// <summary>
        /// Indexer for dictionary.
        /// </summary>
        /// <param name="indexKey">The string key for the dictionary.</param>
        /// <returns>The translated dictionary.</returns>
        public Dictionary<T_Key, T_Value> this[string indexKey]
        {
            get
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            this.parent.Mold[indexKey] =
                                new ScalarYAML(this.moldingAccessCode);
                        }

                        return this.IndexGet(indexKey);
                    }
                    catch (Exception ex)
                    {
                        if (this.parent.IsMolding)
                        {
                            return this.moldingDefault;
                        }
                        else
                        {
                            throw new Spec.InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{indexKey}]",
                                this.type,
                                ex);
                        }
                    }
                }
            }

            set
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        var newMap = new MappingYAML();
                        newMap.ID = $"{this.parent.Properties.ID}[{indexKey}]";

                        foreach (
                            var innerKey in
                            value.Keys.OrderBy(k => this.keyOrder(k)))
                        {
                            var yamlKey =
                                this.keySetter(this.parent.Path, innerKey);
                            var yamlValue = new ScalarYAML(
                                this.valueSetter(this.parent.Path, value[innerKey]));
                            yamlValue.ID = $"{newMap.ID}[{yamlKey}]";

                            newMap.Add(yamlKey, yamlValue);
                        }

                        this.parent.Properties[indexKey] = newMap;
                    }
                    catch (Exception ex)
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{indexKey}]",
                            this.type,
                            ex);
                    }
                }
            }
        }

        internal Dictionary<T_Key, T_Value> IndexGet(string indexKey)
        {
            // If it is looped, stop to search keys in spec.
            var isLooped = this.referredSpecs.Contains(this.parent.ID);
            if (isLooped)
            {
                return new Dictionary<T_Key, T_Value>();
            }

            var ret = new Dictionary<T_Key, T_Value>();

            // Translate keys and values from spec.
            var itsMap = this.parent.Properties.Mapping[indexKey];
            foreach (var yamlKeyValue in itsMap)
            {
                var yamlKey = yamlKeyValue.Key;
                var yamlValue = itsMap.Scalar[yamlKey];

                var innerKey = this.keyGetter(
                    this.parent.Path, yamlKey);
                var innerValue = this.valueGetter(
                    this.parent.Path, yamlValue.Value);

                ret.Add(innerKey, innerValue);
            }

            if (this.parent.Base != null)
            {
                var baseDict = new DictIndexer<T_Key, T_Value>(
                    this.parent.Base,
                    this.keyType,
                    this.keyGetter,
                    this.keySetter,
                    this.keyOrder,
                    this.keyMoldingAccessCode,
                    this.valueType,
                    this.valueGetter,
                    this.valueSetter,
                    this.valueMoldingAccessCode,
                    this.referredSpecs.Add(this.parent.ID))
                    .IndexGet(indexKey);

                // Update its dictionary with base one.
                // Its dictionary's key and value have higher priority than base's one.
                foreach (
                    var kv in baseDict.Where(kv => !ret.ContainsKey(kv.Key)))
                {
                    ret.Add(kv.Key, kv.Value);
                }
            }

            return ret;
        }
    }
}
