// -----------------------------------------------------------------------
// <copyright file="Spec.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using System;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲームインスタンス設定を定義するSpecファイルへのアクセスクラス
    /// Spec file which defines game instance configurations, access class.
    /// </summary>
    public abstract class Spec
    {
        /// <summary>
        /// Gets the Spec ID which shows show to access this instance.
        /// </summary>
        public string ID
        {
            get
            {
                var fromGameDirPath = this.Body.ID;

                // remove the layer name at the head of body ID.
                return string.Join("/", fromGameDirPath.Split('/').Skip(1));
            }
        }

        /// <summary>
        /// Gets the indexer for sub spec.
        /// </summary>
        public SubSpecIndexer SubSpec
        {
            get => new SubSpecIndexer(this.Proxy, this.Body);
        }

        /// <summary>
        /// Gets indexer for integer accessor.
        /// </summary>
        public ValueIndexer<int> Int
        {
            get => new ValueIndexer<int>(
                this.Body,
                "整数",
                (value) => int.Parse(value),
                (value) => value.ToString());
        }

        /// <summary>
        /// Gets indexer for double precision floating point number accessor.
        /// </summary>
        public ValueIndexer<double> Double
        {
            get => new ValueIndexer<double>(
                this.Body,
                "実数",
                (value) => double.Parse(value),
                (value) => value.ToString());
        }

        /// <summary>
        /// Gets indexer for boolean accessor.
        /// </summary>
        public ValueIndexer<bool> Bool
        {
            get => new ValueIndexer<bool>(
                this.Body,
                "真偽値",
                (value) => bool.Parse(value),
                (value) => value.ToString());
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "yes" or "no".
        /// </summary>
        public ValueIndexer<bool> YesNo
        {
            get => new ValueIndexer<bool>(
                this.Body,
                "YES/NO",
                (value) =>
                {
                    if (value.ToLower() == "yes")
                    {
                        return true;
                    }
                    else if (value.ToLower() == "no")
                    {
                        return false;
                    }
                    else
                    {
                        throw new UnexpectedChoiceException(
                            value, "yes", "no");
                    }
                },
                (value) => value ? "yes" : "no");
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "on" or "off".
        /// </summary>
        public ValueIndexer<bool> OnOff
        {
            get => new ValueIndexer<bool>(
                this.Body,
                "ON/OFF",
                (value) =>
                {
                    if (value.ToLower() == "on")
                    {
                        return true;
                    }
                    else if (value.ToLower() == "off")
                    {
                        return false;
                    }
                    else
                    {
                        throw new UnexpectedChoiceException(
                            value, "on", "off");
                    }
                },
                (value) => value ? "on" : "off");
        }

        /// <summary>
        /// Gets or sets the OutSideProxy for file access.
        /// </summary>
        protected OutSideProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets the YAML instance.
        /// </summary>
        protected MappingYAML Body { get; set; }

        /// <summary>
        /// Gets the sub spec.
        /// </summary>
        /// <param name="key">The string key for the sub spec.</param>
        /// <returns>The sub spec.</returns>
        public SpecChild this[string key]
        {
            get => this.SubSpec[key];
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Body.ToString();
        }

        /// <summary>
        /// Indexer class for SubSpec.
        /// </summary>
        public class SubSpecIndexer
        {
            private OutSideProxy proxy;
            private YAML body;

            /// <summary>
            /// Initializes a new instance of the <see cref="SubSpecIndexer"/> class.
            /// </summary>
            /// <param name="proxy">OutSideProxy for file access.</param>
            /// <param name="body">The YAML.</param>
            internal SubSpecIndexer(OutSideProxy proxy, YAML body)
            {
                this.body = body;
            }

            /// <summary>
            /// Indexer for sub spec.
            /// </summary>
            /// <param name="key">The string key for the sub spec.</param>
            /// <returns>The sub spec.</returns>
            public SpecChild this[string key]
            {
                get
                {
                    var childYaml = this.body.Mapping[key, new MappingYAML()];
                    childYaml.ID = $"{this.body.ID}[{key}]";
                    return new SpecChild(this.proxy, childYaml);
                }
            }
        }

        /// <summary>
        /// Indexer class for Int, Double, and Bool.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        public class ValueIndexer<T>
        {
            private YAML body;
            private string type;
            private Func<string, T> getter;
            private Func<T, string> setter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueIndexer{T}"/> class.
            /// </summary>
            /// <param name="body">The YAML.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from string.</param>
            /// <param name="setter">Translation function to string.</param>
            internal ValueIndexer(
                YAML body,
                string type,
                Func<string, T> getter,
                Func<T, string> setter)
            {
                this.body = body;
                this.type = type;
                this.getter = getter;
                this.setter = setter;
            }

            /// <summary>
            /// Indexer for value.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <returns>The translated value.</returns>
            public T this[string key]
            {
                get
                {
                    try
                    {
                        return this.getter(this.body.Scalar[key].ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.body.ID}[{key}]", this.type, ex);
                    }
                }

                set
                {
                    try
                    {
                        this.body[key] = new ScalarYAML(this.setter(value));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.body.ID}[{key}]", this.type, ex);
                    }
                }
            }

            /// <summary>
            /// Indexer for value with default.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <param name="defaultVal">The default value.</param>
            /// <returns>The translated value.</returns>
            public T this[string key, T defaultVal]
            {
                get
                {
                    try
                    {
                        return this.getter(this.body.Scalar[
                            key, new ScalarYAML(this.setter(defaultVal))]
                            .ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.body.ID}[{key}]", this.type, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Specの持つ値が不正である場合の例外
        /// The exception that is thrown when an attempt to
        /// access a spec value or a child which has invalid definition.
        /// </summary>
        public class InvalidSpecAccessException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidSpecAccessException"/> class.
            /// </summary>
            /// <param name="id">The spec access ID.</param>
            /// <param name="type">The access type.</param>
            /// <param name="cause">The innerException.</param>
            internal InvalidSpecAccessException(string id, string type, Exception cause)
                : base($"{id}での{type}アクセス時に問題が発生しました。", cause)
            {
            }
        }

        /// <summary>
        /// 複数の選択肢を期待するSpecアクセスにおいて、選択肢以外の値がある場合の例外
        /// The exception that is thrown when an attempt to
        /// access a choice type spec value with unexpected choice.
        /// </summary>
        public class UnexpectedChoiceException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UnexpectedChoiceException"/> class.
            /// </summary>
            /// <param name="actual">The unexpected value.</param>
            /// <param name="choices">The choices.</param>
            internal UnexpectedChoiceException(string actual, params string[] choices)
                : base(
                      $"{actual}は期待される選択肢" +
                      $"{string.Join(", ", choices)}に含まれていません。")
            {
            }
        }
    }
}
