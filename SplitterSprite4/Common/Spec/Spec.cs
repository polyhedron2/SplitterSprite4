// -----------------------------------------------------------------------
// <copyright file="Spec.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲームインスタンス設定を定義するSpecファイルへのアクセスクラス
    /// Spec file which defines game instance configurations, access class.
    /// </summary>
    public abstract class Spec
    {
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
            get => new SubSpecIndexer(this);
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
        public LiteralIndexer<int> Int
        {
            get => new LiteralIndexer<int>(
                this,
                "整数",
                (value) => int.Parse(value),
                (value) => value.ToString(),
                "Int",
                0);
        }

        /// <summary>
        /// Gets indexer for double precision floating point number accessor.
        /// </summary>
        public LiteralIndexer<double> Double
        {
            get => new LiteralIndexer<double>(
                this,
                "実数",
                (value) => double.Parse(value),
                (value) => value.ToString(),
                "Double",
                0.0);
        }

        /// <summary>
        /// Gets indexer for boolean accessor.
        /// </summary>
        public LiteralIndexer<bool> Bool
        {
            get => new LiteralIndexer<bool>(
                this,
                "真偽値",
                (value) => bool.Parse(value),
                (value) => value.ToString(),
                "Bool",
                false);
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "yes" or "no".
        /// </summary>
        public LiteralIndexer<bool> YesNo
        {
            get => new LiteralIndexer<bool>(
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
                (value) => value ? "yes" : "no",
                "YesNo",
                false);
        }

        /// <summary>
        /// Gets indexer for boolean accessor with "on" or "off".
        /// </summary>
        public LiteralIndexer<bool> OnOff
        {
            get => new LiteralIndexer<bool>(
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
                (value) => value ? "on" : "off",
                "OnOff",
                false);
        }

        /// <summary>
        /// Gets indexer for (int x, int y) tuple accessor.
        /// </summary>
        public LiteralIndexer<(int x, int y)> Int2
        {
            get => new LiteralIndexer<(int x, int y)>(
                this,
                "整数x2",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 2)
                    {
                        throw new ValidationError();
                    }

                    return (
                    int.Parse(splitValues[0]),
                    int.Parse(splitValues[1]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Int2",
                (0, 0));
        }

        /// <summary>
        /// Gets indexer for (int x, int y, int z) tuple accessor.
        /// </summary>
        public LiteralIndexer<(int x, int y, int z)> Int3
        {
            get => new LiteralIndexer<(int x, int y, int z)>(
                this,
                "整数x3",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 3)
                    {
                        throw new ValidationError();
                    }

                    return (
                    int.Parse(splitValues[0]),
                    int.Parse(splitValues[1]),
                    int.Parse(splitValues[2]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Int3",
                (0, 0, 0));
        }

        /// <summary>
        /// Gets indexer for (double x, double y) tuple accessor.
        /// </summary>
        public LiteralIndexer<(double x, double y)> Double2
        {
            get => new LiteralIndexer<(double x, double y)>(
                this,
                "実数x2",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 2)
                    {
                        throw new ValidationError();
                    }

                    return (
                    double.Parse(splitValues[0]),
                    double.Parse(splitValues[1]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Double2",
                (0.0, 0.0));
        }

        /// <summary>
        /// Gets indexer for (double x, double y, double z) tuple accessor.
        /// </summary>
        public LiteralIndexer<(double x, double y, double z)> Double3
        {
            get => new LiteralIndexer<(double x, double y, double z)>(
                this,
                "実数x3",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 3)
                    {
                        throw new ValidationError();
                    }

                    return (
                    double.Parse(splitValues[0]),
                    double.Parse(splitValues[1]),
                    double.Parse(splitValues[2]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Double3",
                (0.0, 0.0, 0.0));
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// </summary>
        public LiteralIndexer<string> Keyword
        {
            get => new LiteralIndexer<string>(
                this,
                "改行なし文字列",
                (value) =>
                {
                    if (value.Contains("\n"))
                    {
                        throw new ValidationError();
                    }

                    return value;
                },
                (value) =>
                {
                    if (value.Contains("\n"))
                    {
                        throw new ValidationError();
                    }

                    return value;
                },
                "Keyword",
                string.Empty);
        }

        /// <summary>
        /// Gets indexer for multi line string accessor.
        /// </summary>
        public LiteralIndexer<string> Text
        {
            get => new LiteralIndexer<string>(
                this,
                "改行あり文字列",
                (value) =>
                {
                    // "YAML上の最終行が[End Of Text]"で
                    // 終わっていないテキストは不正とする
                    // If the text doesn't end with "\n [End of Text]",
                    // the text is invalid.
                    if (!value.EndsWith("\n[End Of Text]"))
                    {
                        throw new ValidationError();
                    }

                    // "[End Of Text]"を除去
                    // Remove "[End Of Text]".
                    return value.Substring(
                        0, value.Length - "\n[End Of Text]".Length);
                },
                (value) =>
                {
                    // 単一行のテキストでもYAML上複数行として扱うため、
                    // 末尾に固定のテキストを一行含める。
                    // To text write into YAML as multi line text
                    // even if the text is single line,
                    // add constant line.
                    return value + "\n[End Of Text]";
                },
                "Text",
                string.Empty);
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
        public static string EncodeDefaultValForMolding(string target)
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
        public static string DecodeDefaultValForMolding(string target)
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
        public LiteralIndexer<int> Range(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            if (parenthesisOpen != '(' &&
                parenthesisOpen != '[' &&
                parenthesisOpen != ']')
            {
                throw new InvalidSpecDefinitionException(
                    "Rangeアクセスの開き括弧に'(', '[', ']'以外の文字" +
                    $"'{parenthesisOpen}'が用いられています。");
            }

            if (parenthesisClose != ')' &&
                parenthesisClose != ']' &&
                parenthesisClose != '[')
            {
                throw new InvalidSpecDefinitionException(
                    "Rangeアクセスの閉じ括弧に')', ']', '['以外の文字" +
                    $"'{parenthesisClose}'が用いられています。");
            }

            var leftInequality = (parenthesisOpen == '[') ? "≦" : "＜";
            var rightInequality = (parenthesisClose == ']') ? "≦" : "＜";
            var rangeText =
                $"{leftBound}{leftInequality}x{rightInequality}{rightBound}";

            // "leftBoundInt <= x < rightBoundInt" is equivalent condition for interger.
            var leftBoundInt =
                (parenthesisOpen == '[') ?
                Math.Ceiling(leftBound) :
                (Math.Floor(leftBound) + 1);
            var rightBoundInt =
                (parenthesisClose == ']') ?
                (Math.Floor(rightBound) + 1) :
                Math.Ceiling(rightBound);

            if (leftBoundInt >= rightBoundInt)
            {
                throw new InvalidSpecDefinitionException(
                    $"Rangeアクセス範囲({rangeText})に" +
                    $"含まれる要素がありません。");
            }

            var moldingAccessCode =
                $"Range, {parenthesisOpen}, {leftBound}," +
                $" {rightBound}, {parenthesisClose}";

            // 最小絶対値でMold用の値を作成
            // moldingDefault has minimum absolute value.
            var moldingDefault =
                (leftBoundInt <= 0 && rightBoundInt > 0) ?
                0 :
                (leftBoundInt > 0) ?
                ((int)leftBoundInt) :
                ((int)(rightBoundInt - 1));

            Func<int, bool> validator = (int val) =>
            {
                var leftBoundCheck =
                    (parenthesisOpen == '[') ?
                    (leftBound <= val) :
                    (leftBound < val);
                var rightBoundCheck =
                    (parenthesisClose == ']') ?
                    (val <= rightBound) :
                    (val < rightBound);

                return leftBoundCheck && rightBoundCheck;
            };

            return new LiteralIndexer<int>(
                this,
                $"整数({rangeText})",
                (value) =>
                {
                    var ret = int.Parse(value);

                    if (!validator(ret))
                    {
                        throw new ValidationError();
                    }

                    return ret;
                },
                (value) =>
                {
                    if (!validator(value))
                    {
                        throw new ValidationError();
                    }

                    return value.ToString();
                },
                moldingAccessCode,
                moldingDefault);
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
        public LiteralIndexer<double> Interval(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            if (parenthesisOpen != '(' &&
                parenthesisOpen != '[' &&
                parenthesisOpen != ']')
            {
                throw new InvalidSpecDefinitionException(
                    "Intervalアクセスの開き括弧に'(', '[', ']'以外の文字" +
                    $"'{parenthesisOpen}'が用いられています。");
            }

            if (parenthesisClose != ')' &&
                parenthesisClose != ']' &&
                parenthesisClose != '[')
            {
                throw new InvalidSpecDefinitionException(
                    "Intervalアクセスの閉じ括弧に')', ']', '['以外の文字" +
                    $"'{parenthesisClose}'が用いられています。");
            }

            var leftInequality = (parenthesisOpen == '[') ? "≦" : "＜";
            var rightInequality = (parenthesisClose == ']') ? "≦" : "＜";
            var rangeText =
                $"{leftBound}{leftInequality}x{rightInequality}{rightBound}";

            if (leftBound >= rightBound)
            {
                throw new InvalidSpecDefinitionException(
                    $"Intervalアクセス範囲({rangeText})に" +
                    $"含まれる要素がありません。");
            }

            var moldingAccessCode =
                $"Interval, {parenthesisOpen}, {leftBound}," +
                $" {rightBound}, {parenthesisClose}";

            // 最小絶対値でMold用の値を作成
            // moldingDefault has minimum absolute value.
            var moldingDefault =
                (leftBound < 0.0 && rightBound > 0.0) ?
                0.0 :
                double.IsNegativeInfinity(leftBound) ?
                rightBound - 1.0 :
                double.IsPositiveInfinity(rightBound) ?
                leftBound + 1.0 :
                (leftBound + rightBound) / 2.0;

            Func<double, bool> validator = (double val) =>
            {
                var leftBoundCheck =
                    (parenthesisOpen == '[') ?
                    (leftBound <= val) :
                    (leftBound < val);
                var rightBoundCheck =
                    (parenthesisClose == ']') ?
                    (val <= rightBound) :
                    (val < rightBound);

                return leftBoundCheck && rightBoundCheck;
            };

            return new LiteralIndexer<double>(
                this,
                $"実数({rangeText})",
                (value) =>
                {
                    var ret = double.Parse(value);

                    if (!validator(ret))
                    {
                        throw new ValidationError();
                    }

                    return ret;
                },
                (value) =>
                {
                    if (!validator(value))
                    {
                        throw new ValidationError();
                    }

                    return value.ToString();
                },
                moldingAccessCode,
                moldingDefault);
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
        public LiteralIndexer<int> Range(
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
        public LiteralIndexer<int> Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// The string length is bounded.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>Indexer for limited keyword accessor.</returns>
        public LiteralIndexer<string> LimitedKeyword(int limit)
        {
            if (limit < 0)
            {
                throw new InvalidSpecDefinitionException(
                    $"LimitedKeywordの上限値に負の値{limit}が設定されています。");
            }

            return new LiteralIndexer<string>(
                this,
                $"改行なし文字列({limit}文字以下)",
                (value) =>
                {
                    if (value.Contains("\n") || value.Length > limit)
                    {
                        throw new ValidationError();
                    }

                    return value;
                },
                (value) =>
                {
                    if (value.Contains("\n") || value.Length > limit)
                    {
                        throw new ValidationError();
                    }

                    return value;
                },
                $"LimitedKeyword, {limit}",
                string.Empty);
        }

        /// <summary>
        /// Gets indexer for SpawnerRoot instance from another spec file.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerRoot type.</typeparam>
        /// <returns>SpawnerRoot instance.</returns>
        public PathIndexer<T> Exterior<T>()
            where T : ISpawnerRoot<object>
        {
            var paramType = typeof(T);
            var moldingDefault = this.MoldingDefault<T>();
            moldingDefault.Spec = SpecRoot.CreateDummy(this.Proxy);

            return new PathIndexer<T>(
                this,
                paramType.Name,
                (path) =>
                {
                    var spec = this.Proxy.SpecPool(path);
                    var spawner = (T)Activator.CreateInstance(
                        spec.SpawnerType);
                    spawner.Spec = spec;

                    return spawner;
                },
                (spawner) => spawner.Spec.Path,
                $"Exterior, {EncodeType(paramType)}",
                moldingDefault);
        }

        /// <summary>
        /// Gets indexer for SpawnerChild instance from spec child.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerChild type.</typeparam>
        /// <returns>SpawnerChild instance.</returns>
        public InteriorIndexer<T> Interior<T>()
            where T : ISpawnerChild<object>
        {
            return new InteriorIndexer<T>(this);
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
        protected T_Spawner MoldingDefault<T_Spawner>()
            where T_Spawner : ISpawner<object>
        {
            var spawnerType = typeof(T_Spawner);

            foreach (var type in this.Proxy.SpawnerTypePool())
            {
                try
                {
                    // 最初にValidation成功したTypeによるインスタンスを返す
                    // Instance from first valid type is returned.
                    return (T_Spawner)ISpawner<object>.ValidateSpawnerType(
                        spawnerType, type);
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
            public SubSpec this[string key]
            {
                get => new SubSpec(this.parent, key);
            }
        }

        /// <summary>
        /// Indexer class for SpecChild.
        /// </summary>
        public class SpecChildIndexer
        {
            private Spec parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="SpecChildIndexer"/> class.
            /// </summary>
            /// <param name="parent">The parent spec instance.</param>
            internal SpecChildIndexer(Spec parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Indexer for spec child.
            /// </summary>
            /// <param name="key">The string key for the spec child.</param>
            /// <param name="type">The SpawnerChild type which this spec child will define.</param>
            /// <returns>The spec child.</returns>
            public SpecChild this[string key, Type type]
            {
                get => new SpecChild(this.parent, key, type);
            }
        }

        /// <summary>
        /// Indexer class for instances which associated with file paths.
        /// </summary>
        /// <typeparam name="T">Type of path associated value.</typeparam>
        public class PathIndexer<T>
        {
            private Spec parent;
            private string type;
            private Func<AgnosticPath, T> getter;
            private Func<T, AgnosticPath> setter;
            private string moldingAccessCode;
            private T moldingDefault;
            private ScalarIndexer<T> internalIndexer;

            /// <summary>
            /// Initializes a new instance of the <see cref="PathIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from agnostic path.</param>
            /// <param name="setter">Translation function to agnostic path.</param>
            /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
            /// <param name="moldingDefault">The default value for molding.</param>
            internal PathIndexer(
                Spec parent,
                string type,
                Func<AgnosticPath, T> getter,
                Func<T, AgnosticPath> setter,
                string moldingAccessCode,
                T moldingDefault)
            {
                this.parent = parent;
                this.type = type;
                this.getter = getter;
                this.setter = setter;
                this.moldingAccessCode = moldingAccessCode;
                this.moldingDefault = moldingDefault;
                this.internalIndexer = new ScalarIndexer<T>(
                    this.parent,
                    this.type,
                    (path, scalar) =>
                    {
                        var relative = AgnosticPath.FromAgnosticPathString(scalar);
                        var fromExecutable = relative + path.Parent;
                        return this.getter(fromExecutable);
                    },
                    (path, value) =>
                    {
                        var fromExecutable = this.setter(value);
                        var relative = fromExecutable - path.Parent;
                        return relative.ToAgnosticPathString();
                    },
                    this.moldingAccessCode,
                    this.moldingDefault,
                    ImmutableList<string>.Empty);
            }

            /// <summary>
            /// Indexer for value.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <returns>The translated value.</returns>
            public T this[string key]
            {
                get => this.internalIndexer[key];
                set { this.internalIndexer[key] = value; }
            }

            /// <summary>
            /// Indexer for value with default.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <param name="defaultPath">The default agnostic path string.</param>
            /// <returns>The translated value.</returns>
            public T this[string key, string defaultPath]
            {
                get
                {
                    try
                    {
                        _ = AgnosticPath.FromAgnosticPathString(defaultPath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecDefinitionException(
                            "デフォルトパスが不正です。", ex);
                    }

                    return this.internalIndexer[key, defaultPath];
                }
            }
        }

        /// <summary>
        /// Indexer class for literal values (Int, Double, and Bool etc).
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        public class LiteralIndexer<T>
        {
            private Spec parent;
            private string type;
            private Func<string, T> getter;
            private Func<T, string> setter;
            private string moldingAccessCode;
            private T moldingDefault;
            private ScalarIndexer<T> internalIndexer;

            /// <summary>
            /// Initializes a new instance of the <see cref="LiteralIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from string.</param>
            /// <param name="setter">Translation function to string.</param>
            /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
            /// <param name="moldingDefault">The default value for molding.</param>
            internal LiteralIndexer(
                Spec parent,
                string type,
                Func<string, T> getter,
                Func<T, string> setter,
                string moldingAccessCode,
                T moldingDefault)
            {
                this.parent = parent;
                this.type = type;
                this.getter = getter;
                this.setter = setter;
                this.moldingAccessCode = moldingAccessCode;
                this.moldingDefault = moldingDefault;
                this.internalIndexer = new ScalarIndexer<T>(
                    this.parent,
                    this.type,
                    (path, scalar) => this.getter(scalar),
                    (path, value) => this.setter(value),
                    this.moldingAccessCode,
                    this.moldingDefault,
                    ImmutableList<string>.Empty);
            }

            /// <summary>
            /// Indexer for literal value.
            /// </summary>
            /// <param name="key">The string key for the literal value.</param>
            /// <returns>The translated value.</returns>
            public T this[string key]
            {
                get => this.internalIndexer[key];
                set { this.internalIndexer[key] = value; }
            }

            /// <summary>
            /// Indexer for literal value.
            /// </summary>
            /// <param name="key">The string key for the literal value.</param>
            /// <param name="defaultVal">The default literal value.</param>
            /// <returns>The translated value.</returns>
            public T this[string key, T defaultVal]
            {
                get
                {
                    try
                    {
                        this.getter(this.setter(defaultVal));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecDefinitionException(
                            "デフォルト値がSpecの値として不正です。", ex);
                    }

                    return this.internalIndexer[key, defaultVal];
                }
            }
        }

        /// <summary>
        /// Indexer class for SpawnerChild instances.
        /// </summary>
        /// <typeparam name="T">Expected SpawnerChild type.</typeparam>
        public class InteriorIndexer<T>
            where T : ISpawnerChild<object>
        {
            private Spec parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="InteriorIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            internal InteriorIndexer(Spec parent)
            {
                // molding default process validates type parameter T.
                parent.MoldingDefault<T>().GetType();

                this.parent = parent;
            }

            /// <summary>
            /// Indexer for SpecChild.
            /// </summary>
            /// <param name="key">The string key for the SpecChild.</param>
            /// <returns>The SpecChild instance.</returns>
            public T this[string key]
            {
                get => this.IndexGet(
                    key,
                    (specChild) => specChild.SpawnerType,
                    $"Spawner, {EncodeType(typeof(T))}");

                set
                {
                    lock (this.parent.Properties)
                    {
                        try
                        {
                            var specChild = this.parent.Child[key, typeof(T)];

                            // valueのTypeがDefaultで取得された場合、
                            // yamlにSpawnerTypeが無いため明示的に書き込む。
                            // Write spawner type explicitly,
                            // because the its spawner type may not be in yaml
                            // in case of default type.
                            specChild.SpawnerType = value.GetType();

                            specChild.Body["properties"] =
                                value.Spec.Properties.Clone(
                                    $"{this.parent.Properties.ID}[{key}]");
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                typeof(T).Name,
                                ex);
                        }
                    }
                }
            }

            /// <summary>
            /// Indexer for SpecChild with default spawner type.
            /// </summary>
            /// <param name="key">The string key for the SpecChild.</param>
            /// <param name="defaultType">Default spawner type.</param>
            /// <returns>The SpecChild instance.</returns>
            public T this[string key, Type defaultType]
            {
                get
                {
                    try
                    {
                        ISpawner<object>.ValidateSpawnerType(
                            typeof(T), defaultType);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecDefinitionException(
                            $"型{defaultType.Name}は" +
                            $"Interior<{typeof(T).Name}>の" +
                            $"デフォルト値として不正です。",
                            ex);
                    }

                    var moldingAccessCode =
                        $"Spawner, {EncodeType(typeof(T))}, " +
                        $"{EncodeType(defaultType)}";

                    return this.IndexGet(
                        key,
                        (specChild) =>
                        {
                            try
                            {
                                return specChild.SpawnerType;
                            }
                            catch (YAML.YAMLKeyUndefinedException)
                            {
                                return defaultType;
                            }
                        },
                        moldingAccessCode);
                }
            }

            private T IndexGet(
                string key,
                Func<SpecChild, Type> getSpawnerType,
                string moldingAccessCode)
            {
                lock (this.parent.Properties)
                {
                    var childSpec = this.parent.Child[key, typeof(T)];

                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            _ = childSpec.Mold;
                            this.parent.Mold[key]["spawner"] =
                                new ScalarYAML(moldingAccessCode);
                        }

                        var type = getSpawnerType(childSpec);
                        var spawner = (T)Activator.CreateInstance(type);
                        spawner.Spec = childSpec;

                        if (this.parent.IsMolding)
                        {
                            // 正当なSpawner型を得られた場合には、
                            // ダミーSpecによるSpawnを実行することで
                            // SpawnwerChild.Spawn()をmoldに記録する。
                            // If valid spawner type is defined in spec,
                            // call DummySpawn with dummy spec to mold calls in
                            // SpawnerChild.Spawn().
                            spawner.DummySpawn();
                        }

                        return spawner;
                    }
                    catch (Exception ex)
                    {
                        if (this.parent.IsMolding)
                        {
                            // 正当なSpawner型を得られなかった場合には、
                            // ダミーのSpecChildを設定した
                            // moldingDefaultを返すことで、
                            // moldingDefault.Spawn()の値をmoldしない。
                            // If valid spawner type is not defined in spec,
                            // return molding default instance
                            // with dummy SpecChild not to mold calls in
                            // moldingDefault.Spawn().
                            var moldingDefault =
                                this.parent.MoldingDefault<T>();
                            moldingDefault.Spec =
                                SpecRoot.CreateDummy(this.parent.Proxy)
                                .Child[key, typeof(T)];
                            return moldingDefault;
                        }
                        else
                        {
                            throw new InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                typeof(T).Name,
                                ex);
                        }
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

        /// <summary>
        /// Common indexer class for scalar value in spec file.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        private class ScalarIndexer<T>
        {
            private Spec parent;
            private string type;
            private Func<AgnosticPath, string, T> getter;
            private Func<AgnosticPath, T, string> setter;
            private string moldingAccessCode;
            private T moldingDefault;
            private ImmutableList<string> referredSpecs;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScalarIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from spec path and string value in spec to indexed value.</param>
            /// <param name="setter">Translation function from spec path and indexed value to string value in spec.</param>
            /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
            /// <param name="moldingDefault">The default value for molding.</param>
            /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
            internal ScalarIndexer(
                Spec parent,
                string type,
                Func<AgnosticPath, string, T> getter,
                Func<AgnosticPath, T, string> setter,
                string moldingAccessCode,
                T moldingDefault,
                ImmutableList<string> referredSpecs)
            {
                this.parent = parent;
                this.type = type;
                this.getter = getter;
                this.setter = setter;
                this.moldingAccessCode = moldingAccessCode;
                this.moldingDefault = moldingDefault;
                this.referredSpecs = referredSpecs;
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
                    lock (this.parent.Properties)
                    {
                        try
                        {
                            if (this.parent.IsMolding)
                            {
                                this.parent.Mold[key] =
                                    new ScalarYAML(this.moldingAccessCode);
                            }

                            return this.IndexGet(key);
                        }
                        catch (Exception ex)
                        {
                            if (this.parent.IsMolding)
                            {
                                return this.moldingDefault;
                            }
                            else
                            {
                                throw new InvalidSpecAccessException(
                                    $"{this.parent.Properties.ID}[{key}]",
                                    this.type,
                                    ex);
                            }
                        }
                    }
                }

                set
                {
                    lock (this.parent.Properties)
                    {
                        try
                        {
                            this.parent.Properties[key] =
                                new ScalarYAML(this.setter(this.parent.Path, value));
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                this.type,
                                ex);
                        }
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
                get => this[
                    key,
                    () => defaultVal,
                    this.setter(this.parent.Path, defaultVal)];
            }

            /// <summary>
            /// Indexer for value with default.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <param name="defaultValInSpec">The default value in spec.</param>
            /// <returns>The translated value.</returns>
            public T this[string key, string defaultValInSpec]
            {
                get => this[
                    key,
                    () => this.getter(this.parent.Path, defaultValInSpec),
                    defaultValInSpec];
            }

            private T this[
                string key,
                Func<T> lazyDefaultVal,
                string defaultValForMolding]
            {
                get
                {
                    lock (this.parent.Properties)
                    {
                        try
                        {
                            if (this.parent.IsMolding)
                            {
                                var accessCodeWithDefault =
                                    this.moldingAccessCode +
                                    ", " +
                                    EncodeDefaultValForMolding(
                                        defaultValForMolding);

                                this.parent.Mold[key] =
                                    new ScalarYAML(accessCodeWithDefault);
                            }

                            try
                            {
                                return this.IndexGet(key);
                            }
                            catch (YAML.YAMLKeyUndefinedException)
                            {
                                return lazyDefaultVal();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (this.parent.IsMolding)
                            {
                                return this.moldingDefault;
                            }
                            else
                            {
                                throw new InvalidSpecAccessException(
                                    $"{this.parent.Properties.ID}[{key}]",
                                    this.type,
                                    ex);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Common index getter process.
            /// </summary>
            /// <param name="key">The string key for the value.</param>
            /// <returns>The translated value.</returns>
            internal T IndexGet(string key)
            {
                try
                {
                    return this.getter(
                        this.parent.Path,
                        this.parent.Properties.Scalar[key].Value);
                }
                catch (YAML.YAMLKeyUndefinedException ex)
                {
                    var isLooped = this.referredSpecs.Contains(
                        this.parent.ID);
                    if (this.parent.Base == null || isLooped)
                    {
                        throw ex;
                    }

                    // Only if base spec is defined and not looped,
                    // base spec is referred.
                    return new ScalarIndexer<T>(
                        this.parent.Base,
                        this.type,
                        this.getter,
                        this.setter,
                        this.moldingAccessCode,
                        this.moldingDefault,
                        this.referredSpecs.Add(this.parent.ID))
                        .IndexGet(key);
                }
            }
        }
    }
}
