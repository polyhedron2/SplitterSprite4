// -----------------------------------------------------------------------
// <copyright file="DummySpawnerDir.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spawner
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Molding default用のダミーSpawnerDirクラス。
    /// Dummy SpawnerDir class for molding default.
    /// </summary>
    /// <typeparam name="T_Spawner">Target SpawnerRoot class.</typeparam>
    public class DummySpawnerDir<T_Spawner> : ISpawnerDir<T_Spawner>
        where T_Spawner : ISpawnerRoot<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySpawnerDir{T_Spawner}"/> class.
        /// </summary>
        internal DummySpawnerDir()
        {
        }

        /// <inheritdoc/>
        public LayeredDir Dir { get; }

        /// <inheritdoc/>
        public IEnumerator<T_Spawner> GetEnumerator()
        {
            return Enumerable.Empty<T_Spawner>().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
