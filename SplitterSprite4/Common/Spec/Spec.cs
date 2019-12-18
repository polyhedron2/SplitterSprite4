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
        /// Gets a YAML for analyze access key and type in a spawner.
        /// If this is not molding, it returns null.
        /// </summary>
        public abstract MappingYAML Mold { get; }

        /// <summary>
        /// Gets a value indicating whether this spec is molding or not.
        /// </summary>
        public bool IsMolding
        {
            get => this.Mold != null;
        }

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
            get => new SubSpecIndexer(this);
        }

        /// <summary>
        /// Gets indexer for integer accessor.
        /// </summary>
        public ValueIndexer<int> Int
        {
            get => new ValueIndexer<int>(
                this,
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
                this,
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
                this,
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
                this,
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
                this,
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
        /// Gets the YAML instance.
        /// </summary>
        public abstract MappingYAML Body { get; }

        /// <summary>
        /// Gets the OutSideProxy for file access.
        /// </summary>
        public abstract OutSideProxy Proxy { get; }

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
            return this.Body.ToString(true);
        }

        /// <summary>
        /// Indexer class for SubSpec.
        /// </summary>
        public class SubSpecIndexer
        {
            private Spec parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="SubSpecIndexer"/> class.
            /// </summary>
            /// <param name="parent">The parent spec instance.</param>
            internal SubSpecIndexer(Spec parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Indexer for sub spec.
            /// </summary>
            /// <param name="key">The string key for the sub spec.</param>
            /// <returns>The sub spec.</returns>
            public SpecChild this[string key]
            {
                get => new SpecChild(this.parent, key);
            }
        }

        /// <summary>
        /// Indexer class for Int, Double, and Bool.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        public class ValueIndexer<T>
        {
            private Spec parent;
            private string type;
            private Func<string, T> getter;
            private Func<T, string> setter;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from string.</param>
            /// <param name="setter">Translation function to string.</param>
            internal ValueIndexer(
                Spec parent,
                string type,
                Func<string, T> getter,
                Func<T, string> setter)
            {
                this.parent = parent;
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
                        return this.getter(
                            this.parent.Body.Scalar[key].ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.parent.Body.ID}[{key}]", this.type, ex);
                    }
                }

                set
                {
                    try
                    {
                        this.parent.Body[key] =
                            new ScalarYAML(this.setter(value));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.parent.Body.ID}[{key}]", this.type, ex);
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
                        return this.getter(this.parent.Body.Scalar[
                            key, new ScalarYAML(this.setter(defaultVal))]
                            .ToString());
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.parent.Body.ID}[{key}]", this.type, ex);
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
