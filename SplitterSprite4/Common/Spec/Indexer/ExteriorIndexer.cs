// -----------------------------------------------------------------------
// <copyright file="ExteriorIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
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
        internal ExteriorIndexer(Spec parent)
            : base(
                parent,
                typeof(T).Name,
                (path) =>
                {
                    var spec = parent.Proxy.SpecPool(path);
                    var spawner = (T)Activator.CreateInstance(
                        spec.SpawnerType);
                    spawner.Spec = spec;

                    return spawner;
                },
                (spawner) => spawner.Spec.Path,
                $"Exterior, {Spec.EncodeType(typeof(T))}",
                CreateMoldingDefault(parent))
        {
        }

        private static T CreateMoldingDefault(Spec parent)
        {
            var moldingDefault = parent.MoldingDefault<T>();
            moldingDefault.Spec = SpecRoot.CreateDummy(parent.Proxy);
            return moldingDefault;
        }
    }
}
