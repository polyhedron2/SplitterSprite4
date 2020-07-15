// -----------------------------------------------------------------------
// <copyright file="SpawnerDir.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec.Indexer;

    /// <summary>
    /// ISpawnerDirの実体クラス。
    /// Entity class for ISpawnerDir.
    /// </summary>
    /// <typeparam name="T_Spawner">Target SpawnerRoot class.</typeparam>
    public class SpawnerDir<T_Spawner> : ISpawnerDir<T_Spawner>
        where T_Spawner : ISpawnerRoot<object>
    {
        private OutSideProxy proxy;
        private List<T_Spawner> spawners;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnerDir{T_Spawner}"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file or spec pool access.</param>
        /// <param name="path">The direcotry path.</param>
        public SpawnerDir(OutSideProxy proxy, AgnosticPath path)
        {
            this.proxy = proxy;
            this.Dir = new LayeredDir(proxy.FileIO, path);
            this.spawners = this.Dir.EnumerateFiles().Select(
                (pth) => pth + this.Dir.Path).Select(
                (pth) => ExteriorIndexer<T_Spawner>.FetchSpawner(this.proxy, pth)).ToList();
        }

        /// <inheritdoc/>
        public LayeredDir Dir { get; }

        /// <inheritdoc/>
        public IEnumerator<T_Spawner> GetEnumerator()
        {
            return this.spawners.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            SpawnerDir<T_Spawner> that;

            try
            {
                that = (SpawnerDir<T_Spawner>)obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return this.Dir.Path == that.Dir.Path;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Dir.Path.GetHashCode();
        }
    }
}
