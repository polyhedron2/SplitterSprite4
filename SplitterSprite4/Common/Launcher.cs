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

        private IEnumerable<Layer> Layers { get; set; }

        private IEnumerable<Layer> LoadLayers()
        {
            foreach (var dir in Proxy.OutSideProxy.FileIO.EnumerateDirectories(
                AgnosticPath.FromAgnosticPathString(string.Empty)))
            {
                var name = dir.ToString();
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

            IEnumerable<Layer> Visit(Layer layer)
            {
                if (visiting.Contains(layer.Name))
                {
                    throw new CyclicDependencyException(layer);
                }

                if (!visited.Contains(layer.Name))
                {
                    visiting.Add(layer.Name);

                    var ret = layer.Dependencies.SelectMany(
                        dependent => Visit(new Layer(dependent))).Concat(
                        new Layer[1] { layer });

                    visiting.Remove(layer.Name);
                    visited.Add(layer.Name);

                    return ret;
                }
                else
                {
                    return new Layer[0];
                }
            }

            return layers.SelectMany(layer => Visit(layer));
        }

        /// <summary>
        /// レイヤーの依存関係が循環している際の例外
        /// The exception that is thrown when an attempt to
        /// load layers that contains cyclic dependencies.
        /// </summary>
        internal class CyclicDependencyException : Exception
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