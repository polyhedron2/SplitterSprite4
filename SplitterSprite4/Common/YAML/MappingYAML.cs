using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    // YAML上の辞書を表現するオブジェクト
    public class MappingYAML : YAML, IEnumerable<KeyValuePair<string, YAML>>
    {
        public MappingYAML()
        {
            Body = new Dictionary<string, YAML>();
            KeyOrder = new List<string>();
        }

        public MappingYAML(string id, YamlMappingNode mapping) : this()
        {
            ID = id;
            foreach (var entry in mapping.Children)
            {
                var childID = $"{ID}[{entry.Key}]";
                var child = translate(childID, entry.Value);
                Add(((YamlScalarNode)entry.Key).Value, child);
            }
        }

        Dictionary<string, YAML> Body { get; set; }
        // Dictionaryはキーの順序保存を保証しないので順序管理用リストを持つ
        List<string> KeyOrder { get; set; }

        public IEnumerator<KeyValuePair<string, YAML>> GetEnumerator()
        {
            foreach (var key in KeyOrder)
            {
                yield return new KeyValuePair<string, YAML>(key, Body[key]);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, YAML value)
        {
            if (!Body.ContainsKey(key))
            {
                KeyOrder.Add(key);
            }
            Body[key] = value;
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
            Add(key, value);
        }

        public override IEnumerable<string> ToStringLines()
        {
            IEnumerable<string> translateCollectionLines(string key)
            {
                // entry.ValueのStringLinesが０行であれば、何も出力しない。
                var i = 0;
                foreach (var line in Body[key].ToStringLines())
                {
                    // entry.ValueのStringLinesが１行以上であれば、
                    // keyを出力してから内容をインデント出力。
                    if (i == 0)
                    {
                        yield return $"{key}:";
                    }
                    yield return $"  {line}";
                    i++;
                }
            }

            IEnumerable<string> translateScalar(string key)
            {
                var scalar = Body[key] as ScalarYAML;
                if (scalar.IsMultiLine)
                {
                    // 複数行であれば、"|+"から始める
                    yield return $"{key}: |+";
                    foreach (var line in scalar.ToStringLines())
                    {
                        yield return $"  {line}";
                    }
                }
                else
                {
                    foreach (var line in Body[key].ToStringLines())
                    {
                        yield return $"{key}: {line}";
                    }
                }
            }

            return KeyOrder.SelectMany(
                key =>
                    (Body[key] is ScalarYAML) ?
                    // Scalarであれば
                    // 単一行なら
                    // key: value
                    // 複数行なら
                    // key: |+
                    //   line0
                    //   line1
                    //     :
                    //   lineN
                    // の形
                    translateScalar(key) :
                    // Sequence, Mappingであれば、
                    // key: 
                    //   value0
                    //   value1
                    //     :
                    //   valueN
                    // の形
                    translateCollectionLines(key));
        }
    }
}
