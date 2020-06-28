// -----------------------------------------------------------------------
// <copyright file="Double3IndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Double3Indexer class.
    /// </summary>
    public class Double3IndexerTests
    {
        /// <summary>
        /// Test the (double x, double y, double z) accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void Double3Test(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"dummy\"",
                "  \"too short\": \"1.0, 2.0\"",
                "  \"too long\": \"1.0, 2.0, 3.0, 4.0\"",
                "  \"initial coordinates\": \"1.2, 3.4, 5.6\"",
                "  \"trigonometric ratio (sin, cos, tan)\":",
                "    \"first\": \"0.0, 1.0, 0.0\"",
                "    \"second\": \"0.5, 0.86602540378, 0.57735026919\"",
                "    \"third\": \"0.70710678118, 0.70710678118, 1.0\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["too short"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["too long"];
            });
            Assert.Equal((1.2, 3.4, 5.6), spec.Double3["initial coordinates"]);
            Assert.Equal(
                (0.0, 1.0, 0.0),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["first"]);
            Assert.Equal(
                (0.5, 0.86602540378, 0.57735026919),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["second"]);
            Assert.Equal(
                (0.70710678118, 0.70710678118, 1.0),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["trigonometric ratio (sin, cos, tan)"].Double3["fourth"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["invalid", (0.0, 0.0, 0.0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["too short", (0.0, 0.0, 0.0)];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double3["too long", (0.0, 0.0, 0.0)];
            });
            Assert.Equal((1.2, 3.4, 5.6), spec.Double3["initial coordinates", (0.0, 0.0, 0.0)]);
            Assert.Equal(
                (0.0, 1.0, 0.0),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["first", (0.0, -1.0, 0.0)]);
            Assert.Equal(
                (0.5, 0.86602540378, 0.57735026919),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["second", (0.0, -1.0, 0.0)]);
            Assert.Equal(
                (0.70710678118, 0.70710678118, 1.0),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["third", (0.0, -1.0, 0.0)]);
            Assert.Equal(
                (0.0, -1.0, 0.0),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["fourth", (0.0, -1.0, 0.0)]);

            // act
            spec.Double3["initial coordinates"] = (101.2, 103.4, 105.6);
            spec["trigonometric ratio (sin, cos, tan)"].Double3["fourth"] =
                (1.0, 0.0, double.PositiveInfinity);

            // assert
            Assert.Equal((101.2, 103.4, 105.6), spec.Double3["initial coordinates"]);
            Assert.Equal(
                (1.0, 0.0, double.PositiveInfinity),
                spec["trigonometric ratio (sin, cos, tan)"].Double3["fourth"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    "  \"too short\": \"1.0, 2.0\"",
                    "  \"too long\": \"1.0, 2.0, 3.0, 4.0\"",
                    "  \"initial coordinates\": \"101.2, 103.4, 105.6\"",
                    "  \"trigonometric ratio (sin, cos, tan)\":",
                    "    \"first\": \"0.0, 1.0, 0.0\"",
                    "    \"second\": \"0.5, 0.86602540378, 0.57735026919\"",
                    "    \"third\": \"0.70710678118, 0.70710678118, 1.0\"",
                    "    \"fourth\": \"1, 0, ∞\""),
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
                    "  \"too short\": \"1.0, 2.0\"",
                    "  \"too long\": \"1.0, 2.0, 3.0, 4.0\"",
                    "  \"initial coordinates\": \"101.2, 103.4, 105.6\"",
                    "  \"trigonometric ratio (sin, cos, tan)\":",
                    "    \"first\": \"0.0, 1.0, 0.0\"",
                    "    \"second\": \"0.5, 0.86602540378, 0.57735026919\"",
                    "    \"third\": \"0.70710678118, 0.70710678118, 1.0\"",
                    "    \"fourth\": \"1, 0, ∞\""),
                reloadedSpec.ToString());
        }
    }
}
