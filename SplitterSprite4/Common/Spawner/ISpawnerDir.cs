// -----------------------------------------------------------------------
// <copyright file="ISpawnerDir.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using System.Collections.Generic;

    /// <summary>
    /// 特定のディレクトリ内のSpawnerRoot一覧を取得するインターフェース。
    /// Interface which enumerates SpawnerRoot instances in a directory.
    /// </summary>
    /// <typeparam name="T_Spawner">Target SpawnerRoot class.</typeparam>
    public interface ISpawnerDir<out T_Spawner> : IEnumerable<T_Spawner>
        where T_Spawner : ISpawnerRoot<object>
    {
        /// <summary>
        /// Gets layered directory.
        /// </summary>
        LayeredDir Dir { get; }
    }
}
