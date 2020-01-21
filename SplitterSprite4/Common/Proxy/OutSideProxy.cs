// -----------------------------------------------------------------------
// <copyright file="OutSideProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// ゲームの実行ファイル外からの全ての入力
    /// （プレイヤー操作、ファイル、時刻など）を管理するプロキシクラス
    /// The proxy class for any input from outside of the executable file
    /// (e.g. player's control, files, time, etc).
    /// </summary>
    public class OutSideProxy
    {
        private Dictionary<string, SpecRoot> specPool =
            new Dictionary<string, SpecRoot>();

        private List<Type> spawnerTypePool = Enumerable.Empty<Type>().ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="OutSideProxy"/> class.
        /// </summary>
        /// <param name="fileIO">A FileIOProxy instance.</param>
        public OutSideProxy(FileIOProxy fileIO)
        {
            this.FileIO = fileIO;
        }

        /// <summary>
        /// Gets proxy for configuration (spec) files, media files,
        /// and program files.
        /// </summary>
        public FileIOProxy FileIO { get; }

        private bool SpawnerTypePoolIsCached
        {
            get => this.spawnerTypePool.Any();
        }

        /// <summary>
        /// SpecRootインスタンスを取得する。
        /// 過去に取得したインスタンスと同一パスであれば、同一インスタンスを返す。
        /// これにより、同一パスのSpecRootインスタンスは唯一つ存在する。
        /// Fetch SpecRoot instance.
        /// If it is second fetching for the path, same instance will be returned.
        /// Therefore, SpecRoot instance with same path is unique.
        /// </summary>
        /// <param name="layeredPath">The spec file path.</param>
        /// <param name="acceptAbsence">Accept absence of the spec file or not.</param>
        /// <returns>The unique SpecRoot instance.</returns>
        public SpecRoot SpecPool(
                AgnosticPath layeredPath,
                bool acceptAbsence = false)
        {
            lock (this.specPool)
            {
                var poolKey = layeredPath.ToAgnosticPathString();
                if (this.specPool.ContainsKey(poolKey))
                {
                    return this.specPool[poolKey];
                }
                else
                {
                    var ret = new SpecRoot(this, layeredPath, acceptAbsence);
                    this.specPool[poolKey] = ret;
                    return ret;
                }
            }
        }

        /// <summary>
        /// アセンブリにロードされた全SpawnerのTypeを取得する。
        /// 一度目はアセンブリにアクセスするが、二回目以降はキャッシュされた結果を返す。
        /// Return all spawner types loaded in assemblies.
        /// At first call, this method access all assemblies,
        /// then the result is cached.
        /// </summary>
        /// <returns>All types loaded in assemblies.</returns>
        public IEnumerable<Type> SpawnerTypePool()
        {
            lock (this.spawnerTypePool)
            {
                if (this.SpawnerTypePoolIsCached)
                {
                    return this.spawnerTypePool;
                }

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
                        this.spawnerTypePool.AddRange(
                            assembly.GetTypes().Where((type) =>
                            {
                                try
                                {
                                    ISpawner<object>.ValidateSpawnerType(
                                        typeof(ISpawner<object>), type);
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

                return this.spawnerTypePool;
            }
        }
    }
}