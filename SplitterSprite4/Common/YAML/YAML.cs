// -----------------------------------------------------------------------
// <copyright file="YAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.RepresentationModel;

    /// <summary>
    /// 読み書き、ファイル保存機能を備えたYAML処理クラス
    /// YAML access class with read, write, and save functions.
    /// </summary>
    public abstract class YAML
    {
        /// <summary>
        /// Gets or sets YAML ID which shows how to access this instance.
        /// </summary>
        public string ID { get; set; } = "undefined";

        /// <summary>
        /// Gets indexer for scalar type YAML.
        /// </summary>
        public ChildIndexer<ScalarYAML> Scalar
        {
            get => new ChildIndexer<ScalarYAML>(this);
        }

        /// <summary>
        /// Gets indexer for sequence type YAML.
        /// </summary>
        public ChildIndexer<SequenceYAML> Sequence
        {
            get => new ChildIndexer<SequenceYAML>(this);
        }

        /// <summary>
        /// Gets indexer for mapping type YAML.
        /// </summary>
        public ChildIndexer<MappingYAML> Mapping
        {
            get => new ChildIndexer<MappingYAML>(this);
        }

        /// <summary>
        /// Indexer access as mapping type YAML.
        /// </summary>
        /// <param name="key">The string key for mapping.</param>
        /// <returns>The child YAML.</returns>
        public YAML this[string key]
        {
            get => new ChildIndexer<YAML>(this)[key];
            set => new ChildIndexer<YAML>(this)[key] = value;
        }

        /// <summary>
        /// Indexer access as sequence type YAML.
        /// </summary>
        /// <param name="key">The integer key for sequence.</param>
        /// <returns>The child YAML.</returns>
        public YAML this[int key]
        {
            get => new ChildIndexer<YAML>(this)[key];
            set => new ChildIndexer<YAML>(this)[key] = value;
        }

        /// <summary>
        /// Indexer access as mapping type YAML.
        /// </summary>
        /// <param name="key">The string key for mapping.</param>
        /// <param name="defaultVal">The default value YAML.</param>
        /// <returns>The child YAML.</returns>
        public YAML this[string key, YAML defaultVal]
        {
            get => new ChildIndexer<YAML>(this)[key, defaultVal];
        }

        /// <summary>
        /// Indexer access as sequence type YAML.
        /// </summary>
        /// <param name="key">The integer key for sequence.</param>
        /// <param name="defaultVal">The default value YAML.</param>
        /// <returns>The child YAML.</returns>
        public YAML this[int key, YAML defaultVal]
        {
            get => new ChildIndexer<YAML>(this)[key, defaultVal];
        }

        /// <summary>
        /// YamlDotNetオブジェクトを独自形式に変換
        /// Translate YamlDotNet instance to YAML instance.
        /// </summary>
        /// <param name="id">The id for the translated instance.</param>
        /// <param name="yamlNode">The target YamlDotNet instance.</param>
        /// <returns>The translated YAML instance.</returns>
        public static YAML Translate(string id, YamlNode yamlNode)
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

        /// <summary>
        /// YAML表現の各行を返すイテレータ
        /// Iterator for each lines in the YAML body.
        /// </summary>
        /// <returns>The iterator.</returns>
        public abstract IEnumerable<string> ToStringLines();

        /// <inheritdoc/>
        public override string ToString() =>
            string.Join("\n", this.ToStringLines());

        /// <summary>
        /// 文字列キーについて値を保持しているか否か
        /// Determines whether the YAML contains the specified string key.
        /// </summary>
        /// <param name="key">The string key.</param>
        /// <returns>The result of determination.</returns>
        protected virtual bool Contains(string key)
        {
            throw new YAMLTypeSlipException<MappingYAML>(this.ID, this);
        }

        /// <summary>
        /// 整数キーについて値を保持しているか否か
        /// Determines whether the YAML contains the specified int key.
        /// </summary>
        /// <param name="key">The integer key.</param>
        /// <returns>The result of determination.</returns>
        protected virtual bool Contains(int key)
        {
            throw new YAMLTypeSlipException<SequenceYAML>(this.ID, this);
        }

        /// <summary>
        /// 文字列インデクサによる値返却
        /// Getter for string indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The string key.</param>
        /// <returns>The YAML value.</returns>
        protected virtual T_Value InnerGetter<T_Value>(string key)
            where T_Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(this.ID, this);
        }

        /// <summary>
        /// 整数インデクサによる値返却
        /// Getter for integer indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The integer key.</param>
        /// <returns>The YAML value.</returns>
        protected virtual T_Value InnerGetter<T_Value>(int key)
            where T_Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(this.ID, this);
        }

        /// <summary>
        /// 文字列インデクサによる値更新
        /// Setter for string indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The string key.</param>
        /// <param name="value">The value for update.</param>
        protected virtual void InnerSetter<T_Value>(string key, T_Value value)
            where T_Value : YAML
        {
            throw new YAMLTypeSlipException<MappingYAML>(this.ID, this);
        }

        /// <summary>
        /// 整数インデクサによる値更新
        /// Setter for integer indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The integer key.</param>
        /// <param name="value">The value for update.</param>
        protected virtual void InnerSetter<T_Value>(int key, T_Value value)
            where T_Value : YAML
        {
            throw new YAMLTypeSlipException<SequenceYAML>(this.ID, this);
        }

        /// <summary>
        /// 文字列インデクサでのデフォルト値アクセスを実現する
        /// Getter with default value for string indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The string key.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>The YAML value.</returns>
        protected T_Value InnerDefaultGetter<T_Value>(string key, T_Value defaultVal)
            where T_Value : YAML
        {
            if (this.Contains(key))
            {
                return this.InnerGetter<T_Value>(key);
            }
            else
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// 整数インデクサでのデフォルト値アクセスを実現する
        /// Getter with default value for integer indexer.
        /// </summary>
        /// <typeparam name="T_Value">
        /// 返却されるYAML型
        /// The return yaml type.
        /// </typeparam>
        /// <param name="key">The integer key.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>The YAML value.</returns>
        protected T_Value InnerDefaultGetter<T_Value>(int key, T_Value defaultVal)
            where T_Value : YAML
        {
            if (this.Contains(key))
            {
                return this.InnerGetter<T_Value>(key);
            }
            else
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// Indexer class for child YAML.
        /// </summary>
        /// <typeparam name="T_CHILD">Type of child YAML.</typeparam>
        public class ChildIndexer<T_CHILD>
            where T_CHILD : YAML
        {
            private YAML parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ChildIndexer{T_CHILD}"/> class.
            /// </summary>
            /// <param name="parent">The parent YAML.</param>
            public ChildIndexer(YAML parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Indexer as mapping type YAML.
            /// </summary>
            /// <param name="key">The string key for mapping YAML.</param>
            /// <returns>The child YAML.</returns>
            public T_CHILD this[string key]
            {
                // 文字列インデクサでの値取得
                get => this.parent.Contains(key) ?
                    this.parent.InnerGetter<T_CHILD>(key) :
                    throw new YAMLKeyUndefinedException(
                        this.parent.ID, key.ToString());

                // 文字列インデクサでの値更新
                set => this.parent.InnerSetter<T_CHILD>(key, value);
            }

            /// <summary>
            /// Indexer as sequence type YAML.
            /// </summary>
            /// <param name="key">The integer key for sequence YAML.</param>
            /// <returns>The child YAML.</returns>
            public T_CHILD this[int key]
            {
                // 整数インデクサでの値取得
                get => this.parent.Contains(key) ?
                    this.parent.InnerGetter<T_CHILD>(key) :
                    throw new YAMLKeyUndefinedException(
                        this.parent.ID, key.ToString());

                // 整数インデクサでの値更新
                set => this.parent.InnerSetter<T_CHILD>(key, value);
            }

            /// <summary>
            /// Indexer as mapping type YAML.
            /// </summary>
            /// <param name="key">The string key for mapping YAML.</param>
            /// <param name="defaultVal">The default YAML.</param>
            /// <returns>The child YAML.</returns>
            public T_CHILD this[string key, T_CHILD defaultVal]
            {
                get => this.parent.InnerDefaultGetter<T_CHILD>(key, defaultVal);
            }

            /// <summary>
            /// Indexer as integer type YAML.
            /// </summary>
            /// <param name="key">The integer key for sequence YAML.</param>
            /// <param name="defaultVal">The default YAML.</param>
            /// <returns>The child YAML.</returns>
            public T_CHILD this[int key, T_CHILD defaultVal]
            {
                get => this.parent.InnerDefaultGetter<T_CHILD>(key, defaultVal);
            }
        }

        /// <summary>
        /// YAMLアクセス時に期待する型と実際の型が異なる際の例外
        /// The exception that is thrown when an attempt to
        /// access a yaml child that does not have expected type.
        /// </summary>
        /// <typeparam name="TExpectedValue">The expected yaml type.</typeparam>
        public class YAMLTypeSlipException<TExpectedValue> : Exception
            where TExpectedValue : YAML
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="YAMLTypeSlipException{TExpectedValue}"/> class.
            /// </summary>
            /// <param name="id">The yaml's ID.</param>
            /// <param name="value">The child yaml.</param>
            public YAMLTypeSlipException(string id, YAML value)
                : base(
                    $"\"{id}\"の値は{value.GetType().Name}形式であり" +
                    $"期待されている{typeof(TExpectedValue).Name}形式では" +
                    $"ありません。")
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="YAMLTypeSlipException{TExpectedValue}"/> class.
            /// </summary>
            /// <param name="id">The yaml's id.</param>
            /// <param name="key">The string key.</param>
            /// <param name="value">The child YAML.</param>
            public YAMLTypeSlipException(string id, string key, YAML value)
                : this($"{id}[{key}]", value)
            {
            }
        }

        /// <summary>
        /// YAML上のキーアクセス対象が存在しない際の例外
        /// The exception that is thrown when an attempt to
        /// access a key that does not exist on the yaml file.
        /// </summary>
        public class YAMLKeyUndefinedException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="YAMLKeyUndefinedException"/> class.
            /// </summary>
            /// <param name="id">The yaml's id.</param>
            /// <param name="key">The string key.</param>
            public YAMLKeyUndefinedException(string id, string key)
                : base($"YAML\"{id}\"上のキー\"{key}\"は未定義です。")
            {
            }
        }

        /// <summary>
        /// ロードしたYAMLが想定したパターン外であった際の例外
        /// The exception that is thrown when an attempt to
        /// parse a YAML file that contains unexpected pattern.
        /// </summary>
        public class InvalidYAMLStyleException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidYAMLStyleException"/> class.
            /// </summary>
            /// <param name="id">The yaml's id.</param>
            public InvalidYAMLStyleException(string id)
                : base($"YAML\"{id}\"上の形式は想定される形式ではありません。")
            {
            }
        }
    }
}