// -----------------------------------------------------------------------
// <copyright file="Spec.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec.Indexer;
    using MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲームインスタンス設定を定義するSpecファイルへのアクセスクラス
    /// Spec file which defines game instance configurations, access class.
    /// </summary>
    public abstract class Spec
    {
        internal static readonly string MAGICWORDDECORATOR = "__";
        internal static readonly string HIDDEN = "HIDDEN";
        internal static readonly string HELD = "HELD";

        /// <summary>
        /// Gets an os-agnostic path of this spec file.
        /// </summary>
        public abstract AgnosticPath Path { get; }

        /// <summary>
        /// Gets a Spec for inheritance.
        /// If a property is not defined in this spec,
        /// the base spec's property is referred.
        /// If the base spec is not defined, this property is null.
        /// </summary>
        public abstract Spec Base { get; }

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
            get => new SubSpecIndexer(this, false);
        }

        /// <summary>
        /// Gets the indexer for spec child.
        /// </summary>
        public SpecChildIndexer Child
        {
            get => new SpecChildIndexer(this);
        }

        /// <summary>
        /// Gets indexer for integer accessor.
        /// </summary>
        public IntIndexer Int
        {
            get => new IntIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for double precision floating point number accessor.
        /// </summary>
        public DoubleIndexer Double
        {
            get => new DoubleIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for boolean accessor.
        /// </summary>
        public BoolIndexer Bool
        {
            get => new BoolIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "yes" or "no".
        /// </summary>
        public YesNoIndexer YesNo
        {
            get => new YesNoIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "on" or "off".
        /// </summary>
        public OnOffIndexer OnOff
        {
            get => new OnOffIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for (int x, int y) tuple accessor.
        /// </summary>
        public Int2Indexer Int2
        {
            get => new Int2Indexer(this, false);
        }

        /// <summary>
        /// Gets indexer for (int x, int y, int z) tuple accessor.
        /// </summary>
        public Int3Indexer Int3
        {
            get => new Int3Indexer(this, false);
        }

        /// <summary>
        /// Gets indexer for (double x, double y) tuple accessor.
        /// </summary>
        public Double2Indexer Double2
        {
            get => new Double2Indexer(this, false);
        }

        /// <summary>
        /// Gets indexer for (double x, double y, double z) tuple accessor.
        /// </summary>
        public Double3Indexer Double3
        {
            get => new Double3Indexer(this, false);
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// </summary>
        public KeywordIndexer Keyword
        {
            get => new KeywordIndexer(this, false);
        }

        /// <summary>
        /// Gets indexer for multi line string accessor.
        /// </summary>
        public TextIndexer Text
        {
            get => new TextIndexer(this, false);
        }

        /// <summary>
        /// Gets KeyDefiner for starting point of DictIndexer.
        /// </summary>
        public KeyDefiner Dict
        {
            get => new KeyDefiner(this);
        }

        /// <summary>
        /// Gets the YAML instance of the spec.
        /// </summary>
        public abstract MappingYAML Body { get; }

        /// <summary>
        /// Gets the YAML instance of properties.
        /// </summary>
        public abstract MappingYAML Properties { get; }

        /// <summary>
        /// Gets the OurSideProxy for file or spec pool access.
        /// </summary>
        internal abstract OutSideProxy Proxy { get; }

        /// <summary>
        /// Gets the sub spec.
        /// </summary>
        /// <param name="key">The string key for the sub spec.</param>
        /// <returns>The sub spec.</returns>
        public SubSpec this[string key]
        {
            get => this.SubSpec[key];
        }

        /// <summary>
        /// Comma separated values are used for molding.
        /// Therefore, if default value contains comma,
        /// encode commas in the default value.
        /// </summary>
        /// <param name="target">Encoding target string.</param>
        /// <returns>Encoded string.</returns>
        public static string EncodeCommas(string target)
        {
            // Mold時の値は複数のパラメータをカンマ区切りでつなげる。
            // そのため、デフォルト値自体がカンマを含む場合には
            // カンマを含まないようにエンコードして区別する。
            // Comma separated values are used for molding.
            // Therefore, if default value contains comma,
            // encode commas in the default value.
            return target.Replace("\\", "\\\\").Replace(",", "\\c");
        }

        /// <summary>
        /// Decode an encoded default value for molding.
        /// </summary>
        /// <param name="target">Decoding target string.</param>
        /// <returns>Decoded string.</returns>
        public static string DecodeCommas(string target)
        {
            var ret = string.Empty;
            var escaped = false;

            foreach (char c in target)
            {
                if (escaped)
                {
                    switch (c)
                    {
                        case '\\':
                            ret += '\\';
                            break;
                        case 'c':
                            ret += ',';
                            break;
                        default:
                            throw new InvalidDefaultValEncoding(target);
                    }

                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                ret += c;
            }

            return ret;
        }

        /// <summary>
        /// Encode type instance into string for scalar value in yaml.
        /// </summary>
        /// <param name="type">Encode target type.</param>
        /// <returns>Encoded string.</returns>
        public static string EncodeType(Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }

        /// <summary>
        /// Decode string into type instance from scalar value in yaml.
        /// </summary>
        /// <param name="code">Encoded string.</param>
        /// <returns>Decoded Type instance.</returns>
        public static Type DecodeType(string code)
        {
            return Type.GetType(code, true);
        }

        /// <summary>
        /// spec上の値は"en-US"カルチャでのパース、文字列化に限定することで、
        /// 環境に依存しない処理とする。
        /// Strings for spec is translated only with "en-US" culture,
        /// for independent processing from environment.
        /// </summary>
        /// <typeparam name="TResult">Function's result type.</typeparam>
        /// <param name="target">Function which will be decorated.</param>
        /// <returns>Decorated function.</returns>
        public static Func<TResult>
            FixCulture<TResult>(Func<TResult> target)
        {
            var specCulture = new CultureInfo("en-US");

            return () =>
            {
                var prevCInfo = Thread.CurrentThread.CurrentCulture;

                try
                {
                    Thread.CurrentThread.CurrentCulture = specCulture;
                    return target();
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = prevCInfo;
                }
            };
        }

        /// <summary>
        /// spec上の値は"en-US"カルチャでのパース、文字列化に限定することで、
        /// 環境に依存しない処理とする。
        /// Strings for spec is translated only with "en-US" culture,
        /// for independent processing from environment.
        /// </summary>
        /// <typeparam name="T1">Function's first arg type.</typeparam>
        /// <typeparam name="T2">Function's second arg type.</typeparam>
        /// <typeparam name="TResult">Function's result type.</typeparam>
        /// <param name="target">Function which will be decorated.</param>
        /// <returns>Decorated function.</returns>
        public static Func<T1, T2, TResult>
            FixCulture<T1, T2, TResult>(Func<T1, T2, TResult> target)
        {
            var specCulture = new CultureInfo("en-US");

            return (x, y) =>
            {
                var prevCInfo = Thread.CurrentThread.CurrentCulture;

                try
                {
                    Thread.CurrentThread.CurrentCulture = specCulture;
                    return target(x, y);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = prevCInfo;
                }
            };
        }

        /// <summary>
        /// Gets indexer for ranged integer accessor.
        /// </summary>
        /// <param name="parenthesisOpen">
        /// ISO 31-11に則った区間表現の左括弧。'(', '[', ']'のいずれか。
        /// Parenthesis open character for interval in accordance with ISO 31-11.
        /// The character must be in '(', '[', or ']'.
        /// </param>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <param name="parenthesisClose">
        /// ISO 31-11に則った区間表現の右括弧。')', ']', '['のいずれか。
        /// Parenthesis close character for interval in accordance with ISO 31-11.
        /// The character must be in ')', ']', or '['.
        /// </param>
        /// <returns>Indexer for ranged interger accessor.</returns>
        public RangeIndexer Range(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return new RangeIndexer(
                this,
                parenthesisOpen,
                leftBound,
                rightBound,
                parenthesisClose,
                false);
        }

        /// <summary>
        /// Gets indexer for double presicion floating point number accessor
        /// in an interval.
        /// </summary>
        /// <param name="parenthesisOpen">
        /// ISO 31-11に則った区間表現の左括弧。'(', '[', ']'のいずれか。
        /// Parenthesis open character for interval in accordance with ISO 31-11.
        /// The character must be in '(', '[', or ']'.
        /// </param>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <param name="parenthesisClose">
        /// ISO 31-11に則った区間表現の右括弧。')', ']', '['のいずれか。
        /// Parenthesis close character for interval in accordance with ISO 31-11.
        /// The character must be in ')', ']', or '['.
        /// </param>
        /// <returns>Indexer for floating point number accesssor in an interval.</returns>
        public IntervalIndexer Interval(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return new IntervalIndexer(
                this,
                parenthesisOpen,
                leftBound,
                rightBound,
                parenthesisClose,
                false);
        }

        /// <summary>
        /// Gets indexer for ranged integer accessor.
        /// </summary>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>Indexer for ranged interger accessor.</returns>
        public RangeIndexer Range(
            double leftBound,
            double rightBound)
        {
            return this.Range('[', leftBound, rightBound, ')');
        }

        /// <summary>
        /// Gets indexer for ranged integer accessor.
        /// </summary>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>Indexer for ranged interger accessor.</returns>
        public RangeIndexer Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// The string length is bounded.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>Indexer for limited keyword accessor.</returns>
        public LimitedKeywordIndexer LimitedKeyword(int limit)
        {
            return new LimitedKeywordIndexer(this, limit, false);
        }

        /// <summary>
        /// Gets indexer for SpawnerRoot instance from another spec file.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerRoot type.</typeparam>
        /// <returns>Indexer for SpawnerRoot instance.</returns>
        public ExteriorIndexer<T> Exterior<T>()
            where T : ISpawnerRoot<object>
        {
            return new ExteriorIndexer<T>(this, false);
        }

        /// <summary>
        /// Gets indexer for SpawnerDir instance from a directory.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerRoot type.</typeparam>
        /// <returns>Indexer for SpawnerDir instance.</returns>
        public ExteriorDirIndexer<T> ExteriorDir<T>()
            where T : ISpawnerRoot<object>
        {
            return new ExteriorDirIndexer<T>(this, false);
        }

        /// <summary>
        /// Gets indexer for SpawnerChild instance from spec child.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerChild type.</typeparam>
        /// <returns>Indexer for SpawnerChild instance.</returns>
        public InteriorIndexer<T> Interior<T>()
            where T : ISpawnerChild<object>
        {
            return new InteriorIndexer<T>(this, false);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Body.ToString(true);
        }

        /// <summary>
        /// Create instance for molding default.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected spawner type.</typeparam>
        /// <returns>Dummy spawner instance.</returns>
        public T_Spawner MoldingDefault<T_Spawner>()
            where T_Spawner : ISpawner<object>
        {
            var spawnerType = typeof(T_Spawner);

            var candidateTypes = Spawner.AllLoadedTypes(this.Proxy);
            foreach (var type in candidateTypes)
            {
                try
                {
                    // 最初にValidation成功したTypeによるインスタンスを返す
                    // Instance from first valid type is returned.
                    return Spawner.ValidateSpawnerType<T_Spawner>(type);
                }
                catch (Exception)
                {
                    // Validation失敗となるTypeは無視する
                    // Invalid types are ignored.
                    continue;
                }
            }

            throw new Spec.InvalidSpecDefinitionException(
                $"正当なサブクラスを持たないSpawnerクラス" +
                $"\"{spawnerType.Name}\"が使用されています。");
        }

        public void SetMagicWord(string key, string word)
        {
            // Magic word decorated word cannot be magic word.
            if (CheckMagicWord(word))
            {
                throw new InvalidMagicWordException(word);
            }

            lock (this.Properties)
            {
                try
                {
                    var scalarVal = DecorateAsMagicWord(word);
                    this.Properties[key] =
                        new ScalarYAML(scalarVal);
                }
                catch (Exception ex)
                {
                    throw new Spec.InvalidSpecAccessException(
                        $"{this.Properties.ID}[{key}]",
                        "Magic word設定",
                        ex);
                }
            }
        }

        public bool IsHidden(string key)
        {
            return this.IsMagic(key, HIDDEN);
        }

        public bool IsHeld(string key)
        {
            return this.IsMagic(key, HELD);
        }

        internal static bool CheckMagicWord(string val)
        {
            return
                val.StartsWith(MAGICWORDDECORATOR) &&
                val.EndsWith(MAGICWORDDECORATOR);
        }

        internal static string DecorateAsMagicWord(string val)
        {
            return MAGICWORDDECORATOR + val + MAGICWORDDECORATOR;
        }

        internal static string UndecorateAsMagicWord(string val)
        {
            return val.Substring(
                MAGICWORDDECORATOR.Length,
                val.Length - (2 * MAGICWORDDECORATOR.Length));
        }

        internal static string FromScalar(string scalar)
        {
            // Basically, the original scalar string is used.
            if (!CheckMagicWord(scalar))
            {
                return scalar;
            }

            var innerStr = UndecorateAsMagicWord(scalar);

            // In __xxx__ case, xxx is used as magic word.
            if (!CheckMagicWord(innerStr))
            {
                throw new MagicWordException(innerStr);
            }

            // If you want to use __xxx__ as non-magic word,
            // ____xxx____ in ScalarYAML works.
            return innerStr;
        }

        internal static string ToScalar(string value)
        {
            // Basically, the original value is used.
            if (!CheckMagicWord(value))
            {
                return value;
            }

            // In __xxx__ case, ____xxx____ will be in ScalarYAML.
            return DecorateAsMagicWord(value);
        }

        internal static string ToMagicScalar(string word)
        {
            // "__xxx__" is used for magic word "xxx".
            return DecorateAsMagicWord(word);
        }

        private bool IsMagic(string key, string magicWord)
        {
            try
            {
                FromScalar(this.Properties.Scalar[key].Value);
            }
            catch (YAML.YAMLTypeSlipException<ScalarYAML>)
            {
                return false;
            }
            catch (YAML.YAMLKeyUndefinedException)
            {
                return false;
            }
            catch (MagicWordException ex)
            {
                return ex.Word == magicWord;
            }

            return false;
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
        /// Specアクセス時の型定義が不正である場合の例外。
        /// The exception that is thrown when an attempt to
        /// define a invalid spec type.
        /// </summary>
        public class InvalidSpecDefinitionException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidSpecDefinitionException"/> class.
            /// </summary>
            /// <param name="message">The exception message.</param>
            internal InvalidSpecDefinitionException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidSpecDefinitionException"/> class.
            /// </summary>
            /// <param name="message">The exception message.</param>
            /// <param name="cause">The inner exception.</param>
            internal InvalidSpecDefinitionException(string message, Exception cause)
                : base(message, cause)
            {
            }
        }

        /// <summary>
        /// Spec上の文字列を指定の型に変更する際の正当性チェックが失敗した際の例外。
        /// The exception that is thrown when an attempt to
        /// validate a invalid spec string.
        /// </summary>
        public class ValidationError : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValidationError"/> class.
            /// </summary>
            internal ValidationError()
                : base()
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

        /// <summary>
        /// Mold時のデフォルト値エンコードが不正である場合の例外。
        /// The exception that is thrown when an attempt to
        /// decode an invalid default value string for molding.
        /// </summary>
        public class InvalidDefaultValEncoding : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidDefaultValEncoding"/> class.
            /// </summary>
            /// <param name="target">The encoded string.</param>
            internal InvalidDefaultValEncoding(string target)
                : base(
                      $"\"{target}\"はMold時のデフォルト値エンコードとして不正です。")
            {
            }
        }

        // Exception for special definition values.
        internal class MagicWordException : Exception
        {
            internal MagicWordException(string word)
            {
                this.Word = word;
            }

            internal string Word { get; }
        }

        internal class InvalidMagicWordException : Exception
        {
            internal InvalidMagicWordException(string word)
                : base($"{word}はMagic Wordの対象として不正です。")
            {
            }
        }

        internal class HiddenKeyException : Exception
        {
            internal HiddenKeyException(string id, string key)
                : base($"\"{id}\"上のキー\"{key}\"は隠蔽されています。")
            {
            }
        }

        internal class HeldKeyException : Exception
        {
            internal HeldKeyException(string id, string key)
                : base($"\"{id}\"上のキー\"{key}\"は空の値のままキーが保持されています。")
            {
            }
        }
    }
}
