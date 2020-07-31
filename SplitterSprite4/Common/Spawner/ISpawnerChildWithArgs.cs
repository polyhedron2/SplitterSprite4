// -----------------------------------------------------------------------
// <copyright file="ISpawnerChildWithArgs.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 引数とSpecChildからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecChild and some args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Args">
    /// Argument or arguments tuple class for spawing.
    /// </typeparam>
    public interface ISpawnerChildWithArgs<out T_Target, T_Args>
        : ISpawnerChild<T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Gets dummy arguments for molding.
        /// </summary>
        /// <returns>Dummy arguments.</returns>
        T_Args DummyArgs();

        /// <summary>
        /// Spawn target instance with SpecChild instance.
        /// </summary>
        /// <param name="args">Argument or arguments tuple.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Args args);
    }
}
