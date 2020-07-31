// -----------------------------------------------------------------------
// <copyright file="ChoiceValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ChoceValueTests
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
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"赤\"",
                    "      \"1\": \"緑\"",
                    "      \"2\": \"青\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var list = spec.List.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["list"];

                // assert
                Assert.Equal(
                    new List<ChoiceIndexerTests.Color>
                    {
                        { ChoiceIndexerTests.Color.RED },
                        { ChoiceIndexerTests.Color.GREEN },
                        { ChoiceIndexerTests.Color.BLUE },
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
                    "      \"0\": \"赤\"",
                    "      \"1\": \"緑\"",
                    "      \"2\": \"黄色\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.List.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["list"];
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
                spec.List.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["list"] = new List<ChoiceIndexerTests.Color>
                {
                    { ChoiceIndexerTests.Color.RED },
                    { ChoiceIndexerTests.Color.GREEN },
                    { ChoiceIndexerTests.Color.BLUE },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"list\":",
                        "    \"DictBody\":",
                        "      \"0\": \"赤\"",
                        "      \"1\": \"緑\"",
                        "      \"2\": \"青\""),
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
                    spec.List.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr)["list"] = new List<ChoiceIndexerTests.Color>
                    {
                        { ChoiceIndexerTests.Color.RED },
                        { ChoiceIndexerTests.Color.GREEN },
                        { ChoiceIndexerTests.Color.YELLOW },
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
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"RED\"",
                    "      \"1\": \"GREEN\"",
                    "      \"2\": \"BLUE\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var list = spec.List.Choice(ChoiceIndexerTests.ListChoices)["list"];

                // assert
                Assert.Equal(
                    new List<ChoiceIndexerTests.Color>
                    {
                        { ChoiceIndexerTests.Color.RED },
                        { ChoiceIndexerTests.Color.GREEN },
                        { ChoiceIndexerTests.Color.BLUE },
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
                    "      \"0\": \"RED\"",
                    "      \"1\": \"GREEN\"",
                    "      \"2\": \"YELLOW\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.List.Choice(ChoiceIndexerTests.ListChoices)["list"];
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
                spec.List.Choice(ChoiceIndexerTests.ListChoices)["list"] = new List<ChoiceIndexerTests.Color>
                {
                    { ChoiceIndexerTests.Color.RED },
                    { ChoiceIndexerTests.Color.GREEN },
                    { ChoiceIndexerTests.Color.BLUE },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"list\":",
                        "    \"DictBody\":",
                        "      \"0\": \"RED\"",
                        "      \"1\": \"GREEN\"",
                        "      \"2\": \"BLUE\""),
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
                    spec.List.Choice(ChoiceIndexerTests.ListChoices)["list"] = new List<ChoiceIndexerTests.Color>
                {
                    { ChoiceIndexerTests.Color.RED },
                    { ChoiceIndexerTests.Color.GREEN },
                    { ChoiceIndexerTests.Color.YELLOW },
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
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"ROSSO\"",
                    "      \"1\": \"VERDE\"",
                    "      \"2\": \"AZZURRO\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var list = spec.List.Choice(ChoiceIndexerTests.DictChoices)["list"];

                // assert
                Assert.Equal(
                    new List<ChoiceIndexerTests.Color>
                    {
                        { ChoiceIndexerTests.Color.RED },
                        { ChoiceIndexerTests.Color.GREEN },
                        { ChoiceIndexerTests.Color.BLUE },
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
                    "      \"0\": \"ROSSO\"",
                    "      \"1\": \"VERDE\"",
                    "      \"2\": \"GIALLO\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.List.Choice(ChoiceIndexerTests.DictChoices)["list"];
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
                spec.List.Choice(ChoiceIndexerTests.DictChoices)["list"] = new List<ChoiceIndexerTests.Color>
                {
                    { ChoiceIndexerTests.Color.RED },
                    { ChoiceIndexerTests.Color.GREEN },
                    { ChoiceIndexerTests.Color.BLUE },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"list\":",
                        "    \"DictBody\":",
                        "      \"0\": \"ROSSO\"",
                        "      \"1\": \"VERDE\"",
                        "      \"2\": \"AZZURRO\""),
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
                    spec.List.Choice(ChoiceIndexerTests.DictChoices)["list"] = new List<ChoiceIndexerTests.Color>
                    {
                        { ChoiceIndexerTests.Color.RED },
                        { ChoiceIndexerTests.Color.GREEN },
                        { ChoiceIndexerTests.Color.YELLOW },
                    };
                });
            }
        }
    }
}
