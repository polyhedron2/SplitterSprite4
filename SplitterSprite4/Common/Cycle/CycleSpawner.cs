// -----------------------------------------------------------------------
// <copyright file="CycleSpawner.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Cycle
{
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Clock;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;

    public class CycleSpawner : ISpawnerRootWithoutArgs<Cycle>
    {
        /// <inheritdoc/>
        public SpecRoot Spec { get; set; }

        /// <inheritdoc/>
        public string Note { get => "ゲームサイクル定義"; }

        /// <inheritdoc/>
        public Cycle Spawn()
        {
            var title = this.Spec.Keyword["タイトル"];
            var fps = this.Spec.Int["FPS"];
            var initialModeName = this.Spec.Keyword["開始モード"];
            var modeSpecs = this.Spec.Dict.Keyword.EnsureKeys(initialModeName).SubSpec["モード一覧"];
            var modes = modeSpecs.ToDictionary(
                kv => kv.Key, kv => this.SpawnMode(kv.Value, modeSpecs.Keys));

            return new Cycle(title, fps, initialModeName, modes);
        }

        private Mode SpawnMode(Spec spec, IEnumerable<string> modeNames)
        {
            var clock = spec.Exterior<ISpawnerRootWithoutArgs<IClock>>()["状態更新"].Spawn();
            var resultToNextMode = clock.ResultCandidates().ToDictionary(
                result => result, result => spec["遷移先"].Choice(modeNames)[result]);

            return new Mode(clock, resultToNextMode);
        }
    }
}
