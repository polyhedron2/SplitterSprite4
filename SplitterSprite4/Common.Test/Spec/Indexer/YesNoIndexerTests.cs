// -----------------------------------------------------------------------
// <copyright file="YesNoIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the YesNoIndexer class.
    /// </summary>
    public class YesNoIndexerTests : ScalarIndexerTests
    {
        /// <summary>
        /// Test the boolean accessor with "yes" or "no".
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public override void ScalarAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"True\"",
                "  \"special\": \"YES\"",
                "  \"story flag\":",
                "    \"first\": \"Yes\"",
                "    \"second\": \"no\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.YesNo["invalid"];
            });
            Assert.True(spec.YesNo["special"]);
            Assert.True(spec["story flag"].YesNo["first"]);
            Assert.False(spec["story flag"].YesNo["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["story flag"].YesNo["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.YesNo["invalid", false];
            });
            Assert.True(spec.YesNo["special", false]);
            Assert.True(spec["story flag"].YesNo["first", false]);
            Assert.False(spec["story flag"].YesNo["second", true]);
            Assert.True(spec["story flag"].YesNo["third", true]);

            // act
            spec.YesNo["special"] = false;
            spec["story flag"].YesNo["third"] = true;

            // assert
            Assert.False(spec.YesNo["special", false]);
            Assert.True(spec["story flag"].YesNo["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"True\"",
                    "  \"special\": \"no\"",
                    "  \"story flag\":",
                    "    \"first\": \"Yes\"",
                    "    \"second\": \"no\"",
                    "    \"third\": \"yes\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"True\"",
                    "  \"special\": \"no\"",
                    "  \"story flag\":",
                    "    \"first\": \"Yes\"",
                    "    \"second\": \"no\"",
                    "    \"third\": \"yes\""),
                reloadedSpec.ToString());
        }
    }
}
