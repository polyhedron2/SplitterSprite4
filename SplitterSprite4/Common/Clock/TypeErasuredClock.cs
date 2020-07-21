// -----------------------------------------------------------------------
// <copyright file="TypeErasuredClock.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Clock
{
    using System.Collections.Generic;

    /// <summary>
    /// １フレームに１度、ゲームの変数状態を表現するIStatusオブジェクトを更新するクラス。
    /// Updater class for IStatus objects which contains variables of game status.
    /// IStatus objects will be updated once a frame.
    /// </summary>
    public abstract class TypeErasuredClock
    {
        /// <summary>
        /// Preprocess IStatus instances.
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        /// Postprocess IStatus instances.
        /// </summary>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// Update IStatus objects.
        /// </summary>
        /// <returns>Update result name.</returns>
        public abstract string TickWithName();

        /// <summary>
        /// Yield result names which may returned by Tick method.
        /// </summary>
        /// <returns>Enum values which may returned by Tick method.</returns>
        public abstract IEnumerable<string> ResultCandidates();
    }
}
