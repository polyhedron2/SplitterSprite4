using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    class RealFileIOProxy : FileIOProxy
    {
        protected override TextReader FetchTextReader(AgnosticPath path)
        {
            if (!File.Exists(path.ToOSFullPathString()))
            {
                throw new FileNotFoundException(path);
            }

            return new StreamReader(
                path.ToOSFullPathString(), Encoding.UTF8);
        }

        protected override TextWriter FetchTextWriter(
            AgnosticPath path, bool append)
        {
            if (!File.Exists(path.ToOSFullDirPathString()))
            {
                throw new FileNotFoundException(path);
            }

            return new StreamWriter(
                path.ToOSFullPathString(), append, Encoding.UTF8);
        }

        protected override void CreateDirectory(AgnosticPath path)
        {
            Directory.CreateDirectory(path.ToOSFullDirPathString());
        }
    }
}
