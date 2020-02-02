// -----------------------------------------------------------------------
// <copyright file="IStatusSpawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Status
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// ゲーム内の状態変数管理クラスIStatus用Spawner。
    /// Spawner for IStatus.
    /// </summary>
    /// <typeparam name="T_Status">Spawn target class.</typeparam>
    public interface IStatusSpawner<out T_Status> : ISpawnerRoot0<T_Status>
        where T_Status : class, IStatus
    {
        /// <summary>
        /// 状態変数管理クラスを作成する。
        /// 状態管理クラスをシングルトンパターンとするため、
        /// このメソッドはSpawnメソッドによってのみ呼び出されるべきである。
        /// Spawn status manager instance.
        /// This method should be called only by Spawn method,
        /// because status manager instance should be singleton.
        /// </summary>
        /// <returns>Status manager instance.</returns>
        T_Status SpawnStatus();

        /// <inheritdoc/>
        T_Status ISpawnerRoot0<T_Status>.Spawn()
        {
            return this.Spec.Proxy.Singleton(
                $"{this.GetType()}.Spawn",
                this.Spec.ID,
                this.SpawnStatus);
        }
    }
}
