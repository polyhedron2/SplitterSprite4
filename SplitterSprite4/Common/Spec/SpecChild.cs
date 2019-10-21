// -----------------------------------------------------------------------
// <copyright file="SpecChild.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Specファイルの子孫Specを表現するSpecクラス
    /// The accessor class for descendant spec.
    /// </summary>
    public class SpecChild : Spec
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecChild"/> class.
        /// </summary>
        /// <param name="proxy">The OusSideProxy for file access.</param>
        /// <param name="body">The YAML.</param>
        internal SpecChild(OutSideProxy proxy, YAML body)
        {
            this.Proxy = proxy;
            this.Body = body;
        }
    }
}
