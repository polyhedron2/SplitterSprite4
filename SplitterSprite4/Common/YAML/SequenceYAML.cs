using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    // YAML上の配列を表すオブジェクト
    public class SequenceYAML : YAML, IEnumerable<YAML>
    {
        public SequenceYAML()
        {
            Children = new List<YAML>();
        }

        public SequenceYAML(string id, YamlSequenceNode sequence)
        {
            ID = id;
            Children = new List<YAML>();

            var i = 0;
            foreach (var child in sequence)
            {
                var translatedChild = translate($"{ID}[{i}]", child);
                Children.Add(translatedChild);
                i++;
            }
        }

        public List<YAML> Children { get; set; }

        public void Add(YAML child) => Children.Add(child);
        public IEnumerator<YAML> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override bool Contains(int key) =>
            0 <= key && key < Children.Count;
        protected override Value InnerGetter<Value>(int key)
        {
            var value = Children[key];
            if (value is Value)
            {
                return value as Value;
            }
            else
            {
                throw new YAMLTypeSlipException<Value>(
                    ID, key.ToString(), value);
            }
        }
        protected override void InnerSetter<Value>(int key, Value value)
        {
            var childID = $"{ID}[{key}]";
            value.ID = childID;
            Children[key] = value;
        }

        public override IEnumerable<string> ToStringLines()
        {
            IEnumerable<string> translateSequence(YAML child)
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

            IEnumerable<string> translateScalar(YAML child)
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
            // この要素の１行目は"- "をつけ、２行目以降は"  "をつける。
            return Children.SelectMany(
                child =>
                    (child is SequenceYAML) ?
                    // Sequenceであれば、
                    // -
                    //   - value0
                    //   - value1
                    //     :
                    //   - valueN
                    // の形
                    translateSequence(child) :
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
                    translateScalar(child) :
                    // Mappingであれば、
                    // - key0: value0
                    //   key1: value1
                    //     :
                    //   keyN: valueN
                    // の形
                    // Scalarであれば、
                    // - key: value
                    // の形
                    child.ToStringLines().Select(
                        (line, index) => (index == 0) ? $"- {line}" : $"  {line}"
                    )
                );
        }
    }
}