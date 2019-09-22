// -----------------------------------------------------------------------
// <copyright file="SequenceYAML.cs" company="MagicKitchen">
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
    /// 配列型のYAMLを表現するクラス
    /// The YAML class for sequence type YAML.
    /// </summary>
    public class SequenceYAML : YAML, IEnumerable<YAML>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceYAML"/> class.
        /// </summary>
        public SequenceYAML()
        {
            this.Children = new List<YAML>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceYAML"/> class.
        /// </summary>
        /// <param name="id">The yaml's ID.</param>
        /// <param name="sequence">YamlDotNet sequence instance.</param>
        public SequenceYAML(string id, YamlSequenceNode sequence)
        {
            this.ID = id;
            this.Children = new List<YAML>();

            var i = 0;
            foreach (var child in sequence)
            {
                YAML translatedChild = Translate($"{this.ID}[{i}]", child);
                this.Children.Add(translatedChild);
                i++;
            }
        }

        private List<YAML> Children { get; set; }

        /// <summary>
        /// Add child yaml to this.
        /// </summary>
        /// <param name="child">The child YAML.</param>
        public void Add(YAML child) => this.Children.Add(child);

        /// <inheritdoc/>
        public IEnumerator<YAML> GetEnumerator()
        {
            return this.Children.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public override IEnumerable<string> ToStringLines()
        {
            IEnumerable<string> TranslateSequence(YAML child)
            {
                var i = 0;
                foreach (var line in child.ToStringLines())
                {
                    if (i == 0)
                    {
                        yield return "-";
                    }

                    yield return $"  {line}";
                    i++;
                }
            }

            IEnumerable<string> TranslateScalar(YAML child)
            {
                var scalar = child as ScalarYAML;
                if (!scalar.IsMultiLine)
                {
                    yield return $"- {scalar}";
                }
                else
                {
                    yield return "- |+";
                    foreach (var line in scalar.ToStringLines())
                    {
                        yield return $"  {line}";
                    }
                }
            }

            // 子要素をインデント。
            // Indent children.
            // この要素の１行目は"- "をつけ、２行目以降は"  "をつける。
            // First line starts with "-", Other lines start with "  ".
            return this.Children.SelectMany(
                child =>
                    (child is SequenceYAML) ?

                    // Sequenceであれば、
                    // -
                    //   - value0
                    //   - value1
                    //     :
                    //   - valueN
                    // の形

                    // In sequence case,
                    // following style.
                    // -
                    //   - value0
                    //   - value1
                    //     :
                    //   - valueN
                    TranslateSequence(child) :
                    (child is ScalarYAML) ?

                    // Scalarであれば
                    // 単一行なら
                    // - value
                    // 複数行なら
                    // - |+
                    //   line0
                    //   line1
                    //     :
                    //   lineN
                    // の形

                    // In single line and scalar case,
                    // following style.
                    // - value
                    // In multi line and scalar case,
                    // following style.
                    // - |+
                    //   line0
                    //   line1
                    //     :
                    //   lineN
                    TranslateScalar(child) :

                    // Mappingであれば、
                    // - key0: value0
                    //   key1: value1
                    //     :
                    //   keyN: valueN
                    // の形

                    // In mapping case,
                    // following style.
                    // - key0: value0
                    //   key1: value1
                    //     :
                    //   keyN: valueN
                    child.ToStringLines().Select((line, index) =>
                        (index == 0) ? $"- {line}" : $"  {line}"));
        }

        /// <inheritdoc/>
        protected override bool Contains(int key) =>
            key >= 0 && key < this.Children.Count;

        /// <inheritdoc/>
        protected override TValue InnerGetter<TValue>(int key)
        {
            YAML value = this.Children[key];
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
        protected override void InnerSetter<TValue>(int key, TValue value)
        {
            string childID = $"{this.ID}[{key}]";
            value.ID = childID;
            this.Children[key] = value;
        }
    }
}