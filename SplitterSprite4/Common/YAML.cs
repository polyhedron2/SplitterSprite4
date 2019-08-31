using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common
{
    abstract class YAML
    {
        public string AccessPath { get; set; }

        public static YAML translate(YamlNode yamlNode, string accessPath)
        {
            if (yamlNode is YamlMappingNode)
            {
                return new MappingYAML(yamlNode as YamlMappingNode);
            }
            else if (yamlNode is YamlSequenceNode)
            {
                return new SequenceYAML(yamlNode as YamlSequenceNode);
            }
            else if (yamlNode is YamlScalarNode)
            {
                return new ScalarYAML(yamlNode as YamlScalarNode);
            }
            else
            {
                throw new InvalidYAMLStyleException(accessPath);
            }
        }

        public abstract IEnumerable<string> ToStringLines();

        protected virtual bool Contains(string key)
        {
            throw new YAMLTypeSlipException<MappingYAML>(AccessPath, this);
        }

        protected virtual bool Contains(int key)
        {
            throw new YAMLTypeSlipException<SequenceYAML>(AccessPath, this);
        }

        protected virtual Value InnerGetter<Value>(string key)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(AccessPath, this);
        }

        protected virtual Value InnerGetter<Value>(int key)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(AccessPath, this);
        }

        protected virtual void InnerSetter<Value>(string key, Value value)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(AccessPath, this);
        }

        protected virtual void InnerSetter<Value>(int key, Value value)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(AccessPath, this);
        }

        protected Value InnerDefaultGetter<Value>(string key, Value defaultVal)
            where Value : YAML
        {
            if (Contains(key))
            {
                return InnerGetter<Value>(key);
            }
            else
            {
                InnerSetter<Value>(key, defaultVal);
                return defaultVal;
            }
        }

        protected Value InnerDefaultGetter<Value>(int key, Value defaultVal)
            where Value : YAML
        {
            if (Contains(key))
            {
                return InnerGetter<Value>(key);
            }
            else
            {
                return defaultVal;
            }
        }

        public ScalarIndexer Scalar { get => new ScalarIndexer(this); }
        public ChildIndexer<SequenceYAML> Sequence { get => new ChildIndexer<SequenceYAML>(this); }
        public ChildIndexer<MappingYAML> Mapping { get => new ChildIndexer<MappingYAML>(this); }

        public YAML this[string key]
        {
            get => new ChildIndexer<YAML>(this)[key];
            set => new ChildIndexer<YAML>(this)[key] = value;
        }

        public YAML this[int key]
        {
            get => new ChildIndexer<YAML>(this)[key];
            set => new ChildIndexer<YAML>(this)[key] = value;
        }

        public YAML this[string key, YAML defaultVal]
        {
            get => new ChildIndexer<YAML>(this)[key, defaultVal];
        }

        public YAML this[int key, YAML defaultVal]
        {
            get => new ChildIndexer<YAML>(this)[key, defaultVal];
        }

        public class ChildIndexer<CHILD> where CHILD : YAML
        {
            public ChildIndexer(YAML parent)
            {
                this.parent = parent;
            }

            YAML parent;

            public CHILD this[string key]
            {
                get => parent.Contains(key) ?
                    parent.InnerGetter<CHILD>(key) :
                    throw new YamlKeyUndefinedException(
                        parent.AccessPath, key.ToString());
                set => parent.InnerSetter<CHILD>(key, value);
            }

            public CHILD this[int key]
            {
                get => parent.Contains(key) ?
                    parent.InnerGetter<CHILD>(key) :
                    throw new YamlKeyUndefinedException(
                        parent.AccessPath, key.ToString());
                set => parent.InnerSetter<CHILD>(key, value);
            }

            public CHILD this[string key, CHILD defaultVal]
            {
                get => parent.InnerDefaultGetter<CHILD>(key, defaultVal);
            }

            public CHILD this[int key, CHILD defaultVal]
            {
                get => parent.InnerDefaultGetter<CHILD>(key, defaultVal);
            }
        }

        public class ScalarIndexer
        {
            public ScalarIndexer(YAML parent)
            {
                this.InnerIndexer = new ChildIndexer<ScalarYAML>(parent);
            }

            ChildIndexer<ScalarYAML> InnerIndexer { get; set; }

            public string this[string key]
            {
                get => InnerIndexer[key].ToString();
                set => InnerIndexer[key] = new ScalarYAML(value);
            }

            public string this[int key]
            {
                get => InnerIndexer[key].ToString();
                set => InnerIndexer[key] = new ScalarYAML(value);
            }

            public string this[string key, string defaultVal]
            {
                get => InnerIndexer[
                    key, new ScalarYAML(defaultVal)].ToString();
            }

            public string this[int key, string defaultVal]
            {
                get => InnerIndexer[
                    key, new ScalarYAML(defaultVal)].ToString();
            }
        }

        public override string ToString() => string.Join("\r\n", ToStringLines());
    }

    class ScalarYAML : YAML
    {
        public ScalarYAML(string value) { Value = value; }
        public ScalarYAML(YamlScalarNode scalar)
        {
            Value = scalar.Value;
        }

        private string Value { get; set; }

        public override IEnumerable<string> ToStringLines()
        {
            return new List<string>() { $"{Value}" };
        }
    }

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
                var childAccessPath = $"{AccessPath}[{entry.Key}]";
                var child = translate(entry.Value, childAccessPath);
                child.AccessPath = childAccessPath;
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
                    AccessPath, key.ToString(), value);
            }
        }
        protected override void InnerSetter<Value>(string key, Value value)
        {
            var childAccessPath = $"{AccessPath}[{key}]";
            value.AccessPath = childAccessPath;
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

    class RootYAML: MappingYAML
    {
        public RootYAML(string pathFromEntryAssembly) :
            base(ReadFile(pathFromEntryAssembly))
        {
            AccessPath = pathFromEntryAssembly;
        }

        public void Reload() => Initialize(ReadFile(AccessPath));

        static YamlMappingNode ReadFile(string pathFromEntryAssembly)
        {
            var rootPath =
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(rootPath, pathFromEntryAssembly);

            using (var reader = new StreamReader(fullPath, Encoding.UTF8))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);
                return (YamlMappingNode)yamlStream.Documents[0].RootNode;
            }
        }
    }

    class YAMLTypeSlipException<ExpectedValue> : Exception
        where ExpectedValue : YAML
    {
        public YAMLTypeSlipException(
            string path, YAML value) :
            base($"\"{path}\"の値は{value.GetType().Name}形式であり" +
                 $"期待されている{typeof(ExpectedValue).Name}形式ではありません。")
        { }

        public YAMLTypeSlipException(
            string path, string key, YAML value) :
            this($"{path}[{key}]", value)
        { }
    }

    class InvalidYAMLStyleException : Exception
    {
        public InvalidYAMLStyleException(string path)
            : base($"YAML\"{path}\"上の形式は想定される形式ではありません。") { }
    }

    class YamlKeyUndefinedException : Exception
    {
        public YamlKeyUndefinedException(string path, string key)
            : base($"YAML\"{path}\"上のキー\"{key}\"は未定義です。") { }
    }
}