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
        private ScalarIndexer<T> internalIndexer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="typeGenerator">The access type string generator func.</param>
        /// <param name="getter">Translation function from agnostic path.</param>
        /// <param name="setter">Translation function to agnostic path.</param>
        /// <param name="moldingAccessCodeGenerator">The generator func of type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        internal PathIndexer(
            Spec parent,
            Func<string> typeGenerator,
            Func<AgnosticPath, T> getter,
            Func<T, AgnosticPath> setter,
            Func<string> moldingAccessCodeGenerator,
            T moldingDefault)
        {
            this.internalIndexer = new ScalarIndexer<T>(
                parent,
                typeGenerator,
                (path, scalar) =>
                {
                    var relative = AgnosticPath.FromAgnosticPathString(scalar);
                    var fromExecutable = relative + path.Parent;
                    return getter(fromExecutable);
                },
                (path, value) =>
                {
                    var fromExecutable = setter(value);
                    var relative = fromExecutable - path.Parent;
                    return relative.ToAgnosticPathString();
                },
                moldingAccessCodeGenerator,
                moldingDefault,
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

        /// <summary>
        /// Remove the key from the spec.
        /// If base spec contains the key, the base value will be referred.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        public void Remove(string key)
        {
            this.internalIndexer.Remove(key);
        }

        /// <summary>
        /// Add special value into the key of the spec.
        /// If base spec contains the key, the base value will be hidden.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        public void Hide(string key)
        {
            this.internalIndexer.Hide(key);
        }
    }
}
