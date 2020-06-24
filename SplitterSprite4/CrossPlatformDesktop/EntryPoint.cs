// -----------------------------------------------------------------------
// <copyright file="EntryPoint.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.CrossPlatformDesktop
{
    using System;
    using MagicKitchen.SplitterSprite4.Framework;

    /// <summary>
    /// The main class.
    /// </summary>
    public static class EntryPoint
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
