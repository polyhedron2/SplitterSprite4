// -----------------------------------------------------------------------
// <copyright file="ExteriorIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;

    /// <summary>
    /// Indexer class for SpawnerRoot instances.
    /// </summary>
    /// <typeparam name="T">Expected SpawnerRoot type.</typeparam>
    public class ExteriorIndexer<T> : PathIndexer<T>
        where T : ISpawnerRoot<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExteriorIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal ExteriorIndexer(Spec parent, bool dictMode)
            : base(
                parent,
                () => typeof(T).Name,
                (path) => ExteriorIndexer<T>.FetchSpawner(parent.Proxy, path),
                (spawner) => spawner.Spec.Path,
                () => $"Exterior, {Spec.EncodeType(typeof(T))}",
                CreateMoldingDefault(parent),
                dictMode)
        {
        }

        public static T FetchSpawner(OutSideProxy proxy, AgnosticPath path)
        {
            return proxy.Singleton(
                $"{typeof(ExteriorIndexer<>)}.FetchSpawner",
                path,
                () =>
                {
                    var spec = SpecRoot.Fetch(proxy, path);
                    var spawner = (T)Activator.CreateInstance(
                        spec.SpawnerType);
                    spawner.Spec = spec;

                    return spawner;
                });
        }

        private static T CreateMoldingDefault(Spec parent)
        {
            var moldingDefault = parent.MoldingDefault<T>();
            moldingDefault.Spec = SpecRoot.CreateDummy(parent.Proxy);
            return moldingDefault;
        }
    }
}
