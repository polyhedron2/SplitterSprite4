using System;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    class RootYAML : MappingYAML
    {
        public RootYAML(AgnosticPath path) : base(ReadFile(path))
        {
            AccessPath = path;
            ID = path.ToAgnosticPathString();
        }

        AgnosticPath AccessPath { get; set; }

        public void Reload() => Initialize(ReadFile(AccessPath));

        static YamlMappingNode ReadFile(AgnosticPath path)
        {
            return OutSideProxy.FileIO.WithReader(path, (reader) =>
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);
                return (YamlMappingNode)yamlStream.Documents[0].RootNode;
            });
        }
    }
}
