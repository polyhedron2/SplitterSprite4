// -----------------------------------------------------------------------
// <copyright file="RealFileIOProxyTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Proxy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using Xunit;

    /// <summary>
    /// Test the RealFileIOProxy class.
    /// </summary>
    public class RealFileIOProxyTests
    {
        /// <summary>
        /// Test the CreateDirectory method.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        [Theory]
        [InlineData("dir/file.txt")]
        [InlineData("dir1/dir2/file.txt")]
        public void CreateDirectoryTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            var dirFullPath = proxy.FileIO.OSFullPath(agnosticPath.Parent);
            Assert.False(Directory.Exists(dirFullPath));

            // act
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);

            // assert
            Assert.True(Directory.Exists(dirFullPath));
        }

        /// <summary>
        /// Test the EnumerateDirectory method.
        /// </summary>
        /// <param name="basePath">The parent direcotry path.</param>
        /// <param name="childrenPaths">The children direcotry paths.</param>
        [Theory]
        [InlineData("parent")]
        [InlineData("parent", "child0")]
        [InlineData("parent", "child0", "child1")]
        [InlineData("parent", "child0", "child1", "child2")]
        [InlineData("foo", "child0", "child1", "child2")]
        [InlineData("bar", "child0", "child1", "child2")]
        public void EnumerateDirectoriesTest(
            string basePath, params string[] childrenPaths)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var parent = AgnosticPath.FromAgnosticPathString(basePath);
            var children = childrenPaths.Select(
                child => AgnosticPath.FromAgnosticPathString($"{basePath}/{child}"));
            proxy.FileIO.CreateDirectory(parent);
            foreach (var child in children)
            {
                proxy.FileIO.CreateDirectory(child);
            }

            // act
            var result = proxy.FileIO.EnumerateDirectories(
                parent).Select(d => d.ToAgnosticPathString()).ToHashSet();

            // assert
            Assert.Equal(childrenPaths.ToHashSet(), result);
        }

        /// <summary>
        /// Test the OSFullPath and OSFullDirPath property.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void FullPathTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            var expectedFullPath = Path.Combine(
                proxy.FileIO.RootPath,
                agnosticPath.ToOSPathString());
            var expectedFullDirPath =
                Path.GetDirectoryName(expectedFullPath);

            // act
            var actualFullPath = proxy.FileIO.OSFullPath(agnosticPath);
            var actualFullDirPath =
                proxy.FileIO.OSFullPath(agnosticPath.Parent);
            if (actualFullDirPath.EndsWith(Path.DirectorySeparatorChar))
            {
                actualFullDirPath = actualFullDirPath.Substring(
                    0, actualFullDirPath.Length - 1);
            }

            // assert
            Assert.Equal(expectedFullPath, actualFullPath);
            Assert.Equal(expectedFullDirPath, actualFullDirPath);
        }

        /// <summary>
        /// Test the FileExists method.
        /// </summary>
        /// <param name="pathStr">The os-agnostic path.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void FileExistsTest(string pathStr)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var path = AgnosticPath.FromAgnosticPathString(pathStr);

            // act
            var beforeCreateDirectory = proxy.FileIO.FileExists(path);
            proxy.FileIO.CreateDirectory(path.Parent);
            var afterCreateDirectory = proxy.FileIO.FileExists(path);
            proxy.FileIO.WithTextWriter(path, false, (writer) =>
            {
                writer.WriteLine("file body");
            });
            var afterCreateFile = proxy.FileIO.FileExists(path);

            // assert
            Assert.False(beforeCreateDirectory);
            Assert.False(afterCreateDirectory);
            Assert.True(afterCreateFile);
        }

        /// <summary>
        /// Test the WithTextReder and WithTextWriter method
        /// with Action instance.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        /// <param name="fileBody">The file body lines.</param>
        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void ActionWithReaderAndWriterTest(
            string path, params string[] fileBody)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            // 書き込み先ファイルのためのディレクトリを作成
            // Create directory for file writing.
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);

            // 各行ごとにファイルに新規書き込み(append=false)
            // File writing with append=false mode.
            foreach (var line in fileBody)
            {
                proxy.FileIO.WithTextWriter(
                    agnosticPath, false, (writer) =>
                    {
                        writer.WriteLine(line);
                    });
            }

            // 書き込んだ内容を取得
            // Read the written body.
            var readLine = string.Empty;
            proxy.FileIO.WithTextReader(
                agnosticPath, (reader) =>
                {
                    readLine = reader.ReadToEnd();
                });

            // assert
            // 新規書き込みの繰り返しであったので、
            // ファイルには最後の行だけ書き込まれている
            // Because append=false,
            // there is only the last line.
            Assert.Equal(
                fileBody[fileBody.Length - 1] + Environment.NewLine,
                readLine);
        }

        /// <summary>
        /// Test the WithTextReder and WithTextWriter method
        /// with Function instance.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        /// <param name="fileBody">The file body lines.</param>
        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void FunctionWithReaderAndWriterTest(
            string path, params string[] fileBody)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            var returnList = new List<string>();

            // act
            // 書き込み先ファイルのためのディレクトリを作成
            // Create directory for file writing.
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);

            // 各行ごとにファイルに新規書き込み(append=false)
            // File writing with append=false mode.
            foreach (var line in fileBody)
            {
                // Functionの結果を戻り値として取得できることを確認
                // The result of the Function is the result of WithTextWriter.
                var returnLine = proxy.FileIO.WithTextWriter(
                    agnosticPath, false, (writer) =>
                    {
                        writer.WriteLine(line);
                        return line;
                    });
                returnList.Add(returnLine);
            }

            // 書き込んだ内容を戻り値として取得
            // Read the written body.
            var readLine = proxy.FileIO.WithTextReader(
                agnosticPath, (reader) =>
                {
                    return reader.ReadToEnd();
                });

            // assert
            for (var i = 0; i < fileBody.Length; i++)
            {
                // Functionの戻り値は入力したlineと一致する
                // The result is as same as the Function.
                Assert.Equal(fileBody[i], returnList[i]);
            }

            // 新規書き込みの繰り返しであったので、
            // ファイルには最後の行だけ書き込まれている
            // Because append=false,
            // there is only the last line.
            Assert.Equal(
                fileBody[fileBody.Length - 1] + Environment.NewLine,
                readLine);
        }

        /// <summary>
        /// Test the WithTextReder and WithTextWriter method
        /// with Action instance, and append mode.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        /// <param name="fileBody">The file body lines.</param>
        [Theory]
        [InlineData("foo.txt", "sample body")]
        [InlineData("foo.txt", "line1", "line2")]
        [InlineData("dir/bar.yaml", "line1", "line2", "line3")]
        [InlineData("dir1/dir2/baz.meta", "line1", "line2", "line3")]
        public void ActionWithReaderAndWriterAppendModeTest(
            string path, params string[] fileBody)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            // 書き込み先ファイルのためのディレクトリを作成
            // Create directory for file writing.
            proxy.FileIO.CreateDirectory(agnosticPath.Parent);

            // 各行ごとにファイルに追加書き込み(append=true)
            // File writing with append=true mode.
            foreach (var line in fileBody)
            {
                proxy.FileIO.WithTextWriter(
                    agnosticPath, true, (writer) =>
                    {
                        writer.WriteLine(line);
                    });
            }

            // 書き込んだ内容を取得
            // Read the written body.
            var readLine = string.Empty;
            proxy.FileIO.WithTextReader(
                agnosticPath, (reader) =>
                {
                    readLine = reader.ReadToEnd();
                });

            // assert
            // 追加書き込みの繰り返しであったので、
            // ファイルには全ての行が書き込まれている
            // Because append=true,
            // there are all of the lines.
            Assert.Equal(
                string.Join(Environment.NewLine, fileBody) + Environment.NewLine,
                readLine);
        }

        /// <summary>
        /// Test behavior when an attempt to read a path without file.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void ReaderWithoutFileTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // assert
            // 開始時点ではpathに至るファイルが存在しない
            // There is no file on the path.
            Assert.False(File.Exists(
                proxy.FileIO.OSFullPath(agnosticPath)));

            // ファイルが無い状態で読み込みをしようとすれば例外
            // Exception will be thrown when an attempt to read the path.
            Assert.Throws<FileIOProxy.AgnosticPathNotFoundException>(() =>
            {
                proxy.FileIO.WithTextReader(
                    agnosticPath, (reader) => { });
            });
        }

        /// <summary>
        /// Test behavior when an attempt to write a path without directory.
        /// </summary>
        /// <param name="path">The os-agnostic file path string.</param>
        [Theory]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.meta")]
        public void WriterWithoutDirectoryTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // assert
            // 開始時点ではpathに至るディレクトリが存在しない
            // There is no file on the path.
            Assert.False(Directory.Exists(
                proxy.FileIO.OSFullPath(agnosticPath.Parent)));

            // ディレクトリが無い状態で書き込みをしようとすれば例外
            // Exception will be thrown when an attempt to write the path.
            Assert.Throws<FileIOProxy.AgnosticPathNotFoundException>(() =>
            {
                proxy.FileIO.WithTextWriter(
                    agnosticPath, false, (writer) => { });
            });
        }

        /// <summary>
        /// Test access out of the game root directory.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("../foo")]
        public void OutOfRootAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // assert
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.CreateDirectory(agnosticPath);
            });
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.EnumerateDirectories(agnosticPath);
            });
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.WithTextReader(agnosticPath, (reader) =>
                {
                });
            });
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.WithTextReader(agnosticPath, (reader) =>
                {
                    return 0;
                });
            });
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.WithTextWriter(agnosticPath, false, (reader) =>
                {
                });
            });
            Assert.Throws<FileIOProxy.OutOfRootAccessException>(() =>
            {
                proxy.FileIO.WithTextWriter(agnosticPath, false, (reader) =>
                {
                    return 0;
                });
            });
        }
    }
}
