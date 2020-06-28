// -----------------------------------------------------------------------
// <copyright file="ScalarIndexerTests.cs" company="MagicKitchen">
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
    public abstract class ScalarIndexerTests
    {
        /// <summary>
        /// Test the scalar accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public abstract void ScalarAccessTest(string path);

        /// <summary>
        /// Test the scalar accessor with multi cultures.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MultiCultureScalarAccessTest(string path)
        {
            var cultureNames = new string[] { "en-US", "ja-JP", "eo-001" };

            foreach (var cultureName in cultureNames)
            {
                var testCInfo = new CultureInfo(cultureName);
                var prevCInfo = Thread.CurrentThread.CurrentCulture;

                try
                {
                    Thread.CurrentThread.CurrentCulture = testCInfo;
                    this.ScalarAccessTest(path);
                }
                finally
                {
                    Thread.CurrentThread.CurrentCulture = prevCInfo;
                }
            }
        }
    }
}
