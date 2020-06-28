// -----------------------------------------------------------------------
// <copyright file="RangeIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the RangeIndexer class.
    /// </summary>
    public class RangeIndexerTests : ScalarIndexerTests
    {
        /// <summary>
        /// Test the ranged integer accessor.
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
                "  \"left outer (-10, 10)\": \"-10\"",
                "  \"left inner (-10, 10)\": \"-9\"",
                "  \"middle (-10, 10)\": \"0\"",
                "  \"right inner (-10, 10)\": \"9\"",
                "  \"right outer (-10, 10)\": \"10\"",
                "  \"left outer [-20, 20]\": \"-21\"",
                "  \"left inner [-20, 20]\": \"-20\"",
                "  \"middle [-20, 20]\": \"0\"",
                "  \"right inner [-20, 20]\": \"20\"",
                "  \"right outer [-20, 20]\": \"21\"",
                "  \"left outer ]-30, 30[\": \"-30\"",
                "  \"left inner ]-30, 30[\": \"-29\"",
                "  \"middle ]-30, 30[\": \"0\"",
                "  \"right inner ]-30, 30[\": \"29\"",
                "  \"right outer ]-30, 30[\": \"30\"",
                "  \"[-∞, 0)\": \"-10000\"",
                "  \"[0, ∞)\": \"10000\"",
                "  \"<-5, 5)\": \"0\"",
                "  \"[-5, 5>\": \"0\"",
                "  \"[5, -5]\": \"0\"",
                "  \"[0.25, 0.75]\": \"0\"",
                "  \"left outer [0, 10)\": \"-1\"",
                "  \"left inner [0, 10)\": \"0\"",
                "  \"right inner [0, 10)\": \"9\"",
                "  \"right outer [0, 10)\": \"10\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range('(', -10, 10, ')')["left outer (-10, 10)"];
            });
            Assert.Equal(
                -9, spec.Range('(', -10, 10, ')')["left inner (-10, 10)"]);
            Assert.Equal(
                0, spec.Range('(', -10, 10, ')')["middle (-10, 10)"]);
            Assert.Equal(
                9, spec.Range('(', -10, 10, ')')["right inner (-10, 10)"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range('(', -10, 10, ')')["right outer (-10, 10)"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range('[', -20, 20, ']')["left outer [-20, 20]"];
            });
            Assert.Equal(
                -20, spec.Range('[', -20, 20, ']')["left inner [-20, 20]"]);
            Assert.Equal(
                0, spec.Range('[', -20, 20, ']')["middle [-20, 20]"]);
            Assert.Equal(
                20, spec.Range('[', -20, 20, ']')["right inner [-20, 20]"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range('[', -20, 20, ']')["right outer [-20, 20]"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(']', -30, 30, '[')["left outer ]-30, 30["];
            });
            Assert.Equal(
                -29, spec.Range(']', -30, 30, '[')["left inner ]-30, 30["]);
            Assert.Equal(
                0, spec.Range(']', -30, 30, '[')["middle ]-30, 30["]);
            Assert.Equal(
                29, spec.Range(']', -30, 30, '[')["right inner ]-30, 30["]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(']', -30, 30, '[')["right outer ]-30, 30["];
            });

            Assert.Equal(
                -10000,
                spec.Range('[', double.NegativeInfinity, 0, ')')["[-∞, 0)"]);
            Assert.Equal(
                10000,
                spec.Range('[', 0, double.PositiveInfinity, ')')["[0, ∞)"]);

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range('<', -5, 5, ')')["<-5, 5)"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range('[', -5, 5, '>')["[-5, 5>"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range('[', 5, -5, ']')["[5, -5]"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range('[', 0.25, 0.75, ']')["[0.25, 0.75]"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(0, 10)["left outer [0, 10)"];
            });
            Assert.Equal(0, spec.Range(0, 10)["left inner [0, 10)"]);
            Assert.Equal(9, spec.Range(0, 10)["right inner [0, 10)"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(0, 10)["right outer [0, 10)"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(10)["left outer [0, 10)"];
            });
            Assert.Equal(0, spec.Range(10)["left inner [0, 10)"]);
            Assert.Equal(9, spec.Range(10)["right inner [0, 10)"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Range(10)["right outer [0, 10)"];
            });

            // get value with default value.
            Assert.Equal(0, spec.Range(10)["left inner [0, 10)", 5]);
            Assert.Equal(5, spec.Range(10)["undefined", 5]);
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range(10)["left inner [0, 10)", -1];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Range(10)["left inner [0, 10)", 10];
            });

            // act
            spec.Range(10)["new value left"] = 0;
            spec.Range(10)["new value right"] = 9;
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Range(10)["invalid"] = -1;
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Range(10)["invalid"] = 10;
            });

            // assert
            Assert.Equal(0, spec.Range(10)["new value left"]);
            Assert.Equal(9, spec.Range(10)["new value right"]);
            Assert.Equal(
                Utility.JoinLines(
                "\"properties\":",
                "  \"left outer (-10, 10)\": \"-10\"",
                "  \"left inner (-10, 10)\": \"-9\"",
                "  \"middle (-10, 10)\": \"0\"",
                "  \"right inner (-10, 10)\": \"9\"",
                "  \"right outer (-10, 10)\": \"10\"",
                "  \"left outer [-20, 20]\": \"-21\"",
                "  \"left inner [-20, 20]\": \"-20\"",
                "  \"middle [-20, 20]\": \"0\"",
                "  \"right inner [-20, 20]\": \"20\"",
                "  \"right outer [-20, 20]\": \"21\"",
                "  \"left outer ]-30, 30[\": \"-30\"",
                "  \"left inner ]-30, 30[\": \"-29\"",
                "  \"middle ]-30, 30[\": \"0\"",
                "  \"right inner ]-30, 30[\": \"29\"",
                "  \"right outer ]-30, 30[\": \"30\"",
                "  \"[-∞, 0)\": \"-10000\"",
                "  \"[0, ∞)\": \"10000\"",
                "  \"<-5, 5)\": \"0\"",
                "  \"[-5, 5>\": \"0\"",
                "  \"[5, -5]\": \"0\"",
                "  \"[0.25, 0.75]\": \"0\"",
                "  \"left outer [0, 10)\": \"-1\"",
                "  \"left inner [0, 10)\": \"0\"",
                "  \"right inner [0, 10)\": \"9\"",
                "  \"right outer [0, 10)\": \"10\"",
                "  \"new value left\": \"0\"",
                "  \"new value right\": \"9\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                "\"properties\":",
                "  \"left outer (-10, 10)\": \"-10\"",
                "  \"left inner (-10, 10)\": \"-9\"",
                "  \"middle (-10, 10)\": \"0\"",
                "  \"right inner (-10, 10)\": \"9\"",
                "  \"right outer (-10, 10)\": \"10\"",
                "  \"left outer [-20, 20]\": \"-21\"",
                "  \"left inner [-20, 20]\": \"-20\"",
                "  \"middle [-20, 20]\": \"0\"",
                "  \"right inner [-20, 20]\": \"20\"",
                "  \"right outer [-20, 20]\": \"21\"",
                "  \"left outer ]-30, 30[\": \"-30\"",
                "  \"left inner ]-30, 30[\": \"-29\"",
                "  \"middle ]-30, 30[\": \"0\"",
                "  \"right inner ]-30, 30[\": \"29\"",
                "  \"right outer ]-30, 30[\": \"30\"",
                "  \"[-∞, 0)\": \"-10000\"",
                "  \"[0, ∞)\": \"10000\"",
                "  \"<-5, 5)\": \"0\"",
                "  \"[-5, 5>\": \"0\"",
                "  \"[5, -5]\": \"0\"",
                "  \"[0.25, 0.75]\": \"0\"",
                "  \"left outer [0, 10)\": \"-1\"",
                "  \"left inner [0, 10)\": \"0\"",
                "  \"right inner [0, 10)\": \"9\"",
                "  \"right outer [0, 10)\": \"10\"",
                "  \"new value left\": \"0\"",
                "  \"new value right\": \"9\""),
                reloadedSpec.ToString());
        }
    }
}
