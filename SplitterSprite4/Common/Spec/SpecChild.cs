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
    /// Specファイルの子孫Specを表現するSpecクラス
    /// The accessor class for descendant spec.
    /// </summary>
    public class SpecChild : Spec
    {
        private Spec parent;
        private string accessKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecChild"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="key">The string key for this child spec.</param>
        internal SpecChild(Spec parent, string key)
        {
            this.parent = parent;
            this.accessKey = key;
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
                    var childYaml = this.parent.Mold.Mapping[
                        this.accessKey, new MappingYAML()];
                    this.parent.Mold.Mapping[this.accessKey] = childYaml;

                    return this.parent.Mold.Mapping[this.accessKey];
                }
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Body
        {
            get
            {
                var childYaml = this.parent.Body.Mapping[
                    this.accessKey, new MappingYAML()];
                childYaml.ID = $"{this.parent.Body.ID}[{this.accessKey}]";
                this.parent.Body.Mapping[this.accessKey] = childYaml;

                return this.parent.Body.Mapping[this.accessKey];
            }
        }

        /// <inheritdoc/>
        public override OutSideProxy Proxy
        {
            get => this.parent.Proxy;
        }
    }
}
