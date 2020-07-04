// -----------------------------------------------------------------------
// <copyright file="KeywordIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for string accessor without line feed code.
    /// </summary>
    public class KeywordIndexer : LiteralIndexer<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeywordIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        internal KeywordIndexer(Spec parent, bool allowHiddenValue)
            : base(
                parent,
                () => "改行なし文字列",
                (value) =>
                {
                    if (value.Contains("\n"))
                    {
                        throw new Spec.ValidationError();
                    }

                    return value;
                },
                (value) =>
                {
                    if (value.Contains("\n"))
                    {
                        throw new Spec.ValidationError();
                    }

                    return value;
                },
                () => "Keyword",
                string.Empty,
                allowHiddenValue)
        {
        }
    }
}
