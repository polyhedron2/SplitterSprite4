// -----------------------------------------------------------------------
// <copyright file="ISpawnerChild2.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    /// <summary>
    /// 2引数とSpecChildからインスタンスを生成するインターフェース。
    /// Instance spawner interface with SpecChild and 2 args.
    /// </summary>
    /// <typeparam name="T_Target">Spawn target class.</typeparam>
    /// <typeparam name="T_Arg1">1st argument class for spawing.</typeparam>
    /// <typeparam name="T_Arg2">2nd argument class for spawing.</typeparam>
    public interface ISpawnerChild2<out T_Target, T_Arg1, T_Arg2>
        : ISpawnerChild<T_Target>
        where T_Target : class
    {
        /// <summary>
        /// Gets dummy 1st argument for molding.
        /// </summary>
        T_Arg1 DummyArg1 { get; }

        /// <summary>
        /// Gets dummy 2nd argument for molding.
        /// </summary>
        T_Arg2 DummyArg2 { get; }

        /// <summary>
        /// Spawn target instance with SpecChild instance.
        /// </summary>
        /// <param name="arg1">1st argument.</param>
        /// <param name="arg2">2nd argument.</param>
        /// <returns>Spawn target.</returns>
        T_Target Spawn(T_Arg1 arg1, T_Arg2 arg2);

        /// <inheritdoc/>
        T_Target ISpawner<T_Target>.DummySpawn()
        {
            return this.Spawn(this.DummyArg1, this.DummyArg2);
        }
    }
}
