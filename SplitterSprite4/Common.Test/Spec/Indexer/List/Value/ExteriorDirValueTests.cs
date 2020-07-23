// -----------------------------------------------------------------------
// <copyright file="ExteriorDirValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ExteriorDirValueTests
    {
        /// <summary>
        /// Test getter.
        /// </summary>
        /// <param name="refererPath">
        /// The os-agnostic path of the referer spec file.
        /// </param>
        /// <param name="dirPath">
        /// The os-agnostic dir path which contains referred spec files.
        /// </param>
        /// <param name="referredPath1">
        /// The os-agnostic path of the first referred spec file.
        /// </param>
        /// <param name="referredPath2">
        /// The os-agnostic path of the second referred spec file.
        /// </param>
        /// <param name="referredPath3">
        /// The os-agnostic path of the third referred spec file.
        /// </param>
        /// <param name="emptyDirPath">
        /// The os-agnostic dir path which contains no spec files.
        /// </param>
        [Theory]
        [InlineData(
            "referer.spec",
            "dir",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec",
            "no_dir")]
        [InlineData(
            "dir1/referer.spec",
            "dir2",
            "dir2/referred1.spec",
            "dir2/referred2.spec",
            "dir2/referred3.spec",
            "no_dir")]
        public void GetterTest(
            string refererPath,
            string dirPath,
            string referredPath1,
            string referredPath2,
            string referredPath3,
            string emptyDirPath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var refererAgnosticPath = AgnosticPath.FromAgnosticPathString(refererPath);
            var dirAgnosticPath = AgnosticPath.FromAgnosticPathString(dirPath);
            var emptyDirAgnosticPath = AgnosticPath.FromAgnosticPathString(emptyDirPath);

            Utility.SetupSpecFile(proxy, refererPath, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"0\": \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"1\": \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, refererAgnosticPath);
            var list = spec.List.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()["list"].Select(
                spawnerDir => string.Join("_", spawnerDir.Select(spawner => spawner.Spawn())));

            // assert
            Assert.Equal(
                new List<string>
                {
                    "referred first_referred second_referred third",
                    string.Empty,
                },
                list);
        }

        /// <summary>
        /// Test getter with invalid value.
        /// </summary>
        /// <param name="refererPath">
        /// The os-agnostic path of the referer spec file.
        /// </param>
        /// <param name="dirPath">
        /// The os-agnostic dir path which contains referred spec files.
        /// </param>
        /// <param name="referredPath1">
        /// The os-agnostic path of the first referred spec file.
        /// </param>
        /// <param name="referredPath2">
        /// The os-agnostic path of the second referred spec file.
        /// </param>
        /// <param name="referredPath3">
        /// The os-agnostic path of the third referred spec file.
        /// </param>
        /// <param name="emptyDirPath">
        /// The os-agnostic dir path which contains no spec files.
        /// </param>
        [Theory]
        [InlineData(
            "referer.spec",
            "dir",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec",
            "no_dir")]
        [InlineData(
            "dir1/referer.spec",
            "dir2",
            "dir2/referred1.spec",
            "dir2/referred2.spec",
            "dir2/referred3.spec",
            "no_dir")]
        public void InvalidValueGetterTest(
            string refererPath,
            string dirPath,
            string referredPath1,
            string referredPath2,
            string referredPath3,
            string emptyDirPath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var refererAgnosticPath = AgnosticPath.FromAgnosticPathString(refererPath);
            var dirAgnosticPath = AgnosticPath.FromAgnosticPathString(dirPath);
            var emptyDirAgnosticPath = AgnosticPath.FromAgnosticPathString(emptyDirPath);

            Utility.SetupSpecFile(proxy, refererPath, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"0\": \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"1\": \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\""));

            this.GenerateInvalidSpawner(proxy, referredPath1);
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, refererAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()["list"];
            });
        }

        /// <summary>
        /// Test setter.
        /// </summary>
        /// <param name="refererPath">
        /// The os-agnostic path of the referer spec file.
        /// </param>
        /// <param name="dirPath">
        /// The os-agnostic dir path which contains referred spec files.
        /// </param>
        /// <param name="referredPath1">
        /// The os-agnostic path of the first referred spec file.
        /// </param>
        /// <param name="referredPath2">
        /// The os-agnostic path of the second referred spec file.
        /// </param>
        /// <param name="referredPath3">
        /// The os-agnostic path of the third referred spec file.
        /// </param>
        /// <param name="emptyDirPath">
        /// The os-agnostic dir path which contains no spec files.
        /// </param>
        [Theory]
        [InlineData(
            "referer.spec",
            "dir",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec",
            "no_dir")]
        [InlineData(
            "dir1/referer.spec",
            "dir2",
            "dir2/referred1.spec",
            "dir2/referred2.spec",
            "dir2/referred3.spec",
            "no_dir")]
        public void SetterTest(
            string refererPath,
            string dirPath,
            string referredPath1,
            string referredPath2,
            string referredPath3,
            string emptyDirPath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var refererAgnosticPath = AgnosticPath.FromAgnosticPathString(refererPath);
            var dirAgnosticPath = AgnosticPath.FromAgnosticPathString(dirPath);
            var emptyDirAgnosticPath = AgnosticPath.FromAgnosticPathString(emptyDirPath);

            Utility.SetupSpecFile(proxy, refererPath, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            var dir = new SpawnerDir<ValidSpawnerRootWithDefaultConstructor>(proxy, dirAgnosticPath);
            var emptyDir = new SpawnerDir<ValidSpawnerRootWithDefaultConstructor>(proxy, emptyDirAgnosticPath);

            // act
            var spec = SpecRoot.Fetch(proxy, refererAgnosticPath);
            spec.List.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()["list"] =
                new List<ISpawnerDir<ValidSpawnerRootWithDefaultConstructor>>
                {
                    dir,
                    emptyDir,
                };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    $"      \"0\": \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\"",
                    $"      \"1\": \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\""),
                spec.ToString());
        }

        private void GenerateSpawner(
            OutSideProxy proxy, string path, string returnValue)
        {
            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                $"    \"{returnValue}\"",
                "    \"[End Of Text]\""));
        }

        private void GenerateInvalidSpawner(
            OutSideProxy proxy, string path)
        {
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"return value\": |+",
                $"    \"dummy\"",
                "    \"[End Of Text]\""));
        }
    }
}
