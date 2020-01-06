﻿// -----------------------------------------------------------------------
// <copyright file="ISpawnerChild1.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 1引数とSpecChildからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecChild and 1 arg.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Arg1">1st argument class for spawing.</typeparam>
    public interface ISpawnerChild1<out T_Target, T_Arg1> : ISpawnerChild
    {
        /// <summary>
        /// Gets dummy 1st argument for molding.
        /// </summary>
        T_Arg1 DummyArg1 { get; }

        /// <summary>
        /// Spawn target instance with SpecChild instance.
        /// </summary>
        /// <param name="arg1">1st argument.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Arg1 arg1);
    }
}
