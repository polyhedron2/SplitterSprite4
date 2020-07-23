// -----------------------------------------------------------------------
// <copyright file="CommonMethodTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List
{
    using System;
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test ListIndexer with Range value.
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\": \"0\"",
                "      \"0.1\": \"2\"",
                "      \"0.01\": \"1\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec.List.Range(10)["list"];

            // assert
            // 0.01: 1 should come before 0.1: 2, because of decimal key order.
            Assert.Equal(
                new List<int>
                {
                    0,
                    1,
                    2,
                },
                list);

            // act
            list = spec.List.Range(10)["list", 5];

            // assert
            // 0.01: 1 should come before 0.1: 2, because of decimal key order.
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\": \"0\"",
                "      \"0.1\": \"1\"",
                "      \"0.2\": \"2\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0.1\": \"6\"",
                "      \"0.2\": \"7\"",
                "      \"0.3\": \"8\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);

            // act
            var list = derivedSpec.List.Range(10)["list"];
            var listWithDefault = derivedSpec.List.Range(10)["list", 5];

            // assert
            Assert.Equal(
                new List<int>
                {
                    0,
                    1,
                    2,
                    8,
                },
                list);
            Assert.Equal(
                new List<int>
                {
                    0,
                    1,
                    2,
                    8,
                },
                listWithDefault);
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
                "    \"list\":",
                "      \"DictBody\":",
                "        \"0\": \"0\"",
                "        \"0.1\": \"2\"",
                "        \"0.01\": \"1\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var list = spec["inner"].List.Range(10)["list"];

            // assert
            // 0.01: 1 should come before 0.1: 2, because of decimal key order.
            Assert.Equal(
                new List<int>
                {
                    0,
                    1,
                    2,
                },
                list);

            // act
            list = spec["inner"].List.Range(10)["list", 5];

            // assert
            // 0.01: 1 should come before 0.1: 2, because of decimal key order.
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
        /// Test getter with empty list.
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
            var list = spec.List.Range(10)["list"];

            // assert
            Assert.Equal(new List<int>(), list);

            // act
            list = spec.List.Range(10)["list", 5];

            // assert
            Assert.Equal(new List<int>(), list);
        }

        /// <summary>
        /// Test getter with invalid value.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InvalidGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0\": \"0\"",
                "      \"0.1\": \"2\"",
                "      \"0.01\": \"100\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.Range(10)["list"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.Range(10)["list", 5];
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
            spec.List.Range(10)["list"] = new List<int>
            {
                1,
                3,
                5,
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"1\"",
                    "      \"1\": \"3\"",
                    "      \"2\": \"5\""),
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"0.5\": \"9\"",
                "      \"1\": \"1\"",
                "      \"2\": \"5\""));

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);
            spec.List.Range(10)["list"] = new List<int>
            {
                1,
                3,
                5,
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseAgnosticPath - derivedAgnosticPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"0\": \"1\"",
                    "      \"0.5\": \"__HIDDEN__\"",
                    "      \"1\": \"3\"",
                    "      \"2\": \"5\""),
                spec.ToString());
            Assert.Equal(
                new List<int>
                {
                    1,
                    3,
                    5,
                },
                spec.List.Range(10)["list"]);
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
            spec["inner"].List.Range(10)["list"] = new List<int>
            {
                1,
                3,
                5,
            };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"inner\":",
                    "    \"list\":",
                    "      \"DictBody\":",
                    "        \"0\": \"1\"",
                    "        \"1\": \"3\"",
                    "        \"2\": \"5\""),
                spec.ToString());
        }

        /// <summary>
        /// Test setter with empty list.
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
            spec.List.Range(10)["list"] = new List<int>();

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\""),
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
        public void InvalidSetterTest(string path)
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
                spec.List.Range(10)["list"] = new List<int>
                {
                    1,
                    5,
                    100,
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"1\": \"11\"",
                "      \"2\": \"12\"",
                "      \"3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"2\": \"22\"",
                "      \"3\": \"23\"",
                "      \"4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.List.Range(100).AsDict.Remove("list", 1);
            derivedSpec.List.Range(100).AsDict.Remove("list", 2);
            baseSpec.List.Range(100).AsDict.Remove("list", 3);
            baseSpec.List.Range(100).AsDict.Remove("list", 4);
            var list = derivedSpec.List.Range(100)["list"];
            var listWithDefault = derivedSpec.List.Range(100)["list", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"2\": \"22\""),
                baseSpec.ToString());
            Assert.Equal(
                new List<int>
                {
                    22,
                    13,
                },
                list);
            Assert.Equal(
                new List<int>
                {
                    22,
                    13,
                },
                listWithDefault);
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"1\": \"11\"",
                "      \"2\": \"12\"",
                "      \"3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"2\": \"22\"",
                "      \"3\": \"23\"",
                "      \"4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.List.Range(100).AsDict.Hide("list", 1);
            derivedSpec.List.Range(100).AsDict.Hide("list", 2);
            baseSpec.List.Range(100).AsDict.Hide("list", 3);
            baseSpec.List.Range(100).AsDict.Hide("list", 4);
            var list = derivedSpec.List.Range(100)["list"];
            var listWithDefault = derivedSpec.List.Range(100)["list", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"1\": \"__HIDDEN__\"",
                    "      \"2\": \"__HIDDEN__\"",
                    "      \"3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"2\": \"22\"",
                    "      \"3\": \"__HIDDEN__\"",
                    "      \"4\": \"__HIDDEN__\""),
                baseSpec.ToString());
            Assert.Equal(
                new List<int>
                {
                    13,
                },
                list);
            Assert.Equal(
                new List<int>
                {
                    13,
                },
                listWithDefault);
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"1\": \"11\"",
                "      \"2\": \"12\"",
                "      \"3\": \"13\""));
            Utility.SetupSpecFile(proxy, baseSpecPathStr, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                "      \"2\": \"22\"",
                "      \"3\": \"23\"",
                "      \"4\": \"24\""));
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            // act
            derivedSpec.List.Range(100).AsDict.Hold("list", 1);
            derivedSpec.List.Range(100).AsDict.Hold("list", 2);
            baseSpec.List.Range(100).AsDict.Hold("list", 3);
            baseSpec.List.Range(100).AsDict.Hold("list", 4);
            var listWithDefault = derivedSpec.List.Range(100)["list", 99];

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"1\": \"__HELD__\"",
                    "      \"2\": \"__HELD__\"",
                    "      \"3\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"list\":",
                    "    \"DictBody\":",
                    "      \"2\": \"22\"",
                    "      \"3\": \"__HELD__\"",
                    "      \"4\": \"__HELD__\""),
                baseSpec.ToString());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.List.Range(100)["list"];
            });
            Assert.Equal(
                new List<int>
                {
                    99,
                    22,
                    13,
                    99,
                },
                listWithDefault);
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
                _ = sp.List.Range(10)["list"];
                _ = sp.List.Range(10)["list default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"list\":",
                    "    \"MoldingType\": \"Dict, ListKey\"",
                    "  \"list default\":",
                    "    \"MoldingType\": \"Dict, ListKey\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、SpecファイルはListのキーをいくつか含んでいる。
        /// Test the MoldSpec method.
        /// The spec contains several keys for the list.
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
                "  \"list\":",
                "    \"DictBody\":",
                "      \"1\": \"100\"",
                "      \"2\": \"__HIDDEN__\"",
                "      \"3\": \"__HELD__\"",
                "  \"list default\":",
                "    \"DictBody\":",
                "      \"1\": \"200\"",
                "      \"2\": \"__HIDDEN__\"",
                "      \"3\": \"__HELD__\"",
                $""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.List.Range(10)["list"];
                _ = sp.List.Range(10)["list default", 2];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"list\":",
                    "    \"MoldingType\": \"Dict, ListKey\"",
                    "    \"DictBody\":",
                    "      \"1\": \"Range, [, 0, 10, )\"",
                    "      \"2\": \"Range, [, 0, 10, )\"",
                    "      \"3\": \"Range, [, 0, 10, )\"",
                    "  \"list default\":",
                    "    \"MoldingType\": \"Dict, ListKey\"",
                    "    \"DictBody\":",
                    "      \"1\": \"Range, [, 0, 10, ), 2\"",
                    "      \"2\": \"Range, [, 0, 10, ), 2\"",
                    "      \"3\": \"Range, [, 0, 10, ), 2\""),
                mold.ToString(true));
        }
    }
}
