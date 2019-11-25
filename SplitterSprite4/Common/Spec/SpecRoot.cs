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
        /// <param name="layeredPath">The spec file path.</param>
        /// <param name="acceptAbsence">Accept absence of the spec file or not.</param>
        public SpecRoot(
                OutSideProxy proxy,
                AgnosticPath layeredPath,
                bool acceptAbsence = false)
            : base(
                  proxy,
                  new RootYAML(
                      proxy,
                      new LayeredFile(proxy, layeredPath).FetchReadPath(),
                      acceptAbsence))
        {
            this.LayeredFile = new LayeredFile(proxy, layeredPath);
        }

        private LayeredFile LayeredFile { get; }

        /// <summary>
        /// Save the spec's values into the save layer.
        /// </summary>
        public void Save()
        {
            (this.Body as RootYAML).SaveAs(
                this.LayeredFile.FetchWritePath());
        }

        /// <summary>
        /// Save the spec's values into the specified layer.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        public void Save(Layer layer)
        {
            (this.Body as RootYAML).SaveAs(
                this.LayeredFile.FetchWritePath(layer));
        }

        /// <summary>
        /// Save the spec's values into the specified layer and path.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        /// <param name="path">Layered path to save the spec.</param>
        public void Save(Layer layer, AgnosticPath path)
        {
            (this.Body as RootYAML).SaveAs(
                new LayeredFile(this.Proxy, path).FetchWritePath(layer));
        }
    }
}
