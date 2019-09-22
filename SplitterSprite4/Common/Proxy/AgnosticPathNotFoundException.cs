// -----------------------------------------------------------------------
// <copyright file="AgnosticPathNotFoundException.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;

    /// <summary>
    /// OS非依存パスに対応するファイルパス上にデータが見つからない際の例外
    /// The exception that is thrown when an attempt to
    /// access a file that does not exist on the correcponding path fails.
    /// </summary>
    public class AgnosticPathNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgnosticPathNotFoundException"/> class.
        /// </summary>
        /// <param name="path">The os-agnostic path.</param>
        public AgnosticPathNotFoundException(AgnosticPath path)
            : base($"ファイル\"{path.ToOSPathString()}\"が見つかりません")
        {
            this.Path = path;
        }

        /// <summary>
        /// Gets the os-agnostic path.
        /// </summary>
        public AgnosticPath Path { get; private set; }
    }
}
