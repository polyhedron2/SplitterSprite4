using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    class MappingYAML : YAML
    {
        public MappingYAML()
        {
            Body = new Dictionary<string, YAML>();
        }

        public MappingYAML(YamlMappingNode mapping)
        {
            Initialize(mapping);
        }

        Dictionary<string, YAML> Body { get; set; }

        public void Add(string key, YAML value) => Body.Add(key, value);

        protected void Initialize(YamlMappingNode mapping)
        {
            Body = new Dictionary<string, YAML>();
            foreach (var entry in mapping.Children)
            {
                var childID = $"{ID}[{entry.Key}]";
                var child = translate(entry.Value, childID);
                child.ID = childID;
                Body[((YamlScalarNode)entry.Key).Value] = child;
            }
        }

        protected override bool Contains(string key) => Body.ContainsKey(key);
        protected override Value InnerGetter<Value>(string key)
        {
            var value = Body[key];
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
        protected override void InnerSetter<Value>(string key, Value value)
        {
            var childID = $"{ID}[{key}]";
            value.ID = childID;
            Body[key] = value;
        }

        public override IEnumerable<string> ToStringLines()
        {
            IEnumerable<string> translateCollectionLines(KeyValuePair<string, YAML> entry)
            {
                // entry.ValueのStringLinesが０行であれば、何も出力しない。
                var i = 0;
                foreach (var line in entry.Value.ToStringLines())
                {
                    // entry.ValueのStringLinesが１行以上であれば、
                    // keyを出力してから内容をインデント出力。
                    if (i == 0)
                    {
                        yield return $"{entry.Key}:";
                    }
                    yield return $"  {line}";
                    i++;
                }
            }

            return Body.SelectMany(
                entry => (entry.Value is ScalarYAML) ?
                    // Scalarであれば$"{key}: {value}"の形
                    entry.Value.ToStringLines().Select(_ => $"{entry.Key}: {_}") :
                    // Sequence, Mappingであれば、
                    // key: 
                    //   value0
                    //   value1
                    //     :
                    //   valueN
                    // の形
                    translateCollectionLines(entry)
            );
        }
    }
}
