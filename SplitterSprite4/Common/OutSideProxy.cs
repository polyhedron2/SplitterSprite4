using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common
{
    class OutSideProxy
    {
        public static FileIOProxy FileIO { get; private set; } =
            new RealFileIOProxy();

        public abstract class FileIOProxy
        {
            public abstract Result WithReader<Result>(
                AgnosticPath path, Func<StreamReader, Result> func);
        }

        private class RealFileIOProxy : FileIOProxy
        {
            public override Result WithReader<Result>(
                AgnosticPath path, Func<StreamReader, Result> func)
            {
                using (var reader = new StreamReader(
                    path.ToOSFullPathString(), Encoding.UTF8))
                {
                    return func(reader);
                }
            }
        }
    }
}
