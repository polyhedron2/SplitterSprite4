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
            spec.Interior<ISpawnerChild<object>>().ExplicitDefault("type is not defined");

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
                    $"    \"spawner\": \"__DEFAULT__\"",
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
                    $"    \"spawner\": \"__DEFAULT__\"",
                    "    \"properties\":",
                    "      \"true\": |+",
                    "        \"qux\"",
                    "        \"[End Of Text]\"",
                    "      \"false\": |+",
                    "        \"quux\"",
                    "        \"[End Of Text]\""),
                reloadedSpec.ToString());
        }
    }
}
