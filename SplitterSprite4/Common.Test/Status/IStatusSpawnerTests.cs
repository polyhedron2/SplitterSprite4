// -----------------------------------------------------------------------
// <copyright file="IStatusSpawnerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Status
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using MagicKitchen.SplitterSprite4.Common.Status;
    using Xunit;

    /// <summary>
    /// Test the IStatusSpawner interface.
    /// </summary>
    public class IStatusSpawnerTests
    {
        /// <summary>
        /// Test the Spawn method of IStatusSpawner.
        /// </summary>
        /// <param name="pathX">The first spec path.</param>
        /// <param name="pathY">The second specc path.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec")]
        [InlineData("dir/baz.spec", "dir1/dir2/qux.spec")]
        [InlineData("dir1/dir2/quux.spec", "dir/corge.spec")]
        public void SpawnTest(string pathX, string pathY)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            Utility.SetupSpecFile(proxy, pathX, Utility.JoinLines(
                "\"properties\":",
                "  \"dummy key\": \"dummy value\""));
            Utility.SetupSpecFile(proxy, pathY, Utility.JoinLines(
                "\"properties\":",
                "  \"dummy key\": \"dummy value\""));
            var agnosticPathX = AgnosticPath.FromAgnosticPathString(pathX);
            var agnosticPathY = AgnosticPath.FromAgnosticPathString(pathY);
            var specX = SpecRoot.Fetch(proxy, agnosticPathX);
            var specY = SpecRoot.Fetch(proxy, agnosticPathY);

            // act
            ISpawnerRoot0<StatusA> spawnerAX1 = new StatusASpawner();
            spawnerAX1.Spec = specX;
            var statusAX1 = spawnerAX1.Spawn();

            ISpawnerRoot0<StatusA> spawnerAX2 = new StatusASpawner();
            spawnerAX2.Spec = specX;
            var statusAX2 = spawnerAX2.Spawn();

            ISpawnerRoot0<StatusA> spawnerAY = new StatusASpawner();
            spawnerAY.Spec = specY;
            var statusAY = spawnerAY.Spawn();

            ISpawnerRoot0<StatusB> spawnerBX = new StatusBSpawner();
            spawnerBX.Spec = specX;
            var statusBX = spawnerBX.Spawn();

            // assert
            // Same objects will be returned for same spec path and same spawner class.
            Assert.Same(statusAX1, statusAX2);

            // In other cases, objects will be different.
            Assert.NotSame(statusAX1, statusAY);
            Assert.NotSame(statusAX1, statusBX);
        }

        private class StatusA : IStatus
        {
        }

        private class StatusB : IStatus
        {
        }

        private class StatusASpawner : IStatusSpawner<StatusA>
        {
            public SpecRoot Spec { get; set; }

            public string Note { get; } = "StatusA";

            public StatusA SpawnStatus()
            {
                return new StatusA();
            }
        }

        private class StatusBSpawner : IStatusSpawner<StatusB>
        {
            public SpecRoot Spec { get; set; }

            public string Note { get; } = "StatusB";

            public StatusB SpawnStatus()
            {
                return new StatusB();
            }
        }
    }
}
