// -----------------------------------------------------------------------
// <copyright file="RangeIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;

    /// <summary>
    /// Indexer class for ranged integer accessor.
    /// </summary>
    public class RangeIndexer : LiteralIndexer<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeIndexer"/> class.
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
        internal RangeIndexer(
            Spec parent,
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose,
            bool allowHiddenValue)
            : base(
                parent,
                () => $"整数({RangeText(parenthesisOpen, leftBound, rightBound, parenthesisClose)})",
                (value) =>
                {
                    var ret = int.Parse(value);

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
                MoldingDefault(
                    parenthesisOpen,
                    leftBound,
                    rightBound,
                    parenthesisClose),
                allowHiddenValue)
        {
            if (parenthesisOpen != '(' &&
                parenthesisOpen != '[' &&
                parenthesisOpen != ']')
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Rangeアクセスの開き括弧に'(', '[', ']'以外の文字" +
                    $"'{parenthesisOpen}'が用いられています。");
            }

            if (parenthesisClose != ')' &&
                parenthesisClose != ']' &&
                parenthesisClose != '[')
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Rangeアクセスの閉じ括弧に')', ']', '['以外の文字" +
                    $"'{parenthesisClose}'が用いられています。");
            }

            // "LeftBoundInt <= x < RightBoundInt" is equivalent condition for interger.
            if (LeftBoundInt(parenthesisOpen, leftBound) >=
                RightBoundInt(rightBound, parenthesisClose))
            {
                var rangeText = RangeText(
                    parenthesisOpen, leftBound, rightBound, parenthesisClose);
                throw new Spec.InvalidSpecDefinitionException(
                    $"Rangeアクセス範囲({rangeText})に" +
                    $"含まれる要素がありません。");
            }
        }

        private static double LeftBoundInt(
            char parenthesisOpen, double leftBound)
        {
            return
                (parenthesisOpen == '[') ?
                Math.Ceiling(leftBound) :
                (Math.Floor(leftBound) + 1);
        }

        private static double RightBoundInt(
            double rightBound, char parenthesisClose)
        {
            return
                (parenthesisClose == ']') ?
                (Math.Floor(rightBound) + 1) :
                Math.Ceiling(rightBound);
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
            int target)
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
                $"Range, {parenthesisOpen}, {leftBound}," +
                $" {rightBound}, {parenthesisClose}";
        }

        private static int MoldingDefault(
            char parenthesisOpen,
            double leftBound,
            double rightBound,
            char parenthesisClose)
        {
            var leftBoundInt = LeftBoundInt(parenthesisOpen, leftBound);
            var rightBoundInt = RightBoundInt(rightBound, parenthesisClose);

            // 最小絶対値でMold用の値を作成
            // moldingDefault has minimum absolute value.
            return
                (leftBoundInt <= 0 && rightBoundInt > 0) ?
                0 :
                (leftBoundInt > 0) ?
                ((int)leftBoundInt) :
                ((int)(rightBoundInt - 1));
        }
    }
}
