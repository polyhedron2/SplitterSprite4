// -----------------------------------------------------------------------
// <copyright file="ValueDefiner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Support class for define value type of DictIndexer.
    /// </summary>
    /// <typeparam name="T_Key">Key type.</typeparam>
    public class ValueDefiner<T_Key>
    {
        private Spec parent;

        private Func<T_Key, IComparable> keyOrder;
        private ScalarIndexer<T_Key> keyScalarIndexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefiner{T_Key}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="keyOrder">
        /// Translation function
        /// from key to IComparable for sorting keys.
        /// </param>
        /// <param name="keyScalarIndexer">
        /// ScalarIndexer object for key access.
        /// </param>
        internal ValueDefiner(
            Spec parent,
            Func<T_Key, IComparable> keyOrder,
            ScalarIndexer<T_Key> keyScalarIndexer)
        {
            this.parent = parent;

            this.keyOrder = keyOrder;
            this.keyScalarIndexer = keyScalarIndexer;
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
        /// Gets DictIndexer whose value-indexer is SubSpec.
        /// </summary>
        /// <returns>DictIndexer whose value-indexer is SubSpec.</returns>
        public DictIndexerGet<T_Key, SubSpec> SubSpec
        {
            get => this.GenerateDictIndexerWithIndexerGet(
                spec => new SubSpecIndexer(spec, true));
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
        /// Gets DictIndexer whose value-indexer is Choice.
        /// </summary>
        /// <typeparam name="T_Value">The type of choice.</typeparam>
        /// <param name="choices">The choices.</param>
        /// <param name="choiceToSpecStr">The function from choice to string on spec.</param>
        /// <returns>DictIndexer whose value-indexer is Choice.</returns>
        public DictIndexerWithDefault<T_Key, T_Value, T_Value> Choice<T_Value>(
            IEnumerable<T_Value> choices,
            Func<T_Value, string> choiceToSpecStr)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new ChoiceIndexer<T_Value>(spec, choices, choiceToSpecStr, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Choice.
        /// </summary>
        /// <typeparam name="T_Value">The type of choice.</typeparam>
        /// <param name="choices">The choices.</param>
        /// <returns>DictIndexer whose value-indexer is Choice.</returns>
        public DictIndexerWithDefault<T_Key, T_Value, T_Value> Choice<T_Value>(
            IEnumerable<T_Value> choices)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new ChoiceIndexer<T_Value>(spec, choices, true));
        }

        /// <summary>
        /// Gets DictIndexer whose value-indexer is Choice.
        /// </summary>
        /// <typeparam name="T_Value">The type of choice.</typeparam>
        /// <param name="choiceToSpecStr">
        /// The mapping from choice to string on spec.
        /// Note: If the type T's Equals method is not implemented well, this mapping may not work well.
        /// </param>
        /// <returns>DictIndexer whose value-indexer is Choice.</returns>
        public DictIndexerWithDefault<T_Key, T_Value, T_Value> Choice<T_Value>(
            IEnumerable<KeyValuePair<T_Value, string>> choiceToSpecStr)
        {
            return this.GenerateDictIndexerWithIndexerWithDefault(
                spec => new ChoiceIndexer<T_Value>(spec, choiceToSpecStr, true));
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

        private DictIndexerGet<T_Key, T_Value>
            GenerateDictIndexerWithIndexerGet<T_Value>(
            Func<Spec, IIndexerGet<T_Value>> valueIndexerGenerator)
        {
            return new DictIndexerGet<T_Key, T_Value>(
                this.parent,
                this.keyOrder,
                this.keyScalarIndexer,
                valueIndexerGenerator,
                ImmutableList<string>.Empty);
        }

        private DictIndexerWithDefault<T_Key, T_Value, T_Default>
            GenerateDictIndexerWithIndexerWithDefault<T_Value, T_Default>(
            Func<Spec, IIndexerWithDefault<T_Value, T_Default>> valueIndexerGenerator)
        {
            return new DictIndexerWithDefault<T_Key, T_Value, T_Default>(
                this.parent,
                this.keyOrder,
                this.keyScalarIndexer,
                valueIndexerGenerator,
                ImmutableList<string>.Empty);
        }
    }
}
