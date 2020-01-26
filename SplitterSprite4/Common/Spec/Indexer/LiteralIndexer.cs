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
    public class LiteralIndexer<T>
    {
        private Spec parent;
        private string type;
        private Func<string, T> getter;
        private Func<T, string> setter;
        private string moldingAccessCode;
        private T moldingDefault;
        private ScalarIndexer<T> internalIndexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="type">The access type string.</param>
        /// <param name="getter">Translation function from string.</param>
        /// <param name="setter">Translation function to string.</param>
        /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        internal LiteralIndexer(
            Spec parent,
            string type,
            Func<string, T> getter,
            Func<T, string> setter,
            string moldingAccessCode,
            T moldingDefault)
        {
            this.parent = parent;
            this.type = type;
            this.getter = getter;
            this.setter = setter;
            this.moldingAccessCode = moldingAccessCode;
            this.moldingDefault = moldingDefault;
            this.internalIndexer = new ScalarIndexer<T>(
                this.parent,
                this.type,
                (path, scalar) => this.getter(scalar),
                (path, value) => this.setter(value),
                this.moldingAccessCode,
                this.moldingDefault,
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Indexer for literal value.
        /// </summary>
        /// <param name="key">The string key for the literal value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key]
        {
            get => this.internalIndexer[key];
            set { this.internalIndexer[key] = value; }
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

                return this.internalIndexer[key, defaultVal];
            }
        }
    }
}
