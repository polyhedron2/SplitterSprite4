// -----------------------------------------------------------------------
// <copyright file="DoubleIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the DoubleIndexer class.
    /// </summary>
    public class DoubleIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the double precision floating point number accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void LiteralAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"dummy\"",
                "  \"pi\": \"3.14159265\"",
                "  \"square root\":",
                "    \"first\": \"1.00000000\"",
                "    \"second\": \"1.41421356\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double["invalid"];
            });
            Assert.Equal(3.14159265, spec.Double["pi"]);
            Assert.Equal(1.00000000, spec["square root"].Double["first"]);
            Assert.Equal(1.41421356, spec["square root"].Double["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["square root"].Double["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double["invalid", 0.0];
            });
            Assert.Equal(3.14159265, spec.Double["pi", 2.71828182]);
            Assert.Equal(1.00000000, spec["square root"].Double["first", -1]);
            Assert.Equal(1.41421356, spec["square root"].Double["second", 0.0]);
            Assert.Equal(1.73050807, spec["square root"].Double["third", 1.73050807]);

            // act
            spec.Double["pi"] = 3;
            spec["square root"].Double["third"] = 1.73050807;

            // assert
            Assert.Equal(3.0, spec.Double["pi"]);
            Assert.Equal(1.73050807, spec["square root"].Double["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"pi\": \"3\"",
                    "  \"square root\":",
                    "    \"first\": \"1.00000000\"",
                    "    \"second\": \"1.41421356\"",
                    "    \"third\": \"1.73050807\""),
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
                    "  \"pi\": \"3\"",
                    "  \"square root\":",
                    "    \"first\": \"1.00000000\"",
                    "    \"second\": \"1.41421356\"",
                    "    \"third\": \"1.73050807\""),
                reloadedSpec.ToString());
        }
    }
}
