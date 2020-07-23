// -----------------------------------------------------------------------
// <copyright file="TextValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class TextValueTests
    {
        /// <summary>
        /// Test getter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void GetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\": |+",
                "        \"１の前の整数\"",
                "        \"最小の非負整数\"",
                "        \"加法単位元\"",
                "        \"[End Of Text]\"",
                "      \"1\": |+",
                "        \"２の前の整数\"",
                "        \"最小のカタラン数\"",
                "        \"乗法単位元\"",
                "        \"[End Of Text]\"",
                "      \"2\": |+",
                "        \"３の前の整数\"",
                "        \"最小の素数\"",
                "        \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.Text["list"];

            // assert
            Assert.Equal(
                new List<string>
                {
                    Utility.JoinLines(
                        "１の前の整数",
                        "最小の非負整数",
                        "加法単位元"),
                    Utility.JoinLines(
                        "２の前の整数",
                        "最小のカタラン数",
                        "乗法単位元"),
                    Utility.JoinLines(
                        "３の前の整数",
                        "最小の素数"),
                },
                list);
        }

        /// <summary>
        /// Test getter with invalid value.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InvalidValueGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"零\": |+",
                "        \"１の前の整数\"",
                "        \"最小の非負整数\"",
                "        \"加法単位元\"",
                "      \"壱\": |+",
                "        \"２の前の整数\"",
                "        \"最小のカタラン数\"",
                "        \"乗法単位元\"",
                "        \"[End Of Text]\"",
                "      \"弐\": |+",
                "        \"３の前の整数\"",
                "        \"最小の素数\"",
                "        \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.Text["list"];
            });
        }

        /// <summary>
        /// Test setter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.List.Text["list"] = new List<string>
            {
                Utility.JoinLines(
                    "１の前の整数",
                    "最小の非負整数",
                    "加法単位元"),
                Utility.JoinLines(
                    "２の前の整数",
                    "最小のカタラン数",
                    "乗法単位元"),
                Utility.JoinLines(
                    "３の前の整数",
                    "最小の素数"),
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": |+",
                    "        \"１の前の整数\"",
                    "        \"最小の非負整数\"",
                    "        \"加法単位元\"",
                    "        \"[End Of Text]\"",
                    "      \"1\": |+",
                    "        \"２の前の整数\"",
                    "        \"最小のカタラン数\"",
                    "        \"乗法単位元\"",
                    "        \"[End Of Text]\"",
                    "      \"2\": |+",
                    "        \"３の前の整数\"",
                    "        \"最小の素数\"",
                    "        \"[End Of Text]\""),
                spec.ToString());
        }
    }
}
