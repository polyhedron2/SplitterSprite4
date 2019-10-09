// -----------------------------------------------------------------------
// <copyright file="InvalidYAMLStyleException.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;

    /// <summary>
    /// ロードしたYAMLが想定したパターン外であった際の例外
    /// The exception that is thrown when an attempt to
    /// parse a YAML file that contains unexpected pattern.
    /// </summary>
    public class InvalidYAMLStyleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidYAMLStyleException"/> class.
        /// </summary>
        /// <param name="id">The yaml's id.</param>
        public InvalidYAMLStyleException(string id)
            : base($"YAML\"{id}\"上の形式は想定される形式ではありません。")
        {
        }
    }
}
