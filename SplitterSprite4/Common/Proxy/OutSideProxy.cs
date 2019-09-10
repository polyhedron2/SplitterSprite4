using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    public class OutSideProxy
    {
        public static FileIOProxy FileIO { get; private set; } =
            new RealFileIOProxy();

        public static void WithTestMode(Action action)
        {
            FileIO.WithTestMode(action);
        }
    }
}