// -----------------------------------------------------------------------
// <copyright file="AgnosticPath.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// ゲームの実行ファイルのあるディレクトリを起点とした相対パスを
    /// OS非依存(OS-Agnostic)に実現するクラス
    /// The os-agnostic relative file path class
    /// from the game's execution file.
    /// </summary>
    public class AgnosticPath
    {
        private AgnosticPath(string path, char separatorChar)
        {
            // 禁止文字チェック
            // Check the prohibited characters.
            foreach (var c in ProhibitedChars)
            {
                // OS依存ファイル区切り文字であれば区切り文字として
                // 入っているので例外とならない
                // If the prohibited character is file separator,
                // no exception will be thrown.
                if (c != separatorChar && path.Contains(c))
                {
                    throw new ProhibitedCharacterContainedException(path, c);
                }
            }

            // OS毎に異なるファイル区切り文字を置換
            // Replace the os-dependent file separator.
            this.InternalPath = path.Replace(separatorChar, InternalSeparatorChar);
        }

        /// <summary>
        /// Gets the internal file separator character.
        /// Os-dependent file separators are replaced with this.
        /// Actually, the replaced file paths are linux-style.
        /// </summary>
        public static char InternalSeparatorChar { get; } = '/';

        // Prohibited characters for file name.
        private static char[] ProhibitedChars { get; } =
        {
            // for Windows
            '\\', '*', '?', '"', '<', '>', '|', ':',

            // for Linux
            '\0',

            // for Windows, Mac, and Linux
            '/',
        };

        private string InternalPath { get; set; }

        /// <summary>
        /// OS非依存化された文字列からのオブジェクト生成
        /// Generate AgnosticPath instance from os-agnostic path string.
        /// </summary>
        /// <param name="agnosticPath">The os-agnostic path string.</param>
        /// <returns>The AgnosticPath instance.</returns>
        public static AgnosticPath FromAgnosticPathString(
            string agnosticPath) =>
            new AgnosticPath(agnosticPath, InternalSeparatorChar);

        /// <summary>
        /// OS依存な文字列からのオブジェクト生成
        /// Generate AgnosticPath instance from os-dependent path string.
        /// </summary>
        /// <param name="agnosticPath">The os-dependent path string.</param>
        /// <returns>The AgnosticPath instance.</returns>
        public static AgnosticPath FromOSPathString(
            string agnosticPath) =>
            new AgnosticPath(agnosticPath, Path.DirectorySeparatorChar);

        /// <summary>
        /// OS非依存化されたファイルパスとして出力
        /// Dump as os-agnostic path string.
        /// </summary>
        /// <returns>The os-agnostic path string.</returns>
        public string ToAgnosticPathString() => this.InternalPath;

        /// <summary>
        /// OS依存なファイルパスとして出力
        /// Dump as os-dependent path string.
        /// </summary>
        /// <returns>The os-dependent path string.</returns>
        public string ToOSPathString() => this.InternalPath.Replace(
            InternalSeparatorChar, Path.DirectorySeparatorChar);

        /// <summary>
        /// 禁止文字がファイルパス内に含まれている際の例外
        /// The exception that is thrown when an attempt to
        /// load file path string that contains a prohibited character.
        /// </summary>
        public class ProhibitedCharacterContainedException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProhibitedCharacterContainedException"/> class.
            /// </summary>
            /// <param name="path">The file path string.</param>
            /// <param name="containedChar">The contained prohibited character.</param>
            public ProhibitedCharacterContainedException(
                string path, char containedChar)
                : base(
                    $"ファイルパス\"{path}\"に" +
                    $"禁止文字'{containedChar}'が含まれています。")
            {
                this.Path = path;
                this.ContainedChar = containedChar;
            }

            /// <summary>
            /// Gets the path string.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Gets the contained prohibited character.
            /// </summary>
            public char ContainedChar { get; private set; }
        }
    }
}