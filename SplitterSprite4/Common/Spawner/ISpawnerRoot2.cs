// -----------------------------------------------------------------------
// <copyright file="ISpawnerRoot2.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 2引数とSpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot and 2 args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Arg1">1st argument class for spawing.</typeparam>
    /// <typeparam name="T_Arg2">2nd argument class for spawing.</typeparam>
    public interface ISpawnerRoot2<out T_Target, T_Arg1, T_Arg2> : ISpawnerRoot
    {
        /// <summary>
        /// Gets dummy 1st argument for molding.
        /// </summary>
        T_Arg1 DummyArg1 { get; }

        /// <summary>
        /// Gets dummy 2nd argument for molding.
        /// </summary>
        T_Arg2 DummyArg2 { get; }

        /// <summary>
        /// Spawn target instance with SpecRoot instance.
        /// </summary>
        /// <param name="arg1">1st argument.</param>
        /// <param name="arg2">2nd argument.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Arg1 arg1, T_Arg2 arg2);
    }
}
