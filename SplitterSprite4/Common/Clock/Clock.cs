// -----------------------------------------------------------------------
// <copyright file="Clock.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Clock
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// １フレームに１度、ゲームの変数状態を表現するIStatusオブジェクトを更新するクラス。
    /// Updater class for IStatus objects which contains variables of game status.
    /// IStatus objects will be updated once a frame.
    /// </summary>
    /// <typeparam name="T_RESULT">The enumaration type for update result.</typeparam>
    public abstract class Clock<T_RESULT> : IClock
        where T_RESULT : Enum
    {
        /// <inheritdoc/>
        public string TickWithName()
        {
            return this.ResultToName(this.Tick());
        }

        /// <inheritdoc/>
        public IEnumerable<string> ResultCandidates()
        {
            foreach (var enumValue in Enum.GetValues(typeof(T_RESULT)))
            {
                yield return this.ResultToName((T_RESULT)enumValue);
            }
        }

        /// <inheritdoc/>
        public virtual void Enter()
        {
        }

        /// <inheritdoc/>
        public virtual void Exit()
        {
        }

        /// <summary>
        /// Update IStatus objects.
        /// </summary>
        /// <returns>Update result.</returns>
        protected abstract T_RESULT Tick();

        /// <summary>
        /// Translate enum value of TickWithEnum's result to name string.
        /// If the enum has description attribute, it will be returned.
        /// In other cases, the enum name will be returned.
        /// </summary>
        /// <param name="clockResult">The result enum value from Clock's TickWithEnum method.</param>
        /// <returns>The name string.</returns>
        private string ResultToName(T_RESULT clockResult)
        {
            var member = clockResult.GetType().GetMember(clockResult.ToString())[0];
            var descriptions = member.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length > 0)
            {
                return ((DescriptionAttribute)descriptions[0]).Description;
            }
            else
            {
                return clockResult.ToString();
            }
        }
    }
}
