// -----------------------------------------------------------------------
// <copyright file="DoubleKeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class DoubleKeyTests
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
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"0.0\": \"zero\"",
                "      \"1.1\": \"one\"",
                "      \"2.2\": \"two\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Double.Keyword["dict"];

            // assert
            Assert.Equal(
                new Dictionary<double, string>
                {
                    { 0.0, "zero" },
                    { 1.1, "one" },
                    { 2.2, "two" },
                },
                dict);
        }

        /// <summary>
        /// Test getter with invalid key.
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
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"zero\": \"zero\"",
                "      \"1.1\": \"one\"",
                "      \"2.2\": \"two\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Double.Keyword["dict"];
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
            spec.Dict.Double.Keyword["dict"] = new Dictionary<double, string>
            {
                { 0.0, "zero" },
                { 1.1, "one" },
                { 2.2, "two" },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"0\": \"zero\"",
                    "      \"1.1\": \"one\"",
                    "      \"2.2\": \"two\""),
                spec.ToString());
        }
    }
}
