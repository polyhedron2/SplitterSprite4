// -----------------------------------------------------------------------
// <copyright file="LauncherTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using Xunit;

    /// <summary>
    /// Test the Launcher class.
    /// </summary>
    public class LauncherTests
    {
        /// <summary>
        /// Test layer sort with total order dependency.
        /// </summary>
        [Fact]
        public void LayerSortTotalOrderTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*  Hasse diagram of the tested dependencies.
             * head -----------------------------------> tail
             *
             * [first]-----[second]-----[third]-----[fourth]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy, "first", "dependencies:", "  - second");
            this.SetupLayerYamlFile(
                proxy, "second", "dependencies:", "  - third");
            this.SetupLayerYamlFile(
                proxy, "third", "dependencies:", "  - fourth");
            this.SetupLayerYamlFile(
                proxy, "fourth", "dependencies: []");

            // act
            var launcher = new Launcher(proxy);

            // assert
            var result =
                launcher.Layers.Select(layer => layer.Name).ToArray();
            Assert.Equal(0, Array.IndexOf(result, "first"));
            Assert.Equal(1, Array.IndexOf(result, "second"));
            Assert.Equal(2, Array.IndexOf(result, "third"));
            Assert.Equal(3, Array.IndexOf(result, "fourth"));
        }

        /// <summary>
        /// Test layer sort with partial order dependency.
        /// </summary>
        [Fact]
        public void LayerSortPartialOrderTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*       Hasse diagram of the tested dependencies.
             * head ---------------------------------------------> tail
             *
             *            [second]-------------------[sixth]
             *            /      \                  /        \
             *           /        \                /          \
             *          /          [fourth]--------------------[eighth]
             *         /          /              /             /
             *        /          /              /             /
             * [first]----------/--------[fifth]             /
             *        \        /                \           /
             *         \      /                  \         /
             *          [third]-------------------[seventh]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy,
                "first",
                "dependencies:",
                "  - second",
                "  - third",
                "  - fifth");
            this.SetupLayerYamlFile(
                proxy,
                "second",
                "dependencies:",
                "  - fourth",
                "  - sixth");
            this.SetupLayerYamlFile(
                proxy,
                "third",
                "dependencies:",
                "  - fourth",
                "  - seventh");
            this.SetupLayerYamlFile(
                proxy,
                "fourth",
                "dependencies:",
                "  - eighth");
            this.SetupLayerYamlFile(
                proxy,
                "fifth",
                "dependencies:",
                "  - sixth",
                "  - seventh");
            this.SetupLayerYamlFile(
                proxy,
                "sixth",
                "dependencies:",
                "  - eighth");
            this.SetupLayerYamlFile(
                proxy,
                "seventh",
                "dependencies:",
                "  - eighth");
            this.SetupLayerYamlFile(
                proxy,
                "eighth",
                "dependencies: []");

            // act
            var launcher = new Launcher(proxy);

            // assert
            var result =
                launcher.Layers.Select(layer => layer.Name).ToArray();
            Assert.Equal(
                new string[]
                {
                        "first",
                        "second",
                        "third",
                        "fourth",
                        "fifth",
                        "sixth",
                        "seventh",
                        "eighth",
                }.ToHashSet(),
                result.ToHashSet());
            Assert.True(
                Array.IndexOf(result, "first") <
                Array.IndexOf(result, "second"));
            Assert.True(
                Array.IndexOf(result, "first") <
                Array.IndexOf(result, "third"));
            Assert.True(
                Array.IndexOf(result, "first") <
                Array.IndexOf(result, "fifth"));
            Assert.True(
                Array.IndexOf(result, "second") <
                Array.IndexOf(result, "fourth"));
            Assert.True(
                Array.IndexOf(result, "second") <
                Array.IndexOf(result, "sixth"));
            Assert.True(
                Array.IndexOf(result, "third") <
                Array.IndexOf(result, "fourth"));
            Assert.True(
                Array.IndexOf(result, "third") <
                Array.IndexOf(result, "seventh"));
            Assert.True(
                Array.IndexOf(result, "fourth") <
                Array.IndexOf(result, "eighth"));
            Assert.True(
                Array.IndexOf(result, "fifth") <
                Array.IndexOf(result, "sixth"));
            Assert.True(
                Array.IndexOf(result, "fifth") <
                Array.IndexOf(result, "seventh"));
            Assert.True(
                Array.IndexOf(result, "sixth") <
                Array.IndexOf(result, "eighth"));
            Assert.True(
                Array.IndexOf(result, "seventh") <
                Array.IndexOf(result, "eighth"));
        }

        /// <summary>
        /// Test layer sort with disconnected partial order dependency.
        /// </summary>
        [Fact]
        public void LayerSortDisconnectedPartialOrderTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*  Hasse diagram of the tested dependencies.
             * head ---------------------------------> tail
             *
             *                  [second]
             *                 /        \
             *                /          \
             *               /            \
             *        [first]              [fourth]
             *               \            /
             *                \          /
             *                 \        /
             *                   [third]
             *
             *                  [sixth]
             *                 /        \
             *                /          \
             *               /            \
             *        [fifth]              [eighth]
             *               \            /
             *                \          /
             *                 \        /
             *                 [seventh]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy,
                "first",
                "dependencies:",
                "  - second",
                "  - third");
            this.SetupLayerYamlFile(
                proxy,
                "second",
                "dependencies:",
                "  - fourth");
            this.SetupLayerYamlFile(
                proxy,
                "third",
                "dependencies:",
                "  - fourth");
            this.SetupLayerYamlFile(
                proxy,
                "fourth",
                "dependencies: []");
            this.SetupLayerYamlFile(
                proxy,
                "fifth",
                "dependencies:",
                "  - sixth",
                "  - seventh");
            this.SetupLayerYamlFile(
                proxy,
                "sixth",
                "dependencies:",
                "  - eighth");
            this.SetupLayerYamlFile(
                proxy,
                "seventh",
                "dependencies:",
                "  - eighth");
            this.SetupLayerYamlFile(
                proxy,
                "eighth",
                "dependencies: []");

            // act
            var launcher = new Launcher(proxy);

            // assert
            var result =
                launcher.Layers.Select(layer => layer.Name).ToArray();
            Assert.Equal(
                new string[]
                {
                        "first",
                        "second",
                        "third",
                        "fourth",
                        "fifth",
                        "sixth",
                        "seventh",
                        "eighth",
                }.ToHashSet(),
                result.ToHashSet());
            Assert.True(
                Array.IndexOf(result, "first") <
                Array.IndexOf(result, "second"));
            Assert.True(
                Array.IndexOf(result, "first") <
                Array.IndexOf(result, "third"));
            Assert.True(
                Array.IndexOf(result, "second") <
                Array.IndexOf(result, "fourth"));
            Assert.True(
                Array.IndexOf(result, "third") <
                Array.IndexOf(result, "fourth"));
            Assert.True(
                Array.IndexOf(result, "fifth") <
                Array.IndexOf(result, "sixth"));
            Assert.True(
                Array.IndexOf(result, "fifth") <
                Array.IndexOf(result, "seventh"));
            Assert.True(
                Array.IndexOf(result, "sixth") <
                Array.IndexOf(result, "eighth"));
            Assert.True(
                Array.IndexOf(result, "seventh") <
                Array.IndexOf(result, "eighth"));
        }

        /// <summary>
        /// Test layer sort will ignore directories without "layer.meta".
        /// </summary>
        [Fact]
        public void NonLayerDirectoryIgnoredTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*  Hasse diagram of the tested dependencies.
             * head -----------------------------------> tail
             *
             * [first]-----[second]-----[third]-----[fourth]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy, "first", "dependencies:", "  - second");
            this.SetupLayerYamlFile(
                proxy, "second", "dependencies:", "  - third");
            this.SetupLayerYamlFile(
                proxy, "third", "dependencies:", "  - fourth");
            this.SetupLayerYamlFile(
                proxy, "fourth", "dependencies: []");

            // These direcotries don't have "layer.meta".
            this.SetupEmptyDirectory(proxy, "fifth");
            this.SetupEmptyDirectory(proxy, "sixth");

            // act
            var launcher = new Launcher(proxy);

            // assert
            var result =
                launcher.Layers.Select(layer => layer.Name).ToArray();

            // There are only directories with "layer.meta".
            Assert.Equal(
                new string[]
                {
                        "first",
                        "second",
                        "third",
                        "fourth",
                }.ToHashSet(),
                result.ToHashSet());
        }

        /// <summary>
        /// Test layer sort with dependency cycle.
        /// </summary>
        [Fact]
        public void CyclicDependencyExceptionTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*     Hasse diagram of the tested dependencies.
             * head --------------------------------------> tail
             *
             * [first]---[second]---[third]---[fourth]---[first]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy, "first", "dependencies:", "  - second");
            this.SetupLayerYamlFile(
                proxy, "second", "dependencies:", "  - third");
            this.SetupLayerYamlFile(
                proxy, "third", "dependencies:", "  - fourth");
            this.SetupLayerYamlFile(
                proxy, "fourth", "dependencies:", "  - first");

            // act & assert
            Assert.Throws<Launcher.CyclicDependencyException>(() =>
            {
                var launcher = new Launcher(proxy);
                var layers = launcher.Layers.ToArray();
            });
        }

        private void SetUpLauncherYamlFile(OutSideProxy proxy)
        {
            var agnosticPath =
                AgnosticPath.FromAgnosticPathString("launcher.meta");

            proxy.FileIO.WithTextWriter(agnosticPath, false, (writer) =>
            {
                writer.Write(Utility.JoinLines(
                    "entry_point: spider/cycle.spec"));
            });
        }

        private void SetupLayerYamlFile(
            OutSideProxy proxy, string name, params string[] lines)
        {
            var agnosticPath =
                AgnosticPath.FromAgnosticPathString($"{name}/layer.meta");
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);
            proxy.FileIO.WithTextWriter(agnosticPath, false, (writer) =>
            {
                writer.Write(Utility.JoinLines(lines));
            });
        }

        private void SetupEmptyDirectory(OutSideProxy proxy, string name)
        {
            proxy.FileIO.CreateDirectory(
                AgnosticPath.FromAgnosticPathString(name));
        }
    }
}
