// -----------------------------------------------------------------------
// <copyright file="LiteralIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using System.Globalization;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// Test class for common implementation of ScalarIndexer.
    /// </summary>
    public abstract class LiteralIndexerTests
    {
        /// <summary>
        /// Test the scalar accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public abstract void LiteralAccessTest(string path);

        /// <summary>
        /// Test the scalar accessor with multi cultures.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="cultureName">The culture name for testing.</param>
        [Theory]
        [InlineData("foo.spec", "ja-JP")]
        [InlineData("foo.spec", "en-US")]
        [InlineData("foo.spec", "eo-001")]
        [InlineData("dir/bar.spec", "ja-JP")]
        [InlineData("dir1/dir2/baz.spec", "ja-JP")]
        public void MultiCultureLiteralAccessTest(string path, string cultureName)
        {
            var testCInfo = new CultureInfo(cultureName);
            var prevCInfo = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = testCInfo;
                this.LiteralAccessTest(path);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = prevCInfo;
            }
        }
    }
}
