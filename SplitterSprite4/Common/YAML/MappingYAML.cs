﻿// -----------------------------------------------------------------------
// <copyright file="MappingYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using YamlDotNet.RepresentationModel;

    /// <summary>
    /// マッピング（ハッシュ、辞書）型のYAMLを表現するクラス
    /// The YAML class for mapping type YAML.
    /// </summary>
    public class MappingYAML : YAML, IEnumerable<KeyValuePair<string, YAML>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingYAML"/> class.
        /// </summary>
        public MappingYAML()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingYAML"/> class.
        /// </summary>
        /// <param name="emptyDefaultScalar">
        /// The scalar value which will be replaced when this mapping is empty.
        /// </param>
        public MappingYAML(string emptyDefaultScalar)
        {
            this.Body = new Dictionary<string, YAML>();
            this.KeyOrder = new List<string>();
            this.EmptyDefaultScalarNullable = emptyDefaultScalar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingYAML"/> class.
        /// </summary>
        /// <param name="id">The yaml's ID.</param>
        /// <param name="mapping">YamlDotNet mapping instance.</param>
        public MappingYAML(string id, YamlMappingNode mapping)
            : this()
        {
            this.ID = id;
            foreach (var entry in mapping.Children)
            {
                var childID = $"{this.ID}[{entry.Key}]";
                var child = Translate(childID, entry.Value);
                this.Add(((YamlScalarNode)entry.Key).Value, child);
            }
        }

        /// <summary>
        /// Gets the string which is used in ToString when this mapping is empty.
        /// </summary>
        public string EmptyDefaultScalarNullable { get; }

        /// <summary>
        /// Gets the length of this mapping.
        /// </summary>
        public int Length
        {
            get => this.Body.Count;
        }

        private Dictionary<string, YAML> Body { get; set; }

        // Dictionaryはキーの順序保存を保証しないので順序管理用リストを持つ
        // The order of the keys in the Dictionary is unspecified.
        private List<string> KeyOrder { get; set; }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, YAML>> GetEnumerator()
        {
            foreach (string key in this.KeyOrder)
            {
                yield return new KeyValuePair<string, YAML>(key, this.Body[key]);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// MappingYAMLを複製する。子YAMLも全て複製される。
        /// Clone this MappingYAML. All descendants are clonsed too.
        /// </summary>
        /// <param name="id">The cloned yaml's ID.</param>
        /// <returns>The cloned yaml.</returns>
        public MappingYAML Clone(string id)
        {
            lock (this)
            {
                var yamlStream = new YamlStream();

                using (
                    StringReader reader =
                    new StringReader(this.ToString(true)))
                {
                    yamlStream.Load(reader);
                }

                var mappingNode =
                    (YamlMappingNode)yamlStream.Documents[0].RootNode;
                return new MappingYAML(id, mappingNode);
            }
        }

        /// <summary>
        /// Add key-value pair to this.
        /// </summary>
        /// <param name="key">The string key.</param>
        /// <param name="value">The value YAML.</param>
        public void Add(string key, YAML value)
        {
            lock (this)
            {
                if (!this.Body.ContainsKey(key))
                {
                    this.KeyOrder.Add(key);
                }

                this.Body[key] = value;
            }
        }

        /// <inheritdoc/>
        public override void Remove(string key)
        {
            lock (this)
            {
                this.Body.Remove(key);
                this.KeyOrder.Remove(key);
            }
        }

        /// <summary>
        /// Check the key is contained by this YAML.
        /// </summary>
        /// <param name="key">The string key.</param>
        /// <returns>Whether this YAML contains the key or not.</returns>
        public bool ContainsKey(string key)
        {
            return this.Body.ContainsKey(key);
        }

        /// <summary>
        /// Check this yaml is emtpy or not.
        /// </summary>
        /// <param name="ignoreEmptyMappingChild">Ignore MappingYAML's child if it's empty collection.</param>
        /// <returns>Whether this yaml is emtpy or not.</returns>
        public bool IsEmptyCollection(bool ignoreEmptyMappingChild)
        {
            try
            {
                this.ToRawStringLines(ignoreEmptyMappingChild).First();
                return false;
            }
            catch (System.InvalidOperationException)
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> ToStringLines(
            bool ignoreEmptyMappingChild)
        {
            var ret = this.ToRawStringLines(ignoreEmptyMappingChild);

            try
            {
                ret.First();
                return ret;
            }
            catch (System.InvalidOperationException)
            {
                if (this.EmptyDefaultScalarNullable == null)
                {
                    return new string[] { "{}" };
                }
                else
                {
                    return new string[] { $"\"{this.EmptyDefaultScalarNullable}\"" };
                }
            }
        }

        /// <inheritdoc/>
        protected override bool Contains(string key) => this.Body.ContainsKey(key);

        /// <inheritdoc/>
        protected override TValue InnerGetter<TValue>(string key)
        {
            YAML value = this.Body[key];
            if (value is TValue)
            {
                return value as TValue;
            }
            else
            {
                throw new YAMLTypeSlipException<TValue>(
                    this.ID, key.ToString(), value);
            }
        }

        /// <inheritdoc/>
        protected override void InnerSetter<TValue>(string key, TValue value)
        {
            string childID = $"{this.ID}[{key}]";
            value.ID = childID;
            this.Add(key, value);
        }

        private IEnumerable<string> ToRawStringLines(
            bool ignoreEmptyMappingChild)
        {
            IEnumerable<string> TranslateCollectionLines(string key)
            {
                string bodyFirstLine = this.Body[key].ToStringLines(ignoreEmptyMappingChild).First();

                // isEmptyCollection cannot defined by "Length == 0", because children YAML can be empty.
                var isEmptyCollection = false;
                if (this.Body[key] is MappingYAML)
                {
                    isEmptyCollection = (this.Body[key] as MappingYAML).IsEmptyCollection(
                        ignoreEmptyMappingChild);
                }
                else if (this.Body[key] is SequenceYAML)
                {
                    isEmptyCollection = bodyFirstLine == "[]";
                }

                if (isEmptyCollection)
                {
                    // If the child is emptyDefaultScalar, isEmptyMappingChild is False.
                    var isEmptyMappingChild =
                        bodyFirstLine == "{}" || bodyFirstLine == "[]";

                    if (!isEmptyMappingChild || !ignoreEmptyMappingChild)
                    {
                        var emptyValue = this.Body[key].ToString(
                            ignoreEmptyMappingChild);
                        yield return $"\"{Escape(key)}\": {emptyValue}";
                    }
                }
                else
                {
                    var i = 0;
                    foreach (string line in
                        this.Body[key].ToStringLines(ignoreEmptyMappingChild))
                    {
                        // entry.ValueのStringLinesが１行以上であれば、
                        // keyを出力してから内容をインデント出力。
                        // If entry.Value has some lines,
                        // key is dumped at first.
                        if (i == 0)
                        {
                            yield return $"\"{Escape(key)}\":";
                        }

                        yield return $"  {line}";
                        i++;
                    }
                }
            }

            IEnumerable<string> TranslateScalar(string key)
            {
                ScalarYAML scalar = this.Body[key] as ScalarYAML;
                if (scalar.IsMultiLine)
                {
                    // 複数行であれば、"|+"から始める
                    // In multi-line case, start with "|+".
                    yield return $"\"{Escape(key)}\": |+";
                    foreach (string line in
                        scalar.ToStringLines(ignoreEmptyMappingChild))
                    {
                        yield return $"  {line}";
                    }
                }
                else
                {
                    foreach (string line in
                        this.Body[key].ToStringLines(ignoreEmptyMappingChild))
                    {
                        yield return $"\"{Escape(key)}\": {line}";
                    }
                }
            }

            var ret = this.KeyOrder.SelectMany(
                key =>
                    (this.Body[key] is ScalarYAML) ?

                    // Scalarであれば
                    // 単一行なら
                    // key: value
                    // の形

                    // In single line and scalar case,
                    // following style.
                    // key: value

                    // 複数行なら
                    // key: |+
                    //   line0
                    //   line1
                    //     :
                    //   lineN
                    // の形

                    // In multi-line and scalar case,
                    // following style.
                    // key: |+
                    //   line0
                    //   line1
                    //     :
                    //   lineN
                    TranslateScalar(key) :

                    // Sequence, Mappingであれば、
                    // key:
                    //   value0
                    //   value1
                    //     :
                    //   valueN
                    // の形

                    // In sequence or mapping case,
                    // following style.
                    // key:
                    //   value0
                    //   value1
                    //     :
                    //   valueN
                    TranslateCollectionLines(key));

            return ret;
        }
    }
}
