// -----------------------------------------------------------------------
// <copyright file="SubSpecValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Value
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class SubSpecValueTests
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
                "      \"zero\":",
                "        \"inner\": \"0\"",
                "      \"one\":",
                "        \"inner\": \"1\"",
                "      \"two\":",
                "        \"inner\": \"2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Keyword.SubSpec["dict"].ToDictionary(
                kv => kv.Key, kv => kv.Value.Int["inner"]);

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "zero", 0 },
                    { "one", 1 },
                    { "two", 2 },
                },
                dict);
        }

        /// <summary>
        /// Test setter of value subspec.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InnerSetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"zero\":",
                "        \"inner\": \"0\"",
                "      \"one\":",
                "        \"inner\": \"1\"",
                "      \"two\":",
                "        \"inner\": \"2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Keyword.SubSpec["dict"];
            dict["zero"].Int["inner"] = 100;
            dict["one"].Int["second inner"] = 111;

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"zero\":",
                    "        \"inner\": \"100\"",
                    "      \"one\":",
                    "        \"inner\": \"1\"",
                    "        \"second inner\": \"111\"",
                    "      \"two\":",
                    "        \"inner\": \"2\""),
                spec.ToString());
        }
    }
}
