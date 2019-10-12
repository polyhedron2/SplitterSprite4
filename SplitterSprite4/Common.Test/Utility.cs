// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    /// <summary>
    /// テスト用汎用実装
    /// Utility class for tests.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Join some lines into a string.
        /// </summary>
        /// <param name="lines">line strings.</param>
        /// <returns>The joined string.</returns>
        public static string JoinLines(params string[] lines)
        {
            return string.Join("\n", lines);
        }
    }
}
