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
        {
            this.Proxy = proxy;
            this.LayeredFile = new LayeredFile(proxy, layeredPath, acceptAbsence);

            AgnosticPath yamlPath;
            try
            {
                yamlPath = this.LayeredFile.FetchReadPath();
            }
            catch (LayeredFile.LayeredFileNotFoundException)
            {
                yamlPath = this.LayeredFile.FetchWritePath(
                    new Layer(proxy, "save", true));
            }

            this.Body = new RootYAML(proxy, yamlPath, acceptAbsence);
        }

        private LayeredFile LayeredFile { get; }

        /// <summary>
        /// Save the spec's values into the save layer.
        /// </summary>
        public void Save()
        {
            var saveLayer = new Layer(this.Proxy, "save", true);
            saveLayer.IsTop = true;
            saveLayer.Save();
            this.Save(saveLayer);
        }

        /// <summary>
        /// Save the spec's values into the specified layer.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        public void Save(Layer layer)
        {
            this.Save(layer, this.LayeredFile.Path);
        }

        /// <summary>
        /// Save the spec's values into the specified layer and path.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        /// <param name="layeredPath">Layered path to save the spec.</param>
        public void Save(Layer layer, AgnosticPath layeredPath)
        {
            var writePath = new LayeredFile(
                this.Proxy, layeredPath, true).FetchWritePath(layer);

            this.Proxy.FileIO.CreateDirectory(writePath.Parent);
            (this.Body as RootYAML).SaveAs(writePath, true);
        }
    }
}
