// -----------------------------------------------------------------------
// <copyright file="Int3IndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Int3Indexer class.
    /// </summary>
    public class Int3IndexerTests : ScalarIndexerTests
    {
        /// <summary>
        /// Test the (int x, int y, int z) accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void ScalarAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"dummy\"",
                "  \"too short\": \"1, 2\"",
                "  \"too long\": \"1, 2, 3, 4\"",
                "  \"coprime trio\": \"6, 15, 10\"",
                "  \"tribonacci number\":",
                "    \"first\": \"0, 0, 1\"",
                "    \"second\": \"0, 1, 1\"",
                "    \"third\": \"1, 1, 2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["too short"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["too long"];
            });
            Assert.Equal((6, 15, 10), spec.Int3["coprime trio"]);
            Assert.Equal(
                (0, 0, 1), spec["tribonacci number"].Int3["first"]);
            Assert.Equal(
                (0, 1, 1), spec["tribonacci number"].Int3["second"]);
            Assert.Equal(
                (1, 1, 2), spec["tribonacci number"].Int3["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["fibonacci number"].Int3["fourth"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["invalid", (0, 0, 0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["too short", (0, 0, 0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int3["too long", (0, 0, 0)];
            });
            Assert.Equal((6, 15, 10), spec.Int3["coprime trio", (-1, -1, -1)]);
            Assert.Equal(
                (0, 0, 1), spec["tribonacci number"].Int3["first", (-1, -1, -1)]);
            Assert.Equal(
                (0, 1, 1), spec["tribonacci number"].Int3["second", (-1, -1, -1)]);
            Assert.Equal(
                (1, 1, 2), spec["tribonacci number"].Int3["third", (-1, -1, -1)]);
            Assert.Equal(
                (1, 2, 4), spec["tribonacci number"].Int3["fourth", (1, 2, 4)]);

            // act
            spec.Int3["coprime trio"] = (15, 35, 21);
            spec["tribonacci number"].Int3["fourth"] = (1, 2, 4);

            // assert
            Assert.Equal((15, 35, 21), spec.Int3["coprime trio"]);
            Assert.Equal((1, 2, 4), spec["tribonacci number"].Int3["fourth"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"too short\": \"1, 2\"",
                    "  \"too long\": \"1, 2, 3, 4\"",
                    "  \"coprime trio\": \"15, 35, 21\"",
                    "  \"tribonacci number\":",
                    "    \"first\": \"0, 0, 1\"",
                    "    \"second\": \"0, 1, 1\"",
                    "    \"third\": \"1, 1, 2\"",
                    "    \"fourth\": \"1, 2, 4\""),
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
                    "  \"too short\": \"1, 2\"",
                    "  \"too long\": \"1, 2, 3, 4\"",
                    "  \"coprime trio\": \"15, 35, 21\"",
                    "  \"tribonacci number\":",
                    "    \"first\": \"0, 0, 1\"",
                    "    \"second\": \"0, 1, 1\"",
                    "    \"third\": \"1, 1, 2\"",
                    "    \"fourth\": \"1, 2, 4\""),
                reloadedSpec.ToString());
        }
    }
}
