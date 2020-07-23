// -----------------------------------------------------------------------
// <copyright file="SubSpecValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\":",
                "        \"inner\": \"0\"",
                "      \"1\":",
                "        \"inner\": \"1\"",
                "      \"2\":",
                "        \"inner\": \"2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.SubSpec["list"].Select(spec => spec.Int["inner"]);

            // assert
            Assert.Equal(
                new List<int>
                {
                    0,
                    1,
                    2,
                },
                list);
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\":",
                "        \"inner\": \"0\"",
                "      \"1\":",
                "        \"inner\": \"1\"",
                "      \"2\":",
                "        \"inner\": \"2\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.SubSpec["list"];
            list[0].Int["inner"] = 100;
            list[1].Int["second inner"] = 111;

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\":",
                    "        \"inner\": \"100\"",
                    "      \"1\":",
                    "        \"inner\": \"1\"",
                    "        \"second inner\": \"111\"",
                    "      \"2\":",
                    "        \"inner\": \"2\""),
                spec.ToString());
        }
    }
}
