// -----------------------------------------------------------------------
// <copyright file="ScalarYAML.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            // Scalarが複数行テキストの場合、各行の前後のダブルクォートが残るため削除
            // In multi-line text scalar case, remove
            // double quotation at the head and tail of the lines.
            if (this.IsMultiLine)
            {
                this.Value = string.Join(
                    "\n",
                    this.Value.Split(
                        new string[] { "\n" },
                        StringSplitOptions.None).Select(
                        line => line.Substring(1, line.Length - 2)));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the scalar is multi line or not.
        /// </summary>
        public bool IsMultiLine
        {
            get => this.Value.Contains("\n");
        }

        /// <summary>
        /// Gets a value of this Scalar.
        /// </summary>
        public string Value { get; private set; }

        /// <inheritdoc/>
        public override IEnumerable<string> ToStringLines(
            bool ignoreEmptyMappingChild)
        {
            return this.Value.Split(
                new string[] { "\n" }, StringSplitOptions.None).Select(
                line => $"\"{line}\"");
        }
    }
}
