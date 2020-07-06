// -----------------------------------------------------------------------
// <copyright file="IIndexerGet.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Interface for Spec class's indexers.
    /// </summary>
    /// <typeparam name="T_Value">Type of associated value.</typeparam>
    public interface IIndexerGet<T_Value>
    {
        T_Value this[string key] { get; }

        void Remove(string key);

        void Hide(string key);

        void Hold(string key);
    }
}
