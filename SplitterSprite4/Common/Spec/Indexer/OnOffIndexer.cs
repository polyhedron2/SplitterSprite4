// -----------------------------------------------------------------------
// <copyright file="OnOffIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for boolean accessor with "on" or "off".
    /// </summary>
    public class OnOffIndexer : LiteralIndexer<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnOffIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        internal OnOffIndexer(Spec parent, bool allowHiddenValue)
            : base(
                parent,
                () => "ON/OFF",
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
                        throw new Spec.UnexpectedChoiceException(
                            value, "on", "off");
                    }
                },
                (value) => value ? "on" : "off",
                () => "OnOff",
                false,
                allowHiddenValue)
        {
        }
    }
}
