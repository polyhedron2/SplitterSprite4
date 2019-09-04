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
                string path, Func<StreamReader, Result> func);
        }

        private class RealFileIOProxy : FileIOProxy
        {
            string rootPath =
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string FullPath(string path)
            {
                return Path.Combine(rootPath, path);
            }

            public override Result WithReader<Result>(
                string path, Func<StreamReader, Result> func)
            {
                using (var reader =
                    new StreamReader(FullPath(path), Encoding.UTF8))
                {
                    return func(reader);
                }
            }
        }
    }
}
