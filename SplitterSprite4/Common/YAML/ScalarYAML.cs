// -----------------------------------------------------------------------
// <copyright file="ScalarYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;
    using System.Collections.Generic;
    using YamlDotNet.RepresentationModel;

    /// <summary>
    /// スカラー型のYAMLを表現するクラス
    /// The YAML class for scalar type YAML.
    /// </summary>
    public class ScalarYAML : YAML
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarYAML"/> class.
        /// </summary>
        /// <param name="valueLines">The value string lines.</param>
        public ScalarYAML(params string[] valueLines)
        {
            this.Value = string.Join("\n", valueLines);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarYAML"/> class.
        /// </summary>
        /// <param name="id">The yaml's ID.</param>
        /// <param name="scalar">YamlDotNet scalar instance.</param>
        public ScalarYAML(string id, YamlScalarNode scalar)
        {
            this.ID = id;
            if (scalar.Value.EndsWith("\n"))
            {
                // YAMLの複数行入力の場合、末尾に改行が自動挿入されるため削除
                // In multi-line value case, remove
                // an auto inserted newline character.
                this.Value =
                    scalar.Value.Substring(0, scalar.Value.Length - 1);
            }
            else
            {
                this.Value = scalar.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the scalar is multi line or not.
        /// </summary>
        public bool IsMultiLine
        {
            get => this.Value.Contains("\n");
        }

        private string Value { get; set; }

        /// <inheritdoc/>
        public override IEnumerable<string> ToStringLines()
        {
            return this.Value.Split(
                new string[] { "\n" }, StringSplitOptions.None);
        }
    }
}
