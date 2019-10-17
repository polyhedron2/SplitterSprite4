// -----------------------------------------------------------------------
// <copyright file="FileIOProxy.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    using System;
    using System.Collections.Generic;
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
        /// Gets or sets the directory path which contains
        /// the game executable file.
        /// </summary>
        public string RootPath { get; protected set; } =
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
        /// OS非依存パス先のサブフォルダ一覧を返却
        /// Enumearate sub directories in the os-agnostic path directory.
        /// </summary>
        /// <param name="path">
        /// OS非依存パス
        /// The os-agnostic path.
        /// </param>
        /// <returns>
        /// サブフォルダのOS非依存パスイテレータ
        /// Iterator of the sub directories.
        /// </returns>
        public abstract IEnumerable<AgnosticPath>
            EnumerateDirectories(AgnosticPath path);

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
    }
}
