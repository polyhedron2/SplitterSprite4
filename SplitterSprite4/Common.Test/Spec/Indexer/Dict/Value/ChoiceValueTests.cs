// -----------------------------------------------------------------------
// <copyright file="ChoiceValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Value
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ChoiceValueTests
    {
        public class ChoiceValueWithListAndFuncTest
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
                    "      \"あか\": \"赤\"",
                    "      \"みどり\": \"緑\"",
                    "      \"あお\": \"青\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<string, ChoiceIndexerTests.Color>
                    {
                    { "あか", ChoiceIndexerTests.Color.RED },
                    { "みどり", ChoiceIndexerTests.Color.GREEN },
                    { "あお", ChoiceIndexerTests.Color.BLUE },
                    },
                    dict);
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
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"あか\": \"赤\"",
                    "      \"みどり\": \"緑\"",
                    "      \"きいろ\": \"黄色\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["dict"];
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
                spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
            {
                { "あか", ChoiceIndexerTests.Color.RED },
                { "みどり", ChoiceIndexerTests.Color.GREEN },
                { "あお", ChoiceIndexerTests.Color.BLUE },
            };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"あお\": \"青\"",
                        "      \"あか\": \"赤\"",
                        "      \"みどり\": \"緑\""),
                    spec.ToString());
            }

            /// <summary>
            /// Test setter with invalid value.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            [Theory]
            [InlineData("foo.spec")]
            [InlineData("dir/bar.spec")]
            [InlineData("dir1/dir2/baz.spec")]
            public void InvalidValueSetterTest(string path)
            {
                // arrange
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
                    spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
                {
                    { "あか", ChoiceIndexerTests.Color.RED },
                    { "みどり", ChoiceIndexerTests.Color.GREEN },
                    { "きいろ", ChoiceIndexerTests.Color.YELLOW },
                };
                });
            }
        }

        public class ChoiceValueWithListTest
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
                    "      \"あか\": \"RED\"",
                    "      \"みどり\": \"GREEN\"",
                    "      \"あお\": \"BLUE\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices)["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<string, ChoiceIndexerTests.Color>
                    {
                        { "あか", ChoiceIndexerTests.Color.RED },
                        { "みどり", ChoiceIndexerTests.Color.GREEN },
                        { "あお", ChoiceIndexerTests.Color.BLUE },
                    },
                    dict);
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
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"あか\": \"RED\"",
                    "      \"みどり\": \"GREEN\"",
                    "      \"きいろ\": \"YELLOW\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices)["dict"];
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
                spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
                {
                    { "あか", ChoiceIndexerTests.Color.RED },
                    { "みどり", ChoiceIndexerTests.Color.GREEN },
                    { "あお", ChoiceIndexerTests.Color.BLUE },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"あお\": \"BLUE\"",
                        "      \"あか\": \"RED\"",
                        "      \"みどり\": \"GREEN\""),
                    spec.ToString());
            }

            /// <summary>
            /// Test setter with invalid value.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            [Theory]
            [InlineData("foo.spec")]
            [InlineData("dir/bar.spec")]
            [InlineData("dir1/dir2/baz.spec")]
            public void InvalidValueSetterTest(string path)
            {
                // arrange
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
                    spec.Dict.Keyword.Choice(ChoiceIndexerTests.ListChoices)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
                    {
                        { "あか", ChoiceIndexerTests.Color.RED },
                        { "みどり", ChoiceIndexerTests.Color.GREEN },
                        { "きいろ", ChoiceIndexerTests.Color.YELLOW },
                    };
                });
            }
        }

        public class ChoiceValueWithDictTest
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
                    "      \"あか\": \"ROSSO\"",
                    "      \"みどり\": \"VERDE\"",
                    "      \"あお\": \"AZZURRO\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Keyword.Choice(ChoiceIndexerTests.DictChoices)["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<string, ChoiceIndexerTests.Color>
                    {
                        { "あか", ChoiceIndexerTests.Color.RED },
                        { "みどり", ChoiceIndexerTests.Color.GREEN },
                        { "あお", ChoiceIndexerTests.Color.BLUE },
                    },
                    dict);
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
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"あか\": \"ROSSO\"",
                    "      \"みどり\": \"VERDE\"",
                    "      \"きいろ\": \"GIALLO\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Keyword.Choice(ChoiceIndexerTests.DictChoices)["dict"];
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
                spec.Dict.Keyword.Choice(ChoiceIndexerTests.DictChoices)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
                {
                    { "あか", ChoiceIndexerTests.Color.RED },
                    { "みどり", ChoiceIndexerTests.Color.GREEN },
                    { "あお", ChoiceIndexerTests.Color.BLUE },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"あお\": \"AZZURRO\"",
                        "      \"あか\": \"ROSSO\"",
                        "      \"みどり\": \"VERDE\""),
                    spec.ToString());
            }

            /// <summary>
            /// Test setter with invalid value.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            [Theory]
            [InlineData("foo.spec")]
            [InlineData("dir/bar.spec")]
            [InlineData("dir1/dir2/baz.spec")]
            public void InvalidValueSetterTest(string path)
            {
                // arrange
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
                    spec.Dict.Keyword.Choice(ChoiceIndexerTests.DictChoices)["dict"] = new Dictionary<string, ChoiceIndexerTests.Color>
                    {
                        { "あか", ChoiceIndexerTests.Color.RED },
                        { "みどり", ChoiceIndexerTests.Color.GREEN },
                        { "きいろ", ChoiceIndexerTests.Color.YELLOW },
                    };
                });
            }
        }
    }
}
