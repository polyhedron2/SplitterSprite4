// -----------------------------------------------------------------------
// <copyright file="Double3KeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class Double3KeyTests
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
                "      \"0.0, 0.0, 1.1\": \"zero-zero-one\"",
                "      \"0.0, 1.1, 0.0\": \"zero-one-zero\"",
                "      \"1.1, 0.0, 0.0\": \"one-zero-zero\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Double3.Keyword["dict"];

            // assert
            Assert.Equal(
                new Dictionary<(double, double, double), string>
                {
                    { (0.0, 0.0, 1.1), "zero-zero-one" },
                    { (0.0, 1.1, 0.0), "zero-one-zero" },
                    { (1.1, 0.0, 0.0), "one-zero-zero" },
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
                "      \"zero\": \"invalid\"",
                "      \"0.0, 0.0, 0.0\": \"zero-zero-zero\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Double3.Keyword["dict"];
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
            spec.Dict.Double3.Keyword["dict"] = new Dictionary<(double, double, double), string>
            {
                { (0.0, 0.0, 1.1), "zero-zero-one" },
                { (0.0, 1.1, 0.0), "zero-one-zero" },
                { (1.1, 0.0, 0.0), "one-zero-zero" },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"0, 0, 1.1\": \"zero-zero-one\"",
                    "      \"0, 1.1, 0\": \"zero-one-zero\"",
                    "      \"1.1, 0, 0\": \"one-zero-zero\""),
                spec.ToString());
        }
    }
}
