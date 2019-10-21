// -----------------------------------------------------------------------
// <copyright file="SpecRoot.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Specファイル本体を表現するSpecクラス
    /// The accessor class for spec file.
    /// </summary>
    public class SpecRoot : Spec
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecRoot"/> class.
        /// </summary>
        /// <param name="proxy">The OusSideProxy for file access.</param>
        /// <param name="path">The spec file path.</param>
        internal SpecRoot(OutSideProxy proxy, AgnosticPath path)
        {
            this.Proxy = proxy;
            this.Body = new RootYAML(proxy, path);
        }
    }
}
