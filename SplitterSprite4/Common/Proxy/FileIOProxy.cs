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
        public string RootPath { get; private set; } =
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // テスト実行中はRootPathをシステムのTEMPファイル領域に切り替える
        public void WithTestMode(Action testAction)
        {
            // テスト並列実行時に干渉しあわないよう、スレッドロックを取得
            lock (this)
            {
                var prevRootPath = RootPath;
                try
                {
                    var testDirName = "SplitterSpriteTest";

                    // TEMP領域にテスト用フォルダを作成
                    RootPath = Path.Combine(
                        Path.GetTempPath(),
                        testDirName,
                        Path.GetRandomFileName());

                    Directory.CreateDirectory(RootPath);

                    try
                    {
                        // テスト実行
                        testAction();
                    }
                    finally
                    {
                        // テスト用フォルダの親フォルダごと削除を試みる
                        // 前のテストで削除できなかったフォルダも削除される
                        TryDeleteDirectory(RootPath);
                    }
                }
                finally
                {
                    // 例外が発生してもRootPathは元に戻す
                    RootPath = prevRootPath;
                }
            }
        }

        void TryDeleteDirectory(string path)
        {
            try
            {
                // テスト用フォルダの削除を試みる
                Directory.Delete(path, true);
            }
            catch
            {
                // TEMP配下なので削除失敗は気にしない。
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
