using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    class SequenceYAML : YAML
    {
        public SequenceYAML()
        {
            Children = new List<YAML>();
        }

        public SequenceYAML(YamlSequenceNode sequence)
        {
            Children = new List<YAML>();

            var i = 0;
            foreach (var child in sequence)
            {
                var translatedChild = translate(child, $"{AccessPath}[{i}]");
                Children.Add(translatedChild);
                i++;
            }
        }

        public List<YAML> Children { get; set; }

        public void Add(YAML child) => Children.Add(child);

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
                    AccessPath, key.ToString(), value);
            }
        }
        protected override void InnerSetter<Value>(int key, Value value)
        {
            var childAccessPath = $"{AccessPath}[{key}]";
            value.AccessPath = childAccessPath;
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

            // 子要素をインデント。
            // この要素の１行目は"- "をつけ、２行目以降は"  "をつける。
            return Children.SelectMany(
                child => (child is SequenceYAML) ?
                    // Sequenceであれば、
                    // -
                    //   - value0
                    //   - value1
                    //     :
                    //   - valueN
                    // の形
                    translateSequence(child) :
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