// -----------------------------------------------------------------------
// <copyright file="BoolValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class BoolValueTests
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
                "      \"0\": \"False\"",
                "      \"1\": \"True\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.Bool["list"];

            // assert
            Assert.Equal(
                new List<bool>
                {
                    false,
                    true,
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
        public void InvalidKeyGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\": \"False!!!\"",
                "      \"1\": \"True\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Bool.Keyword["list"];
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
            spec.List.Bool["list"] = new List<bool>
            {
                false,
                true,
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"False\"",
                    "      \"1\": \"True\""),
                spec.ToString());
        }
    }
}
