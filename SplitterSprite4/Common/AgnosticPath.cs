using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common
{
    class AgnosticPath
    {
        static char[] ProhibitedChars { get; } =
        {
            // for Windows
            '\\', '*', '?', '"', '<', '>', '|', ':', 
            // for Windows, Mac, and Linux
            '/',
            // for Linux
            '\0',
        };

        static char InternalSeparatorChar { get; } = '/';

        private AgnosticPath(string agnosticPath)
        {
            this.InternalPath = agnosticPath;
        }

        string RootPath { get; } =
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        string InternalPath { get; set; }

        private static AgnosticPath FromPathString(string path, char separatorChar)
        {
            foreach(var c in ProhibitedChars)
            {
                if(c != separatorChar && path.Contains(c))
                {
                    throw new ProhibitedCharacterContainedException(path, c);
                }
            }

            return new AgnosticPath(
                path.Replace(separatorChar, InternalSeparatorChar));
        }

        public static AgnosticPath FromAgnosticPathString(
            string agnosticPath) =>
            FromPathString(agnosticPath, InternalSeparatorChar);

        public static AgnosticPath FromOSPathString(string agnosticPath) =>
            FromPathString(agnosticPath, Path.DirectorySeparatorChar);

        public string ToAgnosticPathString() => InternalPath;
        public string ToOSPathString() => InternalPath.Replace(
            InternalSeparatorChar, Path.DirectorySeparatorChar);
        public string ToOSFullPathString() =>
            Path.Combine(RootPath, ToOSPathString());

        public class ProhibitedCharacterContainedException : Exception
        {
            public ProhibitedCharacterContainedException(
                string path, char containedChar) : base(
                    $"ファイルパス\"{path}\"に" +
                    $"禁止文字'{containedChar}'が含まれています。")
            {
            }
        }
    }
}