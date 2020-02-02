// -----------------------------------------------------------------------
// <copyright file="OutSideProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ゲームの実行ファイル外からの全ての入力
    /// （プレイヤー操作、ファイル、時刻など）を管理するプロキシクラス
    /// The proxy class for any input from outside of the executable file
    /// (e.g. player's control, files, time, etc).
    /// </summary>
    public class OutSideProxy
    {
        private Dictionary<(object, object), object> singletonPool =
            new Dictionary<(object, object), object>();

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
        /// Proxyごとのシングルトンパターンを提供。
        /// Provide singleton pattern for each proxy.
        /// </summary>
        /// <typeparam name="T">Expected return value type.</typeparam>
        /// <param name="funcID">ID object for function.</param>
        /// <param name="argsID">ID object for arguments.</param>
        /// <param name="func">The function which create singleton object.</param>
        /// <returns>Singleton object.</returns>
        public T Singleton<T>(object funcID, object argsID, Func<T> func)
        {
            lock (this.singletonPool)
            {
                var key = (funcID, argsID);
                if (this.singletonPool.ContainsKey(key))
                {
                    return (T)this.singletonPool[key];
                }
                else
                {
                    var ret = func();
                    this.singletonPool[key] = ret;
                    return ret;
                }
            }
        }
    }
}