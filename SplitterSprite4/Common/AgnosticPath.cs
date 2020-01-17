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

            // 禁止文字列チェック
            // Check the dummy phrase.
            if (path.Split(separatorChar).Contains(DummyPhrase))
            {
                throw new DummyPhraseContainedException(path);
            }

            // OS毎に異なるファイル区切り文字を置換
            // Replace the os-dependent file separator.
            this.InternalPath = path.Replace(separatorChar, InternalSeparatorChar);

            // 末尾に区切り文字を含まない形を正規形とする。
            // Canonical form doesn't have separator char at the end of it.
            if (this.InternalPath.EndsWith(InternalSeparatorChar))
            {
                this.InternalPath = this.InternalPath.Substring(
                    0, this.InternalPath.Length - 1);
            }
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
                    dummyPrefixedUriStr.EndsWith(InternalSeparatorChar) ?
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

        /// <summary>
        /// Gets a value indicating whether this path not including "../".
        /// </summary>
        public bool IsOnlyDescending
        {
            get => !this.ToAgnosticPathString().StartsWith("../");
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
            get => Uri.UnescapeDataString(this.InternalUri.ToString());
            set
            {
                this.InternalUri = DummyUri.MakeRelativeUri(
                    new Uri(DummyUri, value));
            }
        }

        /// <summary>
        /// OS非依存パス同士の結合。
        /// 右オペランドを起点として左オペランドを追加する。
        /// 結合律が成立する。即ち、((a + b) + c).Equals(a + (b + c))が成立する。
        /// Combine two os-agnostic paths.
        /// Add LEFT operand to RIGHT operand.
        /// This operation is associative.
        /// i.e.: ((a + b) + c).Equals(a + (b + c)).
        /// </summary>
        /// <param name="relative">The left operand path. This is relative path.</param>
        /// <param name="startingPoint">The right operand path. This is starting point path.</param>
        /// <returns>Combined path.</returns>
        public static AgnosticPath operator +(
            AgnosticPath relative, AgnosticPath startingPoint)
        {
            return AgnosticPath.FromOSPathString(Path.Combine(
                startingPoint.ToOSPathString(), relative.ToOSPathString()));
        }

        /// <summary>
        /// OS非依存パス間の相対パスを取得。
        /// 右オペランドから見た左オペランドの相対位置を返す。
        /// ((a + b) - b).Equals(a)かつ((a - b) + b).Equals(a)である。
        /// ((a + b) - c).Equals(a + (b - c))とは限らないことに注意。
        /// 例：
        ///   ("../foo" + "bar") - "foo" = "foo" - "foo" = ""
        ///   "../foo" + ("bar" - "foo") = "../foo" + "../bar" = "../foo"
        /// Calculate relative path between two os-agnostic pathes.
        /// Returns relative path from RIGHT operand to LEFT operand.
        /// `((a + b) - b).Equals(a)` and `((a - b) + b).Equals(a)` is true.
        /// `((a + b) - c).Equals(a + (b - c))` can be false.
        /// E.g.：
        ///   ("../foo" + "bar") - "foo" = "foo" - "foo" = ""
        ///   "../foo" + ("bar" - "foo") = "../foo" + "../bar" = "../foo".
        /// </summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="origin">The original path. It must not contain "../".</param>
        /// <returns>Relative path.</returns>
        public static AgnosticPath operator -(
            AgnosticPath destination, AgnosticPath origin)
        {
            var destUri = new Uri(DummyUri, destination.ToAgnosticPathString());
            var origUri = new Uri(DummyUri, origin.ToAgnosticPathString());

            // origin pathはディレクトリとして解釈するため、'/'をつける。
            // Add '/' to interpret origin path as directory.
            if (!origUri.ToString().EndsWith('/'))
            {
                origUri = new Uri(origUri.ToString() + '/');
            }

            // destination pathとorigin pathの相対関係を維持するため、
            // destination pathにも'/'をつける。
            // Add '/' to interpret dest path as same as origin path.
            if (!destUri.ToString().EndsWith('/'))
            {
                destUri = new Uri(destUri.ToString() + '/');
            }

            var relativeUri = origUri.MakeRelativeUri(destUri);
            var relativeStr = Uri.UnescapeDataString(relativeUri.ToString());

            try
            {
                return new AgnosticPath(relativeStr, InternalSeparatorChar);
            }
            catch (DummyPhraseContainedException ex)
            {
                throw new IndeterminateSubtractionException(
                    destination.ToAgnosticPathString(),
                    origin.ToAgnosticPathString(),
                    ex);
            }
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case AgnosticPath that:
                    return this.ToAgnosticPathString()
                        == that.ToAgnosticPathString();
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ToAgnosticPathString().GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.ToAgnosticPathString();
        }

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

        /// <summary>
        /// 内部処理用のダミー文字列を含むパスを指定した際の例外
        /// The exception that is thrown when an attempt to
        /// create os-agnostic path which contains dummy phrase for internal processing.
        /// </summary>
        public class DummyPhraseContainedException : Exception
        {
            private string path;

            /// <summary>
            /// Initializes a new instance of the <see cref="DummyPhraseContainedException"/> class.
            /// </summary>
            /// <param name="path">The path which conatins dummy phrase.</param>
            public DummyPhraseContainedException(string path)
                : base($"OS非依存パス\"{path}\"に禁止文字列\"{DummyPhrase}\"が含まれています。")
            {
                this.path = path;
            }

            /// <summary>
            /// Gets the path which contains dummy pharse.
            /// </summary>
            public string Path { get => this.path; }
        }

        /// <summary>
        /// 減算の結果が定まらない際の例外
        /// The exception that is thrown when an attempt to
        /// subtract paths whose result is not well-defined.
        /// </summary>
        public class IndeterminateSubtractionException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="IndeterminateSubtractionException"/> class.
            /// </summary>
            /// <param name="left">The left operand.</param>
            /// <param name="right">The right operand.</param>
            /// <param name="innerException">The exception which caused this.</param>
            public IndeterminateSubtractionException(
                string left, string right, Exception innerException)
                : base(
                      $"OS非依存パスの減算\"{left}\" - \"{right}\"の" +
                      $"結果は不定です。",
                      innerException)
            {
            }
        }
    }
}