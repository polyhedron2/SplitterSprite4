using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common
{
    // ゲームの実行ファイルのあるディレクトリを起点とした相対パスを
    // OS非依存(OS-Agnostic)に実現するクラス
    public class AgnosticPath
    {
        // ゲームの実行ファイルのあるディレクトリパス
        static string RootPath { get; } =
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // 各種OSでファイル名・ディレクトリ名に使用してはいけない禁止文字の集合
        static char[] ProhibitedChars { get; } =
        {
            // for Windows
            '\\', '*', '?', '"', '<', '>', '|', ':', 
            // for Linux
            '\0',
            // for Windows, Mac, and Linux
            '/',
        };

        // OS毎に異なるファイル区切り文字を置き換えるための文字
        // '/'を用いるので、実態はLinuxスタイルのファイルパスに一致する。
        public static char InternalSeparatorChar { get; } = '/';

        private AgnosticPath(string path, char separatorChar)
        {
            // 禁止文字チェックと区切り文字置換処理
            foreach (var c in ProhibitedChars)
            {
                // 禁止文字がファイル区切り文字以外として入っていれば例外
                if (c != separatorChar && path.Contains(c))
                {
                    throw new ProhibitedCharacterContainedException(path, c);
                }
            }

            // OS毎に異なるファイル区切り文字を置換
            InternalPath = path.Replace(separatorChar, InternalSeparatorChar);
        }

        string InternalPath { get; set; }

        // OS非依存化された文字列からのオブジェクト生成
        public static AgnosticPath FromAgnosticPathString(
            string agnosticPath) =>
            new AgnosticPath(agnosticPath, InternalSeparatorChar);

        // OS依存な文字列からのオブジェクト生成
        public static AgnosticPath FromOSPathString(
            string agnosticPath) =>
            new AgnosticPath(agnosticPath, Path.DirectorySeparatorChar);

        // OS非依存化された文字列として出力
        public string ToAgnosticPathString() => InternalPath;

        // OS依存なファイルパスとして出力
        public string ToOSPathString() => InternalPath.Replace(
            InternalSeparatorChar, Path.DirectorySeparatorChar);

        // OS依存なフルパスとして出力
        public string ToOSFullPathString() =>
            Path.Combine(RootPath, ToOSPathString());

        // OS依存なフルパスのディレクトリ名として出力
        public string ToOSFullDirPathString() =>
            Path.GetDirectoryName(ToOSFullPathString());

        public class ProhibitedCharacterContainedException : Exception
        {
            public ProhibitedCharacterContainedException(
                string path, char containedChar) : base(
                    $"ファイルパス\"{path}\"に" +
                    $"禁止文字'{containedChar}'が含まれています。")
            {
                Path = path;
                ContainedChar = containedChar;
            }

            public string Path { get; private set;}
            public char ContainedChar { get; private set; }
        }
    }
}