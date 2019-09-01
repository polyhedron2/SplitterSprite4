using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
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
}