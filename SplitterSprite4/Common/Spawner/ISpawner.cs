// -----------------------------------------------------------------------
// <copyright file="ISpawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// Specからインスタンスを生成するインターフェース。
    /// Instance spawner interface with Spec.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawner<out T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Gets explanation note for this spawner class.
        /// </summary>
        string Note { get; }

        /// <summary>
        /// Spawn target with dummy args.
        /// </summary>
        /// <returns>Spawn target.</returns>
        T_Target DummySpawn();
    }
}
