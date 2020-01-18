// -----------------------------------------------------------------------
// <copyright file="SpecChild.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using System;
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
        public override Spec Base
        {
            get => this.parent.Base?.Child[this.accessKey, this.bound];
        }

        /// <summary>
        /// Gets or sets spawner type for this spec.
        /// </summary>
        public Type SpawnerType
        {
            get
            {
                lock (this.Body)
                {
                    try
                    {
                        var type = Type.GetType(
                            this.Body.Scalar["spawner"].Value, true);
                        this.ValidateSpawnerType(this.bound, type);

                        return type;
                    }
                    catch (YAML.YAMLKeyUndefinedException)
                    {
                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSpecAccessException(
                            $"{this.ID}[spawner]", "Spawner", ex);
                    }
                }
            }

            set
            {
                lock (this.Body)
                {
                    try
                    {
                        this.ValidateSpawnerType(this.bound, value);

                        this.Body.Scalar["spawner"] = new ScalarYAML(
                            $"{value.FullName}, " +
                            $"{value.Assembly.GetName().Name}");
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

                    mold.Scalar["spawner"] = new ScalarYAML(
                        $"Spawner, {this.bound.FullName}," +
                        $" {this.bound.Assembly.GetName().Name}");

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
    }
}
