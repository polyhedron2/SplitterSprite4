// -----------------------------------------------------------------------
// <copyright file="LiteralIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using System.Collections.Immutable;

    /// <summary>
    /// Indexer class for literal values (Int, Double, and Bool etc).
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    public class LiteralIndexer<T> : IIndexerWithDefault<T, T>
    {
        private Func<string, T> getter;
        private Func<T, string> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="typeGenerator">The access type string generator func.</param>
        /// <param name="getter">Translation function from string.</param>
        /// <param name="setter">Translation function to string.</param>
        /// <param name="moldingAccessCodeGenerator">The generator func of type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        internal LiteralIndexer(
            Spec parent,
            Func<string> typeGenerator,
            Func<string, T> getter,
            Func<T, string> setter,
            Func<string> moldingAccessCodeGenerator,
            T moldingDefault,
            bool allowHiddenValue)
        {
            this.getter = getter;
            this.setter = setter;
            this.InternalIndexer = new ScalarIndexer<T>(
                parent,
                typeGenerator,
                (path, scalar) => this.getter(scalar),
                (path, value) => this.setter(value),
                moldingAccessCodeGenerator,
                moldingDefault,
                allowHiddenValue,
                ImmutableList<string>.Empty);
        }

        internal ScalarIndexer<T> InternalIndexer { get; }

        /// <summary>
        /// Indexer for literal value.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key]
        {
            get => this.InternalIndexer[key];
            set { this.InternalIndexer[key] = value; }
        }

        /// <summary>
        /// Indexer for literal value.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        /// <param name="defaultVal">The default literal value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key, T defaultVal]
        {
            get
            {
                try
                {
                    this.getter(this.setter(defaultVal));
                }
                catch (Exception ex)
                {
                    throw new Spec.InvalidSpecDefinitionException(
                        "デフォルト値がSpecの値として不正です。", ex);
                }

                return this.InternalIndexer[key, defaultVal];
            }
        }

        /// <summary>
        /// Remove the key from the spec.
        /// If base spec contains the key, the base value will be referred.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        public void Remove(string key)
        {
            this.InternalIndexer.Remove(key);
        }

        /// <summary>
        /// Add special value into the key of the spec.
        /// If base spec contains the key, the base value will be hidden.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        public void Hide(string key)
        {
            this.InternalIndexer.Hide(key);
        }

        /// <summary>
        /// Add special value into the key of the spec.
        /// Even if base spec contains the key, the default value will be used.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        public void ExplicitDefault(string key)
        {
            this.InternalIndexer.ExplicitDefault(key);
        }
    }
}
