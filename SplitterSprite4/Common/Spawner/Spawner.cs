// -----------------------------------------------------------------------
// <copyright file="Spawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// Spawner関連のStaticメソッド管理用クラス
    /// Class for static methods associated with Spawner.
    /// </summary>
    public class Spawner
    {
        private Spawner()
        {
            // インスタンス化しないためにデフォルトコンストラクタを
            // privateコンストラクタで隠す。
            // To avoid instanciation, hide default constructor by
            // this private constructor.
        }

        /// <summary>
        /// アセンブリにロードされた全SpawnerのTypeを取得する。
        /// 一度目はアセンブリにアクセスするが、二回目以降はキャッシュされた結果を返す。
        /// Return all spawner types loaded in assemblies.
        /// At first call, this method access all assemblies,
        /// then the result is cached.
        /// </summary>
        /// <param name="proxy">OutSideProxy for singleton pool access.</param>
        /// <returns>All types loaded in assemblies.</returns>
        public static List<Type> AllLoadedTypes(OutSideProxy proxy)
        {
            return proxy.Singleton(
                $"{typeof(Spawner)}.AllLoadedTypes",
                "no args",
                () =>
                {
                    var ret = Enumerable.Empty<Type>().ToList();

                    // 全てのAssemblyを確認
                    // Check all assemblies.
                    foreach (
                        var assembly in
                        AppDomain.CurrentDomain.GetAssemblies())
                    {
                        // 標準ライブラリ群はSpawnerを持たないためスキップ
                        // Standard libralies are skipped
                        // because they don't have spawners.
                        var assemblyName = assembly.FullName;
                        if (assemblyName.StartsWith("System.") ||
                            assemblyName.StartsWith("System,") ||
                            assemblyName.StartsWith("Microsoft.") ||
                            assemblyName.StartsWith("Windows.") ||
                            assemblyName.StartsWith("WindowsBase,") ||
                            assemblyName.StartsWith("mscorlib,") ||
                            assemblyName.StartsWith("netstandard,") ||
                            assemblyName.StartsWith("xunit.") ||
                            assemblyName.StartsWith("YamlDotNet,") ||
                            assemblyName.StartsWith("Anonymously Hosted " +
                                                    "DynamicMethods Assembly,") ||
                            assemblyName.StartsWith("testhost,"))
                        {
                            continue;
                        }

                        try
                        {
                            // Spawnerのみをフィルタして追加
                            // Add only spawner classes by filtering.
                            ret.AddRange(
                                assembly.GetTypes().Where((type) =>
                                {
                                    try
                                    {
                                        ValidateSpawnerType<ISpawner<object>>(
                                            type);
                                        return true;
                                    }
                                    catch (Exception)
                                    {
                                        return false;
                                    }
                                }));
                        }
                        catch (ReflectionTypeLoadException)
                        {
                            // アクセス不能となるAssemblyは無視する
                            // Unaccessible types are ignored.
                            continue;
                        }
                    }

                    return ret;
                });
        }

        /// <summary>
        /// Validate type instance is valid spawner.
        /// </summary>
        /// <typeparam name="T_Bound">Spawner type's bound.</typeparam>
        /// <param name="type">Validation target type.</param>
        /// <returns>
        /// Instance from 0 parameter constructor of validation target.
        /// </returns>
        public static T_Bound ValidateSpawnerType<T_Bound>(Type type)
        {
            return (T_Bound)ValidateSpawnerType(typeof(T_Bound), type);
        }

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
    }
}
