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
        private FileIOProxy fileIOProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredFile"/> class.
        /// </summary>
        /// <param name="fileIOProxy">The FileIOProxy for file access.</param>
        /// <param name="path">The relative path from layer directory.</param>
        /// <param name="acceptAbsence">Accept absence of the layered file or not.</param>
        public LayeredFile(
            FileIOProxy fileIOProxy, AgnosticPath path, bool acceptAbsence = false)
        {
            if (path.ToAgnosticPathString().StartsWith("../"))
            {
                throw new OutOfLayerAccessException(path);
            }

            this.fileIOProxy = fileIOProxy;
            this.Path = path;

            try
            {
                var meta = this.FetchReadMetaYAML();
                this.Author = meta.Scalar["author", new ScalarYAML()].Value;
                this.Title = meta.Scalar["title", new ScalarYAML()].Value;
            }
            catch (LayeredFileNotFoundException ex)
            {
                if (acceptAbsence)
                {
                    this.Author = string.Empty;
                    this.Title = string.Empty;
                }
                else
                {
                    throw ex;
                }
            }
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

            meta.Overwrite();
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

        private Layer FetchReadLayer()
        {
            try
            {
                return Layer.FetchSortedLayers(this.fileIOProxy).First(
                    layer => this.fileIOProxy.FileExists(
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
            return new RootYAML(this.fileIOProxy, metaFilePath, acceptAbsence: true);
        }

        private RootYAML FetchWriteMetaYAML(Layer writeLayer)
        {
            var metaFilePath = AgnosticPath.FromAgnosticPathString(
                this.FetchWritePath(writeLayer).ToAgnosticPathString() + ".meta");
            return new RootYAML(this.fileIOProxy, metaFilePath, acceptAbsence: true);
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
