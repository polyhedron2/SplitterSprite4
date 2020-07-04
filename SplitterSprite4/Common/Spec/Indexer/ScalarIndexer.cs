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
    internal class ScalarIndexer<T>
    {
        private static readonly string MAGICWORDDECORATOR = "__";
        private static readonly string HIDDEN = "HIDDEN";
        private Spec parent;
        private T moldingDefault;
        private ImmutableList<string> referredSpecs;
        private bool allowHiddenValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="typeGenerator">The access type string generator func.</param>
        /// <param name="getter">Translation function from spec path and string value in spec to indexed value.</param>
        /// <param name="setter">Translation function from spec path and indexed value to string value in spec.</param>
        /// <param name="moldingAccessCodeGenerator">The generator func of type and parameter information for molding.</param>
        /// <param name="moldingDefault">The default value for molding.</param>
        /// <param name="allowHiddenValue">This spec allows hidden value or not.</param>
        /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
        internal ScalarIndexer(
            Spec parent,
            Func<string> typeGenerator,
            Func<AgnosticPath, string, T> getter,
            Func<AgnosticPath, T, string> setter,
            Func<string> moldingAccessCodeGenerator,
            T moldingDefault,
            bool allowHiddenValue,
            ImmutableList<string> referredSpecs)
        {
            this.parent = parent;
            this.TypeGenerator = Spec.FixCulture(typeGenerator);
            this.Getter = Spec.FixCulture(getter);
            this.Setter = Spec.FixCulture(setter);
            this.MoldingAccessCodeGenerator =
                Spec.FixCulture(moldingAccessCodeGenerator);
            this.moldingDefault = moldingDefault;
            this.allowHiddenValue = allowHiddenValue;
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
                    catch (HiddenKeyException ex)
                    {
                        if (this.allowHiddenValue)
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
                        var scalarVal = this.ToScalar(
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
                    catch (HiddenKeyException ex)
                    {
                        if (this.allowHiddenValue)
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
                var val = this.FromScalar(
                    this.parent.Properties.Scalar[key].Value);
                return this.Getter(this.parent.Path, val);
            }
            catch (MagicWordException ex)
            {
                if (ex.Word == HIDDEN)
                {
                    throw new HiddenKeyException(this.parent.ID, key);
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
                    this.allowHiddenValue,
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
            this.SetMagicWord(key, HIDDEN);
        }

        private void SetMagicWord(string key, string word)
        {
            // Magic word decorated word cannot be magic word.
            if (this.CheckMagicWord(word))
            {
                throw new InvalidMagicWordException(word);
            }

            lock (this.parent.Properties)
            {
                try
                {
                    var scalarVal = this.DecorateAsMagicWord(word);
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

        private bool CheckMagicWord(string val)
        {
            return
                val.StartsWith(MAGICWORDDECORATOR) &&
                val.EndsWith(MAGICWORDDECORATOR);
        }

        private string DecorateAsMagicWord(string val)
        {
            return MAGICWORDDECORATOR + val + MAGICWORDDECORATOR;
        }

        private string UndecorateAsMagicWord(string val)
        {
            return val.Substring(
                MAGICWORDDECORATOR.Length,
                val.Length - (2 * MAGICWORDDECORATOR.Length));
        }

        private string FromScalar(string scalar)
        {
            // Basically, the original scalar string is used.
            if (!this.CheckMagicWord(scalar))
            {
                return scalar;
            }

            var innerStr = this.UndecorateAsMagicWord(scalar);

            // In __xxx__ case, xxx is used as magic word.
            if (!this.CheckMagicWord(innerStr))
            {
                throw new MagicWordException(innerStr);
            }

            // If you want to use __xxx__ as non-magic word,
            // ____xxx____ in ScalarYAML works.
            return innerStr;
        }

        private string ToScalar(string value)
        {
            // Basically, the original value is used.
            if (!this.CheckMagicWord(value))
            {
                return value;
            }

            // In __xxx__ case, ____xxx____ will be in ScalarYAML.
            return this.DecorateAsMagicWord(value);
        }

        private string ToMagicScalar(string word)
        {
            // "__xxx__" is used for magic word "xxx".
            return this.DecorateAsMagicWord(word);
        }

        // Exception for special definition values.
        internal class MagicWordException : Exception
        {
            internal MagicWordException(string word)
            {
                this.Word = word;
            }

            internal string Word { get; }
        }

        internal class InvalidMagicWordException : Exception
        {
            internal InvalidMagicWordException(string word)
                : base($"{word}はMagic Wordの対象として不正です。")
            {
            }
        }

        internal class HiddenKeyException : Exception
        {
            internal HiddenKeyException(string id, string key)
                : base($"\"{id}\"上のキー\"{key}\"は隠蔽されています。")
            {
            }
        }
    }
}
