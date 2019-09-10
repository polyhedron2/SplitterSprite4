using System;
using System.Collections.Generic;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    public class AgnosticPathNotFoundException : Exception
    {
        public AgnosticPathNotFoundException(AgnosticPath path) :
            base($"ファイル\"{path.ToOSPathString()}\"が見つかりません")
        {
            Path = path;
        }

        public AgnosticPath Path { get; private set; }
    }
}
