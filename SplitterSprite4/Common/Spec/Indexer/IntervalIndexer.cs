// -----------------------------------------------------------------------
// <copyright file="IntervalIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for double presicion floating point number accessor
    /// in an interval.
    /// </summary>
    public class IntervalIndexer : LiteralIndexer<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
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
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        /// <returns>Indexer for floating point number accesssor in an interval.</returns>
        internal IntervalIndexer(
            Spec parent,
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose,
            bool allowHiddenValue)
            : base(
                parent,
                () => $"実数({RangeText(parenthesisOpen, leftBound, rightBound, parenthesisClose)})",
                (value) =>
                {
                    var ret = double.Parse(value);

                    if (!Validate(
                        parenthesisOpen,
                        leftBound,
                        rightBound,
                        parenthesisClose,
                        ret))
                    {
                        throw new Spec.ValidationError();
                    }

                    return ret;
                },
                (value) =>
                {
                    if (!Validate(
                        parenthesisOpen,
                        leftBound,
                        rightBound,
                        parenthesisClose,
                        value))
                    {
                        throw new Spec.ValidationError();
                    }

                    return value.ToString();
                },
                () => MoldingAccessCode(
                    parenthesisOpen,
                    leftBound,
                    rightBound,
                    parenthesisClose),
                MoldingDefault(leftBound, rightBound),
                allowHiddenValue)
        {
            if (parenthesisOpen != '(' &&
                parenthesisOpen != '[' &&
                parenthesisOpen != ']')
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Intervalアクセスの開き括弧に'(', '[', ']'以外の文字" +
                    $"'{parenthesisOpen}'が用いられています。");
            }

            if (parenthesisClose != ')' &&
                parenthesisClose != ']' &&
                parenthesisClose != '[')
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Intervalアクセスの閉じ括弧に')', ']', '['以外の文字" +
                    $"'{parenthesisClose}'が用いられています。");
            }

            if (leftBound >= rightBound)
            {
                var rangeText = RangeText(
                    parenthesisOpen, leftBound, rightBound, parenthesisClose);
                throw new Spec.InvalidSpecDefinitionException(
                    $"Intervalアクセス範囲({rangeText})に" +
                    $"含まれる要素がありません。");
            }
        }

        private static string RangeText(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            var leftInequality = (parenthesisOpen == '[') ? "≦" : "＜";
            var rightInequality = (parenthesisClose == ']') ? "≦" : "＜";
            return
                $"{leftBound}{leftInequality}x{rightInequality}{rightBound}";
        }

        private static bool Validate(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose,
            double target)
        {
            var leftBoundCheck =
                (parenthesisOpen == '[') ?
                (leftBound <= target) :
                (leftBound < target);
            var rightBoundCheck =
                (parenthesisClose == ']') ?
                (target <= rightBound) :
                (target < rightBound);

            return leftBoundCheck && rightBoundCheck;
        }

        private static string MoldingAccessCode(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            return
                $"Interval, {parenthesisOpen}, {leftBound}," +
                $" {rightBound}, {parenthesisClose}";
        }

        private static double MoldingDefault(
            double leftBound, double rightBound)
        {
            // 最小絶対値でMold用の値を作成
            // moldingDefault has minimum absolute value.
            return
                (leftBound < 0.0 && rightBound > 0.0) ?
                0.0 :
                double.IsNegativeInfinity(leftBound) ?
                rightBound - 1.0 :
                double.IsPositiveInfinity(rightBound) ?
                leftBound + 1.0 :
                (leftBound + rightBound) / 2.0;
        }
    }
}
