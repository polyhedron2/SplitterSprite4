using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    class OutSideProxy
    {
        public static FileIOProxy FileIO { get; private set; } =
            new RealFileIOProxy();
    }
}