// -----------------------------------------------------------------------
// <copyright file="SpawnerRootWithoutValidConstructor.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    /// <summary>
    /// SpawnerRootの誤った実装。０引数コンストラクタを持たない。
    /// Invalid implementation of SpawnerRoot without 0 arg constructor.
    /// </summary>
    public class SpawnerRootWithoutValidConstructor :
        ISpawnerRootWithArgs<string, (bool, string)>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnerRootWithoutValidConstructor"/> class.
        /// </summary>
        /// <param name="dummy">Dummy parameter.</param>
        public SpawnerRootWithoutValidConstructor(int dummy)
        {
            _ = dummy;
        }

        /// <inheritdoc/>
        public SpecRoot Spec { get; set; }

        /// <inheritdoc/>
        public string Note { get; } = "誤って実装されたSpawner";

        /// <inheritdoc/>
        public (bool, string) DummyArgs()
        {
            return (true, "hoge");
        }

        /// <inheritdoc/>
        public string Spawn((bool, string) args)
        {
            (bool arg1, string arg2) = args;

            try
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
            catch (Exception)
            {
                return arg2;
            }
        }
    }
}
