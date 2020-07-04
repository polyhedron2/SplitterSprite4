// -----------------------------------------------------------------------
// <copyright file="IIndexerWithDefault.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Interface for Spec class's indexers with default value.
    /// </summary>
    /// <typeparam name="T_Value">Type of associated value.</typeparam>
    /// <typeparam name="T_Default">Type of associatged default value.</typeparam>
    public interface IIndexerWithDefault<T_Value, T_Default>
        : IIndexerGetSet<T_Value>
    {
        T_Value this[string key, T_Default defaultVal] { get; }
    }
}
