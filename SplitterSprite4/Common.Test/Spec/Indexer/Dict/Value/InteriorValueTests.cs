// -----------------------------------------------------------------------
// <copyright file="InteriorValueTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer.Dict.Value
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    public class InteriorValueTests
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
                "      \"zero\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior zero\"",
                "            \"[End Of Text]\"",
                "      \"one\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior one\"",
                "            \"[End Of Text]\"",
                "      \"two\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior two\"",
                "            \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Keyword.Interior<ISpawnerChildWithoutArgs<string>>()["dict"].ToDictionary(
                kv => kv.Key, kv => kv.Value.Spawn());

            // assert
            Assert.Equal(
                new Dictionary<string, string>
                {
                    { "zero", "interior zero" },
                    { "one", "interior one" },
                    { "two", "interior two" },
                },
                dict);
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
                "      \"zero\":",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior zero\"",
                "            \"[End Of Text]\"",
                "      \"one\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior one\"",
                "            \"[End Of Text]\"",
                "      \"two\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior two\"",
                "            \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Dict.Keyword.Interior<ISpawnerChildWithoutArgs<string>>()["dict"];
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

            Utility.SetupSpecFile(proxy, "zero.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"interior\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "    \"properties\":",
                "      \"return value\": |+",
                "        \"interior zero\"",
                "        \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, "one.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"interior\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "    \"properties\":",
                "      \"return value\": |+",
                "        \"interior one\"",
                "        \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, "two.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"interior\":",
                $"    \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "    \"properties\":",
                "      \"return value\": |+",
                "        \"interior two\"",
                "        \"[End Of Text]\""));

            var zeroInteriorSpawner = SpecRoot.Fetch(
                proxy, AgnosticPath.FromAgnosticPathString("zero.spec"))
                .Interior<ISpawnerChildWithoutArgs<string>>()["interior"];
            var oneInteriorSpawner = SpecRoot.Fetch(
                proxy, AgnosticPath.FromAgnosticPathString("one.spec"))
                .Interior<ISpawnerChildWithoutArgs<string>>()["interior"];
            var twoInteriorSpawner = SpecRoot.Fetch(
                proxy, AgnosticPath.FromAgnosticPathString("two.spec"))
                .Interior<ISpawnerChildWithoutArgs<string>>()["interior"];

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.Dict.Keyword.Interior<ISpawnerChildWithoutArgs<string>>()["dict"] =
                new Dictionary<string, ISpawnerChildWithoutArgs<string>>
                {
                    { "zero", zeroInteriorSpawner },
                    { "one", oneInteriorSpawner },
                    { "two", twoInteriorSpawner },
                };

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"other value\": \"dummy\"",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"one\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"interior one\"",
                    "            \"[End Of Text]\"",
                    "      \"two\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"interior two\"",
                    "            \"[End Of Text]\"",
                    "      \"zero\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"interior zero\"",
                    "            \"[End Of Text]\""),
                spec.ToString());
        }

        /// <summary>
        /// Test setter of value SpecChild.
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
                "  \"dict\":",
                "    \"DictBody\":",
                "      \"zero\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior zero\"",
                "            \"[End Of Text]\"",
                "      \"one\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior one\"",
                "            \"[End Of Text]\"",
                "      \"two\":",
                $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                "        \"properties\":",
                "          \"return value\": |+",
                "            \"interior two\"",
                "            \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            var dict = spec.Dict.Keyword.Interior<ISpawnerChildWithoutArgs<string>>()["dict"];
            dict["zero"].Spec.Text["return value"] = "new zero";
            dict["one"].Spec.Text["other value"] = "new one";

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"dict\":",
                    "    \"DictBody\":",
                    "      \"zero\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"new zero\"",
                    "            \"[End Of Text]\"",
                    "      \"one\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"interior one\"",
                    "            \"[End Of Text]\"",
                    "          \"other value\": |+",
                    "            \"new one\"",
                    "            \"[End Of Text]\"",
                    "      \"two\":",
                    $"        \"spawner\": \"{Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "        \"properties\":",
                    "          \"return value\": |+",
                    "            \"interior two\"",
                    "            \"[End Of Text]\""),
                spec.ToString());
        }
    }
}
