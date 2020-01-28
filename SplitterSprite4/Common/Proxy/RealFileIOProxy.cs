// -----------------------------------------------------------------------
// <copyright file="RealFileIOProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 実際のゲーム中のファイルアクセスを管理するプロキシクラス
    /// FileIOProxy for actual file access.
    /// </summary>
    public class RealFileIOProxy : FileIOProxy
    {
        /// <inheritdoc/>
        public override void CreateDirectory(AgnosticPath path)
        {
            Directory.CreateDirectory(this.ToOSFullPath(path));
        }

        /// <inheritdoc/>
        public override IEnumerable<AgnosticPath>
            EnumerateDirectories(AgnosticPath path)
        {
            var basePath = this.ToOSFullPath(path);
            return Directory.EnumerateDirectories(
                basePath).Select(x => this.FromOSFullPath(x) - path);
        }

        /// <inheritdoc/>
        public override IEnumerable<AgnosticPath>
            EnumerateFiles(AgnosticPath path)
        {
            var basePath = this.ToOSFullPath(path);
            return Directory.EnumerateFiles(
                basePath).Select(x => this.FromOSFullPath(x) - path);
        }

        /// <inheritdoc/>
        public override bool FileExists(AgnosticPath path)
        {
            return File.Exists(this.ToOSFullPath(path));
        }

        /// <inheritdoc/>
        public override bool DirExists(AgnosticPath path)
        {
            return Directory.Exists(this.ToOSFullPath(path));
        }

        /// <inheritdoc/>
        protected override TextReader FetchTextReader(AgnosticPath path)
        {
            if (!File.Exists(this.ToOSFullPath(path)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamReader(this.ToOSFullPath(path), Encoding.UTF8);
        }

        /// <inheritdoc/>
        protected override TextWriter FetchTextWriter(
            AgnosticPath path, bool append)
        {
            if (!Directory.Exists(this.ToOSFullPath(path.Parent)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamWriter(
                this.ToOSFullPath(path), append, Encoding.UTF8);
        }
    }
}
