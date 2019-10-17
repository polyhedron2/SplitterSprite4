// -----------------------------------------------------------------------
// <copyright file="OutSideProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;

    /// <summary>
    /// ゲームの実行ファイル外からの全ての入力
    /// （プレイヤー操作、ファイル、時刻など）を管理するプロキシクラス
    /// The proxy class for any input from outside of the executable file
    /// (e.g. player's control, files, time, etc).
    /// </summary>
    public class OutSideProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutSideProxy"/> class.
        /// </summary>
        /// <param name="fileIO">A FileIOProxy instance.</param>
        public OutSideProxy(FileIOProxy fileIO)
        {
            this.FileIO = fileIO;
        }

        /// <summary>
        /// Gets proxy for configuration (spec) files, media files,
        /// and program files.
        /// </summary>
        public FileIOProxy FileIO { get; }
    }
}