// -----------------------------------------------------------------------
// <copyright file="SpecTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Spec class.
    /// This class test methods which are common between indexers.
    /// </summary>
    public class SpecTests
    {
        /// <summary>
        /// Test spec creation with absence spec file.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void AbsenceSpecTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);

            // assert
            // absent spec is empty mapping.
            Assert.Equal("{}", spec.ToString());

            // Once absent spec is loaded, the spec is pooled.
            // There fore, spec can be fetched even if acceptAbsence=false,
            var pooledSpec = SpecRoot.Fetch(proxy, agnosticPath, false);

            // If spec pool is cleared, spec cannot be fetched.
            Assert.Throws<LayeredFile.LayeredFileNotFoundException>(() =>
            {
                proxy = Utility.PoolClearedProxy(proxy);
                SpecRoot.Fetch(proxy, agnosticPath, false);
            });
        }

        /// <summary>
        /// Test Spec class creation.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="expectedChildID">The expected ID of SpecChild.</param>
        /// <param name="expectedDeepChildID">The expected ID of SpecChild of SpecChild.</param>
        [Theory]
        [InlineData(
            "foo.spec",
            "foo.spec[properties][child]",
            "foo.spec[properties][first][second]")]
        [InlineData(
            "dir/bar.spec",
            "dir/bar.spec[properties][child]",
            "dir/bar.spec[properties][first][second]")]
        [InlineData(
            "dir1/dir2/baz.spec",
            "dir1/dir2/baz.spec[properties][child]",
            "dir1/dir2/baz.spec[properties][first][second]")]
        public void CreationTest(
            string path,
            string expectedChildID,
            string expectedDeepChildID)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines("key: \"value\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal("\"key\": \"value\"", spec.ToString());
            Assert.Equal(path, spec.ID);
            Assert.Equal(expectedChildID, spec["child"].ID);
            Assert.Equal(expectedDeepChildID, spec["first"]["second"].ID);
        }

        /// <summary>
        /// Test that SpecPool method returns
        /// same instance for same os-agnostic path.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecPoolTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines("key: \"value\""));

            // act
            var first = SpecRoot.Fetch(proxy, agnosticPath);
            var second = SpecRoot.Fetch(proxy, agnosticPath);
            var third = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // SpecPool returns same instance for same os-agnostic path.
            Assert.Same(first, second);
            Assert.Same(first, third);
        }

        /// <summary>
        /// Test the save method.
        /// </summary>
        [Fact]
        public void SaveTest()
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString("foo.spec");
            Utility.SetupSpecFile(proxy, "foo.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"key\": \"value\""));
            var newLayer = new Layer(proxy.FileIO, "new_layer", true);
            newLayer.Dependencies = new string[] { "layer" };
            newLayer.Save();
            var newPath = AgnosticPath.FromAgnosticPathString("dir1/dir2/bar.spec");

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);
            spec.Int["int0"] = 0;
            spec.Save(newLayer, newPath);

            // assert
            Assert.Equal(
                "new_layer/dir1/dir2/bar.spec",
                new LayeredFile(proxy.FileIO, newPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"key\": \"value\"",
                    "  \"int0\": \"0\""),
                SpecRoot.Fetch(proxy, newPath).ToString());

            // act
            spec.Int["int1"] = 1;
            spec.Save(newLayer);

            // assert
            Assert.Equal(
                "new_layer/foo.spec",
                new LayeredFile(proxy.FileIO, agnosticPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"key\": \"value\"",
                    "  \"int0\": \"0\"",
                    "  \"int1\": \"1\""),
                SpecRoot.Fetch(proxy, agnosticPath).ToString());

            // act
            spec.Int["int2"] = 2;
            spec.Save();

            // assert

            // ただのSave()はトップ属性を持つ"save"レイヤーに保存する
            // The simple Save() will save the spec into "save" layer which is top.
            Assert.Equal(
                "save/foo.spec",
                new LayeredFile(proxy.FileIO, agnosticPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"key\": \"value\"",
                    "  \"int0\": \"0\"",
                    "  \"int1\": \"1\"",
                    "  \"int2\": \"2\""),
                SpecRoot.Fetch(proxy, agnosticPath).ToString());
        }

        /// <summary>
        /// Test SpecRoot SpawnerType setter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecRootSpawnerTypeSetterTest(string path)
        {
            // arrange
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
                spec.SpawnerType = typeof(ValidSpawnerRootWithDefaultConstructor);

                // assert
                var type = typeof(ValidSpawnerRootWithDefaultConstructor);
                Assert.Equal(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                    spec.ToString());
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
                spec.SpawnerType = typeof(ValidSpawnerRootWithImplementedConstructor);

                // assert
                var type = typeof(ValidSpawnerRootWithImplementedConstructor);
                Assert.Equal(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                    spec.ToString());
            }

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                var proxy = Utility.TestOutSideProxy();
                var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
                spec.SpawnerType = typeof(SpawnerRootWithoutValidConstructor);
            });

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                var proxy = Utility.TestOutSideProxy();
                var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
                spec.SpawnerType = typeof(NonSpawner);
            });
        }

        /// <summary>
        /// Test SpecRoot SpawnerType getter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecRootSpawnerTypeGetterTest(string path)
        {
            // arrange
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(ValidSpawnerRootWithDefaultConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\""));
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Equal(
                    typeof(ValidSpawnerRootWithDefaultConstructor),
                    spec.SpawnerType);
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(ValidSpawnerRootWithImplementedConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\""));
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Equal(
                    typeof(ValidSpawnerRootWithImplementedConstructor),
                    spec.SpawnerType);
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(SpawnerRootWithoutValidConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\""));
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.SpawnerType;
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(NonSpawner);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(type)}\""));
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.SpawnerType;
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"spawner\": \"MagicKitchen.SplitterSprite4.Common.Test.SpecTests+NonExistenceClass, MagicKitchen.SplitterSprite4.Common.Test\""));
                var spec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.SpawnerType;
                });
            }
        }

        /// <summary>
        /// Test SpecChild SpawnerType setter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecChildSpawnerTypeSetterTest(string path)
        {
            // arrange
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var specParent = SpecRoot.Fetch(proxy, agnosticPath, true);
                var type = typeof(ValidSpawnerChildWithDefaultConstructor);
                var specChild = specParent.Child["child", typeof(ISpawnerChild<object>)];
                specChild.SpawnerType = type;

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"child\":",
                        $"    \"spawner\": \"{Spec.EncodeType(type)}\""),
                    specParent.ToString());
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var specParent = SpecRoot.Fetch(proxy, agnosticPath, true);
                var type = typeof(ValidSpawnerChildWithImplementedConstructor);
                var specChild = specParent.Child["child", typeof(ISpawnerChild<object>)];
                specChild.SpawnerType = type;

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"child\":",
                        $"    \"spawner\": \"{Spec.EncodeType(type)}\""),
                    specParent.ToString());
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var specParent = SpecRoot.Fetch(proxy, agnosticPath, true);
                var specChild = specParent.Child["child", typeof(ValidSpawnerChildWithDefaultConstructor)];

                // assert
                // SpawnerType should be sub class of the bound type.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    specChild.SpawnerType = typeof(ValidSpawnerChildWithImplementedConstructor);
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var specParent = SpecRoot.Fetch(proxy, agnosticPath, true);
                var specChild = specParent.Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    specChild.SpawnerType = typeof(SpawnerChildWithoutValidConstructor);
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var specParent = SpecRoot.Fetch(proxy, agnosticPath, true);
                var specChild = specParent.Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    specChild.SpawnerType = typeof(NonSpawner);
                });
            }
        }

        /// <summary>
        /// Test SpecChild SpawnerType getter.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecChildSpawnerTypeGetterTest(string path)
        {
            // arrange
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(ValidSpawnerChildWithDefaultConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(type)}\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Equal(type, specChild.SpawnerType);
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(ValidSpawnerChildWithImplementedConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(type)}\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Equal(type, specChild.SpawnerType);
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(ValidSpawnerChildWithDefaultConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(type)}\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ValidSpawnerChildWithImplementedConstructor)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = specChild.SpawnerType;
                });
            }

            {
                // assert
                var proxy = Utility.TestOutSideProxy();
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = SpecRoot.Fetch(proxy, agnosticPath, true).Child["child", typeof(NonSpawner)];
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(SpawnerChildWithoutValidConstructor);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(type)}\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = specChild.SpawnerType;
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(NonSpawner);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(type)}\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = specChild.SpawnerType;
                });
            }

            {
                // act
                var proxy = Utility.TestOutSideProxy();
                var type = typeof(NonSpawner);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"child\":",
                    "    \"spawner\": \"MagicKitchen.SplitterSprite4.Common.Test.SpecTests+NonExistenceClass, MagicKitchen.SplitterSprite4.Common.Test\""));
                var specChild = SpecRoot.Fetch(proxy, agnosticPath).Child["child", typeof(ISpawnerChild<object>)];

                // assert
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = specChild.SpawnerType;
                });
            }
        }

        /// <summary>
        /// Test sync of spec's value among several spawners.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void SpecSyncTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var rootType = typeof(ValidSpawnerRootWithDefaultConstructor);
            var childType = typeof(ValidSpawnerChildWithDefaultConstructor);

            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(rootType)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"original root value\"",
                "    \"[End Of Text]\"",
                "  \"child\":",
                $"    \"spawner\": \"{Spec.EncodeType(childType)}\"",
                "    \"properties\":",
                "      \"return value\": |+",
                "        \"original child value\"",
                "        \"[End Of Text]\""));

            var startPath = AgnosticPath.FromAgnosticPathString("start.spec");
            Utility.SetupSpecFile(proxy, "start.spec", Utility.JoinLines(
                "\"properties\":",
                $"  \"exterior\": {path}"));

            var startSpec = SpecRoot.Fetch(proxy, startPath);

            var rootSpawner1 = startSpec.Exterior<
                ValidSpawnerRootWithDefaultConstructor>()["exterior"];
            var childSpawner1 = rootSpawner1.Spec.Interior<
                ValidSpawnerChildWithDefaultConstructor>()["child"];
            var rootSpawner2 = startSpec.Exterior<
                ValidSpawnerRootWithDefaultConstructor>()["exterior"];
            var childSpawner2 = rootSpawner2.Spec.Interior<
                ValidSpawnerChildWithDefaultConstructor>()["child"];

            // act
            rootSpawner1.Spec.Text["return value"] = "edited root value";
            childSpawner1.Spec.Text["return value"] = "edited child value";

            // assert
            Assert.Equal(
                "edited root value",
                rootSpawner2.Spec.Text["return value"]);
            Assert.Equal(
                "edited child value",
                childSpawner2.Spec.Text["return value"]);
        }

        /// <summary>
        /// Test UpdateBase method.
        /// </summary>
        /// <param name="derivedSpecPathStr">Derived spec path.</param>
        /// <param name="baseSpecPathStr">Base spec path to updating.</param>
        /// <param name="expectedBasePathInSpec">Expected value in derived spec.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec", "bar.spec")]
        [InlineData("dir/foo.spec", "dir/bar.spec", "bar.spec")]
        [InlineData("dir/dir2/foo.spec", "dir/dir2/bar.spec", "bar.spec")]
        [InlineData("foo.spec", "dir/bar.spec", "dir/bar.spec")]
        [InlineData("foo.spec", "dir/dir2/bar.spec", "dir/dir2/bar.spec")]
        [InlineData("dir/foo.spec", "bar.spec", "../bar.spec")]
        [InlineData("dir/dir2/foo.spec", "bar.spec", "../../bar.spec")]
        [InlineData("dir/foo.spec", "dir2/bar.spec", "../dir2/bar.spec")]
        [InlineData("dir/dir2/foo.spec", "dir3/dir4/bar.spec", "../../dir3/dir4/bar.spec")]
        public void UpdateBaseTest(
            string derivedSpecPathStr,
            string baseSpecPathStr,
            string expectedBasePathInSpec)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath, true);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath, true);
            baseSpec.Save();

            // act
            derivedSpec.UpdateBase(baseSpec);
            derivedSpec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedDerivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);

            // assert
            Assert.Equal(
                $"\"base\": \"{expectedBasePathInSpec}\"",
                derivedSpec.ToString());
            Assert.Equal(
                baseSpecPathStr,
                derivedSpec.Base.ID);
            Assert.Equal(
                $"\"base\": \"{expectedBasePathInSpec}\"",
                reloadedDerivedSpec.ToString());
            Assert.Equal(
                baseSpecPathStr,
                reloadedDerivedSpec.Base.ID);
        }

        /// <summary>
        /// Base Specからの設定値の継承をテスト。
        /// Test property inheritance from base specs.
        /// </summary>
        /// <param name="derivedSpecLayerName">
        /// A derived spec's layer name.
        /// </param>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="intermediateSpecLayerName">
        /// A intermediate spec's layer name.
        /// </param>
        /// <param name="intermediateSpecPathStr">
        /// A spec's os-agnostic path which is base of derived spec.
        /// </param>
        /// <param name="baseSpecLayerName">
        /// A base spec's layer name.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A spec's os-agnostic path which is base of intermediate spec.
        /// </param>
        /// <param name="relativePathFromDerivedToIntermadiateStr">
        /// The relative path string from the derived spec to the intermediate spec.
        /// </param>
        /// <param name="relativePathFromIntermediateToBaseStr">
        /// The relative path string from the intermediate spec to the base spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "intermediate.spec",
            "layer",
            "base.spec",
            "intermediate.spec",
            "base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/intermediate.spec",
            "layer",
            "dir/dir2/base.spec",
            "dir/intermediate.spec",
            "dir2/base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/dir2/intermediate.spec",
            "layer",
            "dir/base.spec",
            "dir/dir2/intermediate.spec",
            "../base.spec")]
        [InlineData(
            "layer1",
            "derived.spec",
            "layer2",
            "dir/dir2/intermediate.spec",
            "layer3",
            "dir/base.spec",
            "dir/dir2/intermediate.spec",
            "../base.spec")]
        public void BaseSpecTest(
            string derivedSpecLayerName,
            string derivedSpecPathStr,
            string intermediateSpecLayerName,
            string intermediateSpecPathStr,
            string baseSpecLayerName,
            string baseSpecPathStr,
            string relativePathFromDerivedToIntermadiateStr,
            string relativePathFromIntermediateToBaseStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            // Spec key means, which spec has the value.
            // For example,
            //   "100" means only derived spec has the value.
            //   "101" means derived spec and base spec have the value.
            //   "111" means derived and intermediate and base spec have the value.
            // Spec value means, which spec the value come from.
            // "0" means the value come from derived spec.
            // "1" means the value come from intermediate spec.
            // "2" means the value come from base spec.
            // Spawner type is in "011" pattern.
            Utility.SetupSpecFile(
                proxy,
                derivedSpecLayerName,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromDerivedToIntermadiateStr}",
                    "\"properties\":",
                    "  \"100\": \"0\"",
                    "  \"101\": \"0\"",
                    "  \"110\": \"0\"",
                    "  \"111\": \"0\"",
                    "  \"inner\":",
                    "    \"100\": \"0\"",
                    "    \"101\": \"0\"",
                    "    \"110\": \"0\"",
                    "    \"111\": \"0\""));
            var rootType = typeof(ValidSpawnerRootWithDefaultConstructor);
            var childType = typeof(ValidSpawnerChildWithDefaultConstructor);
            Utility.SetupSpecFile(
                proxy,
                intermediateSpecLayerName,
                intermediateSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromIntermediateToBaseStr}",
                    $"\"spawner\": \"{Spec.EncodeType(rootType)}\"",
                    "\"properties\":",
                    "  \"010\": \"1\"",
                    "  \"011\": \"1\"",
                    "  \"110\": \"1\"",
                    "  \"111\": \"1\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(childType)}\"",
                    "  \"inner\":",
                    "    \"010\": \"1\"",
                    "    \"011\": \"1\"",
                    "    \"110\": \"1\"",
                    "    \"111\": \"1\""));
            rootType = typeof(ValidSpawnerRootWithImplementedConstructor);
            childType = typeof(ValidSpawnerChildWithImplementedConstructor);
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(rootType)}\"",
                    "\"properties\":",
                    "  \"001\": \"2\"",
                    "  \"011\": \"2\"",
                    "  \"101\": \"2\"",
                    "  \"111\": \"2\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(childType)}\"",
                    "  \"inner\":",
                    "    \"001\": \"2\"",
                    "    \"011\": \"2\"",
                    "    \"101\": \"2\"",
                    "    \"111\": \"2\""));

            // act
            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var childSpec =
                derivedSpec.Child["child", typeof(ISpawnerChild<object>)];

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["000"];
            });
            Assert.Equal(2, derivedSpec.Int["001"]);
            Assert.Equal(1, derivedSpec.Int["010"]);
            Assert.Equal(1, derivedSpec.Int["011"]);
            Assert.Equal(0, derivedSpec.Int["100"]);
            Assert.Equal(0, derivedSpec.Int["101"]);
            Assert.Equal(0, derivedSpec.Int["110"]);
            Assert.Equal(0, derivedSpec.Int["111"]);

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec["inner"].Int["000"];
            });
            Assert.Equal(2, derivedSpec["inner"].Int["001"]);
            Assert.Equal(1, derivedSpec["inner"].Int["010"]);
            Assert.Equal(1, derivedSpec["inner"].Int["011"]);
            Assert.Equal(0, derivedSpec["inner"].Int["100"]);
            Assert.Equal(0, derivedSpec["inner"].Int["101"]);
            Assert.Equal(0, derivedSpec["inner"].Int["110"]);
            Assert.Equal(0, derivedSpec["inner"].Int["111"]);

            Assert.Equal(-1, derivedSpec.Int["000", -1]);
            Assert.Equal(2, derivedSpec.Int["001", -1]);
            Assert.Equal(1, derivedSpec.Int["010", -1]);
            Assert.Equal(1, derivedSpec.Int["011", -1]);
            Assert.Equal(0, derivedSpec.Int["100", -1]);
            Assert.Equal(0, derivedSpec.Int["101", -1]);
            Assert.Equal(0, derivedSpec.Int["110", -1]);
            Assert.Equal(0, derivedSpec.Int["111", -1]);

            Assert.Equal(-1, derivedSpec["inner"].Int["000", -1]);
            Assert.Equal(2, derivedSpec["inner"].Int["001", -1]);
            Assert.Equal(1, derivedSpec["inner"].Int["010", -1]);
            Assert.Equal(1, derivedSpec["inner"].Int["011", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["100", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["101", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["110", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["111", -1]);

            Assert.Equal(
                typeof(ValidSpawnerRootWithDefaultConstructor),
                derivedSpec.SpawnerType);
            Assert.Equal(
                typeof(ValidSpawnerChildWithDefaultConstructor),
                childSpec.SpawnerType);
        }

        /// <summary>
        /// Base Specの設定値のループした継承関係をテスト。
        /// Test property inheritance of looped base specs.
        /// </summary>
        /// <param name="firstSpecLayerName">
        /// The first spec's layer name.
        /// </param>
        /// <param name="firstSpecPathStr">
        /// The first spec's os-agnostic path. This is derived from the second spec.
        /// </param>
        /// <param name="secondSpecLayerName">
        /// The second spec's layer name.
        /// </param>
        /// <param name="secondSpecPathStr">
        /// The second spec's os-agnostic path. This is derived from the third spec.
        /// </param>
        /// <param name="thirdSpecLayerName">
        /// The third spec's layer name.
        /// </param>
        /// <param name="thirdSpecPathStr">
        /// The third spec's os-agnostic path. This is derived from the first spec.
        /// </param>
        /// <param name="relativePathFromFirstToSecondStr">
        /// The relative path string from the first spec to the second spec.
        /// </param>
        /// <param name="relativePathFromSecondToThirdStr">
        /// The relative path string from the second spec to the third spec.
        /// </param>
        /// <param name="relativePathFromThirdToFirstStr">
        /// The relative path string from the third spec to the first spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "first.spec",
            "layer",
            "second.spec",
            "layer",
            "third.spec",
            "second.spec",
            "third.spec",
            "first.spec")]
        [InlineData(
            "layer",
            "first.spec",
            "layer",
            "dir/second.spec",
            "layer",
            "dir/dir2/third.spec",
            "dir/second.spec",
            "dir2/third.spec",
            "../../first.spec")]
        [InlineData(
            "layer1",
            "first.spec",
            "layer2",
            "dir/second.spec",
            "layer3",
            "dir/dir2/third.spec",
            "dir/second.spec",
            "dir2/third.spec",
            "../../first.spec")]
        public void LoopedBaseSpecTest(
            string firstSpecLayerName,
            string firstSpecPathStr,
            string secondSpecLayerName,
            string secondSpecPathStr,
            string thirdSpecLayerName,
            string thirdSpecPathStr,
            string relativePathFromFirstToSecondStr,
            string relativePathFromSecondToThirdStr,
            string relativePathFromThirdToFirstStr)
        {
            // arrange
            var firstSpecPath = AgnosticPath.FromAgnosticPathString(
                firstSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            // Spec key means, which spec has the value.
            // For example,
            //   "100" means only first spec has the value.
            //   "101" means first spec and third spec have the value.
            //   "111" means all of the three specs have the value.
            // Spec value means, which spec the value come from.
            // "0" means the value come from the first spec.
            // "1" means the value come from the second spec.
            // "2" means the value come from the third spec.
            // Spawner type is in "011" pattern.
            Utility.SetupSpecFile(
                proxy,
                firstSpecLayerName,
                firstSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromFirstToSecondStr}",
                    "\"properties\":",
                    "  \"100\": \"0\"",
                    "  \"101\": \"0\"",
                    "  \"110\": \"0\"",
                    "  \"111\": \"0\"",
                    "  \"inner\":",
                    "    \"100\": \"0\"",
                    "    \"101\": \"0\"",
                    "    \"110\": \"0\"",
                    "    \"111\": \"0\""));
            var rootType = typeof(ValidSpawnerRootWithDefaultConstructor);
            var childType = typeof(ValidSpawnerChildWithDefaultConstructor);
            Utility.SetupSpecFile(
                proxy,
                secondSpecLayerName,
                secondSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromSecondToThirdStr}",
                    $"\"spawner\": \"{Spec.EncodeType(rootType)}\"",
                    "\"properties\":",
                    "  \"010\": \"1\"",
                    "  \"011\": \"1\"",
                    "  \"110\": \"1\"",
                    "  \"111\": \"1\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(childType)}\"",
                    "  \"inner\":",
                    "    \"010\": \"1\"",
                    "    \"011\": \"1\"",
                    "    \"110\": \"1\"",
                    "    \"111\": \"1\""));
            rootType = typeof(ValidSpawnerChildWithImplementedConstructor);
            childType = typeof(ValidSpawnerChildWithImplementedConstructor);
            Utility.SetupSpecFile(
                proxy,
                thirdSpecLayerName,
                thirdSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromThirdToFirstStr}",
                    $"\"spawner\": \"{Spec.EncodeType(rootType)}\"",
                    "\"properties\":",
                    "  \"001\": \"2\"",
                    "  \"011\": \"2\"",
                    "  \"101\": \"2\"",
                    "  \"111\": \"2\"",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(childType)}\"",
                    "  \"inner\":",
                    "    \"001\": \"2\"",
                    "    \"011\": \"2\"",
                    "    \"101\": \"2\"",
                    "    \"111\": \"2\""));

            // act
            var firstSpec = SpecRoot.Fetch(proxy, firstSpecPath);
            var childSpec =
                firstSpec.Child["child", typeof(ISpawnerChild<object>)];

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = firstSpec.Int["000"];
            });
            Assert.Equal(2, firstSpec.Int["001"]);
            Assert.Equal(1, firstSpec.Int["010"]);
            Assert.Equal(1, firstSpec.Int["011"]);
            Assert.Equal(0, firstSpec.Int["100"]);
            Assert.Equal(0, firstSpec.Int["101"]);
            Assert.Equal(0, firstSpec.Int["110"]);
            Assert.Equal(0, firstSpec.Int["111"]);

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = firstSpec["inner"].Int["000"];
            });
            Assert.Equal(2, firstSpec["inner"].Int["001"]);
            Assert.Equal(1, firstSpec["inner"].Int["010"]);
            Assert.Equal(1, firstSpec["inner"].Int["011"]);
            Assert.Equal(0, firstSpec["inner"].Int["100"]);
            Assert.Equal(0, firstSpec["inner"].Int["101"]);
            Assert.Equal(0, firstSpec["inner"].Int["110"]);
            Assert.Equal(0, firstSpec["inner"].Int["111"]);

            Assert.Equal(-1, firstSpec.Int["000", -1]);
            Assert.Equal(2, firstSpec.Int["001", -1]);
            Assert.Equal(1, firstSpec.Int["010", -1]);
            Assert.Equal(1, firstSpec.Int["011", -1]);
            Assert.Equal(0, firstSpec.Int["100", -1]);
            Assert.Equal(0, firstSpec.Int["101", -1]);
            Assert.Equal(0, firstSpec.Int["110", -1]);
            Assert.Equal(0, firstSpec.Int["111", -1]);

            Assert.Equal(-1, firstSpec["inner"].Int["000", -1]);
            Assert.Equal(2, firstSpec["inner"].Int["001", -1]);
            Assert.Equal(1, firstSpec["inner"].Int["010", -1]);
            Assert.Equal(1, firstSpec["inner"].Int["011", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["100", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["101", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["110", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["111", -1]);

            Assert.Equal(
                typeof(ValidSpawnerRootWithDefaultConstructor),
                firstSpec.SpawnerType);
            Assert.Equal(
                typeof(ValidSpawnerChildWithDefaultConstructor),
                childSpec.SpawnerType);
        }

        /// <summary>
        /// Base Specの設定先ファイルが存在しないときの挙動をテスト。
        /// Test the spec's behavior when the base spec is absence.
        /// </summary>
        /// <param name="derivedSpecPathStr">The tested spec file's path.</param>
        /// <param name="relativeBasePathStr">The absence base spec file's path.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec")]
        [InlineData("foo.spec", "dir/bar.spec")]
        [InlineData("dir/foo.spec", "bar.spec")]
        [InlineData("dir1/foo.spec", "dir2/bar.spec")]
        public void AbsenceBaseSpecTest(
            string derivedSpecPathStr, string relativeBasePathStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var proxy = Utility.TestOutSideProxy();
            Utility.SetupSpecFile(
                proxy,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativeBasePathStr}",
                    "\"properties\":",
                    "  \"defined\": \"0\""));

            // act
            var spec = SpecRoot.Fetch(proxy, derivedSpecPath);

            // assert
            Assert.Equal(0, spec.Int["defined"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int["undefined"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.SpawnerType;
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Child[
                    "child",
                    typeof(ValidSpawnerChildWithDefaultConstructor)]
                    .SpawnerType;
            });
        }

        /// <summary>
        /// Base SpecのRemove機能をテスト。
        /// Test Remove method for base spec.
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
        public void RemoveBaseTest(
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
                    "  \"first\": \"11\"",
                    "  \"second\": \"12\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"23\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);

            Assert.Equal(11, derivedSpec.Int["first"]);
            Assert.Equal(12, derivedSpec.Int["second"]);
            Assert.Equal(23, derivedSpec.Int["third"]);

            // act
            derivedSpec.RemoveBase();

            // assert
            Assert.Equal(11, derivedSpec.Int["first"]);
            Assert.Equal(12, derivedSpec.Int["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["third"];
            });

            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"first\": \"11\"",
                    "  \"second\": \"12\""),
                derivedSpec.ToString());
        }

        /// <summary>
        /// SpawnerTypeのRemove機能をテスト。
        /// Test Remove method for spawner type.
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
        public void RemoveSpawnerTypeTest(
            string derivedSpecLayerName,
            string derivedSpecPathStr,
            string baseSpecLayerName,
            string baseSpecPathStr,
            string relativePathFromDerivedToBaseStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            var derivedRootType = typeof(ValidSpawnerRootWithDefaultConstructor);
            var derivedChildType = typeof(ValidSpawnerChildWithDefaultConstructor);
            Utility.SetupSpecFile(
                proxy,
                derivedSpecLayerName,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": {relativePathFromDerivedToBaseStr}",
                    $"\"spawner\": \"{Spec.EncodeType(derivedRootType)}\"",
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(derivedChildType)}\""));

            var baseRootType = typeof(ValidSpawnerRootWithImplementedConstructor);
            var baseChildType = typeof(ValidSpawnerChildWithImplementedConstructor);
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    $"\"spawner\": \"{Spec.EncodeType(baseRootType)}\"",
                    "\"properties\":",
                    "  \"child\":",
                    $"    \"spawner\": \"{Spec.EncodeType(baseChildType)}\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var childSpec =
                derivedSpec.Child["child", typeof(ISpawnerChild<object>)];

            Assert.Equal(derivedRootType, derivedSpec.SpawnerType);
            Assert.Equal(derivedChildType, childSpec.SpawnerType);

            // act
            derivedSpec.RemoveSpawnerType();
            childSpec.RemoveSpawnerType();

            // assert
            Assert.Equal(baseRootType, derivedSpec.SpawnerType);
            Assert.Equal(baseChildType, childSpec.SpawnerType);
            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\""),
                derivedSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のRemove機能をテスト。
        /// Test Remove method on LiteralIndexer.
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
        public void LiteralRemoveTest(
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
                    "  \"first\": \"11\"",
                    "  \"second\": \"12\"",
                    "  \"third\": \"13\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"23\"",
                    "  \"fourth\": \"24\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(11, derivedSpec.Int["first"]);
            Assert.Equal(12, derivedSpec.Int["second"]);
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Equal(24, derivedSpec.Int["fourth"]);
            Assert.Equal(11, derivedSpec.Int["first", 31]);
            Assert.Equal(12, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(24, derivedSpec.Int["fourth", 34]);

            // act
            derivedSpec.Int.Remove("first");
            derivedSpec.Int.Remove("second");
            baseSpec.Int.Remove("third");
            baseSpec.Int.Remove("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["first"];
            });
            Assert.Equal(22, derivedSpec.Int["second"]);
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["fourth"];
            });
            Assert.Equal(31, derivedSpec.Int["first", 31]);
            Assert.Equal(22, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(34, derivedSpec.Int["fourth", 34]);

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"third\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のHide機能をテスト。
        /// Test Hide method on LiteralIndexer.
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
        public void LiteralHideTest(
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
                    "  \"first\": \"11\"",
                    "  \"second\": \"12\"",
                    "  \"third\": \"13\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"23\"",
                    "  \"fourth\": \"24\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(11, derivedSpec.Int["first"]);
            Assert.Equal(12, derivedSpec.Int["second"]);
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Equal(24, derivedSpec.Int["fourth"]);
            Assert.Equal(11, derivedSpec.Int["first", 31]);
            Assert.Equal(12, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(24, derivedSpec.Int["fourth", 34]);

            // act
            derivedSpec.Int.Hide("first");
            derivedSpec.Int.Hide("second");
            baseSpec.Int.Hide("third");
            baseSpec.Int.Hide("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["first"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["second"];
            });
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["fourth"];
            });
            Assert.Equal(31, derivedSpec.Int["first", 31]);
            Assert.Equal(32, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(34, derivedSpec.Int["fourth", 34]);

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"first\": \"__HIDDEN__\"",
                    "  \"second\": \"__HIDDEN__\"",
                    "  \"third\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"__HIDDEN__\"",
                    "  \"fourth\": \"__HIDDEN__\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のExplicitDefault機能をテスト。
        /// Test ExplicitDefault method on LiteralIndexer.
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
        public void LiteralExplicitDefaultTest(
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
                    "  \"first\": \"11\"",
                    "  \"second\": \"12\"",
                    "  \"third\": \"13\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"23\"",
                    "  \"fourth\": \"24\""));

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(11, derivedSpec.Int["first"]);
            Assert.Equal(12, derivedSpec.Int["second"]);
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Equal(24, derivedSpec.Int["fourth"]);
            Assert.Equal(11, derivedSpec.Int["first", 31]);
            Assert.Equal(12, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(24, derivedSpec.Int["fourth", 34]);

            // act
            derivedSpec.Int.ExplicitDefault("first");
            derivedSpec.Int.ExplicitDefault("second");
            baseSpec.Int.ExplicitDefault("third");
            baseSpec.Int.ExplicitDefault("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["first"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["second"];
            });
            Assert.Equal(13, derivedSpec.Int["third"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["fourth"];
            });
            Assert.Equal(31, derivedSpec.Int["first", 31]);
            Assert.Equal(32, derivedSpec.Int["second", 32]);
            Assert.Equal(13, derivedSpec.Int["third", 33]);
            Assert.Equal(34, derivedSpec.Int["fourth", 34]);

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{relativePathFromDerivedToBaseStr}\"",
                    "\"properties\":",
                    "  \"first\": \"__DEFAULT__\"",
                    "  \"second\": \"__DEFAULT__\"",
                    "  \"third\": \"13\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"second\": \"22\"",
                    "  \"third\": \"__DEFAULT__\"",
                    "  \"fourth\": \"__DEFAULT__\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のRemove機能をテスト。
        /// Test Remove method on LiteralIndexer.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        /// <param name="referredPathStr11">
        /// The first tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr12">
        /// The second tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr13">
        /// The third tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr22">
        /// The second tested path on base spec.
        /// </param>
        /// <param name="referredPathStr23">
        /// The third tested path on base spec.
        /// </param>
        /// <param name="referredPathStr24">
        /// The fourth tested path on base spec.
        /// </param>
        /// <param name="referredPathStr31">
        /// The first tested path for default spec.
        /// </param>
        /// <param name="referredPathStr32">
        /// The second tested path for default spec.
        /// </param>
        /// <param name="referredPathStr33">
        /// The third tested path for default spec.
        /// </param>
        /// <param name="referredPathStr34">
        /// The fourth tested path for default spec.
        /// </param>
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11/referred.spec",
            "12/referred.spec",
            "13/referred.spec",
            "22/referred.spec",
            "23/referred.spec",
            "24/referred.spec",
            "31/referred.spec",
            "32/referred.spec",
            "33/referred.spec",
            "34/referred.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        public void PathRemoveTest(
            string derivedSpecPathStr,
            string baseSpecPathStr,
            string referredPathStr11,
            string referredPathStr12,
            string referredPathStr13,
            string referredPathStr22,
            string referredPathStr23,
            string referredPathStr24,
            string referredPathStr31,
            string referredPathStr32,
            string referredPathStr33,
            string referredPathStr34)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var referredSpecPath11 = AgnosticPath.FromAgnosticPathString(
                referredPathStr11);
            var referredSpecPath12 = AgnosticPath.FromAgnosticPathString(
                referredPathStr12);
            var referredSpecPath13 = AgnosticPath.FromAgnosticPathString(
                referredPathStr13);
            var referredSpecPath22 = AgnosticPath.FromAgnosticPathString(
                referredPathStr22);
            var referredSpecPath23 = AgnosticPath.FromAgnosticPathString(
                referredPathStr23);
            var referredSpecPath24 = AgnosticPath.FromAgnosticPathString(
                referredPathStr24);
            var referredSpecPath31 = AgnosticPath.FromAgnosticPathString(
                referredPathStr31);
            var referredSpecPath32 = AgnosticPath.FromAgnosticPathString(
                referredPathStr32);
            var referredSpecPath33 = AgnosticPath.FromAgnosticPathString(
                referredPathStr33);
            var referredSpecPath34 = AgnosticPath.FromAgnosticPathString(
                referredPathStr34);
            var proxy = Utility.TestOutSideProxy();

            Utility.SetupSpecFile(
                proxy,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    $"  \"first\": \"{(referredSpecPath11 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"second\": \"{(referredSpecPath12 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath23 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"fourth\": \"{(referredSpecPath24 - baseSpecPath.Parent).ToAgnosticPathString()}\""));

            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            Utility.SetupSpecFile(proxy, referredPathStr11, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"11\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr12, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"12\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr13, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"13\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr22, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"22\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr23, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"23\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr24, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"24\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr31, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"31\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr32, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"32\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr33, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"33\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr34, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"34\"",
                "    \"[End Of Text]\""));

            var defaultPathStr1 =
                (referredSpecPath31 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr2 =
                (referredSpecPath32 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr3 =
                (referredSpecPath33 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr4 =
                (referredSpecPath34 - derivedSpecPath.Parent).ToAgnosticPathString();

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first"].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth"].Spawn());

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            // act
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Remove("first");
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Remove("second");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Remove("third");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Remove("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["first"];
            });
            Assert.Equal(
                "22",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["fourth"];
            });

            Assert.Equal(
                "31",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "22",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "34",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のHide機能をテスト。
        /// Test Hide method on LiteralIndexer.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        /// <param name="referredPathStr11">
        /// The first tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr12">
        /// The second tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr13">
        /// The third tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr22">
        /// The second tested path on base spec.
        /// </param>
        /// <param name="referredPathStr23">
        /// The third tested path on base spec.
        /// </param>
        /// <param name="referredPathStr24">
        /// The fourth tested path on base spec.
        /// </param>
        /// <param name="referredPathStr31">
        /// The first tested path for default spec.
        /// </param>
        /// <param name="referredPathStr32">
        /// The second tested path for default spec.
        /// </param>
        /// <param name="referredPathStr33">
        /// The third tested path for default spec.
        /// </param>
        /// <param name="referredPathStr34">
        /// The fourth tested path for default spec.
        /// </param>
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11/referred.spec",
            "12/referred.spec",
            "13/referred.spec",
            "22/referred.spec",
            "23/referred.spec",
            "24/referred.spec",
            "31/referred.spec",
            "32/referred.spec",
            "33/referred.spec",
            "34/referred.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        public void PathHideTest(
            string derivedSpecPathStr,
            string baseSpecPathStr,
            string referredPathStr11,
            string referredPathStr12,
            string referredPathStr13,
            string referredPathStr22,
            string referredPathStr23,
            string referredPathStr24,
            string referredPathStr31,
            string referredPathStr32,
            string referredPathStr33,
            string referredPathStr34)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var referredSpecPath11 = AgnosticPath.FromAgnosticPathString(
                referredPathStr11);
            var referredSpecPath12 = AgnosticPath.FromAgnosticPathString(
                referredPathStr12);
            var referredSpecPath13 = AgnosticPath.FromAgnosticPathString(
                referredPathStr13);
            var referredSpecPath22 = AgnosticPath.FromAgnosticPathString(
                referredPathStr22);
            var referredSpecPath23 = AgnosticPath.FromAgnosticPathString(
                referredPathStr23);
            var referredSpecPath24 = AgnosticPath.FromAgnosticPathString(
                referredPathStr24);
            var referredSpecPath31 = AgnosticPath.FromAgnosticPathString(
                referredPathStr31);
            var referredSpecPath32 = AgnosticPath.FromAgnosticPathString(
                referredPathStr32);
            var referredSpecPath33 = AgnosticPath.FromAgnosticPathString(
                referredPathStr33);
            var referredSpecPath34 = AgnosticPath.FromAgnosticPathString(
                referredPathStr34);
            var proxy = Utility.TestOutSideProxy();

            Utility.SetupSpecFile(
                proxy,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    $"  \"first\": \"{(referredSpecPath11 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"second\": \"{(referredSpecPath12 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath23 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"fourth\": \"{(referredSpecPath24 - baseSpecPath.Parent).ToAgnosticPathString()}\""));

            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            Utility.SetupSpecFile(proxy, referredPathStr11, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"11\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr12, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"12\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr13, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"13\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr22, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"22\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr23, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"23\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr24, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"24\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr31, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"31\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr32, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"32\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr33, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"33\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr34, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"34\"",
                "    \"[End Of Text]\""));

            var defaultPathStr1 =
                (referredSpecPath31 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr2 =
                (referredSpecPath32 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr3 =
                (referredSpecPath33 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr4 =
                (referredSpecPath34 - derivedSpecPath.Parent).ToAgnosticPathString();

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first"].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth"].Spawn());

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            // act
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Hide("first");
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Hide("second");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Hide("third");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().Hide("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["first"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["second"];
            });
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["fourth"];
            });

            Assert.Equal(
                "31",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "32",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "34",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"first\": \"__HIDDEN__\"",
                    "  \"second\": \"__HIDDEN__\"",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    "  \"third\": \"__HIDDEN__\"",
                    "  \"fourth\": \"__HIDDEN__\""),
                baseSpec.ToString());
        }

        /// <summary>
        /// LiteralIndexer上のExplicitDefault機能をテスト。
        /// Test ExplicitDefault method on LiteralIndexer.
        /// </summary>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A base spec's os-agnostic path.
        /// </param>
        /// <param name="referredPathStr11">
        /// The first tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr12">
        /// The second tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr13">
        /// The third tested path on derived spec.
        /// </param>
        /// <param name="referredPathStr22">
        /// The second tested path on base spec.
        /// </param>
        /// <param name="referredPathStr23">
        /// The third tested path on base spec.
        /// </param>
        /// <param name="referredPathStr24">
        /// The fourth tested path on base spec.
        /// </param>
        /// <param name="referredPathStr31">
        /// The first tested path for default spec.
        /// </param>
        /// <param name="referredPathStr32">
        /// The second tested path for default spec.
        /// </param>
        /// <param name="referredPathStr33">
        /// The third tested path for default spec.
        /// </param>
        /// <param name="referredPathStr34">
        /// The fourth tested path for default spec.
        /// </param>
        [Theory]
        [InlineData(
            "derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11/referred.spec",
            "12/referred.spec",
            "13/referred.spec",
            "22/referred.spec",
            "23/referred.spec",
            "24/referred.spec",
            "31/referred.spec",
            "32/referred.spec",
            "33/referred.spec",
            "34/referred.spec")]
        [InlineData(
            "dir/derived.spec",
            "base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        [InlineData(
            "derived.spec",
            "dir/base.spec",
            "11.spec",
            "12.spec",
            "13.spec",
            "22.spec",
            "23.spec",
            "24.spec",
            "31.spec",
            "32.spec",
            "33.spec",
            "34.spec")]
        public void PathExplicitDefaultTest(
            string derivedSpecPathStr,
            string baseSpecPathStr,
            string referredPathStr11,
            string referredPathStr12,
            string referredPathStr13,
            string referredPathStr22,
            string referredPathStr23,
            string referredPathStr24,
            string referredPathStr31,
            string referredPathStr32,
            string referredPathStr33,
            string referredPathStr34)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var baseSpecPath = AgnosticPath.FromAgnosticPathString(
                baseSpecPathStr);
            var referredSpecPath11 = AgnosticPath.FromAgnosticPathString(
                referredPathStr11);
            var referredSpecPath12 = AgnosticPath.FromAgnosticPathString(
                referredPathStr12);
            var referredSpecPath13 = AgnosticPath.FromAgnosticPathString(
                referredPathStr13);
            var referredSpecPath22 = AgnosticPath.FromAgnosticPathString(
                referredPathStr22);
            var referredSpecPath23 = AgnosticPath.FromAgnosticPathString(
                referredPathStr23);
            var referredSpecPath24 = AgnosticPath.FromAgnosticPathString(
                referredPathStr24);
            var referredSpecPath31 = AgnosticPath.FromAgnosticPathString(
                referredPathStr31);
            var referredSpecPath32 = AgnosticPath.FromAgnosticPathString(
                referredPathStr32);
            var referredSpecPath33 = AgnosticPath.FromAgnosticPathString(
                referredPathStr33);
            var referredSpecPath34 = AgnosticPath.FromAgnosticPathString(
                referredPathStr34);
            var proxy = Utility.TestOutSideProxy();

            Utility.SetupSpecFile(
                proxy,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    $"  \"first\": \"{(referredSpecPath11 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"second\": \"{(referredSpecPath12 - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""));
            Utility.SetupSpecFile(
                proxy,
                baseSpecPathStr,
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"third\": \"{(referredSpecPath23 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    $"  \"fourth\": \"{(referredSpecPath24 - baseSpecPath.Parent).ToAgnosticPathString()}\""));

            var type = typeof(ValidSpawnerRootWithDefaultConstructor);
            Utility.SetupSpecFile(proxy, referredPathStr11, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"11\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr12, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"12\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr13, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"13\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr22, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"22\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr23, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"23\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr24, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"24\"",
                "    \"[End Of Text]\""));

            Utility.SetupSpecFile(proxy, referredPathStr31, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"31\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr32, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"32\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr33, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"33\"",
                "    \"[End Of Text]\""));
            Utility.SetupSpecFile(proxy, referredPathStr34, Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(type)}\"",
                "\"properties\":",
                "  \"return value\": |+",
                "    \"34\"",
                "    \"[End Of Text]\""));

            var defaultPathStr1 =
                (referredSpecPath31 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr2 =
                (referredSpecPath32 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr3 =
                (referredSpecPath33 - derivedSpecPath.Parent).ToAgnosticPathString();
            var defaultPathStr4 =
                (referredSpecPath34 - derivedSpecPath.Parent).ToAgnosticPathString();

            var derivedSpec = SpecRoot.Fetch(proxy, derivedSpecPath);
            var baseSpec = SpecRoot.Fetch(proxy, baseSpecPath);

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first"].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second"].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth"].Spawn());

            Assert.Equal(
                "11",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "12",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "24",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            // act
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().ExplicitDefault("first");
            derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().ExplicitDefault("second");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().ExplicitDefault("third");
            baseSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>().ExplicitDefault("fourth");

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["first"];
            });
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["second"];
            });
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third"].Spawn());
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()["fourth"];
            });

            Assert.Equal(
                "31",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "first", defaultPathStr1].Spawn());
            Assert.Equal(
                "32",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "second", defaultPathStr2].Spawn());
            Assert.Equal(
                "13",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "third", defaultPathStr3].Spawn());
            Assert.Equal(
                "34",
                derivedSpec.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "fourth", defaultPathStr4].Spawn());

            Assert.Equal(
                Utility.JoinLines(
                    $"\"base\": \"{(baseSpecPath - derivedSpecPath.Parent).ToAgnosticPathString()}\"",
                    "\"properties\":",
                    "  \"first\": \"__DEFAULT__\"",
                    "  \"second\": \"__DEFAULT__\"",
                    $"  \"third\": \"{(referredSpecPath13 - derivedSpecPath.Parent).ToAgnosticPathString()}\""),
                derivedSpec.ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    $"  \"second\": \"{(referredSpecPath22 - baseSpecPath.Parent).ToAgnosticPathString()}\"",
                    "  \"third\": \"__DEFAULT__\"",
                    "  \"fourth\": \"__DEFAULT__\""),
                baseSpec.ToString());
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
                _ = sp.Int["foo"];
                _ = sp.Double["bar"];
                _ = sp.Bool["baz"];
                _ = sp.YesNo["qux"];
                _ = sp.OnOff["quux"];
                _ = sp.Range('(', -1, 5, ']')["corge"];
                _ = sp.Interval(
                    '[',
                    double.NegativeInfinity,
                    double.PositiveInfinity,
                    ')')["grault"];
                _ = sp.Int2["garply"];
                _ = sp.Int3["waldo"];
                _ = sp.Double2["fred"];
                _ = sp.Double3["plugh"];
                _ = sp.Keyword["xyzzy"];
                _ = sp.LimitedKeyword(7)["thud"];
                _ = sp.Text["foobar"];
                var exterior = sp.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "foobaz"];
                var exteriorDir = sp.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()[
                    "fooqux"];
                var interior = sp.Interior<ValidSpawnerChildWithDefaultConstructor>()[
                    "fooquux"];
                _ = sp["inner"].Int["inner int"];
                _ = sp.Int["after inner"];
                _ = sp["inner"].Double["inner double"];
                _ = sp["inner"]["inner inner"].Int["inner inner int"];
                _ = sp.Int["日本語キー"];

                // Dummy specによりSpawnは無事実行可能。
                // Spawn can be executed because of dummy spec.
                _ = exterior.Spawn();
                _ = interior.Spawn();
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"foo\": \"Int\"",
                    "  \"bar\": \"Double\"",
                    "  \"baz\": \"Bool\"",
                    "  \"qux\": \"YesNo\"",
                    "  \"quux\": \"OnOff\"",
                    "  \"corge\": \"Range, (, -1, 5, ]\"",
                    "  \"grault\": \"Interval, [, -∞, ∞, )\"",
                    "  \"garply\": \"Int2\"",
                    "  \"waldo\": \"Int3\"",
                    "  \"fred\": \"Double2\"",
                    "  \"plugh\": \"Double3\"",
                    "  \"xyzzy\": \"Keyword\"",
                    "  \"thud\": \"LimitedKeyword, 7\"",
                    "  \"foobar\": \"Text\"",
                    $"  \"foobaz\": \"Exterior, {Spec.EncodeType(typeof(ValidSpawnerRootWithDefaultConstructor))}\"",
                    $"  \"fooqux\": \"ExteriorDir, {Spec.EncodeType(typeof(ValidSpawnerRootWithDefaultConstructor))}\"",
                    "  \"fooquux\":",
                    $"    \"spawner\": \"Spawner, {Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "  \"inner\":",
                    "    \"inner int\": \"Int\"",
                    "    \"inner double\": \"Double\"",
                    "    \"inner inner\":",
                    "      \"inner inner int\": \"Int\"",
                    "  \"after inner\": \"Int\"",
                    "  \"日本語キー\": \"Int\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にデフォルト値指定を含む。
        /// Test the MoldSpec method with default value.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithDefaultTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Int["foo", 10];
                _ = sp.Double["bar", 3.14];
                _ = sp.Bool["baz", true];
                _ = sp.YesNo["qux", false];
                _ = sp.OnOff["quux", true];
                _ = sp.Range('(', -1, 5, ']')["corge", 3];
                _ = sp.Interval(
                    '[',
                    double.NegativeInfinity,
                    double.PositiveInfinity,
                    ')')["grault", double.NegativeInfinity];
                _ = sp.Int2["garply", (1, -2)];
                _ = sp.Int3["waldo", (3, -4, 5)];
                _ = sp.Double2["fred", (6.7, double.PositiveInfinity)];
                _ = sp.Double3["plugh", (double.NegativeInfinity, 8.9, 10.11)];
                _ = sp.Keyword["xyzzy", "\\phrase\\"];
                _ = sp.LimitedKeyword(7)["thud", "limited"];
                _ = sp.Text[
                    "foobar",
                    Utility.JoinLines("good, morning", "nice to meet you!")];
                var exterior = sp.Exterior<ValidSpawnerRootWithDefaultConstructor>()[
                    "foobaz", "default/path"];
                var exteriorDir = sp.ExteriorDir<ValidSpawnerRootWithDefaultConstructor>()[
                    "fooqux", "default/dir"];
                var interior = sp.Interior<ValidSpawnerChildWithDefaultConstructor>()[
                    "fooquux", typeof(ValidSpawnerChildWithDefaultConstructor)];
                _ = sp["inner"].Int["inner int", 100];
                _ = sp.Int["after inner", 1024];
                _ = sp["inner"].Double["inner double", 2.71];
                _ = sp["inner"]["inner inner"].Int["inner inner int", -1];
                _ = sp.Int["日本語キー", 0];

                // Dummy specによりSpawnは無事実行可能。
                // Spawn can be executed because of dummy spec.
                _ = exterior.Spawn();
                _ = interior.Spawn();
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"foo\": \"Int, 10\"",
                    "  \"bar\": \"Double, 3.14\"",
                    "  \"baz\": \"Bool, True\"",
                    "  \"qux\": \"YesNo, no\"",
                    "  \"quux\": \"OnOff, on\"",
                    "  \"corge\": \"Range, (, -1, 5, ], 3\"",
                    "  \"grault\": \"Interval, [, -∞, ∞, ), -∞\"",
                    "  \"garply\": \"Int2, 1\\\\c -2\"",
                    "  \"waldo\": \"Int3, 3\\\\c -4\\\\c 5\"",
                    "  \"fred\": \"Double2, 6.7\\\\c ∞\"",
                    "  \"plugh\": \"Double3, -∞\\\\c 8.9\\\\c 10.11\"",
                    "  \"xyzzy\": \"Keyword, \\\\\\\\phrase\\\\\\\\\"",
                    "  \"thud\": \"LimitedKeyword, 7, limited\"",
                    "  \"foobar\": |+",
                    "    \"Text, good\\\\c morning\"",
                    "    \"nice to meet you!\"",
                    "    \"[End Of Text]\"",
                    $"  \"foobaz\": \"Exterior, {Spec.EncodeType(typeof(ValidSpawnerRootWithDefaultConstructor))}, default/path\"",
                    $"  \"fooqux\": \"ExteriorDir, {Spec.EncodeType(typeof(ValidSpawnerRootWithDefaultConstructor))}, default/dir\"",
                    "  \"fooquux\":",
                    $"    \"spawner\": \"Spawner, {Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}, {Spec.EncodeType(typeof(ValidSpawnerChildWithDefaultConstructor))}\"",
                    "    \"properties\":",
                    "      \"return value\": \"Text\"",
                    "  \"inner\":",
                    "    \"inner int\": \"Int, 100\"",
                    "    \"inner double\": \"Double, 2.71\"",
                    "    \"inner inner\":",
                    "      \"inner inner int\": \"Int, -1\"",
                    "  \"after inner\": \"Int, 1024\"",
                    "  \"日本語キー\": \"Int, 0\""),
                mold.ToString(true));
        }

        /// <summary>
        /// MoldSpecメソッド時に、デフォルト値のエンコードに用いる変換処理をテスト。
        /// Test the encoding method and decoding method for default value in molding.
        /// </summary>
        /// <param name="target">The encoding target.</param>
        [Theory]
        [InlineData("sinple string")]
        [InlineData("string, with, comma")]
        [InlineData("string\\with\\back\\slash")]
        [InlineData("\\,\\\\,,\\\\,,,complexed\\\\pattern")]
        public void EncodeAndDecodeDefaultValForMoldingTest(string target)
        {
            // act
            var encoded = Spec.EncodeCommas(target);
            var decoded = Spec.DecodeCommas(encoded);

            // assert
            Assert.Equal(target, decoded);
            Assert.False(encoded.Contains(','));
        }

        /// <summary>
        /// MoldSpec用エンコードとして不正な値をデコードした際の例外をテスト。
        /// Test decoding method with invalid encode.
        /// </summary>
        /// <param name="target">The invalid decoding target.</param>
        [Theory]
        [InlineData("this is \\ninvalid string.")]
        [InlineData("this is \\0invalid string.")]
        public void DecodingInvalidDefaultValForMoldingTest(string target)
        {
            // assert
            Assert.Throws<Spec.InvalidDefaultValEncoding>(() =>
            {
                _ = Spec.DecodeCommas(target);
            });
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にデフォルト値指定を含み、デフォルト値は別キーから取得される。
        /// Test the MoldSpec method with default value.
        /// The default value will be gotten from another key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="dynamicDefault">The dynamic default value.</param>
        [Theory]
        [InlineData("foo.spec", 0)]
        [InlineData("foo.spec", -10)]
        [InlineData("foo.spec", 100)]
        [InlineData("dir/bar.spec", 0)]
        [InlineData("dir/bar.spec", -10)]
        [InlineData("dir/bar.spec", 100)]
        [InlineData("dir1/dir2/baz.spec", 0)]
        [InlineData("dir1/dir2/baz.spec", -10)]
        [InlineData("dir1/dir2/baz.spec", 100)]
        public void MoldSpecWithDynamicDefaultTest(string path, int dynamicDefault)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                $"  \"default\": \"{dynamicDefault}\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                var def = sp.Int["default"];
                _ = sp.Int["foo", def];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"default\": \"Int\"",
                    $"  \"foo\": \"Int, {dynamicDefault}\""),
                mold.ToString());
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にif分岐を含み、分岐は別キーのフラグで行われる。
        /// Test the MoldSpec method with if-clause.
        /// The flag will be gotten from another key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="dynamicSwitch">The dynamic switching flag.</param>
        [Theory]
        [InlineData("foo.spec", true)]
        [InlineData("foo.spec", false)]
        [InlineData("dir/bar.spec", true)]
        [InlineData("dir/bar.spec", false)]
        [InlineData("dir1/dir2/baz.spec", true)]
        [InlineData("dir1/dir2/baz.spec", false)]
        public void MoldSpecWithDynamicSwitchTest(string path, bool dynamicSwitch)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                $"  \"flag\": \"{dynamicSwitch}\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                var flag = sp.Bool["flag"];
                if (flag)
                {
                    _ = sp.Int["foo"];
                }
                else
                {
                    _ = sp.Double["bar"];
                }
            };
            var mold = spec.MoldSpec(action);

            // assert
            if (dynamicSwitch)
            {
                Assert.Equal(
                    Utility.JoinLines(
                        "\"base\": \"Spec\"",
                        $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                        "\"properties\":",
                        "  \"flag\": \"Bool\"",
                        "  \"foo\": \"Int\""),
                    mold.ToString());
            }
            else
            {
                Assert.Equal(
                    Utility.JoinLines(
                        "\"base\": \"Spec\"",
                        $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                        "\"properties\":",
                        "  \"flag\": \"Bool\"",
                        "  \"bar\": \"Double\""),
                    mold.ToString());
            }
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、specファイルにMagic wordが含まれる。
        /// Test the MoldSpec method with if-clause.
        /// The flag will be gotten from another key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithMagicWordTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"hidden1\": \"__HIDDEN__\"",
                "  \"default1\": \"__DEFAULT__\"",
                "  \"hidden2\": \"__HIDDEN__\"",
                "  \"default2\": \"__DEFAULT__\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Int["hidden1"];
                _ = sp.Int["default1"];
                _ = sp.Int["hidden2", 10];
                _ = sp.Int["default2", 20];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"hidden1\": \"Int\"",
                    "  \"default1\": \"Int\"",
                    "  \"hidden2\": \"Int, 10\"",
                    "  \"default2\": \"Int, 20\""),
                mold.ToString());
        }

        /// <summary>
        /// MoldSpecメソッドのBase Specによる挙動の変化をテスト。
        /// Test the MoldSpec method with base spec.
        /// </summary>
        /// <param name="derivedPathStr">The derived spec's path.</param>
        /// <param name="basePathStr">The base spec's path.</param>
        /// <param name="relativeBasePathStr">The base spec's relative path.</param>
        /// <param name="derivedValue">The derived spec's value.</param>
        /// <param name="baseValue">The base spec's value.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec", "bar.spec", 0, 1)]
        [InlineData("foo.spec", "bar.spec", "bar.spec", 100, 200)]
        [InlineData("foo.spec", "dir/bar.spec", "dir/bar.spec", 100, 200)]
        [InlineData("dir/foo.spec", "bar.spec", "../bar.spec", 100, 200)]
        public void MoldSpecWithBaseSpecTest(
            string derivedPathStr,
            string basePathStr,
            string relativeBasePathStr,
            int derivedValue,
            int baseValue)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedPath =
                AgnosticPath.FromAgnosticPathString(derivedPathStr);
            Utility.SetupSpecFile(proxy, derivedPathStr, Utility.JoinLines(
                $"\"base\": \"{relativeBasePathStr}\"",
                "\"properties\":",
                $"  \"derived\": \"{derivedValue}\""));
            Utility.SetupSpecFile(proxy, basePathStr, Utility.JoinLines(
                "\"properties\":",
                $"  \"base\": \"{baseValue}\""));

            // act
            var derivedSpec = SpecRoot.Fetch(proxy, derivedPath, true);
            Action<Spec> action = (sp) =>
            {
                _ = sp.Int["derived"];
                var referredBase = sp.Int["base"];
                _ = sp.Int["dynamic", referredBase];
            };
            var mold = derivedSpec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"derived\": \"Int\"",
                    "  \"base\": \"Int\"",
                    $"  \"dynamic\": \"Int, {baseValue}\""),
                mold.ToString());
        }
    }
}