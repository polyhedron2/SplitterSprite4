// -----------------------------------------------------------------------
// <copyright file="SpecChild.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using System;
    using System.Collections.Immutable;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Specファイルの部分集合をSpawn対象として切り分けるSpecクラス。
    /// SpawnerChildインスタンスにより使用される。
    /// The accessor class for partial spec file.
    /// Used by SpawnerChild instance.
    /// </summary>
    public class SpecChild : Spec
    {
        private Spec parent;
        private string accessKey;
        private Type bound;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecChild"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="key">The string key for this spec child.</param>
        /// <param name="type">The SpawnerChild type which this spec child will define.</param>
        internal SpecChild(Spec parent, string key, Type type)
        {
            if (!typeof(ISpawnerChild<object>).IsAssignableFrom(type))
            {
                throw new InvalidSpecDefinitionException(
                    $"SpecChildに指定した型{type}は" +
                    $"ISpawnerChild<object>を継承していません。");
            }

            this.parent = parent;
            this.accessKey = key;
            this.bound = type;
        }

        /// <inheritdoc/>
        public override AgnosticPath Path { get => this.parent.Path; }

        /// <inheritdoc/>
        public override Spec Base
        {
            get => this.BaseAsChild;
        }

        /// <summary>
        /// Gets or sets spawner type for this spec.
        /// </summary>
        public Type SpawnerType
        {
            get => this.SpawnerTypeGetter(ImmutableList<string>.Empty);

            set
            {
                lock (this.Body)
                {
                    try
                    {
                        Spawner.ValidateSpawnerType(this.bound, value);
                        this.Body.Scalar["spawner"] =
                            new ScalarYAML(EncodeType(value));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.ID}[spawner]", "Spawner", ex);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Mold
        {
            get
            {
                if (this.parent.Mold == null)
                {
                    return null;
                }
                else
                {
                    var mold = this.parent.Mold.Mapping[
                        this.accessKey, new MappingYAML()];
                    this.parent.Mold.Mapping[this.accessKey] = mold;

                    var spawnerMold = mold.Scalar[
                        "spawner",
                        new ScalarYAML($"Spawner, {EncodeType(this.bound)}")];
                    mold.Scalar["spawner"] = spawnerMold;

                    var ret = mold.Mapping[
                        "properties", new MappingYAML()];
                    mold.Mapping["properties"] = ret;

                    return ret;
                }
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Body
        {
            get
            {
                lock (this.parent.Properties)
                {
                    var childYaml = this.parent.Properties.Mapping[
                        this.accessKey, new MappingYAML()];
                    childYaml.ID =
                        $"{this.parent.Properties.ID}[{this.accessKey}]";
                    this.parent.Properties.Mapping[this.accessKey] = childYaml;

                    return childYaml;
                }
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Properties
        {
            get
            {
                lock (this.parent.Body)
                {
                    var ret = this.Body.Mapping[
                        "properties", new MappingYAML()];
                    ret.ID = $"{this.Body.ID}[properties]";
                    this.Body.Mapping["properties"] = ret;
                    return ret;
                }
            }
        }

        /// <inheritdoc/>
        internal override OutSideProxy Proxy
        {
            get => this.parent.Proxy;
        }

        /// <summary>
        /// Gets a Spec for inheritance as SpecChild.
        /// If a property is not defined in this spec,
        /// the base spec's property is referred.
        /// If the base spec is not defined, this property is null.
        /// </summary>
        internal SpecChild BaseAsChild
        {
            get => this.parent.Base?.Child[this.accessKey, this.bound];
        }

        /// <summary>
        /// Remove spawner type for this spec.
        /// </summary>
        public void RemoveSpawnerType()
        {
            this.Body.Remove("spawner");
        }

        /// <summary>
        /// Internal spawner getter method for base spec access.
        /// </summary>
        /// <param name="referredSpecs">The spec IDs which are referred while base spec referring.</param>
        /// <returns>Spawner type instance.</returns>
        internal Type SpawnerTypeGetter(ImmutableList<string> referredSpecs)
        {
            lock (this.Body)
            {
                try
                {
                    var type = DecodeType(
                        this.Body.Scalar["spawner"].Value);
                    Spawner.ValidateSpawnerType(this.bound, type);

                    return type;
                }
                catch (YAML.YAMLKeyUndefinedException ex)
                {
                    var isLooped = referredSpecs.Contains(this.Body.ID);
                    if (this.Base == null || isLooped)
                    {
                        throw ex;
                    }

                    return this.BaseAsChild.SpawnerTypeGetter(
                        referredSpecs.Add(this.Body.ID));
                }
                catch (Exception ex)
                {
                    throw new InvalidSpecAccessException(
                        $"{this.ID}[spawner]", "Spawner", ex);
                }
            }
        }
    }
}
