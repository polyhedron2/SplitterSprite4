// -----------------------------------------------------------------------
// <copyright file="LayerTests.cs" company="MagicKitchen">
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
    /// Test the Layer class.
    /// </summary>
    public class LayerTests
    {
        /// <summary>
        /// Test the layer creation.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="dependencies">The dependent layers.</param>
        [Theory]
        [InlineData("layer")]
        [InlineData("foo")]
        [InlineData("bar", "foo")]
        [InlineData("baz", "foo", "bar")]
        public void CreationTest(string name, params string[] dependencies)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath =
                AgnosticPath.FromAgnosticPathString($"{name}/layer.meta");
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);
            var yamlBody =
                dependencies.Length == 0 ?
                "dependencies: []" :
                "dependencies:\n" + Utility.JoinLines(
                    dependencies.Select(d => $"  - {d}").ToArray());
            proxy.FileIO.WithTextWriter(agnosticPath, false, (writer) =>
            {
                writer.Write(yamlBody);
            });

            // act
            var layer = new Layer(proxy, name);

            // assert
            Assert.Equal(name, layer.Name);
            Assert.Equal(
                dependencies.ToHashSet(),
                layer.Dependencies.ToHashSet());
        }

        /// <summary>
        /// Test the layer creation which does not exist.
        /// </summary>
        /// <param name="name">The layer name.</param>
        [Theory]
        [InlineData("layer")]
        public void CreationWithEmptyTest(string name)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();

            // act
            var layer = new Layer(proxy, name, acceptEmpty: true);

            // assert
            Assert.Equal(name, layer.Name);
        }

        /// <summary>
        /// Test the layer creation.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="dependencies">The dependent layers.</param>
        [Theory]
        [InlineData("layer")]
        [InlineData("foo")]
        [InlineData("bar", "foo")]
        [InlineData("baz", "foo", "bar")]
        public void SaveTest(string name, params string[] dependencies)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var editLayer = new Layer(proxy, name, acceptEmpty: true);
            editLayer.Dependencies = dependencies;

            // act
            editLayer.Save();
            var layer = new Layer(proxy, name);

            // assert
            Assert.Equal(name, layer.Name);
            Assert.Equal(
                dependencies.ToHashSet(),
                layer.Dependencies.ToHashSet());
        }

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
            var layers = Layer.FetchSortedLayers(proxy);

            // assert
            var result = layers.Select(layer => layer.Name).ToArray();
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
            var layers = Layer.FetchSortedLayers(proxy);

            // assert
            var result = layers.Select(layer => layer.Name).ToArray();
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
            var layers = Layer.FetchSortedLayers(proxy);

            // assert
            var result = layers.Select(layer => layer.Name).ToArray();
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
            var layers = Layer.FetchSortedLayers(proxy);

            // assert
            var result = layers.Select(layer => layer.Name).ToArray();

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
            Assert.Throws<Layer.CyclicDependencyException>(() =>
            {
                var layers = Layer.FetchSortedLayers(proxy).ToArray();
            });
        }

        /// <summary>
        /// Test layer sort with non-existent layer dependency.
        /// </summary>
        [Fact]
        public void NonExistentLayerExceptionTest()
        {
            // arange
            var proxy = Utility.TestOutSideProxy();

            /*     Hasse diagram of the tested dependencies.
             * head --------------------------------------> tail
             *
             * [first]---[second]---[third]---[fourth]---[nonexistence]
             */
            this.SetUpLauncherYamlFile(proxy);
            this.SetupLayerYamlFile(
                proxy, "first", "dependencies:", "  - second");
            this.SetupLayerYamlFile(
                proxy, "second", "dependencies:", "  - third");
            this.SetupLayerYamlFile(
                proxy, "third", "dependencies:", "  - fourth");
            this.SetupLayerYamlFile(
                proxy, "fourth", "dependencies:", "  - nonexistence");

            // act & assert
            Assert.Throws<Layer.NonExistentLayerException>(() =>
            {
                var layers = Layer.FetchSortedLayers(proxy).ToArray();
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
