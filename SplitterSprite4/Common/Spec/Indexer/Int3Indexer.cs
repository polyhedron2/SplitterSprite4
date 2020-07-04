// -----------------------------------------------------------------------
// <copyright file="Int3Indexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for (int x, int y, int z) tuple accessor.
    /// </summary>
    public class Int3Indexer : LiteralIndexer<(int x, int y, int z)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Int3Indexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        internal Int3Indexer(Spec parent, bool allowHiddenValue)
            : base(
                parent,
                () => "整数x3",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 3)
                    {
                        throw new Spec.ValidationError();
                    }

                    return (
                    int.Parse(splitValues[0]),
                    int.Parse(splitValues[1]),
                    int.Parse(splitValues[2]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                () => "Int3",
                (0, 0, 0),
                allowHiddenValue)
        {
        }
    }
}
