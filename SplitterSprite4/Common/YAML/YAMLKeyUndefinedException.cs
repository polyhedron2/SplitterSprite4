// -----------------------------------------------------------------------
// <copyright file="YAMLKeyUndefinedException.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;

    /// <summary>
    /// YAML上のキーアクセス対象が存在しない際の例外
    /// The exception that is thrown when an attempt to
    /// access a key that does not exist on the yaml file.
    /// </summary>
    public class YAMLKeyUndefinedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YAMLKeyUndefinedException"/> class.
        /// </summary>
        /// <param name="id">The yaml's id.</param>
        /// <param name="key">The string key.</param>
        public YAMLKeyUndefinedException(string id, string key)
            : base($"YAML\"{id}\"上のキー\"{key}\"は未定義です。")
        {
        }
    }
}
