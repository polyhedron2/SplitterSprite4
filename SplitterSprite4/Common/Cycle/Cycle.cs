// -----------------------------------------------------------------------
// <copyright file="Cycle.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Cycle
{
    using System.Collections.Generic;

    /// <summary>
    /// ゲームサイクルを表現し、ゲームモードを遷移させるクラス。
    /// Manager class for game-cycle. Game-Cycle is changing Mode objects.
    /// </summary>
    public class Cycle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cycle"/> class.
        /// </summary>
        /// <param name="title">The game title.</param>
        /// <param name="fps">How many times IStatus instances should be updated per seconds.</param>
        /// <param name="initialModeName">The first mode name.</param>
        /// <param name="modes">Mapping from mode name to Mode instance.</param>
        public Cycle(
            string title,
            int fps,
            string initialModeName,
            IDictionary<string, Mode> modes)
        {
            this.Title = title;
            this.FPS = fps;
            this.CurrentModeName = initialModeName;
            this.Modes = modes;

            this.CurrentMode.Enter();
        }

        /// <summary>
        /// Gets game title string.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets integer which shows how many frames per second.
        /// </summary>
        public int FPS { get; }

        /// <summary>
        /// Gets or sets current mode name.
        /// </summary>
        public string CurrentModeName { get; protected set; }

        /// <summary>
        /// Gets current Mode instance.
        /// </summary>
        public Mode CurrentMode
        {
            get => this.Modes[this.CurrentModeName];
        }

        /// <summary>
        /// Gets mapping from name of mode to Mode instance.
        /// </summary>
        public IDictionary<string, Mode> Modes { get; }

        /// <summary>
        /// Update IStatus instances by Clock.
        /// Then, mode may be chaned.
        /// This method must be called once per frame.
        /// </summary>
        public void Update()
        {
            var prevModeName = this.CurrentModeName;
            var nextModeName = this.CurrentMode.Update();
            if (prevModeName != nextModeName)
            {
                this.CurrentMode.Exit();
                this.CurrentModeName = nextModeName;
                this.CurrentMode.Enter();
            }
        }

        /// <summary>
        /// Render or make sounds basing on IStatus.
        /// This method should be called once per frame.
        /// However, it can be skipped when the computer is slow.
        /// </summary>
        public void Draw()
        {
            this.CurrentMode.Draw();
        }
    }
}
