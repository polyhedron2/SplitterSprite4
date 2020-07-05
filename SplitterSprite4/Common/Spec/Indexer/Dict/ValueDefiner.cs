// -----------------------------------------------------------------------
// <copyright file="ValueDefiner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Immutable;
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Support class for define value type of DictIndexer.
    /// </summary>
    /// <typeparam name="T_Key">Key type.</typeparam>
    public class ValueDefiner<T_Key>
    {
        private Spec parent;

        private Func<string> keyTypeGenerator;
        private Func<AgnosticPath, string, T_Key> keyGetter;
        private Func<AgnosticPath, T_Key, string> keySetter;
        private Func<T_Key, IComparable> keyOrder;
        private Func<string> keyMoldingAccessCodeGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefiner{T_Key}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="keyTypeGenerator">
        /// The access type string generator for key.
        /// </param>
        /// <param name="keyGetter">
        /// Translation function
        /// from spec path and string key in spec
        /// to indexed key.
        /// </param>
        /// <param name="keySetter">
        /// Translation function
        /// from spec path and indexed key
        /// to string key in spec.
        /// </param>
        /// <param name="keyOrder">
        /// Translation function
        /// from key to IComparable for sorting keys.
        /// </param>
        /// <param name="keyMoldingAccessCodeGenerator">
        /// The key type and parameter information generator for molding.
        /// </param>
        internal ValueDefiner(
            Spec parent,
            Func<string> keyTypeGenerator,
            Func<AgnosticPath, string, T_Key> keyGetter,
            Func<AgnosticPath, T_Key, string> keySetter,
            Func<T_Key, IComparable> keyOrder,
            Func<string> keyMoldingAccessCodeGenerator)
        {
            this.parent = parent;

            this.keyTypeGenerator = keyTypeGenerator;
            this.keyGetter = keyGetter;
            this.keySetter = keySetter;
            this.keyOrder = keyOrder;
            this.keyMoldingAccessCodeGenerator = keyMoldingAccessCodeGenerator;
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Bool.
        /// </summary>
        public DictIndexerWithDefault<T_Key, bool, bool> Bool
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new BoolIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is YesNo.
        /// </summary>
        public DictIndexerWithDefault<T_Key, bool, bool> YesNo
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new YesNoIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is OnOff.
        /// </summary>
        public DictIndexerWithDefault<T_Key, bool, bool> OnOff
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new OnOffIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Int.
        /// </summary>
        public DictIndexerWithDefault<T_Key, int, int> Int
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new IntIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Int2.
        /// </summary>
        public DictIndexerWithDefault<T_Key, (int, int), (int, int)> Int2
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new Int2Indexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Int3.
        /// </summary>
        public DictIndexerWithDefault<T_Key, (int, int, int), (int, int, int)> Int3
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new Int3Indexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Double.
        /// </summary>
        public DictIndexerWithDefault<T_Key, double, double> Double
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new DoubleIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Double2.
        /// </summary>
        public DictIndexerWithDefault<T_Key, (double, double), (double, double)> Double2
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new Double2Indexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Double3.
        /// </summary>
        public DictIndexerWithDefault<T_Key, (double, double, double), (double, double, double)> Double3
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new Double3Indexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Keyword.
        /// </summary>
        public DictIndexerWithDefault<T_Key, string, string> Keyword
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new KeywordIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Text.
        /// </summary>
        public DictIndexerWithDefault<T_Key, string, string> Text
        {
            get => this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new TextIndexer(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is LimitedKeyword.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>DictIndexer whose value-indexer is LimitedKeyword.</returns>
        public DictIndexerWithDefault<T_Key, string, string> LimitedKeyword(int limit)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new LimitedKeywordIndexer(spec, limit, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Range.
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
        /// <returns>DictIndexer whose value-indexer is Range.</returns>
        public DictIndexerWithDefault<T_Key, int, int> Range(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new RangeIndexer(
                    spec,
                    parenthesisOpen,
                    leftBound,
                    rightBound,
                    parenthesisClose,
                    true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Range.
        /// </summary>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>DictIndexer whose value-indexer is Range.</returns>
        public DictIndexerWithDefault<T_Key, int, int> Range(
            double leftBound,
            double rightBound)
        {
            return this.Range('[', leftBound, rightBound, ')');
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Range.
        /// </summary>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>DictIndexer whose value-indexer is Range.</returns>
        public DictIndexerWithDefault<T_Key, int, int> Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Interval.
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
        /// <returns>DictIndexer whose value-indexer is Interval.</returns>
        public DictIndexerWithDefault<T_Key, double, double> Interval(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new IntervalIndexer(
                    spec,
                    parenthesisOpen,
                    leftBound,
                    rightBound,
                    parenthesisClose,
                    true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Exterior.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>DictIndexer whose value-indexer is Exterior.</returns>
        public DictIndexerWithDefault<T_Key, T_Spawner, string> Exterior<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new ExteriorIndexer<T_Spawner>(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is ExteriorDir.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>DictIndexer whose value-indexer is ExteriorDir.</returns>
        public DictIndexerWithDefault<T_Key, ISpawnerDir<T_Spawner>, string>
            ExteriorDir<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new ExteriorDirIndexer<T_Spawner>(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Interior.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerChild type.</typeparam>
        /// <returns>DictIndexer whose value-indexer is Interior.</returns>
        public DictIndexerWithDefault<T_Key, T_Spawner, Type> Interior<T_Spawner>()
            where T_Spawner : ISpawnerChild<object>
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new InteriorIndexer<T_Spawner>(spec, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is SubSpec.
        /// </summary>
        /// <returns>DictIndexer whose value-indexer is SubSpec.</returns>
        public DictIndexerGet<T_Key, SubSpec> SubSpec()
        {
            return this.GenerateDictIndexerWithIndexerGet(
                spec => new SubSpecIndexer(spec, true));
        }

        private DictIndexerGet<T_Key, T_Value>
            GenerateDictIndexerWithIndexerGet<T_Value>(
            Func<Spec, IIndexerGet<T_Value>> valueIndexerGenerator)
        {
            return new DictIndexerGet<T_Key, T_Value>(
                this.parent,
                this.keyTypeGenerator,
                this.keyGetter,
                this.keySetter,
                this.keyOrder,
                this.keyMoldingAccessCodeGenerator,
                valueIndexerGenerator,
                ImmutableList<string>.Empty);
        }

        private DictIndexerWithDefault<T_Key, T_Value, T_Default>
            GenerateDictIndexerWithIndexerWithDefault<T_Value, T_Default>(
            Func<Spec, IIndexerWithDefault<T_Value, T_Default>> valueIndexerGenerator)
        {
            return new DictIndexerWithDefault<T_Key, T_Value, T_Default>(
                this.parent,
                this.keyTypeGenerator,
                this.keyGetter,
                this.keySetter,
                this.keyOrder,
                this.keyMoldingAccessCodeGenerator,
                valueIndexerGenerator,
                ImmutableList<string>.Empty);
        }
    }
}
