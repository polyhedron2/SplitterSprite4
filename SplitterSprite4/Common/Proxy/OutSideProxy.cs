// -----------------------------------------------------------------------
// <copyright file="OutSideProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// ゲームの実行ファイル外からの全ての入力
    /// （プレイヤー操作、ファイル、時刻など）を管理するプロキシクラス
    /// The proxy class for any input from outside of the executable file
    /// (e.g. player's control, files, time, etc).
    /// </summary>
    public class OutSideProxy
    {
        private Dictionary<string, SpecRoot> specPool =
            new Dictionary<string, SpecRoot>();

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

        /// <summary>
        /// SpecRootインスタンスを取得する。
        /// 過去に取得したインスタンスと同一パスであれば、同一インスタンスを返す。
        /// これにより、同一パスのSpecRootインスタンスは唯一つ存在する。
        /// Fetch SpecRoot instance.
        /// If it is second fetching for the path, same instance will be returned.
        /// Therefore, SpecRoot instance with same path is unique.
        /// </summary>
        /// <param name="layeredPath">The spec file path.</param>
        /// <param name="acceptAbsence">Accept absence of the spec file or not.</param>
        /// <returns>The unique SpecRoot instance.</returns>
        public SpecRoot SpecPool(
                AgnosticPath layeredPath,
                bool acceptAbsence = false)
        {
            lock (this.specPool)
            {
                var poolKey = layeredPath.ToAgnosticPathString();
                if (this.specPool.ContainsKey(poolKey))
                {
                    return this.specPool[poolKey];
                }
                else
                {
                    var ret = new SpecRoot(this, layeredPath, acceptAbsence);
                    this.specPool[poolKey] = ret;
                    return ret;
                }
            }
        }
    }
}