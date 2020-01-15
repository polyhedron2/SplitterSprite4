// -----------------------------------------------------------------------
// <copyright file="RootYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;
    using System.IO;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
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
        /// <param name="fileIOProxy">The FileIOProxy for file access.</param>
        /// <param name="path">The os-agnostic path.</param>
        /// <param name="acceptAbsence">Accept absence of the yaml file.</param>
        public RootYAML(
                FileIOProxy fileIOProxy,
                AgnosticPath path,
                bool acceptAbsence = false)

            // ファイルパスをIDとする
            // ID is the os-agnostic path.
            : base(
                  path.ToAgnosticPathString(),
                  ReadFile(fileIOProxy, path, acceptAbsence))
        {
            this.AccessPath = path;
            this.FileIOProxy = fileIOProxy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RootYAML"/> class.
        /// </summary>
        /// <param name="fileIOProxy">The FileIOProxy for file access.</param>
        /// <param name="agnosticPathStr">The os-agnostic path string.</param>
        /// <param name="acceptAbsence">Accept absence of the yaml file.</param>
        public RootYAML(
                FileIOProxy fileIOProxy,
                string agnosticPathStr,
                bool acceptAbsence = false)
            : this(
                  fileIOProxy,
                  AgnosticPath.FromAgnosticPathString(agnosticPathStr),
                  acceptAbsence)
        {
        }

        // YAMLファイルの存在するパス
        // The YAML file path.
        private AgnosticPath AccessPath { get; set; }

        private FileIOProxy FileIOProxy { get; }

        /// <summary>
        /// ファイル上書き保存を実行
        /// Overwrite the YAML body.
        /// </summary>
        /// <param name="ignoreEmptyMappingChild">Ignore MappingYAML's child if it's empty collection.</param>
        public void Overwrite(bool ignoreEmptyMappingChild = false)
        {
            this.SaveAs(this.AccessPath, ignoreEmptyMappingChild);
        }

        /// <summary>
        /// ファイル保存を指定のパスに実行
        /// Save the YAML body to the path.
        /// </summary>
        /// <param name="savePath">The save path.</param>
        /// <param name="ignoreEmptyMappingChild">Ignore MappingYAML's child if it's empty collection.</param>
        public void SaveAs(AgnosticPath savePath, bool ignoreEmptyMappingChild = false)
        {
            this.FileIOProxy.CreateDirectory(savePath.Parent);
            this.FileIOProxy.WithTextWriter(
                savePath, false, (writer) =>
                {
                    writer.Write(this.ToString(ignoreEmptyMappingChild));
                });
        }

        private static YamlMappingNode ReadFile(
            FileIOProxy fileIOProxy, AgnosticPath path, bool acceptEmpty)
        {
            var yamlStream = new YamlStream();
            try
            {
                fileIOProxy.WithTextReader(path, (reader) =>
                {
                    yamlStream.Load(reader);
                });
            }
            catch (Exception ex)
            {
                if (acceptEmpty)
                {
                    using (StringReader reader = new StringReader("{}"))
                    {
                        yamlStream.Load(reader);
                    }
                }
                else
                {
                    throw ex;
                }
            }

            return (YamlMappingNode)yamlStream.Documents[0].RootNode;
        }
    }
}
