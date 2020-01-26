// -----------------------------------------------------------------------
// <copyright file="Int2Indexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for (int x, int y) tuple accessor.
    /// </summary>
    public class Int2Indexer : LiteralIndexer<(int x, int y)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int2Indexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal Int2Indexer(Spec parent)
            : base(
                parent,
                "整数x2",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 2)
                    {
                        throw new Spec.ValidationError();
                    }

                    return (
                    int.Parse(splitValues[0]),
                    int.Parse(splitValues[1]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Int2",
                (0, 0))
        {
        }
    }
}
