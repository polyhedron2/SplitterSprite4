// -----------------------------------------------------------------------
// <copyright file="YesNoIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for boolean accessor with "yes" or "no".
    /// </summary>
    public class YesNoIndexer : LiteralIndexer<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YesNoIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal YesNoIndexer(Spec parent)
            : base(
                parent,
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
                        throw new Spec.UnexpectedChoiceException(
                            value, "yes", "no");
                    }
                },
                (value) => value ? "yes" : "no",
                "YesNo",
                false)
        {
        }
    }
}
