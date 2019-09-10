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
            if (!File.Exists(OSFullPath(path)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamReader(OSFullPath(path), Encoding.UTF8);
        }

        protected override TextWriter FetchTextWriter(
            AgnosticPath path, bool append)
        {
            if (!Directory.Exists(OSFullDirPath(path)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamWriter(OSFullPath(path), append, Encoding.UTF8);
        }

        public override void CreateDirectory(AgnosticPath path)
        {
            Directory.CreateDirectory(OSFullDirPath(path));
        }
    }
}
