// -----------------------------------------------------------------------
// <copyright file="IntervalIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the IntervalIndexer class.
    /// </summary>
    public class IntervalIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the double accessor with an interval.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void LiteralAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"left outer (-10.5, 10.5)\": \"-10.5\"",
                "  \"left inner (-10.5, 10.5)\": \"-10.499999\"",
                "  \"middle (-10.5, 10.5)\": \"0.0\"",
                "  \"right inner (-10.5, 10.5)\": \"10.499999\"",
                "  \"right outer (-10.5, 10.5)\": \"10.5\"",
                "  \"left outer [-20.5, 20.5]\": \"-20.500001\"",
                "  \"left inner [-20.5, 20.5]\": \"-20.5\"",
                "  \"middle [-20.5, 20.5]\": \"0.0\"",
                "  \"right inner [-20.5, 20.5]\": \"20.5\"",
                "  \"right outer [-20.5, 20.5]\": \"20.500001\"",
                "  \"left outer ]-30.5, 30.5[\": \"-30.5\"",
                "  \"left inner ]-30.5, 30.5[\": \"-30.499999\"",
                "  \"middle ]-30.5, 30.5[\": \"0.0\"",
                "  \"right inner ]-30.5, 30.5[\": \"30.499999\"",
                "  \"right outer ]-30.5, 30.5[\": \"30.5\"",
                "  \"[-∞, 0.5)\": \"-10000.0\"",
                "  \"[0.5, ∞)\": \"10000.0\"",
                "  \"<-5.5, 5.5)\": \"0.0\"",
                "  \"[-5.5, 5.5>\": \"0.0\"",
                "  \"[5.5, -5.5]\": \"0.0\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["left outer (-10.5, 10.5)"];
            });
            Assert.Equal(
                -10.499999, spec.Interval('(', -10.5, 10.5, ')')["left inner (-10.5, 10.5)"]);
            Assert.Equal(
                0.0, spec.Interval('(', -10.5, 10.5, ')')["middle (-10.5, 10.5)"]);
            Assert.Equal(
                10.499999, spec.Interval('(', -10.5, 10.5, ')')["right inner (-10.5, 10.5)"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["right outer (-10.5, 10.5)"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('[', -20.5, 20.5, ']')["left outer [-20.5, 20.5]"];
            });
            Assert.Equal(
                -20.5, spec.Interval('[', -20.5, 20.5, ']')["left inner [-20.5, 20.5]"]);
            Assert.Equal(
                0.0, spec.Interval('[', -20.5, 20.5, ']')["middle [-20.5, 20.5]"]);
            Assert.Equal(
                20.5, spec.Interval('[', -20.5, 20.5, ']')["right inner [-20.5, 20.5]"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('[', -20.5, 20.5, ']')["right outer [-20.5, 20.5]"];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval(']', -30.5, 30.5, '[')["left outer ]-30.5, 30.5["];
            });
            Assert.Equal(
                -30.499999, spec.Interval(']', -30.5, 30.5, '[')["left inner ]-30.5, 30.5["]);
            Assert.Equal(
                0.0, spec.Interval(']', -30.5, 30.5, '[')["middle ]-30.5, 30.5["]);
            Assert.Equal(
                30.499999, spec.Interval(']', -30.5, 30.5, '[')["right inner ]-30.5, 30.5["]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval(']', -30.5, 30.5, '[')["right outer ]-30.5, 30.5["];
            });

            Assert.Equal(
                -10000.0,
                spec.Interval('[', double.NegativeInfinity, 0.5, ')')["[-∞, 0.5)"]);
            Assert.Equal(
                10000.0,
                spec.Interval('[', 0.5, double.PositiveInfinity, ')')["[0.5, ∞)"]);

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('<', -5.5, 5.5, ')')["<-5.5, 5.5)"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('[', -5.5, 5.5, '>')["[-5.5, 5.5>"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('[', 5.5, -5.5, ']')["[5.5, -5.5]"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["left outer (-10.5, 10.5)", 0.1];
            });
            Assert.Equal(
                -10.499999, spec.Interval('(', -10.5, 10.5, ')')["left inner (-10.5, 10.5)", 0.1]);
            Assert.Equal(
                0.0, spec.Interval('(', -10.5, 10.5, ')')["middle (-10.5, 10.5)", 0.1]);
            Assert.Equal(
                10.499999, spec.Interval('(', -10.5, 10.5, ')')["right inner (-10.5, 10.5)", 0.1]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["right outer (-10.5, 10.5)", 0.1];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('[', -20.5, 20.5, ']')["left outer [-20.5, 20.5]", 0.1];
            });
            Assert.Equal(
                -20.5, spec.Interval('[', -20.5, 20.5, ']')["left inner [-20.5, 20.5]", 0.1]);
            Assert.Equal(
                0.0, spec.Interval('[', -20.5, 20.5, ']')["middle [-20.5, 20.5]", 0.1]);
            Assert.Equal(
                20.5, spec.Interval('[', -20.5, 20.5, ']')["right inner [-20.5, 20.5]", 0.1]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval('[', -20.5, 20.5, ']')["right outer [-20.5, 20.5]", 0.1];
            });

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval(']', -30.5, 30.5, '[')["left outer ]-30.5, 30.5[", 0.1];
            });
            Assert.Equal(
                -30.499999, spec.Interval(']', -30.5, 30.5, '[')["left inner ]-30.5, 30.5[", 0.1]);
            Assert.Equal(
                0.0, spec.Interval(']', -30.5, 30.5, '[')["middle ]-30.5, 30.5[", 0.1]);
            Assert.Equal(
                30.499999, spec.Interval(']', -30.5, 30.5, '[')["right inner ]-30.5, 30.5[", 0.1]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interval(']', -30.5, 30.5, '[')["right outer ]-30.5, 30.5[", 0.1];
            });

            Assert.Equal(
                -10000.0,
                spec.Interval('[', double.NegativeInfinity, 0.5, ')')["[-∞, 0.5)", 0.1]);
            Assert.Equal(
                10000.0,
                spec.Interval('[', 0.5, double.PositiveInfinity, ')')["[0.5, ∞)", 1.1]);

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('<', -5.5, 5.5, ')')["<-5.5, 5.5)", 0.1];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('[', -5.5, 5.5, '>')["[-5.5, 5.5>", 0.1];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('[', 5.5, -5.5, ']')["[5.5, -5.5]", 0.1];
            });

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["left outer (-10.5, 10.5)", 11.0];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interval('(', -10.5, 10.5, ')')["left outer (-10.5, 10.5)", -11.0];
            });

            // act
            spec.Interval('[', -5.5, 5.5, ']')["new value left"] = -5.5;
            spec.Interval('[', -5.5, 5.5, ']')["new value right"] = 5.5;
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Interval('[', -5.5, 5.5, ']')["invalid"] = 5.50001;
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Interval('[', -5.5, 5.5, ']')["invalid"] = -5.50001;
            });

            // assert
            Assert.Equal(-5.5, spec.Interval('[', -5.5, 5.5, ']')["new value left"]);
            Assert.Equal(5.5, spec.Interval('[', -5.5, 5.5, ']')["new value right"]);
            Assert.Equal(
                Utility.JoinLines(
                "\"properties\":",
                "  \"left outer (-10.5, 10.5)\": \"-10.5\"",
                "  \"left inner (-10.5, 10.5)\": \"-10.499999\"",
                "  \"middle (-10.5, 10.5)\": \"0.0\"",
                "  \"right inner (-10.5, 10.5)\": \"10.499999\"",
                "  \"right outer (-10.5, 10.5)\": \"10.5\"",
                "  \"left outer [-20.5, 20.5]\": \"-20.500001\"",
                "  \"left inner [-20.5, 20.5]\": \"-20.5\"",
                "  \"middle [-20.5, 20.5]\": \"0.0\"",
                "  \"right inner [-20.5, 20.5]\": \"20.5\"",
                "  \"right outer [-20.5, 20.5]\": \"20.500001\"",
                "  \"left outer ]-30.5, 30.5[\": \"-30.5\"",
                "  \"left inner ]-30.5, 30.5[\": \"-30.499999\"",
                "  \"middle ]-30.5, 30.5[\": \"0.0\"",
                "  \"right inner ]-30.5, 30.5[\": \"30.499999\"",
                "  \"right outer ]-30.5, 30.5[\": \"30.5\"",
                "  \"[-∞, 0.5)\": \"-10000.0\"",
                "  \"[0.5, ∞)\": \"10000.0\"",
                "  \"<-5.5, 5.5)\": \"0.0\"",
                "  \"[-5.5, 5.5>\": \"0.0\"",
                "  \"[5.5, -5.5]\": \"0.0\"",
                "  \"new value left\": \"-5.5\"",
                "  \"new value right\": \"5.5\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                "\"properties\":",
                "  \"left outer (-10.5, 10.5)\": \"-10.5\"",
                "  \"left inner (-10.5, 10.5)\": \"-10.499999\"",
                "  \"middle (-10.5, 10.5)\": \"0.0\"",
                "  \"right inner (-10.5, 10.5)\": \"10.499999\"",
                "  \"right outer (-10.5, 10.5)\": \"10.5\"",
                "  \"left outer [-20.5, 20.5]\": \"-20.500001\"",
                "  \"left inner [-20.5, 20.5]\": \"-20.5\"",
                "  \"middle [-20.5, 20.5]\": \"0.0\"",
                "  \"right inner [-20.5, 20.5]\": \"20.5\"",
                "  \"right outer [-20.5, 20.5]\": \"20.500001\"",
                "  \"left outer ]-30.5, 30.5[\": \"-30.5\"",
                "  \"left inner ]-30.5, 30.5[\": \"-30.499999\"",
                "  \"middle ]-30.5, 30.5[\": \"0.0\"",
                "  \"right inner ]-30.5, 30.5[\": \"30.499999\"",
                "  \"right outer ]-30.5, 30.5[\": \"30.5\"",
                "  \"[-∞, 0.5)\": \"-10000.0\"",
                "  \"[0.5, ∞)\": \"10000.0\"",
                "  \"<-5.5, 5.5)\": \"0.0\"",
                "  \"[-5.5, 5.5>\": \"0.0\"",
                "  \"[5.5, -5.5]\": \"0.0\"",
                "  \"new value left\": \"-5.5\"",
                "  \"new value right\": \"5.5\""),
                reloadedSpec.ToString());
        }
    }
}
