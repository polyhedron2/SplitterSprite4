// -----------------------------------------------------------------------
// <copyright file="Double2IndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Double2Indexer class.
    /// </summary>
    public class Double2IndexerTests : ScalarIndexerTests
    {
        /// <summary>
        /// Test the (double x, double y) accessor.
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
                "  \"too short\": \"1.0\"",
                "  \"too long\": \"1.0, 2.0, 3.0\"",
                "  \"initial coordinates\": \"1.2, 3.4\"",
                "  \"trigonometric ratio (sin, cos)\":",
                "    \"first\": \"0.0, 1.0\"",
                "    \"second\": \"0.5, 0.86602540378\"",
                "    \"third\": \"0.70710678118, 0.70710678118\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["too short"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["too long"];
            });
            Assert.Equal((1.2, 3.4), spec.Double2["initial coordinates"]);
            Assert.Equal(
                (0.0, 1.0),
                spec["trigonometric ratio (sin, cos)"].Double2["first"]);
            Assert.Equal(
                (0.5, 0.86602540378),
                spec["trigonometric ratio (sin, cos)"].Double2["second"]);
            Assert.Equal(
                (0.70710678118, 0.70710678118),
                spec["trigonometric ratio (sin, cos)"].Double2["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["trigonometric ratio (sin, cos)"].Double2["fourth"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["invalid", (0.0, 0.0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["too short", (0.0, 0.0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double2["too long", (0.0, 0.0)];
            });
            Assert.Equal((1.2, 3.4), spec.Double2["initial coordinates"]);
            Assert.Equal(
                (0.0, 1.0),
                spec["trigonometric ratio (sin, cos)"].Double2["first", (0.0, -1.0)]);
            Assert.Equal(
                (0.5, 0.86602540378),
                spec["trigonometric ratio (sin, cos)"].Double2["second", (0.0, -1.0)]);
            Assert.Equal(
                (0.70710678118, 0.70710678118),
                spec["trigonometric ratio (sin, cos)"].Double2["third", (0.0, -1.0)]);
            Assert.Equal(
                (0.0, -1.0),
                spec["trigonometric ratio (sin, cos)"].Double2["fourth", (0.0, -1.0)]);

            // act
            spec.Double2["initial coordinates"] = (101.2, 103.4);
            spec["trigonometric ratio (sin, cos)"].Double2["fourth"] =
                (1.0, 0.0);

            // assert
            Assert.Equal((101.2, 103.4), spec.Double2["initial coordinates"]);
            Assert.Equal(
                (1.0, 0.0),
                spec["trigonometric ratio (sin, cos)"].Double2["fourth"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"too short\": \"1.0\"",
                    "  \"too long\": \"1.0, 2.0, 3.0\"",
                    "  \"initial coordinates\": \"101.2, 103.4\"",
                    "  \"trigonometric ratio (sin, cos)\":",
                    "    \"first\": \"0.0, 1.0\"",
                    "    \"second\": \"0.5, 0.86602540378\"",
                    "    \"third\": \"0.70710678118, 0.70710678118\"",
                    "    \"fourth\": \"1, 0\""),
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
                    "  \"too short\": \"1.0\"",
                    "  \"too long\": \"1.0, 2.0, 3.0\"",
                    "  \"initial coordinates\": \"101.2, 103.4\"",
                    "  \"trigonometric ratio (sin, cos)\":",
                    "    \"first\": \"0.0, 1.0\"",
                    "    \"second\": \"0.5, 0.86602540378\"",
                    "    \"third\": \"0.70710678118, 0.70710678118\"",
                    "    \"fourth\": \"1, 0\""),
                reloadedSpec.ToString());
        }
    }
}
