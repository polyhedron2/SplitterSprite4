// -----------------------------------------------------------------------
// <copyright file="SpawnerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Spawner class.
    /// </summary>
    public class SpawnerTests
    {
        private interface IExtraInterface
        {
            string ExtraFunc();
        }

        private interface IInformalISpawner : ISpawnerRoot<string>
        {
            string InformalSpawn();
        }

        /// <summary>
        /// Test spawn with dummy arguments.
        /// </summary>
        /// <param name="path">The agnostic path for testing.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void DummySpawnTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"dummy key\": \"dummy value\""));
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            var specRoot = SpecRoot.Fetch(proxy, agnosticPath);
            var specChild = specRoot.Child["child", typeof(ISpawnerChild<string>)];

            // act
            var spawnerRootWithoutArgs = new TestSpawnerRootWithoutArgs();
            spawnerRootWithoutArgs.Spec = specRoot;
            var resultRootWithoutArgs =
                Spawner.DummySpawn<string>(spawnerRootWithoutArgs);

            var spawnerRootWithoutArgsAndExtraIF =
                new TestSpawnerRootWithoutArgsAndExtraIF();
            spawnerRootWithoutArgsAndExtraIF.Spec = specRoot;
            var resultRootWithoutArgsAndExtraIF =
                Spawner.DummySpawn<string>(spawnerRootWithoutArgsAndExtraIF);

            var spawnerRootWithArgs = new TestSpawnerRootWithArgs();
            spawnerRootWithArgs.Spec = specRoot;
            var resultRootWithArgs =
                Spawner.DummySpawn<string>(spawnerRootWithArgs);

            var spawnerChildWithoutArgs = new TestSpawnerChildWithoutArgs();
            spawnerChildWithoutArgs.Spec = specChild;
            var resultChildWithoutArgs =
                Spawner.DummySpawn<string>(spawnerChildWithoutArgs);

            var spawnerChildWithArgs = new TestSpawnerChildWithArgs();
            spawnerChildWithArgs.Spec = specChild;
            var resultChildWithArgs =
                Spawner.DummySpawn<string>(spawnerChildWithArgs);

            var informalSpawner = new TestInformalSpawner();
            informalSpawner.Spec = specRoot;

            // assert
            Assert.Equal(
                "SpawnRoot result without args", resultRootWithoutArgs);
            Assert.Equal(
                "SpawnRoot result without args",
                resultRootWithoutArgsAndExtraIF);
            Assert.Equal(
                "SpawnRoot result with (50, 2.71)", resultRootWithArgs);
            Assert.Equal(
                "SpawnChild result without args", resultChildWithoutArgs);
            Assert.Equal(
                "SpawnChild result with (100, 3.14)", resultChildWithArgs);
            Assert.Throws<Spawner.InvalidSpawnerInterfaceImplementation>(() =>
            {
                Spawner.DummySpawn<string>(informalSpawner);
            });
        }

        private class TestSpawnerRootWithoutArgs
            : ISpawnerRootWithoutArgs<string>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "SpawnerRoot without args."; }

            /// <inheritdoc/>
            public string Spawn() => "SpawnRoot result without args";
        }

        private class TestSpawnerRootWithoutArgsAndExtraIF
            : TestSpawnerRootWithoutArgs, IExtraInterface
        {
            public string ExtraFunc() => "Extra result.";
        }

        private class TestSpawnerRootWithArgs
            : ISpawnerRootWithArgs<string, (int, double)>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "SpawnerRoot with 1 args."; }

            /// <inheritdoc/>
            public (int, double) DummyArgs()
            {
                return (50, 2.71);
            }

            /// <inheritdoc/>
            public string Spawn((int, double) args) =>
                $"SpawnRoot result with {args}";
        }

        private class TestSpawnerChildWithoutArgs
            : ISpawnerChildWithoutArgs<string>
        {
            /// <inheritdoc/>
            public SpecChild Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "SpawnerChild without args."; }

            /// <inheritdoc/>
            public string Spawn() => "SpawnChild result without args";
        }

        private class TestSpawnerChildWithArgs
            : ISpawnerChildWithArgs<string, (int, double)>
        {
            /// <inheritdoc/>
            public SpecChild Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "SpawnerChild with 1 args."; }

            /// <inheritdoc/>
            public (int, double) DummyArgs()
            {
                return (100, 3.14);
            }

            /// <inheritdoc/>
            public string Spawn((int, double) args) =>
                $"SpawnChild result with {args}";
        }

        private class TestInformalSpawner : IInformalISpawner
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "SpawnerRoot without args."; }

            public string InformalSpawn() => "Informal Spawn Result";
        }
    }
}
