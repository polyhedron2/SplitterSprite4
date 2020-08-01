// -----------------------------------------------------------------------
// <copyright file="Mode.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Cycle
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Clock;
    using MagicKitchen.SplitterSprite4.Common.Effect;

    /// <summary>
    /// ゲームサイクル中の個別のモードを表現するクラス。
    /// １つのClockオブジェクトと、０個以上のEffectオブジェクトからなる。
    /// Game-Mode class.
    /// It consists of one Clock object and several Effect objects.
    /// </summary>
    public class Mode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mode"/> class.
        /// </summary>
        /// <param name="clock">The Clock instance for updating IStatus instances.</param>
        /// <param name="resultToNextMode">Mapping from name of Clock's result enum to next mode name.</param>
        public Mode(
            IClock clock,
            IDictionary<string, string> resultToNextMode)
        {
            this.Clock = clock;
            this.ResultToNextMode = resultToNextMode;
        }

        /// <summary>
        /// Gets the IClock instance for updating IStatus instances.
        /// </summary>
        public IClock Clock { get; }

        /// <summary>
        /// Gets the mapping from name of IClock's result enum to next mode name.
        /// </summary>
        public IDictionary<string, string> ResultToNextMode { get; }

        /// <summary>
        /// Update IStatus instances by Clock.
        /// This method must be called once per frame.
        /// </summary>
        /// <returns>The next mode name.</returns>
        public string Update()
        {
            var resultName = this.Clock.TickWithName();
            return this.ResultToNextMode[resultName];
        }

        /// <summary>
        /// Render or make sounds basing on IStatus.
        /// This method should be called once per frame.
        /// However, it can be skipped when the computer is slow.
        /// </summary>
        public void Draw()
        {
            this.Clock.Effects.ForEach(effect => effect.Apply());
        }

        /// <summary>
        /// Preprocess IStatus instances.
        /// </summary>
        public void Enter()
        {
            this.Clock.Enter();
        }

        /// <summary>
        /// Postprocess IStatus instances.
        /// </summary>
        public void Exit()
        {
            this.Clock.Exit();
        }
    }
}
