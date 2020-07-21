// -----------------------------------------------------------------------
// <copyright file="Effect.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Effect
{
    /// <summary>
    /// １フレームに１度、ゲームの変数状態を表現するIStatusオブジェクトを元に描画や音声出力を行うクラス。
    /// Renderer or audio output class for IStatus objects which contains variables of game status.
    /// Renderring or audio output will be refleshed once a frame.
    /// </summary>
    public abstract class Effect
    {
        /// <summary>
        /// Render or make sounds based on IStatus objects.
        /// </summary>
        public abstract void Apply();
    }
}
