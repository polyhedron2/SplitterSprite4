// -----------------------------------------------------------------------
// <copyright file="ExteriorDirIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Indexer class for SpawnerDir instances.
    /// </summary>
    /// <typeparam name="T">Expected SpawnerRoot type.</typeparam>
    public class ExteriorDirIndexer<T> : PathIndexer<ISpawnerDir<T>>
        where T : ISpawnerRoot<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExteriorDirIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal ExteriorDirIndexer(Spec parent)
            : base(
                  parent,
                  $"{typeof(T).Name}ディレクトリ",
                  (path) => new SpawnerDir<T>(parent.Proxy, path),
                  (spawnerDir) => spawnerDir.Dir.Path,
                  $"ExteriorDir, {Spec.EncodeType(typeof(T))}",
                  new DummySpawnerDir<T>())
        {
            // Call MoldingDefault method to validate type T.
            _ = parent.MoldingDefault<T>();
        }
    }
}
