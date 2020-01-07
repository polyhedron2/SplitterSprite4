// -----------------------------------------------------------------------
// <copyright file="ISpawnerChild0.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 0引数とSpecChildからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecChild and 0 args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawnerChild0<out T_Target> : ISpawnerChild<T_Target>
    {
        /// <summary>
        /// Spawn target instance with SpecChild instance.
        /// </summary>
        /// <returns>Spawn target.</returns>
        T_Target Spawn();

        /// <inheritdoc/>
        T_Target ISpawner<T_Target>.DummySpawn()
        {
            return this.Spawn();
        }
    }
}
