// -----------------------------------------------------------------------
// <copyright file="Layer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System.Collections.Generic;
    using System.Linq;
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
    }
}
