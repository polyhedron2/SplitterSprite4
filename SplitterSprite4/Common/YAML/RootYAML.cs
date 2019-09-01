using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    class RootYAML : MappingYAML
    {
        public RootYAML(string pathFromEntryAssembly) :
            base(ReadFile(pathFromEntryAssembly))
        {
            AccessPath = pathFromEntryAssembly;
        }

        public void Reload() => Initialize(ReadFile(AccessPath));

        static YamlMappingNode ReadFile(string pathFromEntryAssembly)
        {
            var rootPath =
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(rootPath, pathFromEntryAssembly);

            using (var reader = new StreamReader(fullPath, Encoding.UTF8))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(reader);
                return (YamlMappingNode)yamlStream.Documents[0].RootNode;
            }
        }
    }
}
