// -----------------------------------------------------------------------
// <copyright file="ExteriorValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.List.Value
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ExteriorValueTests
    {
        /// <summary>
        /// Test getter.
        /// </summary>
        /// <param name="derivedPath">
        /// The os-agnostic path of the derived spec file.
        /// </param>
        /// <param name="basePath">
        /// The os-agnostic path of the base spec file.
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
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "derived.spec",
            "base.spec",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec")]
        [InlineData(
            "dir1/derived.spec",
            "dir2/base.spec",
            "dir3/referred1.spec",
            "dir4/referred2.spec",
            "dir5/referred3.spec")]
        public void GetterTest(
            string derivedPath,
            string basePath,
            string referredPath1,
            string referredPath2,
            string referredPath3)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedAgnosticPath = AgnosticPath.FromAgnosticPathString(derivedPath);
            var baseAgnosticPath = AgnosticPath.FromAgnosticPathString(basePath);
            var referredAgnosticPath1 = AgnosticPath.FromAgnosticPathString(referredPath1);
            var referredAgnosticPath2 = AgnosticPath.FromAgnosticPathString(referredPath2);
            var referredAgnosticPath3 = AgnosticPath.FromAgnosticPathString(referredPath3);

            Utility.SetupSpecFile(proxy, derivedPath, Utility.JoinLines(
                $"\"base\": {(baseAgnosticPath - derivedAgnosticPath.Parent).ToAgnosticPathString()}",
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"0\": \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"1\": \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"1\": \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"2\": \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);
            var list = spec.List.Exterior<ValidSpawnerRootWithDefaultConstructor>()["list"].Select(
                spawner => spawner.Spawn());

            // assert
            Assert.Equal(
                new List<string>
                {
                    "referred first",
                    "referred second",
                    "referred third",
                },
                list);
        }

        /// <summary>
        /// Test getter with invalid value.
        /// </summary>
        /// <param name="derivedPath">
        /// The os-agnostic path of the derived spec file.
        /// </param>
        /// <param name="basePath">
        /// The os-agnostic path of the base spec file.
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
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "derived.spec",
            "base.spec",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec")]
        [InlineData(
            "dir1/derived.spec",
            "dir2/base.spec",
            "dir3/referred1.spec",
            "dir4/referred2.spec",
            "dir5/referred3.spec")]
        public void InvalidGetterTest(
            string derivedPath,
            string basePath,
            string referredPath1,
            string referredPath2,
            string referredPath3)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedAgnosticPath = AgnosticPath.FromAgnosticPathString(derivedPath);
            var baseAgnosticPath = AgnosticPath.FromAgnosticPathString(basePath);
            var referredAgnosticPath1 = AgnosticPath.FromAgnosticPathString(referredPath1);
            var referredAgnosticPath2 = AgnosticPath.FromAgnosticPathString(referredPath2);
            var referredAgnosticPath3 = AgnosticPath.FromAgnosticPathString(referredPath3);

            Utility.SetupSpecFile(proxy, derivedPath, Utility.JoinLines(
                $"\"base\": {(baseAgnosticPath - derivedAgnosticPath.Parent).ToAgnosticPathString()}",
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"0\": \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"1\": \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"list\":",
                "    \"DictBody\":",
                $"      \"1\": \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\"",
                $"      \"2\": \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\""));

            this.GenerateInvalidSpawner(proxy, referredPath1);
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.List.Exterior<ValidSpawnerRootWithDefaultConstructor>()["list"];
            });
        }

        /// <summary>
        /// Test setter.
        /// NOTE: referredPath1 ~ 3 must be orderd by lexicographical order.
        /// </summary>
        /// <param name="refererPath">
        /// The os-agnostic path of the referer spec file.
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
        [Theory]
        [InlineData(
            "referer.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "dir/referer.spec",
            "referred1.spec",
            "referred2.spec",
            "referred3.spec")]
        [InlineData(
            "referer.spec",
            "dir/referred1.spec",
            "dir/referred2.spec",
            "dir/referred3.spec")]
        [InlineData(
            "dir1/referer.spec",
            "dir2/referred1.spec",
            "dir3/referred2.spec",
            "dir4/referred3.spec")]
        public void SetterTest(
            string refererPath,
            string referredPath1,
            string referredPath2,
            string referredPath3)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(refererPath);
            Utility.SetupSpecFile(proxy, refererPath, Utility.JoinLines(
                "\"properties\":",
                "  \"other value\": \"dummy\""));
            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");
            var referredSpawner1 = new ValidSpawnerRootWithDefaultConstructor();
            var referredSpawner2 = new ValidSpawnerRootWithDefaultConstructor();
            var referredSpawner3 = new ValidSpawnerRootWithDefaultConstructor();
            var referredAgnosticPath1 = AgnosticPath.FromAgnosticPathString(referredPath1);
            var referredAgnosticPath2 = AgnosticPath.FromAgnosticPathString(referredPath2);
            var referredAgnosticPath3 = AgnosticPath.FromAgnosticPathString(referredPath3);
            referredSpawner1.Spec = SpecRoot.Fetch(proxy, referredAgnosticPath1);
            referredSpawner2.Spec = SpecRoot.Fetch(proxy, referredAgnosticPath2);
            referredSpawner3.Spec = SpecRoot.Fetch(proxy, referredAgnosticPath3);

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.List.Exterior<ValidSpawnerRootWithDefaultConstructor>()["list"] =
                new List<ValidSpawnerRootWithDefaultConstructor>
                {
                    referredSpawner1,
                    referredSpawner2,
                    referredSpawner3,
                };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"list\":",
                    "    \"DictBody\":",
                    $"      \"0\": \"{(referredAgnosticPath1 - agnosticPath.Parent).ToAgnosticPathString()}\"",
                    $"      \"1\": \"{(referredAgnosticPath2 - agnosticPath.Parent).ToAgnosticPathString()}\"",
                    $"      \"2\": \"{(referredAgnosticPath3 - agnosticPath.Parent).ToAgnosticPathString()}\""),
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
