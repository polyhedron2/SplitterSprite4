// -----------------------------------------------------------------------
// <copyright file="ValidSpawnerChildWithDefaultConstructor.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// SpawnerChildの正しい実装。デフォルトコンストラクタを持つ。
    /// Valid implementation of SpawnerChild.
    /// </summary>
    public class ValidSpawnerChildWithDefaultConstructor :
        ISpawnerChildWithoutArgs<string>
    {
        /// <inheritdoc/>
        public SpecChild Spec { get; set; }

        /// <inheritdoc/>
        public string Note { get; } = "正しく実装されたSpawner";

        /// <inheritdoc/>
        public string Spawn()
        {
            return this.Spec.Text["return value"];
        }
    }
}
