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

        /// <summary>
        /// Gets the length of this sequence.
        /// </summary>
        public int Length
        {
            get => this.Children.Count;
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
            IEnumerable<string> TranslateSequence(SequenceYAML child)
            {
                if (child.Length == 0)
                {
                    yield return $"- {child.ToString()}";
                }
                else
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
            }

            IEnumerable<string> TranslateScalar(ScalarYAML child)
            {
                if (!child.IsMultiLine)
                {
                    yield return $"- {child}";
                }
                else
                {
                    yield return "- |+";
                    foreach (var line in child.ToStringLines())
                    {
                        yield return $"  {line}";
                    }
                }
            }

            IEnumerable<string> TranslateMapping(MappingYAML child)
            {
                if (child.Length == 0)
                {
                    yield return $"- {child.ToString()}";
                }
                else
                {
                    var i = 0;
                    foreach (var line in child.ToStringLines())
                    {
                        if (i == 0)
                        {
                            yield return $"- {line}";
                        }
                        else
                        {
                            yield return $"  {line}";
                        }

                        i++;
                    }
                }
            }

            if (this.Length == 0)
            {
                return new string[] { "[]" };
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
                    TranslateSequence(child as SequenceYAML) :
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
                    TranslateScalar(child as ScalarYAML) :

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
                    TranslateMapping(child as MappingYAML));
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