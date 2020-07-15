// -----------------------------------------------------------------------
// <copyright file="ExteriorDirKeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ExteriorDirKeyTests
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"first\"",
                $"      \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"second\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, refererAgnosticPath);
            var dict = spec.Dict.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
            var result = new Dictionary<string, string>();
            foreach (var kv in dict)
            {
                var joinedValues = string.Join("_", kv.Key.Select(spawner => spawner.Spawn()));
                result[joinedValues] = kv.Value;
            }

            // assert
            Assert.Equal(
                new Dictionary<string, string>
                {
                    { "referred first_referred second_referred third", "first" },
                    { string.Empty, "second" },
                },
                result);
        }

        /// <summary>
        /// Test getter with invalid key.
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
        public void InvalidKeyGetterTest(
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"first\"",
                $"      \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"second\""));

            this.GenerateInvalidSpawner(proxy, referredPath1);
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, refererAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
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
            spec.Dict.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"] =
                new Dictionary<ISpawnerDir<ValidSpawnerRootWithDefaultConstructor>, string>
                {
                    { dir, "dir" },
                    { emptyDir, "emptyDir" },
                };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    $"      \"{(dirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"dir\"",
                    $"      \"{(emptyDirAgnosticPath - refererAgnosticPath.Parent).ToAgnosticPathString()}\": \"emptyDir\""),
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
