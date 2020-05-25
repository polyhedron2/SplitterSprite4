// -----------------------------------------------------------------------
// <copyright file="ISpawnerChildWithoutArgs.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 引数なしでSpecChildからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecChild and no args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawnerChildWithoutArgs<out T_Target>
        : ISpawnerChild<T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Spawn target instance with SpecChild instance.
        /// </summary>
        /// <returns>Spawn target.</returns>
        T_Target Spawn();
    }
}
