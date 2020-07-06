// -----------------------------------------------------------------------
// <copyright file="LimitedKeywordIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for length limited string.
    /// </summary>
    public class LimitedKeywordIndexer : LiteralIndexer<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedKeywordIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="limit">The bound of keyword length.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal LimitedKeywordIndexer(Spec parent, int limit, bool dictMode)
            : base(
                parent,
                () => $"改行なし文字列({limit}文字以下)",
                (value) =>
                {
                    if (value.Contains("\n") || value.Length > limit)
                    {
                        throw new Spec.ValidationError();
                    }

                    return value;
                },
                (value) =>
                {
                    if (value.Contains("\n") || value.Length > limit)
                    {
                        throw new Spec.ValidationError();
                    }

                    return value;
                },
                () => $"LimitedKeyword, {limit}",
                string.Empty,
                dictMode)
        {
            if (limit < 0)
            {
                throw new Spec.InvalidSpecDefinitionException(
                    $"LimitedKeywordの上限値に負の値{limit}が設定されています。");
            }
        }
    }
}
