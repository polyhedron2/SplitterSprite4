// -----------------------------------------------------------------------
// <copyright file="ISpawnerRootWithArgs.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 引数とSpecRootからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecRoot and some args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Args">
    /// Argument or arguments tuple class for spawing.
    /// </typeparam>
    public interface ISpawnerRootWithArgs<out T_Target, T_Args>
        : ISpawnerRoot<T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Gets dummy arguments for molding.
        /// </summary>
        T_Args DummyArgs { get; }

        /// <summary>
        /// Spawn target instance with SpecRoot instance.
        /// </summary>
        /// <param name="args">Argument or arguments tuple.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Args args);
    }
}
