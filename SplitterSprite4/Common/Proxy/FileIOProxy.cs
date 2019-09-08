using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    public abstract class FileIOProxy
    {
        protected abstract TextReader FetchTextReader(AgnosticPath path);
        protected abstract TextWriter FetchTextWriter(
            AgnosticPath path, bool append);
        protected abstract void CreateDirectory(AgnosticPath path);

        public Result WithTextReader<Result>(
            AgnosticPath path, Func<TextReader, Result> func)
        {
            using (var reader = FetchTextReader(path))
            {
                return func(reader);
            }
        }

        public void WithTextReader(
            AgnosticPath path, Action<TextReader> action)
        {
            using (var reader = FetchTextReader(path))
            {
                action(reader);
            }
        }

        public Result WithTextWriter<Result>(
            AgnosticPath path, bool append, Func<TextWriter, Result> func)
        {
            using (var writer = FetchTextWriter(path, append))
            {
                return func(writer);
            }
        }

        public void WithTextWriter(
            AgnosticPath path, bool append, Action<TextWriter> action)
        {
            using (var writer = FetchTextWriter(path, append))
            {
                action(writer);
            }
        }

        public Result WithTextWriter<Result>(
            AgnosticPath path, Func<TextWriter, Result> func) =>
            WithTextWriter(path, false, func);

        public void WithTextWriter(
            AgnosticPath path, Action<TextWriter> action) =>
            WithTextWriter(path, false, action);
    }
}
