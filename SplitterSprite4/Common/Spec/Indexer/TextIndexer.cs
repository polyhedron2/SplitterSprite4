// -----------------------------------------------------------------------
// <copyright file="TextIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    /// <summary>
    /// Indexer class for multi line string accessor.
    /// </summary>
    public class TextIndexer : LiteralIndexer<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextIndexer"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal TextIndexer(Spec parent, bool dictMode)
            : base(
                parent,
                () => "改行あり文字列",
                (value) =>
                {
                    // "YAML上の最終行が[End Of Text]"で
                    // 終わっていないテキストは不正とする
                    // If the text doesn't end with "\n [End of Text]",
                    // the text is invalid.
                    if (!value.EndsWith("\n[End Of Text]"))
                    {
                        throw new Spec.ValidationError();
                    }

                    // "[End Of Text]"を除去
                    // Remove "[End Of Text]".
                    return value.Substring(
                        0, value.Length - "\n[End Of Text]".Length);
                },
                (value) =>
                {
                    // 単一行のテキストでもYAML上複数行として扱うため、
                    // 末尾に固定のテキストを一行含める。
                    // To text write into YAML as multi line text
                    // even if the text is single line,
                    // add constant line.
                    return value + "\n[End Of Text]";
                },
                () => "Text",
                string.Empty,
                dictMode)
        {
        }
    }
}
