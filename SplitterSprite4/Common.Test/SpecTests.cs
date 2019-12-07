// -----------------------------------------------------------------------
// <copyright file="SpecTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the Spec class.
    /// </summary>
    public class SpecTests
    {
        /// <summary>
        /// Test spec creation with absence spec file.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void AbsenceSpecTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = new SpecRoot(proxy, agnosticPath, true);

            // assert
            Assert.Equal("{}", spec.ToString());
            Assert.Throws<LayeredFile.LayeredFileNotFoundException>(() =>
            {
                new SpecRoot(proxy, agnosticPath, false);
            });
        }

        /// <summary>
        /// Test Spec class creation.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="expectedChildID">The expected ID of SpecChild.</param>
        /// <param name="expectedDeepChildID">The expected ID of SpecChild of SpecChild.</param>
        [Theory]
        [InlineData(
            "foo.spec",
            "foo.spec[child]",
            "foo.spec[first][second]")]
        [InlineData(
            "dir/bar.spec",
            "dir/bar.spec[child]",
            "dir/bar.spec[first][second]")]
        [InlineData(
            "dir1/dir2/baz.spec",
            "dir1/dir2/baz.spec[child]",
            "dir1/dir2/baz.spec[first][second]")]
        public void CreationTest(
            string path,
            string expectedChildID,
            string expectedDeepChildID)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines("key: value"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal("key: value", spec.ToString());
            Assert.Equal(path, spec.ID);
            Assert.Equal(expectedChildID, spec["child"].ID);
            Assert.Equal(expectedDeepChildID, spec["first"]["second"].ID);
        }

        /// <summary>
        /// Test the save method.
        /// </summary>
        [Fact]
        public void SaveTest()
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString("foo.spec");
            this.SetupSpecFile(proxy, "foo.spec", Utility.JoinLines("key: value"));
            var newLayer = new Layer(proxy, "new_layer", true);
            newLayer.Dependencies = new string[] { "layer" };
            newLayer.Save();
            var newPath = AgnosticPath.FromAgnosticPathString("dir1/dir2/bar.spec");

            // act
            var spec = new SpecRoot(proxy, agnosticPath);
            spec.Int["int0"] = 0;
            spec.Save(newLayer, newPath);

            // assert
            Assert.Equal(
                "new_layer/dir1/dir2/bar.spec",
                new LayeredFile(proxy, newPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "key: value",
                    "int0: 0"),
                new SpecRoot(proxy, newPath).ToString());

            // act
            spec.Int["int1"] = 1;
            spec.Save(newLayer);

            // assert
            Assert.Equal(
                "new_layer/foo.spec",
                new LayeredFile(proxy, agnosticPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "key: value",
                    "int0: 0",
                    "int1: 1"),
                new SpecRoot(proxy, agnosticPath).ToString());

            // act
            spec.Int["int2"] = 2;
            spec.Save();

            // assert

            // ただのSave()はトップ属性を持つ"save"レイヤーに保存する
            // The simple Save() will save the spec into "save" layer which is top.
            Assert.Equal(
                "save/foo.spec",
                new LayeredFile(proxy, agnosticPath).FetchReadPath()
                    .ToAgnosticPathString());
            Assert.Equal(
                Utility.JoinLines(
                    "key: value",
                    "int0: 0",
                    "int1: 1",
                    "int2: 2"),
                new SpecRoot(proxy, agnosticPath).ToString());
        }

        /// <summary>
        /// Test the integer accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void IntTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "invalid: dummy",
                "Answer to the Ultimate Question: 42",
                "taxi number:",
                "  first: 2",
                "  second: 1729"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int["invalid"];
            });
            Assert.Equal(42, spec.Int["Answer to the Ultimate Question"]);
            Assert.Equal(2, spec["taxi number"].Int["first"]);
            Assert.Equal(1729, spec["taxi number"].Int["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["taxi number"].Int["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int["invalid", 0];
            });
            Assert.Equal(42, spec.Int["Answer to the Ultimate Question", 123]);
            Assert.Equal(2, spec["taxi number"].Int["first", -1]);
            Assert.Equal(1729, spec["taxi number"].Int["second", 0]);
            Assert.Equal(87539319, spec["taxi number"].Int["third", 87539319]);

            // act
            spec.Int["Answer to the Ultimate Question"] = 24;
            spec["taxi number"].Int["third"] = 87539319;

            // assert
            Assert.Equal(24, spec.Int["Answer to the Ultimate Question"]);
            Assert.Equal(87539319, spec["taxi number"].Int["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "Answer to the Ultimate Question: 24",
                    "taxi number:",
                    "  first: 2",
                    "  second: 1729",
                    "  third: 87539319"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "Answer to the Ultimate Question: 24",
                    "taxi number:",
                    "  first: 2",
                    "  second: 1729",
                    "  third: 87539319"),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// Test the double precision floating point number accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void DoubleTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "invalid: dummy",
                "pi: 3.14159265",
                "square root:",
                "  first: 1.00000000",
                "  second: 1.41421356"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double["invalid"];
            });
            Assert.Equal(3.14159265, spec.Double["pi"]);
            Assert.Equal(1.00000000, spec["square root"].Double["first"]);
            Assert.Equal(1.41421356, spec["square root"].Double["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["square root"].Double["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Double["invalid", 0.0];
            });
            Assert.Equal(3.14159265, spec.Double["pi", 2.71828182]);
            Assert.Equal(1.00000000, spec["square root"].Double["first", -1]);
            Assert.Equal(1.41421356, spec["square root"].Double["second", 0.0]);
            Assert.Equal(1.73050807, spec["square root"].Double["third", 1.73050807]);

            // act
            spec.Double["pi"] = 3;
            spec["square root"].Double["third"] = 1.73050807;

            // assert
            Assert.Equal(3.0, spec.Double["pi"]);
            Assert.Equal(1.73050807, spec["square root"].Double["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "pi: 3",
                    "square root:",
                    "  first: 1.00000000",
                    "  second: 1.41421356",
                    "  third: 1.73050807"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "pi: 3",
                    "square root:",
                    "  first: 1.00000000",
                    "  second: 1.41421356",
                    "  third: 1.73050807"),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// Test the boolean accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void BoolTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "invalid: dummy",
                "special: True",
                "story flag:",
                "  first: True",
                "  second: False"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Bool["invalid"];
            });
            Assert.True(spec.Bool["special"]);
            Assert.True(spec["story flag"].Bool["first"]);
            Assert.False(spec["story flag"].Bool["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["square root"].Bool["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Bool["invalid", false];
            });
            Assert.True(spec.Bool["special", false]);
            Assert.True(spec["story flag"].Bool["first", false]);
            Assert.False(spec["story flag"].Bool["second", true]);
            Assert.True(spec["story flag"].Bool["third", true]);

            // act
            spec.Bool["special"] = false;
            spec["story flag"].Bool["third"] = true;

            // assert
            Assert.False(spec.Bool["special", false]);
            Assert.True(spec["story flag"].Bool["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "special: False",
                    "story flag:",
                    "  first: True",
                    "  second: False",
                    "  third: True"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: dummy",
                    "special: False",
                    "story flag:",
                    "  first: True",
                    "  second: False",
                    "  third: True"),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// Test the boolean accessor with "yes" or "no".
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void YesNoTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "invalid: True",
                "special: YES",
                "story flag:",
                "  first: Yes",
                "  second: no"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.YesNo["invalid"];
            });
            Assert.True(spec.YesNo["special"]);
            Assert.True(spec["story flag"].YesNo["first"]);
            Assert.False(spec["story flag"].YesNo["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["square root"].YesNo["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.YesNo["invalid", false];
            });
            Assert.True(spec.YesNo["special", false]);
            Assert.True(spec["story flag"].YesNo["first", false]);
            Assert.False(spec["story flag"].YesNo["second", true]);
            Assert.True(spec["story flag"].YesNo["third", true]);

            // act
            spec.YesNo["special"] = false;
            spec["story flag"].YesNo["third"] = true;

            // assert
            Assert.False(spec.YesNo["special", false]);
            Assert.True(spec["story flag"].YesNo["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: True",
                    "special: no",
                    "story flag:",
                    "  first: Yes",
                    "  second: no",
                    "  third: yes"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: True",
                    "special: no",
                    "story flag:",
                    "  first: Yes",
                    "  second: no",
                    "  third: yes"),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// Test the boolean accessor with "on" or "off".
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void OnOffTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "invalid: True",
                "special: ON",
                "story flag:",
                "  first: On",
                "  second: off"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.OnOff["invalid"];
            });
            Assert.True(spec.OnOff["special"]);
            Assert.True(spec["story flag"].OnOff["first"]);
            Assert.False(spec["story flag"].OnOff["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["square root"].OnOff["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.OnOff["invalid", false];
            });
            Assert.True(spec.OnOff["special", false]);
            Assert.True(spec["story flag"].OnOff["first", false]);
            Assert.False(spec["story flag"].OnOff["second", true]);
            Assert.True(spec["story flag"].OnOff["third", true]);

            // act
            spec.OnOff["special"] = false;
            spec["story flag"].OnOff["third"] = true;

            // assert
            Assert.False(spec.OnOff["special", false]);
            Assert.True(spec["story flag"].OnOff["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: True",
                    "special: off",
                    "story flag:",
                    "  first: On",
                    "  second: off",
                    "  third: on"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "invalid: True",
                    "special: off",
                    "story flag:",
                    "  first: On",
                    "  second: off",
                    "  third: on"),
                reloadedSpec.ToString());
        }

        private SpecRoot SetupSpecFile(
            OutSideProxy proxy, string layeredPathStr, string body)
        {
            var layer = new Layer(proxy, "layer", true);
            layer.Save();

            var layeredPath =
                AgnosticPath.FromAgnosticPathString(layeredPathStr);
            var layeredFile = new LayeredFile(proxy, layeredPath, true);

            var writePath = layeredFile.FetchWritePath(layer);
            proxy.FileIO.CreateDirectory(writePath.Parent);

            proxy.FileIO.WithTextWriter(writePath, false, (writer) =>
            {
                writer.Write(body);
            });

            return new SpecRoot(proxy, layeredPath);
        }
    }
}
