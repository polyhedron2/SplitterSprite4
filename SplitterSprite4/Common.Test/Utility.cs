// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System.IO;
    using MagicKitchen.SplitterSprite4.Common.Proxy;

    /// <summary>
    /// テスト用汎用実装
    /// Utility class for tests.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Join some lines into a string.
        /// </summary>
        /// <param name="lines">line strings.</param>
        /// <returns>The joined string.</returns>
        public static string JoinLines(params string[] lines)
        {
            return string.Join("\n", lines);
        }

        /// <summary>
        /// Create a OutSideProxy for unit testing.
        /// </summary>
        /// <returns>OutSideProxy for unit testing.</returns>
        public static OutSideProxy TestOutSideProxy()
        {
            return new OutSideProxy(new TestFileIOProxy());
        }

        /// <summary>
        /// Create a OutSideProxy whose spec pool is cleared.
        /// </summary>
        /// <param name="proxy">Original OutSideProxy.</param>
        /// <returns>Pool cleared proxy.</returns>
        public static OutSideProxy PoolClearedProxy(OutSideProxy proxy)
        {
            return new OutSideProxy(proxy.FileIO);
        }

        private class TestFileIOProxy : RealFileIOProxy
        {
            internal TestFileIOProxy()
            {
                this.ID = System.Diagnostics.Process.GetCurrentProcess().Id;
                this.TestDir = Path.Combine(
                    Path.GetTempPath(), "SplitterSpriteTest");
                this.RootPath = Path.Combine(
                    this.TestDir, $"{Path.GetRandomFileName()}_{this.ID}");

                Directory.CreateDirectory(this.RootPath);
            }

            ~TestFileIOProxy()
            {
                try
                {
                    // テスト用フォルダの削除を試みる
                    // Attempt to delete testing directory.
                    Directory.Delete(this.RootPath, true);

                    // 別テスト実行時の削除失敗フォルダがあれば削除を試みる
                    // Attempt to delete previous testing directories.
                    foreach (var subDir in
                        Directory.EnumerateDirectories(this.TestDir))
                    {
                        int id = int.Parse(
                            subDir.Split('_')[subDir.Split('_').Length - 1]);
                        if (id != this.ID)
                        {
                            Directory.Delete(subDir, true);
                        }
                    }
                }
                catch
                {
                    // TEMP配下なので削除失敗は気にしない。
                    // Failure is ignored, because it is temporary directory.
                }
            }

            private string TestDir { get; }

            private int ID { get; }
        }
    }
}
