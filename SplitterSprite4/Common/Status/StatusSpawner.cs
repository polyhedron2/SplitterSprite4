// -----------------------------------------------------------------------
// <copyright file="StatusSpawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Status
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// ゲーム内の状態変数管理クラスIStatus用Spawner。
    /// Spawner for IStatus.
    /// </summary>
    /// <typeparam name="T_Status">Spawn target class.</typeparam>
    public abstract class StatusSpawner<T_Status> : ISpawnerRootWithoutArgs<T_Status>
        where T_Status : class, IStatus
    {
        public SpecRoot Spec { get; set; }

        public abstract string Note { get; }

        /// <inheritdoc/>
        public T_Status Spawn()
        {
            return this.Spec.Proxy.Singleton(
                $"{this.GetType()}.Spawn",
                this.Spec.ID,
                this.SpawnStatus);
        }

        /// <summary>
        /// 状態変数管理クラスを作成する。
        /// 状態管理クラスをシングルトンパターンとするため、
        /// このメソッドはSpawnメソッドによってのみ呼び出されるべきである。
        /// Spawn status manager instance.
        /// This method should be called only by Spawn method,
        /// because status manager instance should be singleton.
        /// </summary>
        /// <returns>Status manager instance.</returns>
        protected abstract T_Status SpawnStatus();
    }
}
