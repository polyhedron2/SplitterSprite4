﻿// -----------------------------------------------------------------------
// <copyright file="ScalarIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using System.Collections.Immutable;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Common indexer class for scalar value in spec file.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    internal class ScalarIndexer<T>
    {
        private Spec parent;
        private string type;
        private Func<AgnosticPath, string, T> getter;
        private Func<AgnosticPath, T, string> setter;
        private string moldingAccessCode;
        private T moldingDefault;
        private ImmutableList<string> referredSpecs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="type">The access type string.</param>
        /// <param name="getter">Translation function from spec path and string value in spec to indexed value.</param>
        /// <param name="setter">Translation function from spec path and indexed value to string value in spec.</param>
        /// <param name="moldingAccessCode">The type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
        internal ScalarIndexer(
            Spec parent,
            string type,
            Func<AgnosticPath, string, T> getter,
            Func<AgnosticPath, T, string> setter,
            string moldingAccessCode,
            T moldingDefault,
            ImmutableList<string> referredSpecs)
        {
            this.parent = parent;
            this.type = type;
            this.getter = getter;
            this.setter = setter;
            this.moldingAccessCode = moldingAccessCode;
            this.moldingDefault = moldingDefault;
            this.referredSpecs = referredSpecs;
        }

        /// <summary>
        /// Indexer for value.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key]
        {
            get
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            this.parent.Mold[key] =
                                new ScalarYAML(this.moldingAccessCode);
                        }

                        return this.IndexGet(key);
                    }
                    catch (Exception ex)
                    {
                        if (this.parent.IsMolding)
                        {
                            return this.moldingDefault;
                        }
                        else
                        {
                            throw new Spec.InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                this.type,
                                ex);
                        }
                    }
                }
            }

            set
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        this.parent.Properties[key] =
                            new ScalarYAML(this.setter(this.parent.Path, value));
                    }
                    catch (Exception ex)
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            this.type,
                            ex);
                    }
                }
            }
        }

        /// <summary>
        /// Indexer for value with default.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>The translated value.</returns>
        public T this[string key, T defaultVal]
        {
            get => this[
                key,
                () => defaultVal,
                this.setter(this.parent.Path, defaultVal)];
        }

        /// <summary>
        /// Indexer for value with default.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <param name="defaultValInSpec">The default value in spec.</param>
        /// <returns>The translated value.</returns>
        public T this[string key, string defaultValInSpec]
        {
            get => this[
                key,
                () => this.getter(this.parent.Path, defaultValInSpec),
                defaultValInSpec];
        }

        private T this[
            string key,
            Func<T> lazyDefaultVal,
            string defaultValForMolding]
        {
            get
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        if (this.parent.IsMolding)
                        {
                            var accessCodeWithDefault =
                                this.moldingAccessCode +
                                ", " +
                                Spec.EncodeDefaultValForMolding(
                                    defaultValForMolding);

                            this.parent.Mold[key] =
                                new ScalarYAML(accessCodeWithDefault);
                        }

                        try
                        {
                            return this.IndexGet(key);
                        }
                        catch (YAML.YAMLKeyUndefinedException)
                        {
                            return lazyDefaultVal();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.parent.IsMolding)
                        {
                            return this.moldingDefault;
                        }
                        else
                        {
                            throw new Spec.InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                this.type,
                                ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Common index getter process.
        /// </summary>
        /// <param name="key">The string key for the value.</param>
        /// <returns>The translated value.</returns>
        internal T IndexGet(string key)
        {
            try
            {
                return this.getter(
                    this.parent.Path,
                    this.parent.Properties.Scalar[key].Value);
            }
            catch (YAML.YAMLKeyUndefinedException ex)
            {
                var isLooped = this.referredSpecs.Contains(
                    this.parent.ID);
                if (this.parent.Base == null || isLooped)
                {
                    throw ex;
                }

                // Only if base spec is defined and not looped,
                // base spec is referred.
                return new ScalarIndexer<T>(
                    this.parent.Base,
                    this.type,
                    this.getter,
                    this.setter,
                    this.moldingAccessCode,
                    this.moldingDefault,
                    this.referredSpecs.Add(this.parent.ID))
                    .IndexGet(key);
            }
        }
    }
}