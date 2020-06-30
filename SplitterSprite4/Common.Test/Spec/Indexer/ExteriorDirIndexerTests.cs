// -----------------------------------------------------------------------
// <copyright file="ExteriorDirIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the ExteriorDirIndexer class.
    /// </summary>
    public class ExteriorDirIndexerTests : LiteralIndexerTests
    {
        /// <summary>
        /// Test the SpawnerDir accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        public override void LiteralAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();

            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            var validDirPath = AgnosticPath.FromAgnosticPathString("validDir");
            var invalidDirPath = AgnosticPath.FromAgnosticPathString("invalidDir");

            var validDirRelativePathStr =
                (validDirPath - agnosticPath.Parent).ToAgnosticPathString();
            var invalidDirRelativePathStr =
                (invalidDirPath - agnosticPath.Parent).ToAgnosticPathString();
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                $"  \"valid\": \"{validDirRelativePathStr}\"",
                $"  \"invalid\": \"{invalidDirRelativePathStr}\""));

            proxy.FileIO.CreateDirectory(validDirPath);
            proxy.FileIO.CreateDirectory(invalidDirPath);

            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            for (int i = 0; i < 10; i++)
            {
                foreach (var dir in
                    new AgnosticPath[] { validDirPath, invalidDirPath })
                {
                    var pathInDir =
                        AgnosticPath.FromAgnosticPathString($"{i}.spec") + dir;
                    var pathInDirStr = pathInDir.ToAgnosticPathString();
                    Utility.SetupSpecFile(proxy, pathInDirStr, Utility.JoinLines(
                        $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                        "\"properties\":",
                        "  \"return value\": |+",
                        $"    \"{i} th spec.\"",
                        "    \"[End Of Text]\""));
                }
            }

            var invalidFilePathStr = (
                AgnosticPath.FromAgnosticPathString("invalid.spec") +
                invalidDirPath).ToAgnosticPathString();
            Utility.SetupSpecFile(proxy, invalidFilePathStr, Utility.JoinLines(
                $"\"spawner\": \"invalid spawner type\"",
                "\"properties\":",
                "  \"return value\": |+",
                $"    \"invalid spec.\"",
                "    \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            // Invalid type parameter
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = spec.ExteriorDir<
                    SpawnerRootWithoutValidConstructor>();
            });

            // Refer to spawner directory which contains invalid file.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()["invalid"];
            });

            // Type mismatch
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.ExteriorDir<
                    ValidSpawnerRootWithImplementedConstructor>()["valid"];
            });

            // undefined value
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()["undefined"];
            });

            // valid pattern
            {
                var validDir = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()["valid"];

                var actual = validDir.Select(sp => sp.Spawn()).ToHashSet();
                var expected =
                    Enumerable.Range(0, 10).Select(i => $"{i} th spec.")
                    .ToHashSet();
                Assert.Equal(expected, actual);
            }

            // get value with default value.
            // Refer to spawner directory which contains invalid file.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()["invalid", "default/dir"];
            });

            // Type mismatch
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.ExteriorDir<
                    ValidSpawnerRootWithImplementedConstructor>()["valid", "default/dir"];
            });

            // undefined value
            {
                var validDir = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()[
                    "undefined", validDirRelativePathStr];

                var actual = validDir.Select(sp => sp.Spawn()).ToHashSet();
                var expected =
                    Enumerable.Range(0, 10).Select(i => $"{i} th spec.")
                    .ToHashSet();
                Assert.Equal(expected, actual);
            }

            // valid pattern
            {
                var validDir = spec.ExteriorDir<
                    ValidSpawnerRootWithDefaultConstructor>()["valid", "default/dir"];

                var actual = validDir.Select(sp => sp.Spawn()).ToHashSet();
                var expected =
                    Enumerable.Range(0, 10).Select(i => $"{i} th spec.")
                    .ToHashSet();
                Assert.Equal(expected, actual);
            }

            // act
            var spawnerDir =
                spec.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid"];
            spec.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()[
                "undefined"] = spawnerDir;

            // assert
            var actualPath =
                spec.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()[
                    "undefined"].Dir.Path;
            Assert.Equal(validDirPath, actualPath);

            // spec is editted.
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"valid\": \"{validDirRelativePathStr}\"",
                    $"  \"invalid\": \"{invalidDirRelativePathStr}\"",
                    $"  \"undefined\": \"{validDirRelativePathStr}\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"valid\": \"{validDirRelativePathStr}\"",
                    $"  \"invalid\": \"{invalidDirRelativePathStr}\"",
                    $"  \"undefined\": \"{validDirRelativePathStr}\""),
                reloadedSpec.ToString());
        }
    }
}
