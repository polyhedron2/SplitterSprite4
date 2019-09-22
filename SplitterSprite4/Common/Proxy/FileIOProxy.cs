// -----------------------------------------------------------------------
// <copyright file="FileIOProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// ゲームの設定ファイル、メディアファイル、プログラムファイルへの
    /// アクセスを一括管理するプロキシクラス
    /// The proxy class for game's configuration (spec) files, media files,
    /// and program files.
    /// </summary>
    public abstract class FileIOProxy
    {
        /// <summary>
        /// Gets the directory path which contains the game executable file.
        /// </summary>
        public string RootPath { get; private set; } =
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        /// <summary>
        /// OS非依存パス先のファイルを格納するディレクトリを作成
        /// Create the directory that contains the os-agnostic path.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        public abstract void CreateDirectory(AgnosticPath path);

        /// <summary>
        /// テスト実行中はRootPathをシステムのTEMPファイル領域に切り替える
        /// Switch the RootPath to temporary directory while testing.
        /// </summary>
        /// <param name="testAction">
        /// テスト実行するActionインスタンス
        /// The Action instance for testing.
        /// </param>
        public void WithTestMode(Action testAction)
        {
            // テスト並列実行時に干渉しあわないよう、スレッドロックを取得
            // Thread lock for parallel testing.
            lock (this)
            {
                var prevRootPath = this.RootPath;
                try
                {
                    var testDirName = "SplitterSpriteTest";

                    // TEMP領域にテスト用フォルダを作成
                    // Create temporary directory for testing.
                    this.RootPath = Path.Combine(
                        Path.GetTempPath(),
                        testDirName,
                        Path.GetRandomFileName());

                    Directory.CreateDirectory(this.RootPath);

                    try
                    {
                        // テスト実行
                        // Testing.
                        testAction();
                    }
                    finally
                    {
                        // テスト用フォルダの親フォルダごと削除を試みる
                        // 前のテストで削除できなかったフォルダも削除される
                        // Attempt to delete the testing directory.
                        this.TryDeleteDirectory(this.RootPath);
                    }
                }
                finally
                {
                    // 例外が発生してもRootPathは元に戻す
                    // Make RootPath back to the original.
                    this.RootPath = prevRootPath;
                }
            }
        }

        /// <summary>
        /// OS依存なフルパスとして出力
        /// Dump as OS-dependent full-path.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <returns>The full-path.</returns>
        public string OSFullPath(AgnosticPath path) =>
            Path.Combine(this.RootPath, path.ToOSPathString());

        /// <summary>
        /// OS依存なフルパスのディレクトリ名を出力
        /// Dump the directory of OS-dependent full-path.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <returns>The direcotry path.</returns>
        public string OSFullDirPath(AgnosticPath path) =>
            Path.GetDirectoryName(this.OSFullPath(path));

        /// <summary>
        /// Pythonのwith構文ライクにテキストリーダを使用
        /// Use text reader instance like Python's with-statement.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <param name="action">
        /// テキストリーダ利用Action
        /// The Action instance which use the text reader.
        /// </param>
        public void WithTextReader(
            AgnosticPath path, Action<TextReader> action)
        {
            using (var reader = this.FetchTextReader(path))
            {
                action(reader);
            }
        }

        /// <summary>
        /// Pythonのwith構文ライクにテキストリーダを使用
        /// Use text reader instance like Python's with-statement.
        /// </summary>
        /// <typeparam name="T_Result">The Func's result type.</typeparam>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <param name="func">
        /// テキストリーダ利用Func
        /// The Func instance which use the text reader.
        /// </param>
        /// <returns>The Func's result.</returns>
        public T_Result WithTextReader<T_Result>(
            AgnosticPath path, Func<TextReader, T_Result> func)
        {
            using (var reader = this.FetchTextReader(path))
            {
                return func(reader);
            }
        }

        /// <summary>
        /// Pythonのwith構文ライクにテキストライタを使用
        /// Use text writer instance like Python's with-statement.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <param name="append">
        /// 追記モードか否か
        /// The flag whether append-mode or not.
        /// </param>
        /// <param name="action">
        /// テキストリーダ利用Action
        /// The Action instance which use the text reader.
        /// </param>
        public void WithTextWriter(
            AgnosticPath path, bool append, Action<TextWriter> action)
        {
            using (var writer = this.FetchTextWriter(path, append))
            {
                action(writer);
            }
        }

        /// <summary>
        /// Pythonのwith構文ライクにテキストライタを使用
        /// Use text writer instance like Python's with-statement.
        /// </summary>
        /// <typeparam name="T_Result">The Func's result type.</typeparam>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <param name="append">
        /// 追記モードか否か
        /// The flag whether append-mode or not.
        /// </param>
        /// <param name="func">
        /// テキストリーダ利用Func
        /// The Func instance which use the text reader.
        /// </param>
        /// <returns>The Func's result.</returns>
        public T_Result WithTextWriter<T_Result>(
            AgnosticPath path, bool append, Func<TextWriter, T_Result> func)
        {
            using (var writer = this.FetchTextWriter(path, append))
            {
                return func(writer);
            }
        }

        /// <summary>
        /// OS非依存パス先のファイルに対するテキストリーダを取得
        /// Fetch the text reader instance for the os-agnostic path.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <returns>The text reader instance.</returns>
        protected abstract TextReader FetchTextReader(AgnosticPath path);

        /// <summary>
        /// OS非依存パス先のファイルに対するテキストライタを取得
        /// Fetch the text writer instance for the os-agnostic path.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <param name="append">
        /// 追記モードか否か
        /// The flag whether append-mode or not.
        /// </param>
        /// <returns>The text writer instance.</returns>
        protected abstract TextWriter FetchTextWriter(
            AgnosticPath path, bool append);

        private void TryDeleteDirectory(string path)
        {
            try
            {
                // テスト用フォルダの削除を試みる
                // Attempt to delete testing directory.
                Directory.Delete(path, true);
            }
            catch
            {
                // TEMP配下なので削除失敗は気にしない。
                // Failure is ignored, because it is temporary directory.
            }
        }
    }
}
