// -----------------------------------------------------------------------
// <copyright file="ChoiceKeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Key
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ChoiceKeyTests
    {
        public class ChoiceKeyWithListAndFuncTests
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
                    "      \"赤\": \"あか\"",
                    "      \"緑\": \"みどり\"",
                    "      \"青\": \"あお\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr).Keyword["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.BLUE, "あお" },
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
                    "      \"赤\": \"あか\"",
                    "      \"緑\": \"みどり\"",
                    "      \"黄色\": \"きいろ\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr).Keyword["dict"];
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
                spec.Dict.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                {
                    { ChoiceIndexerTests.Color.RED, "あか" },
                    { ChoiceIndexerTests.Color.GREEN, "みどり" },
                    { ChoiceIndexerTests.Color.BLUE, "あお" },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"赤\": \"あか\"",
                        "      \"緑\": \"みどり\"",
                        "      \"青\": \"あお\""),
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
                    spec.Dict.Choice(ChoiceIndexerTests.ListChoices, ChoiceIndexerTests.ChoiceToSpecStr).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.YELLOW, "きいろ" },
                    };
                });
            }
        }

        public class ChoiceKeyWithListTests
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
                    "      \"RED\": \"あか\"",
                    "      \"GREEN\": \"みどり\"",
                    "      \"BLUE\": \"あお\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Choice(ChoiceIndexerTests.ListChoices).Keyword["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.BLUE, "あお" },
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
                    "      \"RED\": \"あか\"",
                    "      \"GREEN\": \"みどり\"",
                    "      \"YELLOW\": \"きいろ\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Choice(ChoiceIndexerTests.ListChoices).Keyword["dict"];
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
                spec.Dict.Choice(ChoiceIndexerTests.ListChoices).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                {
                    { ChoiceIndexerTests.Color.RED, "あか" },
                    { ChoiceIndexerTests.Color.GREEN, "みどり" },
                    { ChoiceIndexerTests.Color.BLUE, "あお" },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"RED\": \"あか\"",
                        "      \"GREEN\": \"みどり\"",
                        "      \"BLUE\": \"あお\""),
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
                    spec.Dict.Choice(ChoiceIndexerTests.ListChoices).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.YELLOW, "きいろ" },
                    };
                });
            }
        }

        public class ChoiceKeyWithDictTests
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
                    "      \"ROSSO\": \"あか\"",
                    "      \"VERDE\": \"みどり\"",
                    "      \"AZZURRO\": \"あお\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var dict = spec.Dict.Choice(ChoiceIndexerTests.DictChoices).Keyword["dict"];

                // assert
                Assert.Equal(
                    new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.BLUE, "あお" },
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
                    "      \"ROSSO\": \"あか\"",
                    "      \"VERDE\": \"みどり\"",
                    "      \"GIALLO\": \"きいろ\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Dict.Choice(ChoiceIndexerTests.DictChoices).Keyword["dict"];
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
                spec.Dict.Choice(ChoiceIndexerTests.DictChoices).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                {
                    { ChoiceIndexerTests.Color.RED, "あか" },
                    { ChoiceIndexerTests.Color.GREEN, "みどり" },
                    { ChoiceIndexerTests.Color.BLUE, "あお" },
                };

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"other value\": \"dummy\"",
                        "  \"dict\":",
                        "    \"DictBody\":",
                        "      \"AZZURRO\": \"あお\"",
                        "      \"ROSSO\": \"あか\"",
                        "      \"VERDE\": \"みどり\""),
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
                    spec.Dict.Choice(ChoiceIndexerTests.DictChoices).Keyword["dict"] = new Dictionary<ChoiceIndexerTests.Color, string>
                    {
                        { ChoiceIndexerTests.Color.RED, "あか" },
                        { ChoiceIndexerTests.Color.GREEN, "みどり" },
                        { ChoiceIndexerTests.Color.YELLOW, "きいろ" },
                    };
                });
            }
        }
    }
}
