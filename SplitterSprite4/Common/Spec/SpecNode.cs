// -----------------------------------------------------------------------
// <copyright file="SpecNode.cs" company="MagicKitchen">
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
    public class SpecNode : Spec
    {
        private Spec parent;
        private string accessKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecNode"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="key">The string key for this child spec.</param>
        internal SpecNode(Spec parent, string key)
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
                var childYaml = this.parent.Properties.Mapping[
                    this.accessKey, new MappingYAML()];
                childYaml.ID =
                    $"{this.parent.Properties.ID}[{this.accessKey}]";
                this.parent.Properties.Mapping[this.accessKey] = childYaml;

                return this.parent.Properties.Mapping[this.accessKey];
            }
        }

        /// <inheritdoc/>
        public override MappingYAML Properties
        {
            get => this.Body;
        }

        /// <inheritdoc/>
        public override OutSideProxy Proxy
        {
            get => this.parent.Proxy;
        }
    }
}
