// -----------------------------------------------------------------------
// <copyright file="KeywordIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the KeywordIndexer class.
    /// </summary>
    public class KeywordIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the string accessor without line feed code.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void LiteralAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": |+",
                "    \"1st line\"",
                "    \"2nd line\"",
                "  \"magic\": \"__magic word__\"",
                "  \"non-magic\": \"____non-magic word____\"",
                "  \"father name\": \"masuo\"",
                "  \"mother name\": \"sazae\"",
                "  \"children names\":",
                "    \"first\": \"tarao\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Keyword["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Keyword["magic"];
            });
            Assert.Equal("__non-magic word__", spec.Keyword["non-magic"]);
            Assert.Equal("masuo", spec.Keyword["father name"]);
            Assert.Equal("sazae", spec.Keyword["mother name"]);
            Assert.Equal(
                "tarao", spec["children names"].Keyword["first"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["children names"].Keyword["second"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Keyword["invalid", "bogus"];
            });
            Assert.Equal("masuo", spec.Keyword["father name", "bogus"]);
            Assert.Equal("sazae", spec.Keyword["mother name", "bogus"]);
            Assert.Equal(
                "tarao", spec["children names"].Keyword["first", "bogus"]);
            Assert.Equal(
                "bogus", spec["children names"].Keyword["second", "bogus"]);

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec["children names"].Keyword["second", "bo\ngus"];
            });

            // act
            spec.Keyword["non-magic"] = "__new non-magic word__";
            spec.Keyword["grand father name"] = "namihei";
            spec.Keyword["grand mother name"] = "fune";
            spec["children names"].Keyword["second"] = "hitode";
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Keyword["uncle"] = "ka\ntsu\no";
            });

            // assert
            Assert.Equal("namihei", spec.Keyword["grand father name"]);
            Assert.Equal("fune", spec.Keyword["grand mother name"]);
            Assert.Equal("hitode", spec["children names"].Keyword["second"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": |+",
                    "    \"1st line\"",
                    "    \"2nd line\"",
                    "  \"magic\": \"__magic word__\"",
                    "  \"non-magic\": \"____new non-magic word____\"",
                    "  \"father name\": \"masuo\"",
                    "  \"mother name\": \"sazae\"",
                    "  \"children names\":",
                    "    \"first\": \"tarao\"",
                    "    \"second\": \"hitode\"",
                    "  \"grand father name\": \"namihei\"",
                    "  \"grand mother name\": \"fune\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": |+",
                    "    \"1st line\"",
                    "    \"2nd line\"",
                    "  \"magic\": \"__magic word__\"",
                    "  \"non-magic\": \"____new non-magic word____\"",
                    "  \"father name\": \"masuo\"",
                    "  \"mother name\": \"sazae\"",
                    "  \"children names\":",
                    "    \"first\": \"tarao\"",
                    "    \"second\": \"hitode\"",
                    "  \"grand father name\": \"namihei\"",
                    "  \"grand mother name\": \"fune\""),
                reloadedSpec.ToString());
        }
    }
}
