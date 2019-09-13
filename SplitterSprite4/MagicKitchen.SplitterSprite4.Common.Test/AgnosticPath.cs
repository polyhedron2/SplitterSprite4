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
            // OS�ˑ��ȕ�����𐶐�
            var osPathStr =
                agnosticPathStr.Replace('/', Path.DirectorySeparatorChar);

            // act
            // OS��ˑ��p�X�ƈˑ��p�X����AgnosticPath�𐶐�
            var pathA = AgnosticPath.FromAgnosticPathString(agnosticPathStr);
            var pathO = AgnosticPath.FromOSPathString(osPathStr);

            // assert
            foreach (var path in new List<AgnosticPath> { pathA, pathO })
            {
                // �������ꂽAgnosticPath�͂ǂ�����������e��\������
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
            // �֎~�������܂ރp�X���g���ĂQ�ʂ�̕��@��AgnosticPath���������݂�
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
                // ��O�I�u�W�F�N�g�ɂ͎��s�����p�X�ƌ��o�����֎~�������܂܂��
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
                    // �֎~����'\'���܂�AgnosticPath���������݂�
                    AgnosticPath.FromAgnosticPathString("foo\\bar");
                });

            // assert
            // ��O�I�u�W�F�N�g�ɂ͎��s�����p�X�ƌ��o�����֎~�������܂܂��
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
                    // �Q�ʂ�̃t�@�C����؂蕶���𗼕��܂ރp�X��
                    // AgnosticPath���������݂�
                    AgnosticPath.FromOSPathString(prohibitedPathStr);
                });

            // assert
            Assert.Equal(prohibitedPathStr, ex.Path);
            // ���݂�OS�̋�؂蕶���̓t�@�C�����E�f�B���N�g�����ȊO��
            // �p�r�ł��邽�ߑ����̋�؂蕶���ɂČ��m�����
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
