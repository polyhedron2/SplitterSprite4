// -----------------------------------------------------------------------
// <copyright file="RootYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using YamlDotNet.RepresentationModel;

    /// <summary>
    /// YAMLファイル実体を表現するクラス
    /// The YAML class for a YAML file.
    /// </summary>
    public class RootYAML : MappingYAML
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootYAML"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file access.</param>
        /// <param name="path">The os-agnostic path.</param>
        public RootYAML(Proxy.OutSideProxy proxy, AgnosticPath path)

            // ファイルパスをIDとする
            // ID is the os-agnostic path.
            : base(path.ToAgnosticPathString(), ReadFile(proxy, path))
        {
            this.AccessPath = path;
            this.Proxy = proxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RootYAML"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file access.</param>
        /// <param name="agnosticPathStr">The os-agnostic path string.</param>
        public RootYAML(Proxy.OutSideProxy proxy, string agnosticPathStr)
            : this(proxy, AgnosticPath.FromAgnosticPathString(agnosticPathStr))
        {
        }

        // YAMLファイルの存在するパス
        // The YAML file path.
        private AgnosticPath AccessPath { get; set; }

        private Proxy.OutSideProxy Proxy { get; }

        /// <summary>
        /// ファイル保存を実行
        /// Save the YAML body.
        /// </summary>
        public void Save()
        {
            this.Proxy.FileIO.WithTextWriter(
                this.AccessPath, false, (writer) =>
                {
                    writer.Write(this.ToString());
                });
        }

        private static YamlMappingNode ReadFile(
            Proxy.OutSideProxy proxy, AgnosticPath path)
        {
            var yamlStream = new YamlStream();
            proxy.FileIO.WithTextReader(path, (reader) =>
            {
                yamlStream.Load(reader);
            });
            return (YamlMappingNode)yamlStream.Documents[0].RootNode;
        }
    }
}
