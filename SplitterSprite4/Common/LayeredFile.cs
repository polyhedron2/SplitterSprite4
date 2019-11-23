// -----------------------------------------------------------------------
// <copyright file="LayeredFile.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// レイヤーによるファイルの重ね合わせ結果を表現するクラス
    /// File class as results of layers sort.
    /// </summary>
    public class LayeredFile
    {
        private OutSideProxy proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredFile"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file access.</param>
        /// <param name="path">The relative path from layer directory.</param>
        public LayeredFile(OutSideProxy proxy, AgnosticPath path)
        {
            if (path.ToAgnosticPathString().StartsWith("../"))
            {
                throw new OutOfLayerAccessException(path);
            }

            this.proxy = proxy;
            this.Path = path;

            var meta = this.FetchReadMetaYAML();
            this.Author = meta["author", new ScalarYAML()].ToString();
            this.Title = meta["title", new ScalarYAML()].ToString();
        }

        /// <summary>
        /// Gets the relative path from layer directory.
        /// </summary>
        public AgnosticPath Path { get; }

        /// <summary>
        /// Gets or sets the author name. If it's undefined, empty string.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the title. If it's undefined, empty string.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// メタデータ情報を保存
        /// Save the metadata.
        /// </summary>
        /// <param name="writeLayer">The layer for file write.</param>
        public void SaveMetaData(Layer writeLayer)
        {
            var meta = this.FetchWriteMetaYAML(writeLayer);

            void UpdateMeta(string key, string value)
            {
                if (value == string.Empty)
                {
                    meta.Remove(key);
                }
                else
                {
                    meta[key] = new ScalarYAML(value);
                }
            }

            UpdateMeta("author", this.Author);
            UpdateMeta("title", this.Title);

            meta.Save();
        }

        /// <summary>
        /// メタデータ情報を保存
        /// Save the metadata.
        /// </summary>
        public void SaveMetaData()
        {
            // 読み込みに用いられる最上位レイヤーに対して書き込むため
            // FetchReadLayerを用いる。
            // Use FetchReadLayer to write into the layer which is used to read.
            this.SaveMetaData(this.FetchReadLayer());
        }

        /// <summary>
        /// Gets the os-agnostic path for file read.
        /// </summary>
        /// <returns>The os-agnostic path for file read.</returns>
        public AgnosticPath FetchReadPath()
        {
            return this.FetchReadLayer().Path + this.Path;
        }

        /// <summary>
        /// Gets the os-agnostic path for file write.
        /// </summary>
        /// <param name="writeLayer">The layer for file write.</param>
        /// <returns>The os-agnostic path for file write.</returns>
        public AgnosticPath FetchWritePath(Layer writeLayer)
        {
            return writeLayer.Path + this.Path;
        }

        /// <summary>
        /// Gets the os-agnostic path for file write.
        /// </summary>
        /// <returns>The os-agnostic path for file write.</returns>
        public AgnosticPath FetchWritePath()
        {
            return this.FetchWritePath(new Layer(this.proxy, "save", true));
        }

        private Layer FetchReadLayer()
        {
            try
            {
                return Layer.FetchSortedLayers(this.proxy).First(
                    layer => this.proxy.FileIO.FileExists(
                        layer.Path + this.Path));
            }
            catch (InvalidOperationException)
            {
                // どのレイヤーにもファイルが存在しない
                // No layer has the file.
                throw new LayeredFileNotFoundException(this.Path);
            }
        }

        private RootYAML FetchReadMetaYAML()
        {
            var metaFilePath = AgnosticPath.FromAgnosticPathString(
                this.FetchReadPath().ToAgnosticPathString() + ".meta");
            return new RootYAML(this.proxy, metaFilePath, acceptEmpty: true);
        }

        private RootYAML FetchWriteMetaYAML(Layer writeLayer)
        {
            var metaFilePath = AgnosticPath.FromAgnosticPathString(
                this.FetchWritePath(writeLayer).ToAgnosticPathString() + ".meta");
            return new RootYAML(this.proxy, metaFilePath, acceptEmpty: true);
        }

        /// <summary>
        /// レイヤーディレクトリ外にアクセスが実行された際の例外
        /// The exception that is thrown when an attempt to
        /// access out of the layer directory path.
        /// </summary>
        public class OutOfLayerAccessException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OutOfLayerAccessException"/> class.
            /// </summary>
            /// <param name="path">The relative path from layer directory.</param>
            internal OutOfLayerAccessException(AgnosticPath path)
                : base(
                      $"レイヤーディレクトリ外" +
                      $"\"{path.ToAgnosticPathString()}\"へのアクセス")
            {
            }
        }

        /// <summary>
        /// レイヤーが重ね合わされたファイルパス上にデータが見つからない際の例外
        /// The exception that is thrown when an attempt to
        /// access a file that does not exist on any layers.
        /// </summary>
        public class LayeredFileNotFoundException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LayeredFileNotFoundException"/> class.
            /// </summary>
            /// <param name="path">The os-agnostic path.</param>
            public LayeredFileNotFoundException(AgnosticPath path)
                : base($"ファイル\"{path.ToOSPathString()}\"が見つかりません")
            {
                this.Path = path;
            }

            /// <summary>
            /// Gets the os-agnostic path.
            /// </summary>
            public AgnosticPath Path { get; private set; }
        }
    }
}
