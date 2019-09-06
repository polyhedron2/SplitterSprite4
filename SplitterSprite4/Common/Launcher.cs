using System;
using System.Collections.Generic;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common
{
    public class Launcher
    {
        static List<Layer> _layers;
        internal static List<Layer> Layers
        {
            get => _layers;
        }

        public static string Sample()
        {
            var yaml = new YAML.RootYAML(
                AgnosticPath.FromAgnosticPathString("launcher.meta"));
            return yaml.ToString();
        }
    }
}
