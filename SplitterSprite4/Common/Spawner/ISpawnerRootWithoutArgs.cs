// -----------------------------------------------------------------------
// <copyright file="ISpawnerRootWithoutArgs.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 引数なしでSpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot and no args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawnerRootWithoutArgs<out T_Target>
        : ISpawnerRoot<T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Spawn target instance with SpecRoot instance.
        /// </summary>
        /// <returns>Spawn target.</returns>
        T_Target Spawn();
    }
}
