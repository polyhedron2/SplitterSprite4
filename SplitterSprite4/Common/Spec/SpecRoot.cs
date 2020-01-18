﻿// -----------------------------------------------------------------------
// <copyright file="SpecRoot.cs" company="MagicKitchen">
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
    /// Specファイル本体を表現するSpecクラス。
    /// SpawnerRootインスタンスにより使用される。
    /// The accessor class for spec file.
    /// Used by SpawnerRoot instance.
    /// </summary>
    public class SpecRoot : Spec
    {
        private MappingYAML mold;
        private RootYAML body;
        private OutSideProxy proxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecRoot"/> class.
        /// </summary>
        /// <param name="proxy">The OutSideProxy for file or spec pool access.</param>
        /// <param name="layeredPath">The spec file path.</param>
        /// <param name="acceptAbsence">Accept absence of the spec file or not.</param>
        internal SpecRoot(
                OutSideProxy proxy,
                AgnosticPath layeredPath,
                bool acceptAbsence = false)
        {
            this.proxy = proxy;
            this.LayeredFile =
                new LayeredFile(proxy.FileIO, layeredPath, acceptAbsence);

            AgnosticPath yamlPath;
            try
            {
                yamlPath = this.LayeredFile.FetchReadPath();
            }
            catch (LayeredFile.LayeredFileNotFoundException)
            {
                yamlPath = this.LayeredFile.FetchWritePath(
                    new Layer(proxy.FileIO, "save", true));
            }

            this.body = new RootYAML(proxy.FileIO, yamlPath, acceptAbsence);
        }

        /// <inheritdoc/>
        public override Spec Base
        {
            get
            {
                lock (this.Body)
                {
                    try
                    {
                        var baseRelativePath = AgnosticPath.FromAgnosticPathString(
                            this.Body.Scalar["base"].Value);
                        var baseLayeredPath =
                            baseRelativePath + this.LayeredFile.Path.Parent;
                        return this.Proxy.SpecPool(baseLayeredPath);
                    }
                    catch (YAML.YAMLKeyUndefinedException)
                    {
                        return null;
                    }
                }
            }
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
                        var type = Type.GetType(this.Body.Scalar["spawner"].Value, true);
                        this.ValidateSpawnerType(typeof(ISpawnerRoot<object>), type);

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
                        this.ValidateSpawnerType(typeof(ISpawnerRoot<object>), value);

                        this.Body.Scalar["spawner"] =
                            new ScalarYAML($"{value.FullName}, {value.Assembly.GetName().Name}");
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
                if (this.mold == null)
                {
                    return null;
                }
                else
                {
                    var ret = this.mold.Mapping["properties", new MappingYAML()];
                    this.mold.Scalar["base"] = new ScalarYAML("Spec");
                    var baseType = typeof(ISpawnerRoot<object>);
                    this.mold.Scalar["spawner"] = new ScalarYAML(
                        $"Spawner, {baseType.FullName}," +
                        $" {baseType.Assembly.GetName().Name}");
                    this.mold.Mapping["properties"] = ret;
                    return ret;
                }
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Body
        {
            get => this.body;
        }

        /// <inheritdoc/>
        public override MappingYAML Properties
        {
            get
            {
                lock (this.Body)
                {
                    var ret = this.Body.Mapping["properties", new MappingYAML()];
                    ret.ID = $"{this.Body.ID}[properties]";
                    this.Body.Mapping["properties"] = ret;
                    return ret;
                }
            }
        }

        /// <inheritdoc/>
        internal override OutSideProxy Proxy
        {
            get => this.proxy;
        }

        private LayeredFile LayeredFile { get; }

        /// <summary>
        /// Save the spec's values into the save layer.
        /// </summary>
        public void Save()
        {
            var saveLayer = new Layer(this.Proxy.FileIO, "save", true);
            saveLayer.IsTop = true;
            saveLayer.Save();
            this.Save(saveLayer);
        }

        /// <summary>
        /// Save the spec's values into the specified layer.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        public void Save(Layer layer)
        {
            this.Save(layer, this.LayeredFile.Path);
        }

        /// <summary>
        /// Save the spec's values into the specified layer and path.
        /// </summary>
        /// <param name="layer">Layer to save the spec.</param>
        /// <param name="layeredPath">Layered path to save the spec.</param>
        public void Save(Layer layer, AgnosticPath layeredPath)
        {
            var writePath = new LayeredFile(
                this.Proxy.FileIO, layeredPath, true).FetchWritePath(layer);

            this.Proxy.FileIO.CreateDirectory(writePath.Parent);
            this.body.SaveAs(writePath, true);
        }

        /// <summary>
        /// Specを利用する処理の設定値アクセスキーと型を取得。
        /// Analyze access key and type in an action for spec.
        /// </summary>
        /// <param name="action">Analysis target action.</param>
        /// <returns>The analysis result yaml.</returns>
        public MappingYAML MoldSpec(Action<SpecRoot> action)
        {
            var prevMold = this.mold;
            var ret = new MappingYAML();

            try
            {
                this.mold = ret;
                action(this);
            }
            finally
            {
                this.mold = prevMold;
            }

            return ret;
        }

        /// <summary>
        /// BaseとなるSpecを更新する。
        /// Update base spec.
        /// </summary>
        /// <param name="newBase">New base spec for udpating.</param>
        public void UpdateBase(SpecRoot newBase)
        {
            lock (this.Body)
            {
                var baseRelativePath =
                    newBase.LayeredFile.Path - this.LayeredFile.Path.Parent;
                this.Body.Scalar["base"] = new ScalarYAML(
                    baseRelativePath.ToAgnosticPathString());
            }
        }
    }
}
