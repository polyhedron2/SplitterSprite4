// -----------------------------------------------------------------------
// <copyright file="Launcher.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
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
        /// <param name="proxy">The OutSideProxy for file access.</param>
        public Launcher(OutSideProxy proxy)
        {
            var yaml = new RootYAML(proxy.FileIO, "launcher.meta");
            var entryPoint = yaml["entry_point"].ToString();
        }
    }
}