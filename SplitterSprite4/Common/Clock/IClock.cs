// -----------------------------------------------------------------------
// <copyright file="IClock.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Clock
{
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Effect;

    /// <summary>
    /// １フレームに１度、ゲームの変数状態を表現するIStatusオブジェクトを更新するクラス。
    /// Updater class for IStatus objects which contains variables of game status.
    /// IStatus objects will be updated once a frame.
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Gets Effect instances which draw the IStatus objects.
        /// </summary>
        List<Effect> Effects { get; }

        /// <summary>
        /// Preprocess IStatus instances.
        /// </summary>
        void Enter();

        /// <summary>
        /// Postprocess IStatus instances.
        /// </summary>
        void Exit();

        /// <summary>
        /// Update IStatus objects.
        /// </summary>
        /// <returns>Update result name.</returns>
        string TickWithName();

        /// <summary>
        /// Yield result names which may returned by Tick method.
        /// </summary>
        /// <returns>Enum values which may returned by Tick method.</returns>
        IEnumerable<string> ResultCandidates();
    }
}
