using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    // 読み書き、ファイル保存機能を備えたYAML処理クラス
    public abstract class YAML
    {
        // YamlDotNetオブジェクトを独自形式に変換
        public static YAML translate(string id, YamlNode yamlNode)
        {
            if (yamlNode is YamlMappingNode)
            {
                return new MappingYAML(id, yamlNode as YamlMappingNode);
            }
            else if (yamlNode is YamlSequenceNode)
            {
                return new SequenceYAML(id, yamlNode as YamlSequenceNode);
            }
            else if (yamlNode is YamlScalarNode)
            {
                return new ScalarYAML(id, yamlNode as YamlScalarNode);
            }
            else
            {
                throw new InvalidYAMLStyleException(id);
            }
        }

        // どのYAMLファイルのどのフィールドで
        // アクセスしたYAMLオブジェクトかを表現
        public string ID { get; set; } = "undefined";

        // YAML表現の各行を出力するイテレータ
        public abstract IEnumerable<string> ToStringLines();

        // 文字列インデクサアクセスが可能かどうか
        protected virtual bool Contains(string key)
        {
            throw new YAMLTypeSlipException<MappingYAML>(ID, this);
        }

        // 整数インデクサアクセスが可能かどうか
        protected virtual bool Contains(int key)
        {
            throw new YAMLTypeSlipException<SequenceYAML>(ID, this);
        }

        // 文字列インデクサによる値返却
        protected virtual Value InnerGetter<Value>(string key)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(ID, this);
        }

        // 整数インデクサによる値返却
        protected virtual Value InnerGetter<Value>(int key)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(ID, this);
        }

        // 文字列インデクサによる値更新
        protected virtual void InnerSetter<Value>(string key, Value value)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(ID, this);
        }

        // 整数インデクサによる値更新
        protected virtual void InnerSetter<Value>(int key, Value value)
            where Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(ID, this);
        }

        // 文字列インデクサでのデフォルト値アクセスを実現する
        protected Value InnerDefaultGetter<Value>(string key, Value defaultVal)
            where Value : YAML
        {
            // 指定されたキーを持っていなければデフォルト値を返す
            if (Contains(key))
            {
                return InnerGetter<Value>(key);
            }
            else
            {
                return defaultVal;
            }
        }

        // 整数インデクサでのデフォルト値アクセスを実現する
        protected Value InnerDefaultGetter<Value>(int key, Value defaultVal)
            where Value : YAML
        {
            // 指定されたキーを持っていなければデフォルト値を返す
            if (Contains(key))
            {
                return InnerGetter<Value>(key);
            }
            else
            {
                return defaultVal;
            }
        }

        // Scalar, Sequence, Mappingの型指定をしたインデクサアクセス
        public ChildIndexer<ScalarYAML> Scalar
        {
            get => new ChildIndexer<ScalarYAML>(this);
        }

        public ChildIndexer<SequenceYAML> Sequence
        {
            get => new ChildIndexer<SequenceYAML>(this);
        }

        public ChildIndexer<MappingYAML> Mapping
        {
            get => new ChildIndexer<MappingYAML>(this);
        }

        // 型の指定をしないインデクサアクセス
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

        // 型の指定をしないデフォルト値つきインデクサアクセス
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
                // 文字列インデクサでの値取得
                get => parent.Contains(key) ?
                    parent.InnerGetter<CHILD>(key) :
                    throw new YAMLKeyUndefinedException(
                        parent.ID, key.ToString());
                // 文字列インデクサでの値更新
                set => parent.InnerSetter<CHILD>(key, value);
            }

            public CHILD this[int key]
            {
                // 整数インデクサでの値取得
                get => parent.Contains(key) ?
                    parent.InnerGetter<CHILD>(key) :
                    throw new YAMLKeyUndefinedException(
                        parent.ID, key.ToString());
                // 整数インデクサでの値更新
                set => parent.InnerSetter<CHILD>(key, value);
            }

            // 文字列インデクサでのデフォルト値つき値取得
            public CHILD this[string key, CHILD defaultVal]
            {
                get => parent.InnerDefaultGetter<CHILD>(key, defaultVal);
            }

            // 整数インデクサでのデフォルト値つき値取得
            public CHILD this[int key, CHILD defaultVal]
            {
                get => parent.InnerDefaultGetter<CHILD>(key, defaultVal);
            }
        }

        public override string ToString() =>
            string.Join("\n", ToStringLines());
    }
}