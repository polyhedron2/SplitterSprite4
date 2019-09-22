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
        /// Gets proxy for configuration (spec) files, media files,
        /// and program files.
        /// </summary>
        public static FileIOProxy FileIO { get; private set; } =
            new RealFileIOProxy();

        /// <summary>
        /// テスト実行中はRootPathをシステムのTEMPファイル領域に切り替える
        /// Switch the RootPath to temporary directory while testing.
        /// </summary>
        /// <param name="action">
        /// テスト実行するActionインスタンス
        /// The Action instance for testing.
        /// </param>
        public static void WithTestMode(Action action)
        {
            FileIO.WithTestMode(action);
        }
    }
}