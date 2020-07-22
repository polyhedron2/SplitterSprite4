// -----------------------------------------------------------------------
// <copyright file="ValueDefiner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer.List
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Support class for define value type of ListIndexer.
    /// </summary>
    public class ValueDefiner
    {
        private Spec parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefiner"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal ValueDefiner(Spec parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Bool.
        /// </summary>
        public ListIndexerWithDefault<bool, bool> Bool
        {
            get => new ListIndexerWithDefault<bool, bool>(this.DefineDecimalKey().Bool);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is YesNo.
        /// </summary>
        public ListIndexerWithDefault<bool, bool> YesNo
        {
            get => new ListIndexerWithDefault<bool, bool>(this.DefineDecimalKey().YesNo);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is OnOff.
        /// </summary>
        public ListIndexerWithDefault<bool, bool> OnOff
        {
            get => new ListIndexerWithDefault<bool, bool>(this.DefineDecimalKey().OnOff);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Int.
        /// </summary>
        public ListIndexerWithDefault<int, int> Int
        {
            get => new ListIndexerWithDefault<int, int>(this.DefineDecimalKey().Int);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Int2.
        /// </summary>
        public ListIndexerWithDefault<(int, int), (int, int)> Int2
        {
            get => new ListIndexerWithDefault<(int, int), (int, int)>(this.DefineDecimalKey().Int2);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Int3.
        /// </summary>
        public ListIndexerWithDefault<(int, int, int), (int, int, int)> Int3
        {
            get => new ListIndexerWithDefault<(int, int, int), (int, int, int)>(this.DefineDecimalKey().Int3);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Double.
        /// </summary>
        public ListIndexerWithDefault<double, double> Double
        {
            get => new ListIndexerWithDefault<double, double>(this.DefineDecimalKey().Double);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Double2.
        /// </summary>
        public ListIndexerWithDefault<(double, double), (double, double)> Double2
        {
            get => new ListIndexerWithDefault<(double, double), (double, double)>(this.DefineDecimalKey().Double2);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Double3.
        /// </summary>
        public ListIndexerWithDefault<(double, double, double), (double, double, double)> Double3
        {
            get => new ListIndexerWithDefault<(double, double, double), (double, double, double)>(this.DefineDecimalKey().Double3);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Keyword.
        /// </summary>
        public ListIndexerWithDefault<string, string> Keyword
        {
            get => new ListIndexerWithDefault<string, string>(this.DefineDecimalKey().Keyword);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Text.
        /// </summary>
        public ListIndexerWithDefault<string, string> Text
        {
            get => new ListIndexerWithDefault<string, string>(this.DefineDecimalKey().Text);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is SubSpec.
        /// </summary>
        public ListIndexerGet<SubSpec> SubSpec
        {
            get => new ListIndexerGet<SubSpec>(this.DefineDecimalKey().SubSpec);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is LimitedKeyword.
        /// </summary>
        /// <param name="limit">The bound of keyword length.</param>
        /// <returns>ListIndexer whose value-indexer is LimitedKeyword.</returns>
        public ListIndexerWithDefault<string, string> LimitedKeyword(int limit)
        {
            return new ListIndexerWithDefault<string, string>(this.DefineDecimalKey().LimitedKeyword(limit));
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Range.
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
        /// <returns>ListIndexer whose value-indexer is Range.</returns>
        public ListIndexerWithDefault<int, int> Range(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return new ListIndexerWithDefault<int, int>(
                this.DefineDecimalKey().Range(parenthesisOpen, leftBound, rightBound, parenthesisClose));
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Range.
        /// </summary>
        /// <param name="leftBound">
        /// 区間表現の下限。
        /// The left bound of the interval.
        /// </param>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>ListIndexer whose value-indexer is Range.</returns>
        public ListIndexerWithDefault<int, int> Range(
            double leftBound,
            double rightBound)
        {
            return this.Range('[', leftBound, rightBound, ')');
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Range.
        /// </summary>
        /// <param name="rightBound">
        /// 区間表現の上限。
        /// The right bound of the interval.
        /// </param>
        /// <returns>ListIndexer whose value-indexer is Range.</returns>
        public ListIndexerWithDefault<int, int> Range(double rightBound)
        {
            return this.Range(0.0, rightBound);
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Interval.
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
        /// <returns>ListIndexer whose value-indexer is Interval.</returns>
        public ListIndexerWithDefault<double, double> Interval(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return new ListIndexerWithDefault<double, double>(
                this.DefineDecimalKey().Interval(parenthesisOpen, leftBound, rightBound, parenthesisClose));
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Exterior.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>ListIndexer whose value-indexer is Exterior.</returns>
        public ListIndexerWithDefault<T_Spawner, string> Exterior<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return new ListIndexerWithDefault<T_Spawner, string>(
                this.DefineDecimalKey().Exterior<T_Spawner>());
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is ExteriorDir.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerRoot type.</typeparam>
        /// <returns>ListIndexer whose value-indexer is ExteriorDir.</returns>
        public ListIndexerWithDefault<ISpawnerDir<T_Spawner>, string>
            ExteriorDir<T_Spawner>()
            where T_Spawner : ISpawnerRoot<object>
        {
            return new ListIndexerWithDefault<ISpawnerDir<T_Spawner>, string>(
                this.DefineDecimalKey().ExteriorDir<T_Spawner>());
        }

        /// <summary>
        /// Gets ListIndexer whose value-indexer is Interior.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected SpawnerChild type.</typeparam>
        /// <returns>ListIndexer whose value-indexer is Interior.</returns>
        public ListIndexerWithDefault<T_Spawner, Type> Interior<T_Spawner>()
            where T_Spawner : ISpawnerChild<object>
        {
            return new ListIndexerWithDefault<T_Spawner, Type>(
                this.DefineDecimalKey().Interior<T_Spawner>());
        }

        private Dict.ValueDefiner<decimal> DefineDecimalKey()
        {
            return new Dict.ValueDefiner<decimal>(
                this.parent, x => x, new ListKeyIndexer(this.parent).InternalIndexer);
        }
    }
}
