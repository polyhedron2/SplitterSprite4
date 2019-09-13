using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    public class AgnosticPathTests
    {
        [Theory]
        [InlineData("foo.txt")]
        [InlineData("bar.yaml")]
        [InlineData("baz.meta")]
        [InlineData("dir/foo.txt")]
        [InlineData("dir/dir2/foo.txt")]
        public void CreationTest(string agnosticPathStr)
        {
            // arrange
            // OS依存な文字列を生成
            var osPathStr =
                agnosticPathStr.Replace('/', Path.DirectorySeparatorChar);

            // act
            // OS非依存パスと依存パスからAgnosticPathを生成
            var pathA = AgnosticPath.FromAgnosticPathString(agnosticPathStr);
            var pathO = AgnosticPath.FromOSPathString(osPathStr);

            // assert
            foreach (var path in new List<AgnosticPath> { pathA, pathO })
            {
                // 生成されたAgnosticPathはどちらも同じ内容を表現する
                Assert.Equal(agnosticPathStr, path.ToAgnosticPathString());
                Assert.Equal(osPathStr, path.ToOSPathString());
            }
        }

        [Theory]
        [InlineData("*asterisk*", '*')]
        [InlineData("?question?", '?')]
        [InlineData("\"quotation\"", '\"')]
        [InlineData("<bra", '<')]
        [InlineData("ket>", '>')]
        [InlineData("|vertial|", '|')]
        [InlineData(":colon:", ':')]
        [InlineData("null\0", '\0')]
        [InlineData("multi*prohibited?chars", '*')]
        [InlineData("prohibited/chars:with/separator", ':')]
        public void ProhibitedPathTest(
            string prohibitedPathStr, char prohibitedChar)
        {
            // act
            // 禁止文字を含むパスを使って２通りの方法でAgnosticPath生成を試みる
            var exFromAgnosticPath = Assert.Throws<
                AgnosticPath.ProhibitedCharacterContainedException>(() =>
                {
                    AgnosticPath.FromAgnosticPathString(prohibitedPathStr);
                });
            var exFromOSPath = Assert.Throws<
                AgnosticPath.ProhibitedCharacterContainedException>(() =>
                {
                    AgnosticPath.FromOSPathString(prohibitedPathStr);
                });

            // assert
            var exList = new List<
                AgnosticPath.ProhibitedCharacterContainedException>
            {
                exFromAgnosticPath, exFromOSPath,
            };
            foreach (var ex in exList)
            {
                // 例外オブジェクトには失敗したパスと検出した禁止文字が含まれる
                Assert.Equal(prohibitedPathStr, ex.Path);
                Assert.Equal(prohibitedChar, ex.ContainedChar);
            }
        }

        [Fact]
        public void ProhibitedPathTestWithBackSlash()
        {
            // act
            var ex = Assert.Throws<
                AgnosticPath.ProhibitedCharacterContainedException>(() =>
                {
                    // 禁止文字'\'を含むAgnosticPath生成を試みる
                    AgnosticPath.FromAgnosticPathString("foo\\bar");
                });

            // assert
            // 例外オブジェクトには失敗したパスと検出した禁止文字が含まれる
            Assert.Equal("foo\\bar", ex.Path);
            Assert.Equal('\\', ex.ContainedChar);
        }

        [Theory]
        [InlineData("foo\\bar/baz")]
        [InlineData("foo/bar\\baz")]
        public void ProhibitedPathTestWithAnotherOSSeparator(
            string prohibitedPathStr)
        {
            // act
            var ex = Assert.Throws<
                AgnosticPath.ProhibitedCharacterContainedException>(() =>
                {
                    // ２通りのファイル区切り文字を両方含むパスで
                    // AgnosticPath生成を試みる
                    AgnosticPath.FromOSPathString(prohibitedPathStr);
                });

            // assert
            Assert.Equal(prohibitedPathStr, ex.Path);
            // 現在のOSの区切り文字はファイル名・ディレクトリ名以外の
            // 用途であるため他方の区切り文字にて検知される
            if (Path.DirectorySeparatorChar == '\\')
            {
                Assert.Equal('/', ex.ContainedChar);
            }
            else
            {
                Assert.Equal('\\', ex.ContainedChar);
            }
        }
    }
}
