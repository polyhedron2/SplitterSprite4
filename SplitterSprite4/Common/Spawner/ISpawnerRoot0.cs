﻿// -----------------------------------------------------------------------
// <copyright file="ISpawnerRoot0.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 0引数とSpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot and 0 args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawnerRoot0<out T_Target> : ISpawnerRoot
    {
        /// <summary>
        /// Spawn target instance with SpecRoot instance.
        /// </summary>
        /// <returns>Spawn target.</returns>
        T_Target Spawn();
    }
}
