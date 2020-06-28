// -----------------------------------------------------------------------
// <copyright file="ValidSpawnerRootWithDefaultConstructor.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// SpawnerRootの正しい実装。デフォルトコンストラクタを持つ。
    /// Valid implementation of SpawnerRoot.
    /// </summary>
    public class ValidSpawnerRootWithDefaultConstructor :
        ISpawnerRootWithoutArgs<string>
    {
        /// <inheritdoc/>
        public SpecRoot Spec { get; set; }

        /// <inheritdoc/>
        public string Note { get; } = "正しく実装されたSpawner";

        /// <inheritdoc/>
        public string Spawn()
        {
            return this.Spec.Text["return value"];
        }
    }
}
