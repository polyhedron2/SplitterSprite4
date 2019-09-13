using MagicKitchen.SplitterSprite4.Common.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace MagicKitchen.SplitterSprite4.Common.Test.Proxy
{
    public class RealFileIOProxyTests
    {
        [Theory]
        [InlineData("dir/file.txt")]
        [InlineData("dir1/dir2/file.txt")]
        public void CreateDirectoryTest(string path)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                var dirFullPath =
                    OutSideProxy.FileIO.OSFullDirPath(agnosticPath);
                Assert.False(Directory.Exists(dirFullPath));

                // act
                OutSideProxy.FileIO.CreateDirectory(agnosticPath);

                // assert
                Assert.True(Directory.Exists(dirFullPath));
            });
        }

        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void FullPathTest(string path)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                // フルパスとしてはゲーム実行ファイルのあるフォルダにagnosticPathを
                // つなげたものを期待する
                var expectedFullPath = Path.Combine(
                    OutSideProxy.FileIO.RootPath,
                    agnosticPath.ToOSPathString());
                var expectedFullDirPath =
                    Path.GetDirectoryName(expectedFullPath);

                // act
                var actualFullPath = OutSideProxy.FileIO.OSFullPath(agnosticPath);
                var actualFullDirPath =
                    OutSideProxy.FileIO.OSFullDirPath(agnosticPath);

                // assert
                Assert.Equal(expectedFullPath, actualFullPath);
                Assert.Equal(expectedFullDirPath, actualFullDirPath);
            });
        }

        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void ActionWithReaderAndWriterTest(
            string path, params string[] fileBody)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

                // act
                // 書き込み先ファイルのためのディレクトリを作成
                OutSideProxy.FileIO.CreateDirectory(agnosticPath);

                // 各行ごとにファイルに新規書き込み(append=false)
                foreach (var line in fileBody)
                {
                    OutSideProxy.FileIO.WithTextWriter(
                        agnosticPath, false, (writer) =>
                        {
                            writer.WriteLine(line);
                        });
                }

                // 書き込んだ内容を取得
                var readLine = "";
                OutSideProxy.FileIO.WithTextReader(
                    agnosticPath, (reader) =>
                    {
                        readLine = reader.ReadToEnd();
                    });

                // assert
                // 新規書き込みの繰り返しであったので、
                // ファイルには最後の行だけ書き込まれている
                Assert.Equal(
                    fileBody[fileBody.Length - 1] + Environment.NewLine,
                    readLine);
            });
        }

        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void FunctionWithReaderAndWriterTest(
            string path, params string[] fileBody)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                var returnList = new List<string>();

                // act
                // 書き込み先ファイルのためのディレクトリを作成
                OutSideProxy.FileIO.CreateDirectory(agnosticPath);

                // 各行ごとにファイルに新規書き込み(append=false)
                foreach (var line in fileBody)
                {
                    // Functionの結果を戻り値として取得できることを確認
                    var returnLine = OutSideProxy.FileIO.WithTextWriter(
                        agnosticPath, false, (writer) =>
                        {
                            writer.WriteLine(line);
                            return line;
                        });
                    returnList.Add(returnLine);
                }

                // 書き込んだ内容を戻り値として取得
                var readLine = OutSideProxy.FileIO.WithTextReader(
                    agnosticPath, (reader) =>
                    {
                        return reader.ReadToEnd();
                    });

                // assert
                for (var i = 0; i < fileBody.Length; i++)
                {
                    // Functionの戻り値は入力したlineと一致する
                    Assert.Equal(fileBody[i], returnList[i]);
                }

                // 新規書き込みの繰り返しであったので、
                // ファイルには最後の行だけ書き込まれている
                Assert.Equal(
                    fileBody[fileBody.Length - 1] + Environment.NewLine,
                    readLine);
            });
        }

        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void ActionWithReaderAndWriterAppendModeTest(
            string path, params string[] fileBody)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

                // act
                // 書き込み先ファイルのためのディレクトリを作成
                OutSideProxy.FileIO.CreateDirectory(agnosticPath);

                // 各行ごとにファイルに追加書き込み(append=true)
                foreach (var line in fileBody)
                {
                    OutSideProxy.FileIO.WithTextWriter(
                        agnosticPath, true, (writer) =>
                        {
                            writer.WriteLine(line);
                        });
                }

                // 書き込んだ内容を取得
                var readLine = "";
                OutSideProxy.FileIO.WithTextReader(
                    agnosticPath, (reader) =>
                    {
                        readLine = reader.ReadToEnd();
                    });

                // assert
                // 追加書き込みの繰り返しであったので、
                // ファイルには全ての行が書き込まれている
                Assert.Equal(
                    string.Join(Environment.NewLine, fileBody) + Environment.NewLine,
                    readLine);
            });
        }

        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void ReaderWithoutFileTest(string path)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

                // assert
                // 開始時点ではpathに至るファイルが存在しない
                Assert.False(File.Exists(
                    OutSideProxy.FileIO.OSFullPath(agnosticPath)));
                // ファイルが無い状態で読み込みをしようとすれば例外
                Assert.Throws<AgnosticPathNotFoundException>(() =>
                {
                    OutSideProxy.FileIO.WithTextReader(
                        agnosticPath, (reader) => { });
                });
            });
        }

        [Theory]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void WriterWithoutDirectoryTest(string path)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

                // assert
                // 開始時点ではpathに至るディレクトリが存在しない
                Assert.False(Directory.Exists(
                    OutSideProxy.FileIO.OSFullDirPath(agnosticPath)));
                // ディレクトリが無い状態で書き込みをしようとすれば例外
                Assert.Throws<AgnosticPathNotFoundException>(() =>
                {
                    OutSideProxy.FileIO.WithTextWriter(
                        agnosticPath, false, (writer) => { });
                });
            });
        }
    }
}
