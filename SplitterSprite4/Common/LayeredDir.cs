// -----------------------------------------------------------------------
// <copyright file="LayeredDir.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;

    /// <summary>
    /// レイヤーによるディレクトリの重ね合わせ結果を表現するクラス
    /// Direcotry class as results of layers sort.
    /// </summary>
    public class LayeredDir
    {
        private FileIOProxy fileIOProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredDir"/> class.
        /// </summary>
        /// <param name="fileIOProxy">The FileIOProxy for file access.</param>
        /// <param name="path">The relative path from layer directory.</param>
        public LayeredDir(FileIOProxy fileIOProxy, AgnosticPath path)
        {
            if (path.ToAgnosticPathString().StartsWith("../"))
            {
                throw new Layer.OutOfLayerAccessException(path);
            }

            this.fileIOProxy = fileIOProxy;
            this.Path = path;
        }

        /// <summary>
        /// Gets the relative path from layer directory.
        /// </summary>
        public AgnosticPath Path { get; }

        /// <summary>
        /// 各レイヤーの同一パスディレクトリ内の
        /// サブディレクトリ一覧の和集合を返す。
        /// Enumarate union set of sub direcotries
        /// in this directory for each layers.
        /// </summary>
        /// <returns>Sub directory enumerator.</returns>
        public IEnumerable<AgnosticPath> EnumerateDirectories()
        {
            return Layer.FetchSortedLayers(this.fileIOProxy).Select(
                (layer) => this.Path + layer.Path).Where(
                (path) => this.fileIOProxy.DirExists(path)).Select(
                (path) => this.fileIOProxy.EnumerateDirectories(path)).Aggregate(
                (dirs1, dirs2) => dirs1.Union(dirs2));
        }

        /// <summary>
        /// 各レイヤーの同一パスディレクトリ内の
        /// ファイル一覧の和集合を返す。
        /// Enumarate union set of files in this directory for each layers.
        /// </summary>
        /// <returns>Sub directory enumerator.</returns>
        public IEnumerable<AgnosticPath> EnumerateFiles()
        {
            return Layer.FetchSortedLayers(this.fileIOProxy).Select(
                (layer) => this.Path + layer.Path).Where(
                (path) => this.fileIOProxy.DirExists(path)).Select(
                (path) => this.fileIOProxy.EnumerateFiles(path)).Aggregate(
                Enumerable.Empty<AgnosticPath>(),
                (files1, files2) => files1.Union(files2));
        }
    }
}
