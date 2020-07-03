// -----------------------------------------------------------------------
// <copyright file="DictIndexerWithScalarValue.cs" company="MagicKitchen">
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
    public class DictIndexerWithScalarValue<T_Key, T_Value>
    {
        private Spec parent;

        private Func<string> keyTypeGenerator;
        private Func<AgnosticPath, string, T_Key> keyGetter;
        private Func<AgnosticPath, T_Key, string> keySetter;
        private Func<T_Key, IComparable> keyOrder;
        private Func<string> keyMoldingAccessCodeGenerator;

        private Func<string> valueTypeGenerator;
        private Func<AgnosticPath, string, T_Value> valueGetter;
        private Func<AgnosticPath, T_Value, string> valueSetter;
        private Func<string> valueMoldingAccessCodeGenerator;

        private string type;
        private string moldingAccessCode;
        private Dictionary<T_Key, T_Value> moldingDefault;
        private ImmutableList<string> referredSpecs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictIndexerWithScalarValue{T_Key, T_Value}"/> class.
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
        /// <param name="valueTypeGenerator">
        /// The access type string generator for value.
        /// </param>
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
        /// <param name="valueMoldingAccessCodeGenerator">
        /// The value type and parameter information generator for molding.
        /// </param>
        /// <param name="referredSpecs">
        /// The spec IDs which are referred while base spec referring.
        /// </param>
        internal DictIndexerWithScalarValue(
            Spec parent,
            Func<string> keyTypeGenerator,
            Func<AgnosticPath, string, T_Key> keyGetter,
            Func<AgnosticPath, T_Key, string> keySetter,
            Func<T_Key, IComparable> keyOrder,
            Func<string> keyMoldingAccessCodeGenerator,
            Func<string> valueTypeGenerator,
            Func<AgnosticPath, string, T_Value> valueGetter,
            Func<AgnosticPath, T_Value, string> valueSetter,
            Func<string> valueMoldingAccessCodeGenerator,
            ImmutableList<string> referredSpecs)
        {
            this.parent = parent;

            this.keyTypeGenerator = Spec.FixCulture(keyTypeGenerator);
            this.keyGetter = Spec.FixCulture(keyGetter);
            this.keySetter = Spec.FixCulture(keySetter);
            this.keyOrder = keyOrder;
            this.keyMoldingAccessCodeGenerator =
                Spec.FixCulture(keyMoldingAccessCodeGenerator);

            this.valueTypeGenerator = Spec.FixCulture(valueTypeGenerator);
            this.valueGetter = Spec.FixCulture(valueGetter);
            this.valueSetter = Spec.FixCulture(valueSetter);
            this.valueMoldingAccessCodeGenerator =
                Spec.FixCulture(valueMoldingAccessCodeGenerator);

            this.type =
                $"マッピング({this.keyTypeGenerator()}" +
                $"=>{this.valueTypeGenerator()})";
            this.moldingAccessCode = string.Join(
                ", ",
                Spec.EncodeCommas(this.keyMoldingAccessCodeGenerator()),
                Spec.EncodeCommas(this.valueMoldingAccessCodeGenerator()));
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
                var baseDict = new DictIndexerWithScalarValue<T_Key, T_Value>(
                    this.parent.Base,
                    this.keyTypeGenerator,
                    this.keyGetter,
                    this.keySetter,
                    this.keyOrder,
                    this.keyMoldingAccessCodeGenerator,
                    this.valueTypeGenerator,
                    this.valueGetter,
                    this.valueSetter,
                    this.valueMoldingAccessCodeGenerator,
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
