// -----------------------------------------------------------------------
// <copyright file="SpecChild.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecChild"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="key">The string key for this spec child.</param>
        internal SpecChild(Spec parent, string key)
        {
            this.parent = parent;
            this.accessKey = key;
        }

        /// <inheritdoc/>
        public override Spec Base
        {
            get => this.parent.Base?.Child[this.accessKey];
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

                    var ret = mold.Mapping["properties", new MappingYAML()];
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
                var childYaml = this.parent.Properties.Mapping[
                    this.accessKey, new MappingYAML()];
                childYaml.ID =
                    $"{this.parent.Properties.ID}[{this.accessKey}]";
                this.parent.Properties.Mapping[this.accessKey] = childYaml;

                return childYaml;
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Properties
        {
            get
            {
                var ret = this.Body.Mapping["properties", new MappingYAML()];
                ret.ID = $"{this.Body.ID}[properties]";
                this.Body.Mapping["properties"] = ret;
                return ret;
            }
        }

        /// <inheritdoc/>
        public override OutSideProxy Proxy
        {
            get => this.parent.Proxy;
        }
    }
}
