// -----------------------------------------------------------------------
// <copyright file="LayeredFileTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;
    using Xunit;

    /// <summary>
    /// Test the LayeredFile class.
    /// </summary>
    public class LayeredFileTests
    {
        /// <summary>
        /// Test the LayeredFile creation.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void CreationTest(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            this.SetUpLayers(proxy, "first", "second", "third");
            this.SetUpFile(proxy, layeredPathStr, "second", "third");

            // act
            var lFile = new LayeredFile(proxy, layeredPath);

            // assert
            Assert.Equal(layeredPathStr, lFile.Path.ToAgnosticPathString());
            Assert.Equal(string.Empty, lFile.Author);
            Assert.Equal(string.Empty, lFile.Title);
            Assert.Equal(
                $"second/{layeredPathStr}",
                lFile.FetchReadPath().ToAgnosticPathString());
            Assert.Equal(
                $"first/{layeredPathStr}",
                lFile.FetchWritePath(
                    new Layer(proxy, "first")).ToAgnosticPathString());
        }

        /// <summary>
        /// Test the LayeredFile creation with meta data.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void CreationTestWithMeta(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            this.SetUpLayers(proxy, "first", "second", "third");
            this.SetUpFile(proxy, layeredPathStr, "second", "third");
            this.SetUpMeta(proxy, layeredPathStr, "first", "second", "third");

            // act
            var lFile = new LayeredFile(proxy, layeredPath);

            // assert
            Assert.Equal(layeredPathStr, lFile.Path.ToAgnosticPathString());

            // the metadata of the second layer.
            Assert.Equal("second_author", lFile.Author);
            Assert.Equal("second_title", lFile.Title);

            Assert.Equal(
                $"second/{layeredPathStr}",
                lFile.FetchReadPath().ToAgnosticPathString());
            Assert.Equal(
                $"first/{layeredPathStr}",
                lFile.FetchWritePath(
                    new Layer(proxy, "first")).ToAgnosticPathString());
        }

        /// <summary>
        /// Test the LayeredFile creation with invalid path.
        /// </summary>
        /// <param name="outerPath">The os-agnostic path for out of layers.</param>
        [Theory]
        [InlineData("../foo.txt")]
        [InlineData("../../foo.txt")]
        public void CreationTestWithOuterPath(string outerPath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath = AgnosticPath.FromAgnosticPathString(outerPath);

            // assert
            Assert.Throws<LayeredFile.OutOfLayerAccessException>(() =>
            {
                new LayeredFile(proxy, layeredPath);
            });
        }

        /// <summary>
        /// Test metadata update process.
        /// </summary>
        /// <param name="layeredPathStr">The os-agnostic path.</param>
        /// <param name="newAuthor">The new author.</param>
        /// <param name="newTitle">The new title.</param>
        [Theory]
        [InlineData("foo.txt", "new author", "new title")]
        [InlineData("dir/foo.txt", "new author", "new title")]
        [InlineData("dir/dir2/foo.txt", "new author", "new title")]
        [InlineData("foo.txt", "", "")]
        [InlineData("dir/foo.txt", "", "")]
        [InlineData("dir/dir2/foo.txt", "", "")]
        public void MetaDataUpdateTest(
            string layeredPathStr, string newAuthor, string newTitle)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            this.SetUpLayers(proxy, "layer");
            this.SetUpFile(proxy, layeredPathStr, "layer");
            var layeredPath = AgnosticPath.FromAgnosticPathString(layeredPathStr);
            var lFileForEdit = new LayeredFile(proxy, layeredPath);

            // act
            lFileForEdit.Author = newAuthor;
            lFileForEdit.Title = newTitle;
            lFileForEdit.SaveMetaData(new Layer(proxy, "layer"));

            // assert
            var lFile = new LayeredFile(proxy, layeredPath);
            Assert.Equal(newAuthor, lFile.Author);
            Assert.Equal(newTitle, lFile.Title);
        }

        /// <summary>
        /// Test metadata update process with specified layer.
        /// </summary>
        /// <param name="layeredPathStr">The os-agnostic path.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void MetaDataUpdateTestWithSpecifiedLayer(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            this.SetUpLayers(proxy, "first", "second", "third");
            this.SetUpFile(proxy, layeredPathStr, "second", "third");
            var layeredPath = AgnosticPath.FromAgnosticPathString(layeredPathStr);
            var lFileForEdit = new LayeredFile(proxy, layeredPath);

            // act
            lFileForEdit.Author = "third_author";
            lFileForEdit.Title = "third_author";
            lFileForEdit.SaveMetaData(new Layer(proxy, "third"));

            // assert
            // 最上位でないレイヤーに書き込んでも変化はない
            // There are no changes with update for non-top layer.
            var lFileThird = new LayeredFile(proxy, layeredPath);
            Assert.Equal(string.Empty, lFileThird.Author);
            Assert.Equal(string.Empty, lFileThird.Title);

            // act
            lFileForEdit.Author = "second_author";
            lFileForEdit.Title = "second_author";
            lFileForEdit.SaveMetaData(new Layer(proxy, "second"));

            // assert
            // 最上位でないレイヤーに書き込めば変化する
            // The meta data is changed with update for top layer.
            var lFileSecond = new LayeredFile(proxy, layeredPath);
            Assert.Equal("second_author", lFileSecond.Author);
            Assert.Equal("second_author", lFileSecond.Title);

            // act
            lFileForEdit.Author = "first_author";
            lFileForEdit.Title = "first_author";
            lFileForEdit.SaveMetaData(new Layer(proxy, "first"));

            // assert
            // ファイル実体のないレイヤーに書き込んでも利用されない
            // There are no changes with update for a layer with no file.
            var lFileFirst = new LayeredFile(proxy, layeredPath);
            Assert.Equal("second_author", lFileFirst.Author);
            Assert.Equal("second_author", lFileFirst.Title);
        }

        /// <summary>
        /// Test FetchReadPath method.
        /// </summary>
        /// <param name="layeredPathStr">The layered path.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void CreationTestWithoutFile(string layeredPathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            this.SetUpLayers(proxy, "first", "second", "third");

            // assert
            Assert.Throws<LayeredFile.LayeredFileNotFoundException>(() =>
            {
                new LayeredFile(proxy, layeredPath);
            });
        }

        private void SetUpLayers(
            OutSideProxy proxy, params string[] layerNames)
        {
            for (int i = 0; i < layerNames.Length - 1; i++)
            {
                var layer = new Layer(proxy, layerNames[i], acceptEmpty: true);
                layer.Dependencies = new string[] { layerNames[i + 1] };
                layer.Save();
            }

            {
                var layer = new Layer(
                    proxy,
                    layerNames[layerNames.Length - 1],
                    acceptEmpty: true);
                layer.Save();
            }
        }

        private void SetUpFile(
            OutSideProxy proxy,
            string layeredPathStr,
            params string[] layerNames)
        {
            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            foreach (var layerName in layerNames)
            {
                var layer = new Layer(proxy, layerName);
                proxy.FileIO.CreateDirectory(
                    (layer.Path + layeredPath).Parent);
                proxy.FileIO.WithTextWriter(
                    layer.Path + layeredPath, false, (writer) =>
                {
                    writer.WriteLine("file body");
                });
            }
        }

        private void SetUpMeta(
            OutSideProxy proxy,
            string layeredPathStr,
            params string[] layerNames)
        {
            var metaPath = AgnosticPath.FromAgnosticPathString(
                layeredPathStr + ".meta");
            foreach (var layerName in layerNames)
            {
                var layer = new Layer(proxy, layerName, true);
                var meta = new RootYAML(proxy, layer.Path + metaPath, true);
                meta["author"] = new ScalarYAML($"{layerName}_author");
                meta["title"] = new ScalarYAML($"{layerName}_title");
                meta.Overwrite();
            }
        }
    }
}
