// -----------------------------------------------------------------------
// <copyright file="ExteriorKeyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Key
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class ExteriorKeyTests
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived first\"",
                $"      \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived second\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base second\"",
                $"      \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base third\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);
            var dict = spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
            var result = new Dictionary<string, string>();
            foreach (var kv in dict)
            {
                result[kv.Key.Spawn()] = kv.Value;
            }

            // assert
            Assert.Equal(
                new Dictionary<string, string>
                {
                    { "referred first", "derived first" },
                    { "referred second", "derived second" },
                    { "referred third", "base third" },
                },
                result);
        }

        /// <summary>
        /// Test getter with invalid key.
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
        public void InvalidReferredSpecOnDerivedGetterTest(
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived first\"",
                $"      \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived second\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base second\"",
                $"      \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base third\""));

            this.GenerateInvalidSpawner(proxy, referredPath1);
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
            });
        }

        /// <summary>
        /// Test getter with invalid key.
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
        public void InvalidReferredSpecOnBaseGetterTest(
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived first\"",
                $"      \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived second\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base second\"",
                $"      \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base third\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateInvalidSpawner(proxy, referredPath3);

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
            });
        }

        /// <summary>
        /// Test getter with invalid key.
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
        public void InvalidReferenceOnDerivedGetterTest(
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived first\"",
                $"      \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived second\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base second\"",
                $"      \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base third\""));

            this.GenerateSpawner(proxy, referredPath2, "referred second");
            this.GenerateSpawner(proxy, referredPath3, "referred third");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
            });
        }

        /// <summary>
        /// Test getter with invalid key.
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
        public void InvalidReferenceOnBaseGetterTest(
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
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath1 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived first\"",
                $"      \"{(referredAgnosticPath2 - derivedAgnosticPath.Parent).ToAgnosticPathString()}\": \"derived second\""));
            Utility.SetupSpecFile(proxy, basePath, Utility.JoinLines(
                "\"properties\":",
                "  \"dict\":",
                "    \"DictBody\":",
                $"      \"{(referredAgnosticPath2 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base second\"",
                $"      \"{(referredAgnosticPath3 - baseAgnosticPath.Parent).ToAgnosticPathString()}\": \"base third\""));

            this.GenerateSpawner(proxy, referredPath1, "referred first");
            this.GenerateSpawner(proxy, referredPath2, "referred second");

            // act
            var spec = SpecRoot.Fetch(proxy, derivedAgnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"];
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
            spec.Dict.Exterior<ValidSpawnerRootWithDefaultConstructor>().Keyword["dict"] =
                new Dictionary<ValidSpawnerRootWithDefaultConstructor, string>
                {
                    { referredSpawner1, "first" },
                    { referredSpawner2, "second" },
                    { referredSpawner3, "third" },
                };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    $"      \"{(referredAgnosticPath1 - agnosticPath.Parent).ToAgnosticPathString()}\": \"first\"",
                    $"      \"{(referredAgnosticPath2 - agnosticPath.Parent).ToAgnosticPathString()}\": \"second\"",
                    $"      \"{(referredAgnosticPath3 - agnosticPath.Parent).ToAgnosticPathString()}\": \"third\""),
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
