// -----------------------------------------------------------------------
// <copyright file="DoubleValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class DoubleValueTests
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
                "      \"0\": \"0.0\"",
                "      \"1\": \"1.1\"",
                "      \"2\": \"2.2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.Double["list"];

            // assert
            Assert.Equal(
                new List<double>
                {
                    0.0,
                    1.1,
                    2.2,
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
                "      \"0\": \"zero\"",
                "      \"1\": \"1.1\"",
                "      \"2\": \"2.2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.Double["list"];
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
            spec.List.Double["list"] = new List<double>
            {
                0.0,
                1.1,
                2.2,
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"0\"",
                    "      \"1\": \"1.1\"",
                    "      \"2\": \"2.2\""),
                spec.ToString());
        }
    }
}
