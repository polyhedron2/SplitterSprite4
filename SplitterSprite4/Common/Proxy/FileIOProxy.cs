using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.Proxy
{
    // ゲームの設定ファイル、メディアファイル、プログラムファイルへのアクセスを
    // 一括管理するプロキシクラス
    public abstract class FileIOProxy
    {
        protected abstract TextReader FetchTextReader(AgnosticPath path);
        protected abstract TextWriter FetchTextWriter(
            AgnosticPath path, bool append);
        public abstract void CreateDirectory(AgnosticPath path);

        // ゲームの実行ファイルのあるディレクトリパス
        protected static string RootPath { get; private set; } =
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // テスト実行中はRootPathをシステムのTEMPファイル領域に切り替える
        public void WithTestMode(Action testAction)
        {
            var prevRootPath = RootPath;
            try
            {
                // TEMP領域にテスト用フォルダを作成
                RootPath = Path.Combine(
                    Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(RootPath);
                try
                {
                    // テスト実行
                    testAction();
                }
                finally
                {
                    // 例外が発生してもテスト用フォルダは削除する
                    Directory.Delete(RootPath, true);
                }
            }
            finally
            {
                // 例外が発生してもRootPathは元に戻す
                RootPath = prevRootPath;
            }
        }

        // OS依存なフルパスとして出力
        public string OSFullPath(AgnosticPath path) =>
            Path.Combine(RootPath, path.ToOSPathString());

        // OS依存なフルパスのディレクトリ名を出力
        public string OSFullDirPath(AgnosticPath path) =>
            Path.GetDirectoryName(OSFullPath(path));

        public void WithTextReader(
            AgnosticPath path, Action<TextReader> action)
        {
            using (var reader = FetchTextReader(path))
            {
                action(reader);
            }
        }

        public Result WithTextReader<Result>(
            AgnosticPath path, Func<TextReader, Result> func)
        {
            using (var reader = FetchTextReader(path))
            {
                return func(reader);
            }
        }

        public void WithTextWriter(
            AgnosticPath path, bool append, Action<TextWriter> action)
        {
            using (var writer = FetchTextWriter(path, append))
            {
                action(writer);
            }
        }

        public Result WithTextWriter<Result>(
            AgnosticPath path, bool append, Func<TextWriter, Result> func)
        {
            using (var writer = FetchTextWriter(path, append))
            {
                return func(writer);
            }
        }
    }
}
