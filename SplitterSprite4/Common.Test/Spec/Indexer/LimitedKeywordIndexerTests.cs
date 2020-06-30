// -----------------------------------------------------------------------
// <copyright file="LimitedKeywordIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the LimitedKeywordIndexer class.
    /// </summary>
    public class LimitedKeywordIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the string accessor without line feed code.
        /// The string length is bounded.
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
                "    \"1st\"",
                "    \"2nd\"",
                "  \"too long\": \"8 length\"",
                "  \"長さオーバー\": \"これは８文字です\"",
                "  \"name\": \"7length\"",
                "  \"名前\": \"ななもじの名前\"",
                "  \"children names\":",
                "    \"first\": \"child\"",
                "    \"second\": \"こどもの名前\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.LimitedKeyword(-1)["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["too long"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["長さオーバー"];
            });
            Assert.Equal("7length", spec.LimitedKeyword(7)["name"]);
            Assert.Equal("ななもじの名前", spec.LimitedKeyword(7)["名前"]);
            Assert.Equal(
                "child", spec["children names"].LimitedKeyword(7)["first"]);
            Assert.Equal(
                "こどもの名前", spec["children names"].LimitedKeyword(7)["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["children names"].LimitedKeyword(7)["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.LimitedKeyword(-1)["invalid", "bogus"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["invalid", "bogus"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["too long", "bogus"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.LimitedKeyword(7)["長さオーバー", "bogus"];
            });
            Assert.Equal("7length", spec.LimitedKeyword(7)["name", "bogus"]);
            Assert.Equal("ななもじの名前", spec.LimitedKeyword(7)["名前", "bogus"]);
            Assert.Equal(
                "child", spec["children names"].LimitedKeyword(7)["first", "bogus"]);
            Assert.Equal(
                "こどもの名前", spec["children names"].LimitedKeyword(7)["second", "bogus"]);
            Assert.Equal(
                "bogus", spec["children names"].LimitedKeyword(7)["third", "bogus"]);

            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec["children names"].LimitedKeyword(7)["second", "bo\ngus"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec["children names"].LimitedKeyword(7)["second", "too long default"];
            });

            // act
            spec.LimitedKeyword(7)["name"] = "length7";
            spec.LimitedKeyword(7)["名前"] = "名前がななもじ";
            spec["children names"].LimitedKeyword(7)["third"] = "3rd";
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.LimitedKeyword(7)["改行あり"] = "な\nま\nえ";
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.LimitedKeyword(7)["name"] = "too long name";
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.LimitedKeyword(7)["名前"] = "ながすぎるなまえ";
            });

            // assert
            Assert.Equal("length7", spec.LimitedKeyword(7)["name"]);
            Assert.Equal("名前がななもじ", spec.LimitedKeyword(7)["名前"]);
            Assert.Equal("3rd", spec["children names"].LimitedKeyword(7)["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": |+",
                    "    \"1st\"",
                    "    \"2nd\"",
                    "  \"too long\": \"8 length\"",
                    "  \"長さオーバー\": \"これは８文字です\"",
                    "  \"name\": \"length7\"",
                    "  \"名前\": \"名前がななもじ\"",
                    "  \"children names\":",
                    "    \"first\": \"child\"",
                    "    \"second\": \"こどもの名前\"",
                    "    \"third\": \"3rd\""),
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
                    "    \"1st\"",
                    "    \"2nd\"",
                    "  \"too long\": \"8 length\"",
                    "  \"長さオーバー\": \"これは８文字です\"",
                    "  \"name\": \"length7\"",
                    "  \"名前\": \"名前がななもじ\"",
                    "  \"children names\":",
                    "    \"first\": \"child\"",
                    "    \"second\": \"こどもの名前\"",
                    "    \"third\": \"3rd\""),
                reloadedSpec.ToString());
        }
    }
}
