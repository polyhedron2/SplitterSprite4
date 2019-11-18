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
    /// ファイルパスをOS非依存(OS-Agnostic)に実現するクラス
    /// The os-agnostic file path class.
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

        /// <summary>
        /// Gets the os-agnostic path of a directory that contains this path.
        /// </summary>
        public AgnosticPath Parent
        {
            get
            {
                var dummyPrefixedUri = new Uri(DummyUri, this.InternalUri);
                var dummyPrefixedUriStr = dummyPrefixedUri.ToString();
                dummyPrefixedUriStr =
                    dummyPrefixedUriStr.EndsWith("/") ?
                    dummyPrefixedUriStr.Substring(
                        0, dummyPrefixedUriStr.Length - 1) :
                    dummyPrefixedUriStr;
                var parentLength = dummyPrefixedUriStr.LastIndexOf(
                    InternalSeparatorChar) + 1;
                var dummyPrefixedParent = new Uri(
                    dummyPrefixedUri.ToString().Substring(0, parentLength));
                var parentUri = DummyUri.MakeRelativeUri(dummyPrefixedParent);

                return FromAgnosticPathString(parentUri.ToString());
            }
        }

        private static string DummyPhrase { get; } =
            "DummyPhraseForSplitterSpriteAgnosticPath";

        // Prohibited characters for file name.
        private static char[] ProhibitedChars { get; } =
        {
            // for Windows
            // ':'もファイル名禁止文字だがボリューム名("C:"など)表現のため除外
            // ':' cannot be contained in windows file name,
            // but not prohibited for volume name.
            '\\', '*', '?', '"', '<', '>', '|',

            // for Linux
            '\0',

            // for Windows, Mac, and Linux
            '/',
        };

        private static Uri DummyUri { get; } = new Uri("http://" +
            string.Concat(Enumerable.Repeat(DummyPhrase + "/", 100)));

        // ファイルパス正規化のためURIインスタンスとして保持する
        // Store as Uri instance for canonicalization.
        private Uri InternalUri { get; set; }

        private string InternalPath
        {
            get => this.InternalUri.ToString().Replace("%25", "%");
            set
            {
                this.InternalUri = DummyUri.MakeRelativeUri(
                    new Uri(DummyUri, value.Replace("%", "%25")));
            }
        }

        /// <summary>
        /// OS非依存パス同士の結合
        /// Combine two os-agnostic paths.
        /// </summary>
        /// <param name="a">The first operand path.</param>
        /// <param name="b">The second operand path.</param>
        /// <returns>Combined path.</returns>
        public static AgnosticPath operator +(AgnosticPath a, AgnosticPath b)
        {
            return AgnosticPath.FromOSPathString(Path.Combine(
                a.ToOSPathString(), b.ToOSPathString()));
        }

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
        /// <param name="osPath">The os-dependent path string.</param>
        /// <returns>The AgnosticPath instance.</returns>
        public static AgnosticPath FromOSPathString(
            string osPath) =>
            new AgnosticPath(osPath, Path.DirectorySeparatorChar);

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