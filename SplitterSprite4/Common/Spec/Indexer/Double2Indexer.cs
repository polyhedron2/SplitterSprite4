// -----------------------------------------------------------------------
// <copyright file="Double2Indexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for (double x, double y) tuple accessor.
    /// </summary>
    public class Double2Indexer : LiteralIndexer<(double x, double y)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Double2Indexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal Double2Indexer(Spec parent)
            : base(
                parent,
                () => "実数x2",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 2)
                    {
                        throw new Spec.ValidationError();
                    }

                    return (
                    double.Parse(splitValues[0]),
                    double.Parse(splitValues[1]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                () => "Double2",
                (0.0, 0.0))
        {
        }
    }
}
