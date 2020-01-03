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
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲームインスタンス設定を定義するSpecファイルへのアクセスクラス
    /// Spec file which defines game instance configurations, access class.
    /// </summary>
    public abstract class Spec
    {
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
        /// Gets indexer for integer accessor.
        /// </summary>
        public ValueIndexer<int> Int
        {
            get => new ValueIndexer<int>(
                this,
                "整数",
                (value) => int.Parse(value),
                (value) => value.ToString(),
                "Int",
                0,
                ImmutableList<string>.Empty);
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
                (value) => value.ToString(),
                "Double",
                0.0,
                ImmutableList<string>.Empty);
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
                (value) => value.ToString(),
                "Bool",
                false,
                ImmutableList<string>.Empty);
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
                (value) => value ? "yes" : "no",
                "YesNo",
                false,
                ImmutableList<string>.Empty);
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
                (value) => value ? "on" : "off",
                "OnOff",
                false,
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for (int x, int y) tuple accessor.
        /// </summary>
        public ValueIndexer<(int x, int y)> Int2
        {
            get => new ValueIndexer<(int x, int y)>(
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
                (0, 0),
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for (int x, int y, int z) tuple accessor.
        /// </summary>
        public ValueIndexer<(int x, int y, int z)> Int3
        {
            get => new ValueIndexer<(int x, int y, int z)>(
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
                (0, 0, 0),
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for (double x, double y) tuple accessor.
        /// </summary>
        public ValueIndexer<(double x, double y)> Double2
        {
            get => new ValueIndexer<(double x, double y)>(
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
                (0.0, 0.0),
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for (double x, double y, double z) tuple accessor.
        /// </summary>
        public ValueIndexer<(double x, double y, double z)> Double3
        {
            get => new ValueIndexer<(double x, double y, double z)>(
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
                (0.0, 0.0, 0.0),
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// </summary>
        public ValueIndexer<string> Keyword
        {
            get => new ValueIndexer<string>(
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
                string.Empty,
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Gets indexer for multi line string accessor.
        /// </summary>
        public ValueIndexer<string> Text
        {
            get => new ValueIndexer<string>(
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
                string.Empty,
                ImmutableList<string>.Empty);
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
        /// Gets the OutSideProxy for file access.
        /// </summary>
        public abstract OutSideProxy Proxy { get; }

        /// <summary>
        /// Gets the sub spec.
        /// </summary>
        /// <param name="key">The string key for the sub spec.</param>
        /// <returns>The sub spec.</returns>
        public SpecNode this[string key]
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
        public ValueIndexer<int> Range(
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

            return new ValueIndexer<int>(
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
                moldingDefault,
                ImmutableList<string>.Empty);
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
        public ValueIndexer<double> Interval(
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

            return new ValueIndexer<double>(
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
                moldingDefault,
                ImmutableList<string>.Empty);
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
        public ValueIndexer<int> Range(
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
        public ValueIndexer<int> Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets indexer for string accessor without line feed code.
        /// The string length is bounded.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>Indexer for limited keyword accessor.</returns>
        public ValueIndexer<string> LimitedKeyword(int limit)
        {
            if (limit < 0)
            {
                throw new InvalidSpecDefinitionException(
                    $"LimitedKeywordの上限値に負の値{limit}が設定されています。");
            }

            return new ValueIndexer<string>(
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
                string.Empty,
                ImmutableList<string>.Empty);
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
            public SpecNode this[string key]
            {
                get => new SpecNode(this.parent, key);
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
            private string moldingAccessCode;
            private T moldingDefault;
            private ImmutableList<string> referredSpecs;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueIndexer{T}"/> class.
            /// </summary>
            /// <param name="parent">The parent spec.</param>
            /// <param name="type">The access type string.</param>
            /// <param name="getter">Translation function from string.</param>
            /// <param name="setter">Translation function to string.</param>
            /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
            /// <param name="moldingDefault">The default value for molding.</param>
            /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
            internal ValueIndexer(
                Spec parent,
                string type,
                Func<string, T> getter,
                Func<T, string> setter,
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
                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            this.parent.Mold[key] =
                                new ScalarYAML(this.moldingAccessCode);
                        }

                        try
                        {
                            return this.getter(
                                this.parent.Properties.Scalar[key].Value);
                        }
                        catch (YAML.YAMLKeyUndefinedException ex)
                        {
                            var isLooped =
                                this.referredSpecs.Contains(this.parent.ID);
                            if (this.parent.Base == null || isLooped)
                            {
                                throw ex;
                            }

                            // Only if base spec is defined and not looped,
                            // base spec is referred.
                            return new ValueIndexer<T>(
                                this.parent.Base,
                                this.type,
                                this.getter,
                                this.setter,
                                this.moldingAccessCode,
                                this.moldingDefault,
                                this.referredSpecs.Add(this.parent.ID))[
                                key];
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

                set
                {
                    try
                    {
                        this.parent.Properties[key] =
                            new ScalarYAML(this.setter(value));
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
                        this.getter(this.setter(defaultVal));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecDefinitionException(
                            "デフォルト値がSpecの値として不正です。", ex);
                    }

                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            var accessCodeWithDefault =
                                this.moldingAccessCode +
                                ", " +
                                EncodeDefaultValForMolding(this.setter(defaultVal));

                            this.parent.Mold[key] =
                                new ScalarYAML(accessCodeWithDefault);
                        }

                        try
                        {
                            return this.getter(
                                this.parent.Properties.Scalar[key].Value);
                        }
                        catch (YAML.YAMLKeyUndefinedException)
                        {
                            var isLooped =
                                this.referredSpecs.Contains(this.parent.ID);
                            if (this.parent.Base == null || isLooped)
                            {
                                return defaultVal;
                            }

                            // Only if base spec is defined and not looped,
                            // base spec is referred.
                            return new ValueIndexer<T>(
                                this.parent.Base,
                                this.type,
                                this.getter,
                                this.setter,
                                this.moldingAccessCode,
                                this.moldingDefault,
                                this.referredSpecs.Add(this.parent.ID))[
                                key, defaultVal];
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
    }
}
