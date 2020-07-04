// -----------------------------------------------------------------------
// <copyright file="IIndexerGetSet.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Interface for Spec class's indexers.
    /// </summary>
    /// <typeparam name="T_Value">Type of associated value.</typeparam>
    public interface IIndexerGetSet<T_Value> : IIndexerGet<T_Value>
    {
        new T_Value this[string key] { get; set; }
    }
}
