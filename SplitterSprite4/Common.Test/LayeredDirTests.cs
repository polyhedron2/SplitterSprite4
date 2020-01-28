// -----------------------------------------------------------------------
// <copyright file="LayeredDirTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using Xunit;

    /// <summary>
    /// Test the LayeredDir class.
    /// </summary>
    public class LayeredDirTests
    {
        /// <summary>
        /// Test the LayeredFile creation.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("")]
        [InlineData("dir")]
        [InlineData("dir/dir2")]
        [InlineData("dir/dir2/dir3")]
        public void CreationTest(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);

            // act
            var layeredDir = new LayeredDir(proxy.FileIO, layeredPath);

            // assert
            Assert.Equal(
                layeredPathStr, layeredDir.Path.ToAgnosticPathString());
        }

        /// <summary>
        /// Test the LayeredFile creation with out of layer path.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("../")]
        [InlineData("../../")]
        [InlineData("../dir")]
        public void CreationTestWithOuterPath(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);

            // assert
            Assert.Throws<Layer.OutOfLayerAccessException>(() =>
            {
                new LayeredDir(proxy.FileIO, layeredPath);
            });
        }

        /// <summary>
        /// Test the EnumerateDirectories method.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("dir")]
        [InlineData("dir/dir2")]
        [InlineData("dir/dir2/dir3")]
        public void EnumerateDirectoriesTest(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            var layeredDir = new LayeredDir(proxy.FileIO, layeredPath);

            this.SetUpLayers(proxy, "first", "second", "third");

            // directories will be enumerated.
            this.SetUpDir(proxy, "second", layeredPathStr, "dir1");
            this.SetUpDir(proxy, "second", layeredPathStr, "dir2");
            this.SetUpDir(proxy, "third", layeredPathStr, "dir2");
            this.SetUpDir(proxy, "third", layeredPathStr, "dir3");

            // files will be ignored.
            this.SetUpFile(proxy, "first", layeredPathStr, "file1");
            this.SetUpFile(proxy, "second", layeredPathStr, "file2");
            this.SetUpFile(proxy, "third", layeredPathStr, "file3");

            // act
            var expected = new string[] { "dir1", "dir2", "dir3" }.ToHashSet();
            var actual = layeredDir.EnumerateDirectories().Select(
                (path) => path.ToAgnosticPathString()).ToHashSet();

            // assert
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test the EnumerateFiles method.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("dir")]
        [InlineData("dir/dir2")]
        [InlineData("dir/dir2/dir3")]
        public void EnumerateFilesTest(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            var layeredDir = new LayeredDir(proxy.FileIO, layeredPath);

            this.SetUpLayers(proxy, "first", "second", "third");

            // directories will be enumerated.
            this.SetUpFile(proxy, "second", layeredPathStr, "file1");
            this.SetUpFile(proxy, "second", layeredPathStr, "file2");
            this.SetUpFile(proxy, "third", layeredPathStr, "file2");
            this.SetUpFile(proxy, "third", layeredPathStr, "file3");

            // files will be ignored.
            this.SetUpDir(proxy, "first", layeredPathStr, "dir1");
            this.SetUpDir(proxy, "second", layeredPathStr, "dir2");
            this.SetUpDir(proxy, "third", layeredPathStr, "dir3");

            // act
            var expected = new string[] { "file1", "file2", "file3" }.ToHashSet();
            var actual = layeredDir.EnumerateFiles().Select(
                (path) => path.ToAgnosticPathString()).ToHashSet();

            // assert
            Assert.Equal(expected, actual);
        }

        private void SetUpLayers(
            OutSideProxy proxy, params string[] layerNames)
        {
            for (int i = 0; i < layerNames.Length - 1; i++)
            {
                var layer = new Layer(proxy.FileIO, layerNames[i], acceptEmpty: true);
                layer.Dependencies = new string[] { layerNames[i + 1] };
                layer.Save();
            }

            {
                var layer = new Layer(
                    proxy.FileIO,
                    layerNames[layerNames.Length - 1],
                    acceptEmpty: true);
                layer.Save();
            }
        }

        private void SetUpFile(
            OutSideProxy proxy,
            string layerName,
            string parentLayeredPathStr,
            string childNameStr)
        {
            var parentLayeredPath =
                AgnosticPath.FromAgnosticPathString(parentLayeredPathStr);
            var childPath = AgnosticPath.FromAgnosticPathString(childNameStr);
            var layer = new Layer(proxy.FileIO, layerName);

            proxy.FileIO.CreateDirectory(parentLayeredPath + layer.Path);
            proxy.FileIO.WithTextWriter(
                childPath + parentLayeredPath + layer.Path, false, (writer) =>
                {
                    writer.WriteLine("file body");
                });
        }

        private void SetUpDir(
            OutSideProxy proxy,
            string layerName,
            string parentLayeredPathStr,
            string childNameStr)
        {
            var parentLayeredPath =
                AgnosticPath.FromAgnosticPathString(parentLayeredPathStr);
            var childPath = AgnosticPath.FromAgnosticPathString(childNameStr);
            var layer = new Layer(proxy.FileIO, layerName);

            proxy.FileIO.CreateDirectory(
                childPath + parentLayeredPath + layer.Path);
        }
    }
}
