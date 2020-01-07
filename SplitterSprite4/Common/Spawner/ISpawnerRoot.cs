// -----------------------------------------------------------------------
// <copyright file="ISpawnerRoot.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// SpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    public interface ISpawnerRoot<out T_Target> : ISpawner<T_Target>
    {
        /// <summary>
        /// Gets or sets spec instance for spawning target.
        /// The spec will be automatically set.
        /// </summary>
        SpecRoot Spec { get; set; }
    }
}
