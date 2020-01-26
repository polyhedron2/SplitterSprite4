// -----------------------------------------------------------------------
// <copyright file="Double3Indexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for (double x, double y, double z) tuple accessor.
    /// </summary>
    public class Double3Indexer
        : LiteralIndexer<(double x, double y, double z)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Double3Indexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal Double3Indexer(Spec parent)
            : base(
                parent,
                "実数x3",
                (value) =>
                {
                    var splitValues = value.Split(',');
                    if (splitValues.Length != 3)
                    {
                        throw new Spec.ValidationError();
                    }

                    return (
                    double.Parse(splitValues[0]),
                    double.Parse(splitValues[1]),
                    double.Parse(splitValues[2]));
                },
                (value) =>
                {
                    var ret = value.ToString();
                    ret = ret.Substring(1, ret.Length - 2);
                    return ret;
                },
                "Double3",
                (0.0, 0.0, 0.0))
        {
        }
    }
}
