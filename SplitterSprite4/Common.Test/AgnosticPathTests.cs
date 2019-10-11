// -----------------------------------------------------------------------
// <copyright file="AgnosticPathTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    /// <summary>
    /// Test the AgnosticPath class.
    /// </summary>
    public class AgnosticPathTests
    {
        /// <summary>
        /// Test the instance generator methods.
        /// </summary>
        /// <param name="agnosticPathStr">The os-agnostic path string.</param>
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
            // Os-dependent path string.
            var osPathStr =
                agnosticPathStr.Replace('/', Path.DirectorySeparatorChar);

            // act
            // OS非依存パスと依存パスからAgnosticPathを生成
            // Generate AgnosticPath instance from
            // os -agnostic and os-dependent path.
            var pathA = AgnosticPath.FromAgnosticPathString(agnosticPathStr);
            var pathO = AgnosticPath.FromOSPathString(osPathStr);

            // assert
            foreach (var path in new List<AgnosticPath> { pathA, pathO })
            {
                // 生成されたAgnosticPathはどちらも同じ内容を表現する
                // Both of the AgnosticPath instance have same path.
                Assert.Equal(agnosticPathStr, path.ToAgnosticPathString());
                Assert.Equal(osPathStr, path.ToOSPathString());
            }
        }

        /// <summary>
        /// Test prohibited character check.
        /// </summary>
        /// <param name="prohibitedPathStr">
        /// Os-agnostic path string that contains prohibited character.
        /// </param>
        /// <param name="prohibitedChar">
        /// The contained prohibited character.
        /// </param>
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
            // Attempt to generate AgnosticPath instance from the prohibited path.
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
                Assert.Equal(prohibitedPathStr, ex.Path);
                Assert.Equal(prohibitedChar, ex.ContainedChar);
            }
        }

        /// <summary>
        /// Test prohibited character check of '\'.
        /// </summary>
        [Fact]
        public void ProhibitedPathTestWithBackSlash()
        {
            // act
            var ex = Assert.Throws<
                AgnosticPath.ProhibitedCharacterContainedException>(() =>
                {
                    // 禁止文字'\'を含むAgnosticPath生成を試みる
                    // Attempt to generate AgnosticPath from the prohibited path.
                    AgnosticPath.FromAgnosticPathString("foo\\bar");
                });

            // assert
            Assert.Equal("foo\\bar", ex.Path);
            Assert.Equal('\\', ex.ContainedChar);
        }

        /// <summary>
        /// Test prohibited character check of the os-dependent file separator.
        /// </summary>
        /// <param name="prohibitedPathStr">
        /// Os-agnostic path string that contains prohibited character.
        /// </param>
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
                    // Attempt to generate AgnosticPath instance
                    // from the prohibited path with file separator.
                    AgnosticPath.FromOSPathString(prohibitedPathStr);
                });

            // assert
            Assert.Equal(prohibitedPathStr, ex.Path);

            // 現在のOSの区切り文字はファイル名・ディレクトリ名以外の
            // 用途であるため他方の区切り文字にて検知される
            // The current OS's file separator is not prohibited.
            // Then, another separator is detected.
            if (Path.DirectorySeparatorChar == '\\')
            {
                Assert.Equal('/', ex.ContainedChar);
            }
            else
            {
                Assert.Equal('\\', ex.ContainedChar);
            }
        }

        /// <summary>
        /// Test the parent directory property.
        /// </summary>
        /// <param name="agnosticPathStr">The os-agnostic path string.</param>
        /// <param name="expectedParent">The expected parent path string.</param>
        [Theory]
        [InlineData("../..", "../../../")]
        [InlineData("../../", "../../../")]
        [InlineData("..", "../../")]
        [InlineData("../", "../../")]
        [InlineData("", "../")]
        [InlineData(".", "../")]
        [InlineData("foo.txt", "")]
        [InlineData("bar.yaml", "")]
        [InlineData("baz.meta", "")]
        [InlineData("dir/", "")]
        [InlineData("dir/foo.txt", "dir/")]
        [InlineData("dir/dir2/", "dir/")]
        [InlineData("dir/dir2/foo.txt", "dir/dir2/")]
        public void ParentTest(string agnosticPathStr, string expectedParent)
        {
            // arrange
            var path = AgnosticPath.FromAgnosticPathString(agnosticPathStr);

            // act
            var parent = path.Parent;

            // assert
            Assert.Equal(
                expectedParent,
                parent.ToAgnosticPathString());
        }

        /// <summary>
        /// Test canonicalization process in constractor.
        /// </summary>
        /// <param name="agnosticPathStr">The os-agnostic path string.</param>
        /// <param name="canonicalPathStr">The expected canonicalized path.</param>
        [Theory]
        [InlineData("../foo.txt", "../foo.txt")]
        [InlineData("./foo.txt", "foo.txt")]
        [InlineData("dir/../foo.txt", "foo.txt")]
        [InlineData("dir/./foo.txt", "dir/foo.txt")]
        [InlineData("dir1/dir2/../foo.txt", "dir1/foo.txt")]
        [InlineData("dir1/dir2/../../../foo.txt", "../foo.txt")]
        [InlineData("../dir1/dir2/foo.txt", "../dir1/dir2/foo.txt")]
        public void CanonicalizationTest(
            string agnosticPathStr, string canonicalPathStr)
        {
            // arrange
            var path = AgnosticPath.FromAgnosticPathString(agnosticPathStr);

            // assert
            Assert.Equal(canonicalPathStr, path.ToAgnosticPathString());
        }
    }
}