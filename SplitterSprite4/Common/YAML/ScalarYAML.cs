using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    class ScalarYAML : YAML
    {
        public ScalarYAML(string value) { Value = value; }
        public ScalarYAML(YamlScalarNode scalar)
        {
            Value = scalar.Value;
        }

        private string Value { get; set; }

        public override IEnumerable<string> ToStringLines()
        {
            return new List<string>() { $"{Value}" };
        }
    }
}
