// -----------------------------------------------------------------------
// <copyright file="ValidSpawnerChildWithImplementedConstructor.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// SpawnerChildの正しい実装。0引数コンストラクタを持つ。
    /// Valid implementation of SpawnerChild with 0 arg constructor.
    /// </summary>
    public class ValidSpawnerChildWithImplementedConstructor :
        ISpawnerChildWithArgs<string, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidSpawnerChildWithImplementedConstructor"/> class.
        /// </summary>
        public ValidSpawnerChildWithImplementedConstructor()
        {
        }

        /// <inheritdoc/>
        public SpecChild Spec { get; set; }

        /// <inheritdoc/>
        public string Note { get; } = "正しく実装されたSpawner";

        /// <inheritdoc/>
        public bool DummyArgs { get => true; }

        /// <inheritdoc/>
        public string Spawn(bool arg1)
        {
            if (arg1)
            {
                return this.Spec.Text["true"];
            }
            else
            {
                return this.Spec.Text["false"];
            }
        }
    }
}
