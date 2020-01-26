// -----------------------------------------------------------------------
// <copyright file="PathIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using System.Collections.Immutable;

    /// <summary>
    /// Indexer class for instances which associated with file paths.
    /// </summary>
    /// <typeparam name="T">Type of path associated value.</typeparam>
    public class PathIndexer<T>
    {
        private Spec parent;
        private string type;
        private Func<AgnosticPath, T> getter;
        private Func<T, AgnosticPath> setter;
        private string moldingAccessCode;
        private T moldingDefault;
        private ScalarIndexer<T> internalIndexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="type">The access type string.</param>
        /// <param name="getter">Translation function from agnostic path.</param>
        /// <param name="setter">Translation function to agnostic path.</param>
        /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        internal PathIndexer(
            Spec parent,
            string type,
            Func<AgnosticPath, T> getter,
            Func<T, AgnosticPath> setter,
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
                (path, scalar) =>
                {
                    var relative = AgnosticPath.FromAgnosticPathString(scalar);
                    var fromExecutable = relative + path.Parent;
                    return this.getter(fromExecutable);
                },
                (path, value) =>
                {
                    var fromExecutable = this.setter(value);
                    var relative = fromExecutable - path.Parent;
                    return relative.ToAgnosticPathString();
                },
                this.moldingAccessCode,
                this.moldingDefault,
                ImmutableList<string>.Empty);
        }

        /// <summary>
        /// Indexer for value.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key]
        {
            get => this.internalIndexer[key];
            set { this.internalIndexer[key] = value; }
        }

        /// <summary>
        /// Indexer for value with default.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <param name="defaultPath">The default agnostic path string.</param>
        /// <returns>The translated value.</returns>
        public T this[string key, string defaultPath]
        {
            get
            {
                try
                {
                    _ = AgnosticPath.FromAgnosticPathString(defaultPath);
                }
                catch (Exception ex)
                {
                    throw new Spec.InvalidSpecDefinitionException(
                        "デフォルトパスが不正です。", ex);
                }

                return this.internalIndexer[key, defaultPath];
            }
        }
    }
}
