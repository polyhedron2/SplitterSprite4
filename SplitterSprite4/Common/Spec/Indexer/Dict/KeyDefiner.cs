// -----------------------------------------------------------------------
// <copyright file="KeyDefiner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Support class for define key type of DictIndexer.
    /// </summary>
    public class KeyDefiner
    {
        private Spec parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDefiner"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal KeyDefiner(Spec parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Bool.
        /// </summary>
        public ValueDefiner<bool> Bool
        {
            get => new ValueDefiner<bool>(
                this.parent, x => x, new BoolIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is YesNo.
        /// </summary>
        public ValueDefiner<bool> YesNo
        {
            get => new ValueDefiner<bool>(
                this.parent, x => x, new YesNoIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is OnOff.
        /// </summary>
        public ValueDefiner<bool> OnOff
        {
            get => new ValueDefiner<bool>(
                this.parent, x => x, new OnOffIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Int.
        /// </summary>
        public ValueDefiner<int> Int
        {
            get => new ValueDefiner<int>(
                this.parent, x => x, new IntIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Int2.
        /// </summary>
        public ValueDefiner<(int, int)> Int2
        {
            get => new ValueDefiner<(int, int)>(
                this.parent, x => x, new Int2Indexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Int3.
        /// </summary>
        public ValueDefiner<(int, int, int)> Int3
        {
            get => new ValueDefiner<(int, int, int)>(
                this.parent, x => x, new Int3Indexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Double.
        /// </summary>
        public ValueDefiner<double> Double
        {
            get => new ValueDefiner<double>(
                this.parent, x => x, new DoubleIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Double2.
        /// </summary>
        public ValueDefiner<(double, double)> Double2
        {
            get => new ValueDefiner<(double, double)>(
                this.parent, x => x, new Double2Indexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Double3.
        /// </summary>
        public ValueDefiner<(double, double, double)> Double3
        {
            get => new ValueDefiner<(double, double, double)>(
                this.parent, x => x, new Double3Indexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Keyword.
        /// </summary>
        public ValueDefiner<string> Keyword
        {
            get => new ValueDefiner<string>(
                this.parent, x => x, new KeywordIndexer(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is LimitedKeyword.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>ValueDefiner whose key-indexer is LimitedKeyword.</returns>
        public ValueDefiner<string> LimitedKeyword(int limit)
        {
            return new ValueDefiner<string>(
                this.parent, x => x, new LimitedKeywordIndexer(this.parent, limit, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Range.
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
        /// <returns>ValueDefiner whose key-indexer is Range.</returns>
        public ValueDefiner<int> Range(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            var keyScalarIndexer = new RangeIndexer(
                this.parent,
                parenthesisOpen,
                leftBound,
                rightBound,
                parenthesisClose,
                false).InternalIndexer;

            return new ValueDefiner<int>(
                this.parent, x => x, keyScalarIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Range.
        /// </summary>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>ValueDefiner whose key-indexer is Range.</returns>
        public ValueDefiner<int> Range(
            double leftBound,
            double rightBound)
        {
            return this.Range('[', leftBound, rightBound, ')');
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Range.
        /// </summary>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>ValueDefiner whose key-indexer is Range.</returns>
        public ValueDefiner<int> Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Interval.
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
        /// <returns>ValueDefiner whose key-indexer is Interval.</returns>
        public ValueDefiner<double> Interval(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            var keyScalarIndexer = new IntervalIndexer(
                this.parent,
                parenthesisOpen,
                leftBound,
                rightBound,
                parenthesisClose,
                false).InternalIndexer;

            return new ValueDefiner<double>(
                this.parent, x => x, keyScalarIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Choice.
        /// </summary>
        /// <typeparam name="T">The type of choice.</typeparam>
        /// <param name="choices">The choices.</param>
        /// <param name="choiceToSpecStr">The function from choice to string on spec.</param>
        /// <returns>ValueDefiner whose key-indexer is Choice.</returns>
        public ValueDefiner<T> Choice<T>(
            IEnumerable<T> choices,
            Func<T, string> choiceToSpecStr)
        {
            return new ValueDefiner<T>(
                this.parent,
                x => choices.ToList().IndexOf(x),
                new ChoiceIndexer<T>(this.parent, choices, choiceToSpecStr, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Choice.
        /// </summary>
        /// <typeparam name="T">The type of choice.</typeparam>
        /// <param name="choices">The choices.</param>
        /// <returns>ValueDefiner whose key-indexer is Choice.</returns>
        public ValueDefiner<T> Choice<T>(
            IEnumerable<T> choices)
        {
            return new ValueDefiner<T>(
                this.parent,
                x => choices.ToList().IndexOf(x),
                new ChoiceIndexer<T>(this.parent, choices, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Choice.
        /// </summary>
        /// <typeparam name="T">The type of choice.</typeparam>
        /// <param name="choiceToSpecStr">
        /// The mapping from choice to string on spec.
        /// Note: If the type T's Equals method is not implemented well, this mapping may not work well.
        /// </param>
        /// <returns>ValueDefiner whose key-indexer is Choice.</returns>
        public ValueDefiner<T> Choice<T>(
            IEnumerable<KeyValuePair<T, string>> choiceToSpecStr)
        {
            return new ValueDefiner<T>(
                this.parent,
                x => choiceToSpecStr.Select(kv => kv.Key).ToList().IndexOf(x),
                new ChoiceIndexer<T>(this.parent, choiceToSpecStr, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is Exterior.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>ValueDefiner whose key-indexer is Exterior.</returns>
        public ValueDefiner<T_Spawner> Exterior<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return new ValueDefiner<T_Spawner>(
                this.parent,
                x => x.Spec.ID,
                new ExteriorIndexer<T_Spawner>(this.parent, false).InternalIndexer);
        }

        /// <summary>
        /// Gets ValueDefiner whose key-indexer is ExteriorDir.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>ValueDefiner whose key-indexer is ExteriorDir.</returns>
        public ValueDefiner<ISpawnerDir<T_Spawner>> ExteriorDir<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return new ValueDefiner<ISpawnerDir<T_Spawner>>(
                this.parent,
                x => x.Dir.Path.ToAgnosticPathString(),
                new ExteriorDirIndexer<T_Spawner>(this.parent, false).InternalIndexer);
        }
    }
}
