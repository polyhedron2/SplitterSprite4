// -----------------------------------------------------------------------
// <copyright file="YAMLTypeSlipException.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;

    /// <summary>
    /// YAMLアクセス時に期待する型と実際の型が異なる際の例外
    /// The exception that is thrown when an attempt to
    /// access a yaml child that does not have expected type.
    /// </summary>
    /// <typeparam name="TExpectedValue">The expected yaml type.</typeparam>
    public class YAMLTypeSlipException<TExpectedValue> : Exception
        where TExpectedValue : YAML
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YAMLTypeSlipException{TExpectedValue}"/> class.
        /// </summary>
        /// <param name="id">The yaml's ID.</param>
        /// <param name="value">The child yaml.</param>
        public YAMLTypeSlipException(string id, YAML value)
            : base(
                $"\"{id}\"の値は{value.GetType().Name}形式であり" +
                $"期待されている{typeof(TExpectedValue).Name}形式では" +
                $"ありません。")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YAMLTypeSlipException{TExpectedValue}"/> class.
        /// </summary>
        /// <param name="id">The yaml's id.</param>
        /// <param name="key">The string key.</param>
        /// <param name="value">The child YAML.</param>
        public YAMLTypeSlipException(string id, string key, YAML value)
            : this($"{id}[{key}]", value)
        {
        }
    }
}
