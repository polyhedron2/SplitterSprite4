﻿// -----------------------------------------------------------------------
// <copyright file="IntervalKeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Key
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class IntervalKeyTests
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
            // arInterval
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"0\": \"zero\"",
                "      \"2\": \"two\"",
                "      \"4\": \"four\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Interval('[', 0.0, 5.0, ')').Keyword["dict"];

            // assert
            Assert.Equal(
                new Dictionary<double, string>
                {
                    { 0, "zero" },
                    { 2, "two" },
                    { 4, "four" },
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
            // arInterval
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"0\": \"zero\"",
                "      \"2\": \"two\"",
                "      \"5\": \"five\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Interval('[', 0.0, 5.0, ')').Keyword["dict"];
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
            // arInterval
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.Dict.Interval('[', 0.0, 5.0, ')').Keyword["dict"] = new Dictionary<double, string>
            {
                { 0, "zero" },
                { 2, "two" },
                { 4, "four" },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"0\": \"zero\"",
                    "      \"2\": \"two\"",
                    "      \"4\": \"four\""),
                spec.ToString());
        }

        /// <summary>
        /// Test setter with invalid key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InvalidKeySetterTest(string path)
        {
            // arInterval
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                spec.Dict.Interval('[', 0.0, 5.0, ')').Keyword["dict"] = new Dictionary<double, string>
                {
                    { 0, "zero" },
                    { 2, "two" },
                    { 5, "five" },
                };
            });
        }
    }
}
