// -----------------------------------------------------------------------
// <copyright file="ISpawnerRoot1.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 1引数とSpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot and 1 arg.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Arg1">1st argument class for spawing.</typeparam>
    public interface ISpawnerRoot1<out T_Target, T_Arg1>
        : ISpawnerRoot<T_Target>
    {
        /// <summary>
        /// Gets dummy 1st argument for molding.
        /// </summary>
        T_Arg1 DummyArg1 { get; }

        /// <summary>
        /// Spawn target instance with SpecRoot instance.
        /// </summary>
        /// <param name="arg1">1st argument.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Arg1 arg1);

        /// <inheritdoc/>
        T_Target ISpawner<T_Target>.DummySpawn()
        {
            return this.Spawn(this.DummyArg1);
        }
    }
}
