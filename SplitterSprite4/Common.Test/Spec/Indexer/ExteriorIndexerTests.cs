// -----------------------------------------------------------------------
// <copyright file="ExteriorIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the ExteriorIndexer class.
    /// </summary>
    public class ExteriorIndexerTests
    {
        /// <summary>
        /// Test the SpawnerRoot accessor.
        /// </summary>
        /// <param name="derivedPath">
        /// The os-agnostic path of the derived spec file.
        /// </param>
        /// <param name="basePath">
        /// The os-agnostic path of the base spec file.
        /// </param>
        /// <param name="referredPath">
        /// The os-agnostic path of the referred spec file.
        /// </param>
        /// <param name="baseFromDerived">
        /// Relative path from derived spec to base spec file.
        /// </param>
        /// <param name="referredFromDerived">
        /// Relative path from derived spec to referred spec file.
        /// </param>
        /// <param name="referredFromBase">
        /// Relative path from base spec to referred spec file.
        /// </param>
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "referred.spec",
            "base.spec",
            "referred.spec",
            "referred.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "referred.spec",
            "../base.spec",
            "../referred.spec",
            "referred.spec")]
        [InlineData(
            "dir/dir2/derived.spec",
            "base.spec",
            "referred.spec",
            "../../base.spec",
            "../../referred.spec",
            "referred.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "referred.spec",
            "dir/base.spec",
            "referred.spec",
            "../referred.spec")]
        [InlineData(
            "derived.spec",
            "dir/dir2/base.spec",
            "referred.spec",
            "dir/dir2/base.spec",
            "referred.spec",
            "../../referred.spec")]
        [InlineData(
            "derived.spec",
            "base.spec",
            "dir/referred.spec",
            "base.spec",
            "dir/referred.spec",
            "dir/referred.spec")]
        [InlineData(
            "derived.spec",
            "base.spec",
            "dir/dir2/referred.spec",
            "base.spec",
            "dir/dir2/referred.spec",
            "dir/dir2/referred.spec")]
        public void ExteriorTest(
            string derivedPath,
            string basePath,
            string referredPath,
            string baseFromDerived,
            string referredFromDerived,
            string referredFromBase)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();

            var agnosticDerivedPath =
                AgnosticPath.FromAgnosticPathString(derivedPath);
            Utility.SetupSpecFile(proxy, derivedPath, Utility.JoinLines(
                $"\"base\": \"{baseFromDerived}\""));

            var agnosticBasePath =
                AgnosticPath.FromAgnosticPathString(basePath);
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"invalid\": \"dummy\"",
                $"  \"valid\": \"{referredFromBase}\""));

            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            var agnosticReferredPath =
                AgnosticPath.FromAgnosticPathString(referredPath);
            Utility.SetupSpecFile(proxy, referredPath, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"spawned value\"",
                "    \"[End Of Text]\""));

            // act
            var derivedSpec = SpecRoot.Fetch(proxy, agnosticDerivedPath);
            var baseSpec = SpecRoot.Fetch(proxy, agnosticBasePath);

            // assert
            // get value without default value.
            // Invalid type parameter
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = derivedSpec.Exterior<
                    SpawnerRootWithoutValidConstructor>();
            });

            // Refer to absence spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()["invalid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()["invalid"];
            });

            // Type mismatch
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithImplementedConstructor>()["valid"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithImplementedConstructor>()["valid"];
            });

            // undefined value
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()["undefined"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()["undefined"];
            });

            // valid pattern
            Assert.Equal(
                "spawned value",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid"].Spawn());
            Assert.Equal(
                "spawned value",
                baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid"].Spawn());

            // get value with default value.
            // Invalid default path
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()[
                    "invalid", "invalid|default|path"];
            });
            Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()[
                    "invalid", "invalid|default|path"];
            });

            // Refer to absence spec
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()[
                    "invalid", "default/path"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()[
                    "invalid", "default/path"];
            });

            // Type mismatch
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<
                    ValidSpawnerRootWithImplementedConstructor>()[
                    "valid", "default/path"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithImplementedConstructor>()[
                    "valid", "default/path"];
            });

            // undefined value
            Assert.Equal(
                "spawned value",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "undefined", referredFromDerived].Spawn());
            Assert.Equal(
                "spawned value",
                baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "undefined", referredFromBase].Spawn());

            // valid pattern
            Assert.Equal(
                "spawned value",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid", "default/path"].Spawn());
            Assert.Equal(
                "spawned value",
                baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid", "default/path"].Spawn());

            // act
            var spawner =
                baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "valid"];
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                "undefined"] = spawner;

            // assert
            Assert.Equal(
                "spawned value",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "undefined"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = baseSpec.Exterior<
                    ValidSpawnerRootWithDefaultConstructor>()["undefined"];
            });

            // derived spec is editted.
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{baseFromDerived}\"",
                    "\"properties\":",
                    $"  \"undefined\": \"{referredFromDerived}\""),
                derivedSpec.ToString());

            // base spec is not editted.
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"dummy\"",
                    $"  \"valid\": \"{referredFromBase}\""),
                baseSpec.ToString());

            // act
            derivedSpec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticDerivedPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{baseFromDerived}\"",
                    "\"properties\":",
                    $"  \"undefined\": \"{referredFromDerived}\""),
                reloadedSpec.ToString());
        }
    }
}
