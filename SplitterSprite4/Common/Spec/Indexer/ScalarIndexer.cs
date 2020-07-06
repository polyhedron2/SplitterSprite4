// -----------------------------------------------------------------------
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
    public class ScalarIndexer<T>
    {
        private Spec parent;
        private T moldingDefault;
        private ImmutableList<string> referredSpecs;
        private bool dictMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="typeGenerator">The access type string generator func.</param>
        /// <param name="getter">Translation function from spec path and string value in spec to indexed value.</param>
        /// <param name="setter">Translation function from spec path and indexed value to string value in spec.</param>
        /// <param name="moldingAccessCodeGenerator">The generator func of type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
        internal ScalarIndexer(
            Spec parent,
            Func<string> typeGenerator,
            Func<AgnosticPath, string, T> getter,
            Func<AgnosticPath, T, string> setter,
            Func<string> moldingAccessCodeGenerator,
            T moldingDefault,
            bool dictMode,
            ImmutableList<string> referredSpecs)
        {
            this.parent = parent;
            this.TypeGenerator = Spec.FixCulture(typeGenerator);
            this.Getter = Spec.FixCulture(getter);
            this.Setter = Spec.FixCulture(setter);
            this.MoldingAccessCodeGenerator =
                Spec.FixCulture(moldingAccessCodeGenerator);
            this.moldingDefault = moldingDefault;
            this.dictMode = dictMode;
            this.referredSpecs = referredSpecs;
        }

        internal Func<string> TypeGenerator { get; }

        internal Func<AgnosticPath, string, T> Getter { get; }

        internal Func<AgnosticPath, T, string> Setter { get; }

        internal Func<string> MoldingAccessCodeGenerator { get; }

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
                                new ScalarYAML(this.MoldingAccessCodeGenerator());
                        }

                        return this.IndexGet(key);
                    }
                    catch (Spec.HiddenKeyException ex)
                    {
                        if (this.dictMode)
                        {
                            throw ex;
                        }
                        else if (this.parent.IsMolding)
                        {
                            return this.moldingDefault;
                        }
                        else
                        {
                            throw new Spec.InvalidSpecAccessException(
                                $"{this.parent.Properties.ID}[{key}]",
                                this.TypeGenerator(),
                                ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.parent.IsMolding)
                        {
                            return this.moldingDefault;
                        }

                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            this.TypeGenerator(),
                            ex);
                    }
                }
            }

            set
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        var scalarVal = Spec.ToScalar(
                            this.Setter(this.parent.Path, value));
                        this.parent.Properties[key] =
                            new ScalarYAML(scalarVal);
                    }
                    catch (Exception ex)
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            this.TypeGenerator(),
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
                this.Setter(this.parent.Path, defaultVal)];
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
                () => this.Getter(this.parent.Path, defaultValInSpec),
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
                                this.MoldingAccessCodeGenerator() +
                                ", " +
                                Spec.EncodeCommas(
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
                    catch (Spec.DefaultKeyException)
                    {
                        return lazyDefaultVal();
                    }
                    catch (Spec.HiddenKeyException ex)
                    {
                        if (this.dictMode)
                        {
                            throw ex;
                        }
                        else
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

                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            this.TypeGenerator(),
                            ex);
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
                var val = Spec.FromScalar(
                    this.parent.Properties.Scalar[key].Value);
                return this.Getter(this.parent.Path, val);
            }
            catch (Spec.MagicWordException ex)
            {
                if (ex.Word == Spec.DEFAULT)
                {
                    throw new Spec.DefaultKeyException(this.parent.ID, key);
                }

                if (ex.Word == Spec.HIDDEN)
                {
                    throw new Spec.HiddenKeyException(this.parent.ID, key);
                }

                throw ex;
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
                    this.TypeGenerator,
                    this.Getter,
                    this.Setter,
                    this.MoldingAccessCodeGenerator,
                    this.moldingDefault,
                    this.dictMode,
                    this.referredSpecs.Add(this.parent.ID))
                    .IndexGet(key);
            }
        }

        internal void Remove(string key)
        {
            this.parent.Properties.Remove(key);
        }

        internal void Hide(string key)
        {
            this.parent.SetMagicWord(key, Spec.HIDDEN);
        }

        internal void ExplicitDefault(string key)
        {
            this.parent.SetMagicWord(key, Spec.DEFAULT);
        }
    }
}
