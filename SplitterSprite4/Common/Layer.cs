// -----------------------------------------------------------------------
// <copyright file="Layer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲーム素材の重ね合わせレイヤークラス
    /// The layer class for game parts.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file access.</param>
        /// <param name="name">The layer name.</param>
        public Layer(Proxy.OutSideProxy proxy, string name)
        {
            this.Name = name;
            var yaml = new RootYAML(proxy, $"{name}/layer.meta");
            this.Dependencies = yaml.Sequence["dependencies"].Select(
                child => child.ToString());
        }

        /// <summary>
        /// Gets the layer name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the layer's dependencies.
        /// </summary>
        public IEnumerable<string> Dependencies { get; }

        /// <summary>
        /// 依存関係でトポロジカルソートしたレイヤー群を取得。
        /// Fetch the sorted layers which sorted topologically
        /// by dependency relation.
        /// </summary>
        /// <param name="proxy">The OutSideProxy.</param>
        /// <returns>The sorted layers.</returns>
        public static IEnumerable<Layer> FetchSortedLayers(
            Proxy.OutSideProxy proxy)
        {
            return SortLayers(proxy, LoadLayers(proxy));
        }

        private static IEnumerable<Layer> LoadLayers(Proxy.OutSideProxy proxy)
        {
            foreach (var dir in proxy.FileIO.EnumerateDirectories(
                AgnosticPath.FromAgnosticPathString(string.Empty)))
            {
                var name = dir.ToAgnosticPathString();
                Layer layer;

                try
                {
                    layer = new Layer(proxy, name);
                }
                catch (FileIOProxy.AgnosticPathNotFoundException)
                {
                    continue;
                }

                yield return layer;
            }
        }

        private static IEnumerable<Layer> SortLayers(
            Proxy.OutSideProxy proxy, IEnumerable<Layer> layers)
        {
            var visiting = new HashSet<string>();
            var visited = new HashSet<string>();

            Layer FetchDependeeLayer(Layer dependerLayer, string dependeeName)
            {
                try
                {
                    return new Layer(proxy, dependeeName);
                }
                catch (FileIOProxy.AgnosticPathNotFoundException ex)
                {
                    throw new NonExistentLayerException(dependerLayer, ex);
                }
            }

            IEnumerable<Layer> Visit(Layer layer)
            {
                if (visiting.Contains(layer.Name))
                {
                    throw new CyclicDependencyException(layer);
                }

                if (!visited.Contains(layer.Name))
                {
                    visiting.Add(layer.Name);

                    foreach (var dependeeName in layer.Dependencies)
                    {
                        foreach (var l in
                            Visit(FetchDependeeLayer(layer, dependeeName)))
                        {
                            yield return l;
                        }
                    }

                    yield return layer;

                    visiting.Remove(layer.Name);
                    visited.Add(layer.Name);
                }
            }

            // 依存先から依存元の順になっているので、逆転する
            // Reverse to sort by "from depender to dependee" order.
            return layers.SelectMany(layer => Visit(layer)).Reverse();
        }

        /// <summary>
        /// レイヤーの依存先が存在していない場合の例外
        /// The exception that is thrown when an attempt to
        /// a layer depends on a non-existent layer.
        /// </summary>
        public class NonExistentLayerException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NonExistentLayerException"/> class.
            /// </summary>
            /// <param name="layer">The depending layer.</param>
            /// <param name="cause">The AgnosticPathNotFoundException.</param>
            internal NonExistentLayerException(
                Layer layer, FileIOProxy.AgnosticPathNotFoundException cause)
                : base(
                      $"{layer.Name}の依存先{cause.Path.Parent}は" +
                      "有効なレイヤーとして存在していません。",
                      cause)
            {
            }
        }

        /// <summary>
        /// レイヤーの依存関係が循環している際の例外
        /// The exception that is thrown when an attempt to
        /// load layers that contains cyclic dependencies.
        /// </summary>
        public class CyclicDependencyException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CyclicDependencyException"/> class.
            /// </summary>
            /// <param name="layer">The layer that contains cyclic dependency.</param>
            internal CyclicDependencyException(Layer layer)
                : base($"\"{layer.Name}\"レイヤーを含む" +
                       $"循環参照的な依存関係が存在します。")
            {
            }
        }
    }
}
