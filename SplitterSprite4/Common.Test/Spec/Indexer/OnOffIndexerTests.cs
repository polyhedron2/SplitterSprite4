// -----------------------------------------------------------------------
// <copyright file="OnOffIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the OnOffIndexer class.
    /// </summary>
    public class OnOffIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the boolean accessor with "on" or "off".
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void LiteralAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"True\"",
                "  \"special\": \"ON\"",
                "  \"story flag\":",
                "    \"first\": \"On\"",
                "    \"second\": \"off\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.OnOff["invalid"];
            });
            Assert.True(spec.OnOff["special"]);
            Assert.True(spec["story flag"].OnOff["first"]);
            Assert.False(spec["story flag"].OnOff["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["story flag"].OnOff["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.OnOff["invalid", false];
            });
            Assert.True(spec.OnOff["special", false]);
            Assert.True(spec["story flag"].OnOff["first", false]);
            Assert.False(spec["story flag"].OnOff["second", true]);
            Assert.True(spec["story flag"].OnOff["third", true]);

            // act
            spec.OnOff["special"] = false;
            spec["story flag"].OnOff["third"] = true;

            // assert
            Assert.False(spec.OnOff["special", false]);
            Assert.True(spec["story flag"].OnOff["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"True\"",
                    "  \"special\": \"off\"",
                    "  \"story flag\":",
                    "    \"first\": \"On\"",
                    "    \"second\": \"off\"",
                    "    \"third\": \"on\""),
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
                    "  \"special\": \"off\"",
                    "  \"story flag\":",
                    "    \"first\": \"On\"",
                    "    \"second\": \"off\"",
                    "    \"third\": \"on\""),
                reloadedSpec.ToString());
        }
    }
}
