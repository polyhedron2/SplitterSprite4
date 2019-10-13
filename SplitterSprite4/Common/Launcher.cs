// -----------------------------------------------------------------------
// <copyright file="Launcher.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// ゲームを起動するクラス
    /// The launcher for the game.
    /// </summary>
    public class Launcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Launcher"/> class.
        /// </summary>
        public Launcher()
        {
            var yaml = new RootYAML("launcher.meta");
            var entryPoint = yaml["entry_point"].ToString();

            /* レイヤーを依存関係順にトポロジカルソートする。
             * Sort layers topologically by dependency relation.
             */
            this.Layers = this.SortLayers(this.LoadLayers());
        }

        /// <summary>
        /// Gets the sorted layers.
        /// </summary>
        public IEnumerable<Layer> Layers { get; }

        private IEnumerable<Layer> LoadLayers()
        {
            foreach (var dir in Proxy.OutSideProxy.FileIO.EnumerateDirectories(
                AgnosticPath.FromAgnosticPathString(string.Empty)))
            {
                var name = dir.ToAgnosticPathString();
                Layer layer;

                try
                {
                    layer = new Layer(name);
                }
                catch (Proxy.AgnosticPathNotFoundException)
                {
                    continue;
                }

                yield return layer;
            }
        }

        private IEnumerable<Layer> SortLayers(IEnumerable<Layer> layers)
        {
            var visiting = new HashSet<string>();
            var visited = new HashSet<string>();

            Layer FetchDependeeLayer(Layer dependerLayer, string dependeeName)
            {
                try
                {
                    return new Layer(dependeeName);
                }
                catch (Proxy.AgnosticPathNotFoundException ex)
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
                Layer layer, Proxy.AgnosticPathNotFoundException cause)
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