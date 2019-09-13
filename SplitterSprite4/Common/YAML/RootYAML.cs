using System;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    // YAMLファイル実体に対応するオブジェクト
    public class RootYAML : MappingYAML
    {
        // YAMLファイルからMappingを取得
        static YamlMappingNode ReadFile(AgnosticPath path)
        {
            var yamlStream = new YamlStream();
            Proxy.OutSideProxy.FileIO.WithTextReader(path, (reader) =>
            {
                yamlStream.Load(reader);
            });
            return (YamlMappingNode) yamlStream.Documents[0].RootNode;
        }

        // ファイルパスをIDとして、ファイルの中身を読み込み
        public RootYAML(AgnosticPath path) :
            base(path.ToAgnosticPathString(), ReadFile(path))
        {
            AccessPath = path;
        }

        // ファイル保存
        public void Save()
        {
            Proxy.OutSideProxy.FileIO.WithTextWriter(
                AccessPath, false, (writer) =>
                {
                    writer.Write(ToString());
                });
        }

        // YAMLファイルの存在するパス
        AgnosticPath AccessPath { get; set; }
    }
}
