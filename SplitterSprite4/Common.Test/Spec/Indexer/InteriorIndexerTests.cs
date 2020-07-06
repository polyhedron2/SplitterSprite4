// -----------------------------------------------------------------------
// <copyright file="InteriorIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the InteriorIndexer class.
    /// </summary>
    public class InteriorIndexerTests
    {
        /// <summary>
        /// Test the SpawnerChild accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void InteriorTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\":",
                $"    \"spawner\": \"this is not a spawner type.\"",
                "  \"without valid constructor\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(SpawnerChildWithoutValidConstructor))}\"",
                "    \"properties\":",
                "      \"true\": |+",
                "        \"foo\"",
                "        \"[End Of Text]\"",
                "      \"false\": |+",
                "        \"bar\"",
                "        \"[End Of Text]\"",
                "  \"nonspawner\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(NonSpawner))}\"",
                "  \"child\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "    \"properties\":",
                "      \"return value\": |+",
                "        \"baz\"",
                "        \"[End Of Text]\"",
                "  \"type is not defined\":",
                "    \"properties\":",
                "      \"true\": |+",
                "        \"qux\"",
                "        \"[End Of Text]\"",
                "      \"false\": |+",
                "        \"quux\"",
                "        \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            // invalid type parameter
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interior<
                    SpawnerChildWithoutValidConstructor>();
            });

            // invalid type in spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<ISpawnerChild<object>>()["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ISpawnerChild<object>>()["without valid constructor"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<ISpawnerChild<object>>()["nonspawner"];
            });

            // mismatch between type parameter and type in spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ValidSpawnerChildWithImplementedConstructor>()["child"];
            });

            // undefined type
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ISpawnerChild<object>>()["type is not defined"];
            });

            // valid pattern
            Assert.Equal(
                "baz",
                spec.Interior<ISpawnerChildWithoutArgs<string>>()["child"].Spawn());

            // get value with default value.
            var defaultType = typeof(ValidSpawnerChildWithImplementedConstructor);

            // invalid type parameter
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interior<
                    SpawnerChildWithoutValidConstructor>()[
                    "invalid", defaultType];
            });

            // invalid type in spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ISpawnerChild<object>>()["invalid", defaultType];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ISpawnerChild<object>>()[
                    "without valid constructor", defaultType];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ISpawnerChild<object>>()["nonspawner", defaultType];
            });

            // mismatch between type parameter and type in spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Interior<
                    ValidSpawnerChildWithImplementedConstructor>()[
                    "child", defaultType];
            });

            // mismatch between type parameter and default type
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.Interior<
                    ValidSpawnerChildWithDefaultConstructor>()[
                    "child", defaultType];
            });

            // undefined type
            Assert.Equal(
                "qux",
                spec.Interior<ISpawnerChildWithArgs<string, bool>>()[
                    "type is not defined", defaultType].Spawn(true));
            Assert.Equal(
                "quux",
                spec.Interior<ISpawnerChildWithArgs<string, bool>>()[
                    "type is not defined", defaultType].Spawn(false));

            // valid pattern
            Assert.Equal(
                "baz",
                (spec.Interior<ISpawnerChild<string>>()[
                    "child", defaultType] as ISpawnerChildWithoutArgs<string>).Spawn());

            // act
            spec.Interior<ISpawnerChild<object>>()["child"] =
                spec.Interior<ISpawnerChild<object>>()[
                    "type is not defined", defaultType];
            spec.Interior<ISpawnerChild<object>>().Hold("type is not defined");

            // assert
            Assert.Equal(
                "qux",
                spec.Interior<
                    ISpawnerChildWithArgs<string, bool>>()["child"].Spawn(true));
            Assert.Equal(
                "quux",
                spec.Interior<
                    ISpawnerChildWithArgs<string, bool>>()["child"].Spawn(false));
            Assert.Equal(
                "qux",
                spec.Interior<ISpawnerChildWithArgs<string, bool>>()[
                    "type is not defined", defaultType].Spawn(true));
            Assert.Equal(
                "quux",
                spec.Interior<ISpawnerChildWithArgs<string, bool>>()[
                    "type is not defined", defaultType].Spawn(false));
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\":",
                    $"    \"spawner\": \"this is not a spawner type.\"",
                    "  \"without valid constructor\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(SpawnerChildWithoutValidConstructor))}\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"foo\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"bar\"",
                    "        \"[End Of Text]\"",
                    "  \"nonspawner\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(NonSpawner))}\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithImplementedConstructor))}\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"qux\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"quux\"",
                    "        \"[End Of Text]\"",
                    "  \"type is not defined\":",
                    $"    \"spawner\": \"__HELD__\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"qux\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"quux\"",
                    "        \"[End Of Text]\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\":",
                    $"    \"spawner\": \"this is not a spawner type.\"",
                    "  \"without valid constructor\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(SpawnerChildWithoutValidConstructor))}\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"foo\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"bar\"",
                    "        \"[End Of Text]\"",
                    "  \"nonspawner\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(NonSpawner))}\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithImplementedConstructor))}\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"qux\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"quux\"",
                    "        \"[End Of Text]\"",
                    "  \"type is not defined\":",
                    $"    \"spawner\": \"__HELD__\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"qux\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"quux\"",
                    "        \"[End Of Text]\""),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// InternalIndexer上のRemove機能をテスト。
        /// Test Remove method on InternalIndexer.
        /// </summary>
        /// <param name="derivedSpecLayerName">
        /// A derived spec's layer name.
        /// </param>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecLayerName">
        /// A base spec's layer name.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        /// <param name="relativePathFromDerivedToBaseStr">
        /// The relative path string from the derived spec to the base spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "base.spec",
            "base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/base.spec",
            "dir/base.spec")]
        [InlineData(
            "layer",
            "dir/derived.spec",
            "layer",
            "base.spec",
            "../base.spec")]
        [InlineData(
            "layer1",
            "derived.spec",
            "layer2",
            "dir/base.spec",
            "dir/base.spec")]
        public void InternalRemoveTest(
            string derivedSpecLayerName,
            string derivedSpecPathStr,
            string baseSpecLayerName,
            string baseSpecPathStr,
            string relativePathFromDerivedToBaseStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            Utility.SetupSpecFile(
                proxy,
                derivedSpecLayerName,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"11\"",
                    "        \"[End Of Text]\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"12\"",
                    "        \"[End Of Text]\"",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"13\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"22\"",
                    "        \"[End Of Text]\"",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"23\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"24\"",
                    "        \"[End Of Text]\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(
                "11",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["first"].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["third"].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["fourth"].Spawn());

            // act
            derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>().Remove("first");
            derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>().Remove("second");
            baseSpec.Interior<ISpawnerChildWithoutArgs<string>>().Remove("third");
            baseSpec.Interior<ISpawnerChildWithoutArgs<string>>().Remove("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["first"].Spawn();
            });
            Assert.Equal(
                "22",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["third"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["fourth"].Spawn();
            });

            var defaultType = typeof(ValidSpawnerChildWithDefaultConstructor);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "first", defaultType].Spawn();
            });
            Assert.Equal(
                "22",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "second", defaultType].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "third", defaultType].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "fourth", defaultType].Spawn();
            });

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"13\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"22\"",
                    "        \"[End Of Text]\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// InternalIndexer上のHide機能をテスト。
        /// Test Hide method on InternalIndexer.
        /// </summary>
        /// <param name="derivedSpecLayerName">
        /// A derived spec's layer name.
        /// </param>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecLayerName">
        /// A base spec's layer name.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        /// <param name="relativePathFromDerivedToBaseStr">
        /// The relative path string from the derived spec to the base spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "base.spec",
            "base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/base.spec",
            "dir/base.spec")]
        [InlineData(
            "layer",
            "dir/derived.spec",
            "layer",
            "base.spec",
            "../base.spec")]
        [InlineData(
            "layer1",
            "derived.spec",
            "layer2",
            "dir/base.spec",
            "dir/base.spec")]
        public void InternalHideTest(
            string derivedSpecLayerName,
            string derivedSpecPathStr,
            string baseSpecLayerName,
            string baseSpecPathStr,
            string relativePathFromDerivedToBaseStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            Utility.SetupSpecFile(
                proxy,
                derivedSpecLayerName,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"11\"",
                    "        \"[End Of Text]\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"12\"",
                    "        \"[End Of Text]\"",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"13\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"22\"",
                    "        \"[End Of Text]\"",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"23\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"24\"",
                    "        \"[End Of Text]\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(
                "11",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["first"].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["third"].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["fourth"].Spawn());

            // act
            derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>().Hide("first");
            derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>().Hide("second");
            baseSpec.Interior<ISpawnerChildWithoutArgs<string>>().Hide("third");
            baseSpec.Interior<ISpawnerChildWithoutArgs<string>>().Hide("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["first"].Spawn();
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["second"].Spawn();
            });
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["third"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()["fourth"].Spawn();
            });

            var defaultType = typeof(ValidSpawnerChildWithDefaultConstructor);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "first", defaultType].Spawn();
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "second", defaultType].Spawn();
            });
            Assert.Equal(
                "13",
                derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "third", defaultType].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Interior<ISpawnerChildWithoutArgs<string>>()[
                    "fourth", defaultType].Spawn();
            });

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"first\": \"__HIDDEN__\"",
                    "  \"second\": \"__HIDDEN__\"",
                    "  \"third\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"13\"",
                    "        \"[End Of Text]\"",
                    "  \"fourth\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"first\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "  \"second\":",
                    $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": |+",
                    "        \"22\"",
                    "        \"[End Of Text]\"",
                    "  \"third\": \"__HIDDEN__\"",
                    "  \"fourth\": \"__HIDDEN__\""),
                baseSpec.ToString());
        }
    }
}
