// -----------------------------------------------------------------------
// <copyright file="MappingYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System.Collections;
    using System.Collections.Generic;
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
        {
            this.Body = new Dictionary<string, YAML>();
            this.KeyOrder = new List<string>();
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
        /// Add key-value pair to this.
        /// </summary>
        /// <param name="key">The string key.</param>
        /// <param name="value">The value YAML.</param>
        public void Add(string key, YAML value)
        {
            if (!this.Body.ContainsKey(key))
            {
                this.KeyOrder.Add(key);
            }

            this.Body[key] = value;
        }

        /// <inheritdoc/>
        public override IEnumerable<string> ToStringLines(
            bool ignoreEmptyMappingChild)
        {
            IEnumerable<string> TranslateCollectionLines(string key)
            {
                string bodyFirstLine;
                bodyFirstLine = this.Body[key].ToStringLines(ignoreEmptyMappingChild).First();
                var isEmptyCollection =
                    bodyFirstLine == "{}" || bodyFirstLine == "[]";

                if (isEmptyCollection)
                {
                    if (!ignoreEmptyMappingChild)
                    {
                        var emptyValue = this.Body[key].ToString(
                            ignoreEmptyMappingChild);
                        yield return $"{key}: {emptyValue}";
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
                            yield return $"{key}:";
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
                    yield return $"{key}: |+";
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
                        yield return $"{key}: {line}";
                    }
                }
            }

            if (this.Length == 0)
            {
                return new string[] { "{}" };
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

            try
            {
                ret.First();
                return ret;
            }
            catch (System.InvalidOperationException)
            {
                return new string[] { "{}" };
            }
        }

        /// <inheritdoc/>
        public override void Remove(string key)
        {
            this.Body.Remove(key);
            this.KeyOrder.Remove(key);
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
    }
}
