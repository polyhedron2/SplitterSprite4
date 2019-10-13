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
    internal class RealFileIOProxy : FileIOProxy
    {
        /// <inheritdoc/>
        public override void CreateDirectory(AgnosticPath path)
        {
            Directory.CreateDirectory(this.OSFullPath(path));
        }

        /// <inheritdoc/>
        public override IEnumerable<AgnosticPath>
            EnumerateDirectories(AgnosticPath path)
        {
            var basePath = this.OSFullPath(path);
            AgnosticPath SubDirPath(string fullPath)
            {
                // + 1 is for separator length.
                var subDirName = fullPath.Substring(basePath.Length + 1);
                return AgnosticPath.FromOSPathString(subDirName);
            }

            return Directory.EnumerateDirectories(
                basePath).Select(subDirStr => SubDirPath(subDirStr));
        }

        /// <inheritdoc/>
        protected override TextReader FetchTextReader(AgnosticPath path)
        {
            if (!File.Exists(this.OSFullPath(path)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamReader(this.OSFullPath(path), Encoding.UTF8);
        }

        /// <inheritdoc/>
        protected override TextWriter FetchTextWriter(
            AgnosticPath path, bool append)
        {
            if (!Directory.Exists(this.OSFullDirPath(path)))
            {
                throw new AgnosticPathNotFoundException(path);
            }

            return new StreamWriter(
                this.OSFullPath(path), append, Encoding.UTF8);
        }
    }
}
