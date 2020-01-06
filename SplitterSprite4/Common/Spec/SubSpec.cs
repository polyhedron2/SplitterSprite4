// -----------------------------------------------------------------------
// <copyright file="SubSpec.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// SpecRootやSpecChildの中での名前空間の分割を表現するSpec。
    /// Spec class which is used to represent namespaces in SpecRoot or SpecChild.
    /// </summary>
    public class SubSpec : Spec
    {
        private Spec parent;
        private string accessKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSpec"/> class.
        /// </summary>
        /// <param name="parent">The parent spec instance.</param>
        /// <param name="key">The string key for this sub spec.</param>
        internal SubSpec(Spec parent, string key)
        {
            this.parent = parent;
            this.accessKey = key;
        }

        /// <inheritdoc/>
        public override Spec Base
        {
            get => this.parent.Base?.SubSpec[this.accessKey];
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

                    return childYaml;
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
            get => this.Body;
        }

        /// <inheritdoc/>
        public override OutSideProxy Proxy
        {
            get => this.parent.Proxy;
        }
    }
}
