// -----------------------------------------------------------------------
// <copyright file="ISpawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec;

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

        /// <summary>
        /// Validate type instance is valid spawner.
        /// </summary>
        /// <param name="bound">Spawner type's bound.</param>
        /// <param name="type">Validation target type.</param>
        /// <returns>
        /// Instance from 0 parameter constructor of validation target.
        /// </returns>
        public static object ValidateSpawnerType(Type bound, Type type)
        {
            // boundのサブクラスである必要がある。
            // The type must be sub class of bound.
            if (!bound.IsAssignableFrom(type))
            {
                throw new Spec.ValidationError();
            }

            // Spawnerはゼロ引数コンストラクタからインスタンス生成可能である。
            // Spawner instance must be created with constructor without parameters.
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Create instance for molding default.
        /// </summary>
        /// <typeparam name="T_Spawner">Expected spawner type.</typeparam>
        /// <param name="proxy">OutSideProxy for type pool cache.</param>
        /// <returns>Dummy spawner instance.</returns>
        public static T_Spawner MoldingDefault<T_Spawner>(OutSideProxy proxy)
            where T_Spawner : ISpawner<object>
        {
            var spawnerType = typeof(T_Spawner);

            foreach (var type in proxy.TypePool())
            {
                try
                {
                    // 最初にValidation成功したTypeによるインスタンスを返す
                    // Instance from first valid type is returned.
                    return (T_Spawner)ValidateSpawnerType(
                        spawnerType, type);
                }
                catch (Exception)
                {
                    // Validation失敗となるTypeは無視する
                    // Invalid types are ignored.
                    continue;
                }
            }

            throw new Spec.InvalidSpecDefinitionException(
                $"正当なサブクラスを持たないSpawnerクラス" +
                $"\"{spawnerType.Name}\"が使用されています。");
        }
    }
}
