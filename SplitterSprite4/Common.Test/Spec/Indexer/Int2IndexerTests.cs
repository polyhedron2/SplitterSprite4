// -----------------------------------------------------------------------
// <copyright file="Int2IndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Int2Indexer class.
    /// </summary>
    public class Int2IndexerTests
    {
        /// <summary>
        /// Test the (int x, int y) accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void Int2Test(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"dummy\"",
                "  \"too short\": \"1\"",
                "  \"too long\": \"1, 2, 3\"",
                "  \"coprime pair\": \"729, 1000\"",
                "  \"fibonacci number\":",
                "    \"first\": \"0, 1\"",
                "    \"second\": \"1, 1\"",
                "    \"third\": \"1, 2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["too short"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["too long"];
            });
            Assert.Equal((729, 1000), spec.Int2["coprime pair"]);
            Assert.Equal(
                (0, 1), spec["fibonacci number"].Int2["first"]);
            Assert.Equal(
                (1, 1), spec["fibonacci number"].Int2["second"]);
            Assert.Equal(
                (1, 2), spec["fibonacci number"].Int2["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["fibonacci number"].Int2["fourth"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["invalid", (0, 0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["too short", (0, 0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int2["too long", (0, 0)];
            });
            Assert.Equal((729, 1000), spec.Int2["coprime pair", (-1, -1)]);
            Assert.Equal(
                (0, 1), spec["fibonacci number"].Int2["first", (-1, -1)]);
            Assert.Equal(
                (1, 1), spec["fibonacci number"].Int2["second", (-1, -1)]);
            Assert.Equal(
                (1, 2), spec["fibonacci number"].Int2["third", (-1, -1)]);
            Assert.Equal(
                (2, 3), spec["fibonacci number"].Int2["fourth", (2, 3)]);

            // act
            spec.Int2["coprime pair"] = (3, 10);
            spec["fibonacci number"].Int2["fourth"] = (2, 3);

            // assert
            Assert.Equal((3, 10), spec.Int2["coprime pair"]);
            Assert.Equal((2, 3), spec["fibonacci number"].Int2["fourth"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"too short\": \"1\"",
                    "  \"too long\": \"1, 2, 3\"",
                    "  \"coprime pair\": \"3, 10\"",
                    "  \"fibonacci number\":",
                    "    \"first\": \"0, 1\"",
                    "    \"second\": \"1, 1\"",
                    "    \"third\": \"1, 2\"",
                    "    \"fourth\": \"2, 3\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"too short\": \"1\"",
                    "  \"too long\": \"1, 2, 3\"",
                    "  \"coprime pair\": \"3, 10\"",
                    "  \"fibonacci number\":",
                    "    \"first\": \"0, 1\"",
                    "    \"second\": \"1, 1\"",
                    "    \"third\": \"1, 2\"",
                    "    \"fourth\": \"2, 3\""),
                reloadedSpec.ToString());
        }
    }
}
