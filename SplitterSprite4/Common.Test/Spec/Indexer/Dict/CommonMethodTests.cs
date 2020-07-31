// -----------------------------------------------------------------------
// <copyright file="CommonMethodTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict
{
    using System;
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test DictIndexer with LimitedKeyword key and Range value.
    /// This class tests common methods among several type definitions.
    /// </summary>
    public class CommonMethodTests
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
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.LimitedKeyword(5).Range(10)["dict"];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);

            // act
            dict = spec.Dict.LimitedKeyword(5).Range(10)["dict", 2];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);
        }

        /// <summary>
        /// Test getter with base spec.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        public void GetterWithBaseTest(
            string derivedSpecPathStr, string baseSpecPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(baseSpecPathStr);
            Utility.SetupSpecFile(proxy, derivedSpecPathStr, Utility.JoinLines(
                $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"11\"",
                "      \"key2\": \"12\"",
                "      \"key3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key2\": \"22\"",
                "      \"key3\": \"23\"",
                "      \"key4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);

            // act
            var dict = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict"];
            var dictWithDefault = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict", 99];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key1", 11 },
                    { "key2", 12 },
                    { "key3", 13 },
                    { "key4", 24 },
                },
                dict);
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key1", 11 },
                    { "key2", 12 },
                    { "key3", 13 },
                    { "key4", 24 },
                },
                dictWithDefault);
        }

        /// <summary>
        /// Test getter from sub spec.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InnerGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"inner\":",
                "    \"dict\":",
                "      \"DictBody\":",
                "        \"a\": \"0\"",
                "        \"abc\": \"5\"",
                "        \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec["inner"].Dict.LimitedKeyword(5).Range(10)["dict"];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);

            // act
            dict = spec["inner"].Dict.LimitedKeyword(5).Range(10)["dict", 2];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);
        }

        /// <summary>
        /// Test getter with empty dictionary.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void EmptyGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.LimitedKeyword(5).Range(10)["dict"];

            // assert
            Assert.Equal(
                new Dictionary<string, int>(),
                dict);

            // act
            dict = spec.Dict.LimitedKeyword(5).Range(10)["dict", 2];

            // assert
            Assert.Equal(
                new Dictionary<string, int>(),
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
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcdefghij\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).Range(10)["dict"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).Range(10)["dict", 2];
            });
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
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcde\": \"100\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).Range(10)["dict"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).Range(10)["dict", 2];
            });
        }

        /// <summary>
        /// Test getter with ensured keys.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void EnsuredKeyGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.LimitedKeyword(5).EnsureKeys("a", "abc").EnsureKeys("abcde").Range(10)["dict"];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);

            // act
            dict = spec.Dict.LimitedKeyword(5).EnsureKeys("a", "abc").EnsureKeys("abcde").Range(10)["dict", 2];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                dict);
        }

        /// <summary>
        /// Test getter with base spec.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        public void EnsuredKeyGetterWithBaseTest(
            string derivedSpecPathStr, string baseSpecPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(baseSpecPathStr);
            Utility.SetupSpecFile(proxy, derivedSpecPathStr, Utility.JoinLines(
                $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"11\"",
                "      \"key2\": \"12\"",
                "      \"key3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key2\": \"22\"",
                "      \"key3\": \"23\"",
                "      \"key4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);

            // act
            var dict = derivedSpec.Dict.LimitedKeyword(5).EnsureKeys("key4").Range(100)["dict"];
            var dictWithDefault = derivedSpec.Dict.LimitedKeyword(5).EnsureKeys("key4").Range(100)["dict", 99];

            // assert
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key1", 11 },
                    { "key2", 12 },
                    { "key3", 13 },
                    { "key4", 24 },
                },
                dict);
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key1", 11 },
                    { "key2", 12 },
                    { "key3", 13 },
                    { "key4", 24 },
                },
                dictWithDefault);
        }

        /// <summary>
        /// Test getter with ensured keys, but these are not contained in the spec.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void EnsuredKeyWithoutEntityGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).EnsureKeys("A", "ABC").EnsureKeys("ABCDE").Range(10)["dict"];
            });

            // assert
            var dict = spec.Dict.LimitedKeyword(5).EnsureKeys("A", "ABC").EnsureKeys("ABCDE").Range(10)["dict", 2];
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                    { "A", 2 },
                    { "ABC", 2 },
                    { "ABCDE", 2 },
                },
                dict);
        }

        /// <summary>
        /// Test invalid ensured keys.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InvalidEnsuredKeyTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"a\": \"0\"",
                "      \"abc\": \"5\"",
                "      \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Dict.LimitedKeyword(5).EnsureKeys("ABCDEFG");
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
            spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
            {
                { "a", 0 },
                { "abc", 5 },
                { "abcde", 9 },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"a\": \"0\"",
                    "      \"abc\": \"5\"",
                    "      \"abcde\": \"9\""),
                spec.ToString());

            // act
            spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
            {
                { "abcde", 9 },
                { "a", 0 },
                { "abc", 5 },
            };

            // assert

            // Keys are sorted, then spec body doesn't depends on the dictionary's order.
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"a\": \"0\"",
                    "      \"abc\": \"5\"",
                    "      \"abcde\": \"9\""),
                spec.ToString());
        }

        /// <summary>
        /// Test setter with base spec.
        /// </summary>
        /// <param name="derivedPath">The os-agnostic path of the derived spec file.</param>
        /// <param name="basePath">The os-agnostic path of the base spec file.</param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        [InlineData("dir1/derived.spec", "dir2/base.spec")]
        public void SetterWithBaseTest(string derivedPath, string basePath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedAgnosticPath = AgnosticPath.FromAgnosticPathString(derivedPath);
            var baseAgnosticPath = AgnosticPath.FromAgnosticPathString(basePath);
            Utility.SetupSpecFile(proxy, derivedPath, Utility.JoinLines(
                $"\"base\": \"{(baseAgnosticPath - derivedAgnosticPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"other value\": \"dummy\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\"",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"aaaaa\": \"3\"",
                "      \"abc\": \"1\"",
                "      \"abcde\": \"9\""));

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);
            spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
            {
                { "a", 0 },
                { "abc", 5 },
                { "abcde", 9 },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseAgnosticPath - derivedAgnosticPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"a\": \"0\"",
                    "      \"aaaaa\": \"__HIDDEN__\"",
                    "      \"abc\": \"5\"",
                    "      \"abcde\": \"9\""),
                spec.ToString());
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 9 },
                },
                spec.Dict.LimitedKeyword(5).Range(10)["dict"]);
        }

        /// <summary>
        /// Test setter to sub spec.
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
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec["inner"].Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
            {
                { "a", 0 },
                { "abc", 5 },
                { "abcde", 9 },
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"inner\":",
                    "    \"dict\":",
                    "      \"DictBody\":",
                    "        \"a\": \"0\"",
                    "        \"abc\": \"5\"",
                    "        \"abcde\": \"9\""),
                spec.ToString());

            // act
            spec["inner"].Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
            {
                { "abcde", 9 },
                { "a", 0 },
                { "abc", 5 },
            };

            // assert

            // Keys are sorted, then spec body doesn't depends on the dictionary's order.
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"inner\":",
                    "    \"dict\":",
                    "      \"DictBody\":",
                    "        \"a\": \"0\"",
                    "        \"abc\": \"5\"",
                    "        \"abcde\": \"9\""),
                spec.ToString());
        }

        /// <summary>
        /// Test setter with empty dictionary.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void EmptySetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>();

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\""),
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
                spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcdefghij", 9 },
                };
            });
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
                spec.Dict.LimitedKeyword(5).Range(10)["dict"] = new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "abc", 5 },
                    { "abcde", 100 },
                };
            });
        }

        /// <summary>
        /// Test Remove method.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        public void RemoveTest(
            string derivedSpecPathStr, string baseSpecPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(baseSpecPathStr);
            Utility.SetupSpecFile(proxy, derivedSpecPathStr, Utility.JoinLines(
                $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"11\"",
                "      \"key2\": \"12\"",
                "      \"key3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key2\": \"22\"",
                "      \"key3\": \"23\"",
                "      \"key4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Remove("dict", "key1");
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Remove("dict", "key2");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Remove("dict", "key3");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Remove("dict", "key4");
            var dict = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict"];
            var dictWithDefault = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key2\": \"22\""),
                baseSpec.ToString());
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key2", 22 },
                    { "key3", 13 },
                },
                dict);
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key2", 22 },
                    { "key3", 13 },
                },
                dictWithDefault);
        }

        /// <summary>
        /// Test Hide method.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        public void HideTest(
            string derivedSpecPathStr, string baseSpecPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(baseSpecPathStr);
            Utility.SetupSpecFile(proxy, derivedSpecPathStr, Utility.JoinLines(
                $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"11\"",
                "      \"key2\": \"12\"",
                "      \"key3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key2\": \"22\"",
                "      \"key3\": \"23\"",
                "      \"key4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Hide("dict", "key1");
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Hide("dict", "key2");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Hide("dict", "key3");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Hide("dict", "key4");
            var dict = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict"];
            var dictWithDefault = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key1\": \"__HIDDEN__\"",
                    "      \"key2\": \"__HIDDEN__\"",
                    "      \"key3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key2\": \"22\"",
                    "      \"key3\": \"__HIDDEN__\"",
                    "      \"key4\": \"__HIDDEN__\""),
                baseSpec.ToString());
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key3", 13 },
                },
                dict);
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key3", 13 },
                },
                dictWithDefault);
        }

        /// <summary>
        /// Test Hold method.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        [Theory]
        [InlineData("derived.spec", "base.spec")]
        [InlineData("dir/derived.spec", "base.spec")]
        [InlineData("derived.spec", "dir/base.spec")]
        public void HoldTest(
            string derivedSpecPathStr, string baseSpecPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(baseSpecPathStr);
            Utility.SetupSpecFile(proxy, derivedSpecPathStr, Utility.JoinLines(
                $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"11\"",
                "      \"key2\": \"12\"",
                "      \"key3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key2\": \"22\"",
                "      \"key3\": \"23\"",
                "      \"key4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Hold("dict", "key1");
            derivedSpec.Dict.LimitedKeyword(5).Range(100).Hold("dict", "key2");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Hold("dict", "key3");
            baseSpec.Dict.LimitedKeyword(5).Range(100).Hold("dict", "key4");
            var dictWithDefault = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key1\": \"__HELD__\"",
                    "      \"key2\": \"__HELD__\"",
                    "      \"key3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"key2\": \"22\"",
                    "      \"key3\": \"__HELD__\"",
                    "      \"key4\": \"__HELD__\""),
                baseSpec.ToString());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Dict.LimitedKeyword(5).Range(100)["dict"];
            });
            Assert.Equal(
                new Dictionary<string, int>
                {
                    { "key1", 99 },
                    { "key2", 22 },
                    { "key3", 13 },
                    { "key4", 99 },
                },
                dictWithDefault);
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// Test the MoldSpec method.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Dict.LimitedKeyword(5).Range(10)["dict"];
                _ = sp.Dict.LimitedKeyword(5).Range(10)["dict default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "  \"dict default\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、SpecファイルはDictのキーをいくつか含んでいる。
        /// Test the MoldSpec method.
        /// The spec contains several keys for the dictionary.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithKeysTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"100\"",
                "      \"key2\": \"__HIDDEN__\"",
                "      \"key3\": \"__HELD__\"",
                "  \"dict default\":",
                "    \"DictBody\":",
                "      \"key1\": \"200\"",
                "      \"key2\": \"__HIDDEN__\"",
                "      \"key3\": \"__HELD__\"",
                $""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Dict.LimitedKeyword(5).Range(10)["dict"];
                _ = sp.Dict.LimitedKeyword(5).Range(10)["dict default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key1\": \"Range, [, 0, 10, )\"",
                    "      \"key2\": \"Range, [, 0, 10, )\"",
                    "      \"key3\": \"Range, [, 0, 10, )\"",
                    "  \"dict default\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key1\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key2\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key3\": \"Range, [, 0, 10, ), 2\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、EnsuredKeysが適用されている。
        /// Test the MoldSpec method.
        /// Several keys are ensured.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithEnsuredKeysTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"another key\": \"another value\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Dict.LimitedKeyword(5).EnsureKeys("key3", "key4").EnsureKeys("key5").Range(10)["dict"];
                _ = sp.Dict.LimitedKeyword(5).EnsureKeys("key3", "key4").EnsureKeys("key5").Range(10)["dict default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key3\": \"Range, [, 0, 10, )\"",
                    "      \"key4\": \"Range, [, 0, 10, )\"",
                    "      \"key5\": \"Range, [, 0, 10, )\"",
                    "  \"dict default\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key3\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key4\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key5\": \"Range, [, 0, 10, ), 2\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、EnsuredKeysが適用されており、通常のキーも含む。
        /// Test the MoldSpec method.
        /// Several keys are ensured and several keys are contained.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithEnsuredKeysAndNormalKeysTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"key1\": \"100\"",
                "      \"key2\": \"__HIDDEN__\"",
                "      \"key3\": \"__HELD__\"",
                "  \"dict default\":",
                "    \"DictBody\":",
                "      \"key1\": \"200\"",
                "      \"key2\": \"__HIDDEN__\"",
                "      \"key3\": \"__HELD__\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Dict.LimitedKeyword(5).EnsureKeys("key3", "key4").EnsureKeys("key5").Range(10)["dict"];
                _ = sp.Dict.LimitedKeyword(5).EnsureKeys("key3", "key4").EnsureKeys("key5").Range(10)["dict default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key1\": \"Range, [, 0, 10, )\"",
                    "      \"key2\": \"Range, [, 0, 10, )\"",
                    "      \"key3\": \"Range, [, 0, 10, )\"",
                    "      \"key4\": \"Range, [, 0, 10, )\"",
                    "      \"key5\": \"Range, [, 0, 10, )\"",
                    "  \"dict default\":",
                    "    \"MoldingType\": \"Dict, LimitedKeyword\\\\c 5\"",
                    "    \"DictBody\":",
                    "      \"key1\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key2\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key3\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key4\": \"Range, [, 0, 10, ), 2\"",
                    "      \"key5\": \"Range, [, 0, 10, ), 2\""),
                mold.ToString(true));
        }
    }
}
