﻿// -----------------------------------------------------------------------
// <copyright file="SpecTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System;
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
            "foo.spec[properties][child]",
            "foo.spec[properties][first][second]")]
        [InlineData(
            "dir/bar.spec",
            "dir/bar.spec[properties][child]",
            "dir/bar.spec[properties][first][second]")]
        [InlineData(
            "dir1/dir2/baz.spec",
            "dir1/dir2/baz.spec[properties][child]",
            "dir1/dir2/baz.spec[properties][first][second]")]
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
            this.SetupSpecFile(proxy, "foo.spec", Utility.JoinLines(
                "properties:",
                "  key: value"));
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
                    "properties:",
                    "  key: value",
                    "  int0: 0"),
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
                    "properties:",
                    "  key: value",
                    "  int0: 0",
                    "  int1: 1"),
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
                    "properties:",
                    "  key: value",
                    "  int0: 0",
                    "  int1: 1",
                    "  int2: 2"),
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
                "properties:",
                "  invalid: dummy",
                "  Answer to the Ultimate Question: 42",
                "  taxi number:",
                "    first: 2",
                "    second: 1729"));

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
                    "properties:",
                    "  invalid: dummy",
                    "  Answer to the Ultimate Question: 24",
                    "  taxi number:",
                    "    first: 2",
                    "    second: 1729",
                    "    third: 87539319"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "properties:",
                    "  invalid: dummy",
                    "  Answer to the Ultimate Question: 24",
                    "  taxi number:",
                    "    first: 2",
                    "    second: 1729",
                    "    third: 87539319"),
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
                "properties:",
                "  invalid: dummy",
                "  pi: 3.14159265",
                "  square root:",
                "    first: 1.00000000",
                "    second: 1.41421356"));

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
                    "properties:",
                    "  invalid: dummy",
                    "  pi: 3",
                    "  square root:",
                    "    first: 1.00000000",
                    "    second: 1.41421356",
                    "    third: 1.73050807"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "properties:",
                    "  invalid: dummy",
                    "  pi: 3",
                    "  square root:",
                    "    first: 1.00000000",
                    "    second: 1.41421356",
                    "    third: 1.73050807"),
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
                "properties:",
                "  invalid: dummy",
                "  special: True",
                "  story flag:",
                "    first: True",
                "    second: False"));

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
                _ = spec["story flag"].Bool["third"];
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
                    "properties:",
                    "  invalid: dummy",
                    "  special: False",
                    "  story flag:",
                    "    first: True",
                    "    second: False",
                    "    third: True"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "properties:",
                    "  invalid: dummy",
                    "  special: False",
                    "  story flag:",
                    "    first: True",
                    "    second: False",
                    "    third: True"),
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
                "properties:",
                "  invalid: True",
                "  special: YES",
                "  story flag:",
                "    first: Yes",
                "    second: no"));

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
                _ = spec["story flag"].YesNo["third"];
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
                    "properties:",
                    "  invalid: True",
                    "  special: no",
                    "  story flag:",
                    "    first: Yes",
                    "    second: no",
                    "    third: yes"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "properties:",
                    "  invalid: True",
                    "  special: no",
                    "  story flag:",
                    "    first: Yes",
                    "    second: no",
                    "    third: yes"),
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
                "properties:",
                "  invalid: True",
                "  special: ON",
                "  story flag:",
                "    first: On",
                "    second: off"));

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
                _ = spec["story flag"].OnOff["third"];
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
                    "properties:",
                    "  invalid: True",
                    "  special: off",
                    "  story flag:",
                    "    first: On",
                    "    second: off",
                    "    third: on"),
                spec.ToString());

            // act
            spec.Save();
            var reloadedSpec = new SpecRoot(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "properties:",
                    "  invalid: True",
                    "  special: off",
                    "  story flag:",
                    "    first: On",
                    "    second: off",
                    "    third: on"),
                reloadedSpec.ToString());
        }

        /// <summary>
        /// Base Specからの設定値の継承をテスト。
        /// Test property inheritance from base specs.
        /// </summary>
        /// <param name="derivedSpecLayerName">
        /// A derived spec's layer name.
        /// </param>
        /// <param name="derivedSpecPathStr">
        /// A derived spec's os-agnostic path.
        /// </param>
        /// <param name="intermediateSpecLayerName">
        /// A intermediate spec's layer name.
        /// </param>
        /// <param name="intermediateSpecPathStr">
        /// A spec's os-agnostic path which is base of derived spec.
        /// </param>
        /// <param name="baseSpecLayerName">
        /// A base spec's layer name.
        /// </param>
        /// <param name="baseSpecPathStr">
        /// A spec's os-agnostic path which is base of intermediate spec.
        /// </param>
        /// <param name="relativePathFromDerivedToIntermadiateStr">
        /// The relative path string from the derived spec to the intermediate spec.
        /// </param>
        /// <param name="relativePathFromIntermediateToBaseStr">
        /// The relative path string from the intermediate spec to the base spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "intermediate.spec",
            "layer",
            "base.spec",
            "intermediate.spec",
            "base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/intermediate.spec",
            "layer",
            "dir/dir2/base.spec",
            "dir/intermediate.spec",
            "dir2/base.spec")]
        [InlineData(
            "layer",
            "derived.spec",
            "layer",
            "dir/dir2/intermediate.spec",
            "layer",
            "dir/base.spec",
            "dir/dir2/intermediate.spec",
            "../base.spec")]
        [InlineData(
            "layer1",
            "derived.spec",
            "layer2",
            "dir/dir2/intermediate.spec",
            "layer3",
            "dir/base.spec",
            "dir/dir2/intermediate.spec",
            "../base.spec")]
        public void BaseSpecTest(
            string derivedSpecLayerName,
            string derivedSpecPathStr,
            string intermediateSpecLayerName,
            string intermediateSpecPathStr,
            string baseSpecLayerName,
            string baseSpecPathStr,
            string relativePathFromDerivedToIntermadiateStr,
            string relativePathFromIntermediateToBaseStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            // Spec key means, which spec has the value.
            // For example,
            //   "100" means only derived spec has the value.
            //   "101" means derived spec and base spec have the value.
            //   "111" means derived and intermediate and base spec have the value.
            // Spec value means, which spec the value come from.
            // "0" means the value come from derived spec.
            // "1" means the value come from intermediate spec.
            // "2" means the value come from base spec.
            this.SetupSpecFile(
                proxy,
                derivedSpecLayerName,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativePathFromDerivedToIntermadiateStr}",
                    "properties:",
                    "  100: 0",
                    "  101: 0",
                    "  110: 0",
                    "  111: 0",
                    "  inner:",
                    "    100: 0",
                    "    101: 0",
                    "    110: 0",
                    "    111: 0"));
            this.SetupSpecFile(
                proxy,
                intermediateSpecLayerName,
                intermediateSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativePathFromIntermediateToBaseStr}",
                    "properties:",
                    "  010: 1",
                    "  011: 1",
                    "  110: 1",
                    "  111: 1",
                    "  inner:",
                    "    010: 1",
                    "    011: 1",
                    "    110: 1",
                    "    111: 1"));
            this.SetupSpecFile(
                proxy,
                baseSpecLayerName,
                baseSpecPathStr,
                Utility.JoinLines(
                    "properties:",
                    "  001: 2",
                    "  011: 2",
                    "  101: 2",
                    "  111: 2",
                    "  inner:",
                    "    001: 2",
                    "    011: 2",
                    "    101: 2",
                    "    111: 2"));

            // act
            var derivedSpec = new SpecRoot(proxy, derivedSpecPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec.Int["000"];
            });
            Assert.Equal(2, derivedSpec.Int["001"]);
            Assert.Equal(1, derivedSpec.Int["010"]);
            Assert.Equal(1, derivedSpec.Int["011"]);
            Assert.Equal(0, derivedSpec.Int["100"]);
            Assert.Equal(0, derivedSpec.Int["101"]);
            Assert.Equal(0, derivedSpec.Int["110"]);
            Assert.Equal(0, derivedSpec.Int["111"]);

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = derivedSpec["inner"].Int["000"];
            });
            Assert.Equal(2, derivedSpec["inner"].Int["001"]);
            Assert.Equal(1, derivedSpec["inner"].Int["010"]);
            Assert.Equal(1, derivedSpec["inner"].Int["011"]);
            Assert.Equal(0, derivedSpec["inner"].Int["100"]);
            Assert.Equal(0, derivedSpec["inner"].Int["101"]);
            Assert.Equal(0, derivedSpec["inner"].Int["110"]);
            Assert.Equal(0, derivedSpec["inner"].Int["111"]);

            Assert.Equal(-1, derivedSpec.Int["000", -1]);
            Assert.Equal(2, derivedSpec.Int["001", -1]);
            Assert.Equal(1, derivedSpec.Int["010", -1]);
            Assert.Equal(1, derivedSpec.Int["011", -1]);
            Assert.Equal(0, derivedSpec.Int["100", -1]);
            Assert.Equal(0, derivedSpec.Int["101", -1]);
            Assert.Equal(0, derivedSpec.Int["110", -1]);
            Assert.Equal(0, derivedSpec.Int["111", -1]);

            Assert.Equal(-1, derivedSpec["inner"].Int["000", -1]);
            Assert.Equal(2, derivedSpec["inner"].Int["001", -1]);
            Assert.Equal(1, derivedSpec["inner"].Int["010", -1]);
            Assert.Equal(1, derivedSpec["inner"].Int["011", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["100", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["101", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["110", -1]);
            Assert.Equal(0, derivedSpec["inner"].Int["111", -1]);
        }

        /// <summary>
        /// Base Specの設定値のループした継承関係をテスト。
        /// Test property inheritance of looped base specs.
        /// </summary>
        /// <param name="firstSpecLayerName">
        /// The first spec's layer name.
        /// </param>
        /// <param name="firstSpecPathStr">
        /// The first spec's os-agnostic path. This is derived from the second spec.
        /// </param>
        /// <param name="secondSpecLayerName">
        /// The second spec's layer name.
        /// </param>
        /// <param name="secondSpecPathStr">
        /// The second spec's os-agnostic path. This is derived from the third spec.
        /// </param>
        /// <param name="thirdSpecLayerName">
        /// The third spec's layer name.
        /// </param>
        /// <param name="thirdSpecPathStr">
        /// The third spec's os-agnostic path. This is derived from the first spec.
        /// </param>
        /// <param name="relativePathFromFirstToSecondStr">
        /// The relative path string from the first spec to the second spec.
        /// </param>
        /// <param name="relativePathFromSecondToThirdStr">
        /// The relative path string from the second spec to the third spec.
        /// </param>
        /// <param name="relativePathFromThirdToFirstStr">
        /// The relative path string from the third spec to the first spec.
        /// </param>
        [Theory]
        [InlineData(
            "layer",
            "first.spec",
            "layer",
            "second.spec",
            "layer",
            "third.spec",
            "second.spec",
            "third.spec",
            "first.spec")]
        [InlineData(
            "layer",
            "first.spec",
            "layer",
            "dir/second.spec",
            "layer",
            "dir/dir2/third.spec",
            "dir/second.spec",
            "dir2/third.spec",
            "../../first.spec")]
        [InlineData(
            "layer1",
            "first.spec",
            "layer2",
            "dir/second.spec",
            "layer3",
            "dir/dir2/third.spec",
            "dir/second.spec",
            "dir2/third.spec",
            "../../first.spec")]
        public void LoopedBaseSpecTest(
            string firstSpecLayerName,
            string firstSpecPathStr,
            string secondSpecLayerName,
            string secondSpecPathStr,
            string thirdSpecLayerName,
            string thirdSpecPathStr,
            string relativePathFromFirstToSecondStr,
            string relativePathFromSecondToThirdStr,
            string relativePathFromThirdToFirstStr)
        {
            // arrange
            var firstSpecPath = AgnosticPath.FromAgnosticPathString(
                firstSpecPathStr);
            var proxy = Utility.TestOutSideProxy();

            // Spec key means, which spec has the value.
            // For example,
            //   "100" means only first spec has the value.
            //   "101" means first spec and third spec have the value.
            //   "111" means all of the three specs have the value.
            // Spec value means, which spec the value come from.
            // "0" means the value come from the first spec.
            // "1" means the value come from the second spec.
            // "2" means the value come from the third spec.
            this.SetupSpecFile(
                proxy,
                firstSpecLayerName,
                firstSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativePathFromFirstToSecondStr}",
                    "properties:",
                    "  100: 0",
                    "  101: 0",
                    "  110: 0",
                    "  111: 0",
                    "  inner:",
                    "    100: 0",
                    "    101: 0",
                    "    110: 0",
                    "    111: 0"));
            this.SetupSpecFile(
                proxy,
                secondSpecLayerName,
                secondSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativePathFromSecondToThirdStr}",
                    "properties:",
                    "  010: 1",
                    "  011: 1",
                    "  110: 1",
                    "  111: 1",
                    "  inner:",
                    "    010: 1",
                    "    011: 1",
                    "    110: 1",
                    "    111: 1"));
            this.SetupSpecFile(
                proxy,
                thirdSpecLayerName,
                thirdSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativePathFromThirdToFirstStr}",
                    "properties:",
                    "  001: 2",
                    "  011: 2",
                    "  101: 2",
                    "  111: 2",
                    "  inner:",
                    "    001: 2",
                    "    011: 2",
                    "    101: 2",
                    "    111: 2"));

            // act
            var firstSpec = new SpecRoot(proxy, firstSpecPath);

            // assert
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = firstSpec.Int["000"];
            });
            Assert.Equal(2, firstSpec.Int["001"]);
            Assert.Equal(1, firstSpec.Int["010"]);
            Assert.Equal(1, firstSpec.Int["011"]);
            Assert.Equal(0, firstSpec.Int["100"]);
            Assert.Equal(0, firstSpec.Int["101"]);
            Assert.Equal(0, firstSpec.Int["110"]);
            Assert.Equal(0, firstSpec.Int["111"]);

            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = firstSpec["inner"].Int["000"];
            });
            Assert.Equal(2, firstSpec["inner"].Int["001"]);
            Assert.Equal(1, firstSpec["inner"].Int["010"]);
            Assert.Equal(1, firstSpec["inner"].Int["011"]);
            Assert.Equal(0, firstSpec["inner"].Int["100"]);
            Assert.Equal(0, firstSpec["inner"].Int["101"]);
            Assert.Equal(0, firstSpec["inner"].Int["110"]);
            Assert.Equal(0, firstSpec["inner"].Int["111"]);

            Assert.Equal(-1, firstSpec.Int["000", -1]);
            Assert.Equal(2, firstSpec.Int["001", -1]);
            Assert.Equal(1, firstSpec.Int["010", -1]);
            Assert.Equal(1, firstSpec.Int["011", -1]);
            Assert.Equal(0, firstSpec.Int["100", -1]);
            Assert.Equal(0, firstSpec.Int["101", -1]);
            Assert.Equal(0, firstSpec.Int["110", -1]);
            Assert.Equal(0, firstSpec.Int["111", -1]);

            Assert.Equal(-1, firstSpec["inner"].Int["000", -1]);
            Assert.Equal(2, firstSpec["inner"].Int["001", -1]);
            Assert.Equal(1, firstSpec["inner"].Int["010", -1]);
            Assert.Equal(1, firstSpec["inner"].Int["011", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["100", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["101", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["110", -1]);
            Assert.Equal(0, firstSpec["inner"].Int["111", -1]);
        }

        /// <summary>
        /// Base Specの設定先ファイルが存在しないときの挙動をテスト。
        /// Test the spec's behavior when the base spec is absence.
        /// </summary>
        /// <param name="derivedSpecPathStr">The tested spec file's path.</param>
        /// <param name="relativeBasePathStr">The absence base spec file's path.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec")]
        [InlineData("foo.spec", "dir/bar.spec")]
        [InlineData("dir/foo.spec", "bar.spec")]
        [InlineData("dir1/foo.spec", "dir2/bar.spec")]
        public void AbsenceBaseSpecTest(
            string derivedSpecPathStr, string relativeBasePathStr)
        {
            // arrange
            var derivedSpecPath = AgnosticPath.FromAgnosticPathString(
                derivedSpecPathStr);
            var proxy = Utility.TestOutSideProxy();
            this.SetupSpecFile(
                proxy,
                derivedSpecPathStr,
                Utility.JoinLines(
                    $"base: {relativeBasePathStr}",
                    "properties:",
                    "  defined: 0"));

            // act
            var spec = new SpecRoot(proxy, derivedSpecPath);

            // assert
            Assert.Equal(0, spec.Int["defined"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Int["undefined"];
            });
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// Test the MoldSpec method.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = new SpecRoot(proxy, agnosticPath, true);
            Action<Spec> action = (Spec sp) =>
            {
                _ = sp.Int["foo"];
                _ = sp.Double["bar"];
                _ = sp.Bool["baz"];
                _ = sp.YesNo["qux"];
                _ = sp.OnOff["quux"];
                _ = sp["inner"].Int["inner int"];
                _ = sp.Int["after inner"];
                _ = sp["inner"].Double["inner double"];
                _ = sp["inner"]["inner inner"].Int["inner inner int"];
                _ = sp.Int["日本語キー"];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "base: Spec",
                    "properties:",
                    "  foo: Int",
                    "  bar: Double",
                    "  baz: Bool",
                    "  qux: YesNo",
                    "  quux: OnOff",
                    "  inner:",
                    "    inner int: Int",
                    "    inner double: Double",
                    "    inner inner:",
                    "      inner inner int: Int",
                    "  after inner: Int",
                    "  日本語キー: Int"),
                mold.ToString());
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にデフォルト値指定を含む。
        /// Test the MoldSpec method with default value.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldSpecWithDefaultTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = new SpecRoot(proxy, agnosticPath, true);
            Action<Spec> action = (Spec sp) =>
            {
                _ = sp.Int["foo", 10];
                _ = sp.Double["bar", 3.14];
                _ = sp.Bool["baz", true];
                _ = sp.YesNo["qux", false];
                _ = sp.OnOff["quux", true];
                _ = sp["inner"].Int["inner int", 100];
                _ = sp.Int["after inner", 1024];
                _ = sp["inner"].Double["inner double", 2.71];
                _ = sp["inner"]["inner inner"].Int["inner inner int", -1];
                _ = sp.Int["日本語キー", 0];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "base: Spec",
                    "properties:",
                    "  foo: Int, 10",
                    "  bar: Double, 3.14",
                    "  baz: Bool, True",
                    "  qux: YesNo, no",
                    "  quux: OnOff, on",
                    "  inner:",
                    "    inner int: Int, 100",
                    "    inner double: Double, 2.71",
                    "    inner inner:",
                    "      inner inner int: Int, -1",
                    "  after inner: Int, 1024",
                    "  日本語キー: Int, 0"),
                mold.ToString());
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にデフォルト値指定を含み、デフォルト値は別キーから取得される。
        /// Test the MoldSpec method with default value.
        /// The default value will be gotten from another key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="dynamicDefault">The dynamic default value.</param>
        [Theory]
        [InlineData("foo.spec", 0)]
        [InlineData("foo.spec", -10)]
        [InlineData("foo.spec", 100)]
        [InlineData("dir/bar.spec", 0)]
        [InlineData("dir/bar.spec", -10)]
        [InlineData("dir/bar.spec", 100)]
        [InlineData("dir1/dir2/baz.spec", 0)]
        [InlineData("dir1/dir2/baz.spec", -10)]
        [InlineData("dir1/dir2/baz.spec", 100)]
        public void MoldSpecWithDynamicDefaultTest(string path, int dynamicDefault)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "properties:",
                $"  default: {dynamicDefault}"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath, true);
            Action<Spec> action = (Spec sp) =>
            {
                var def = sp.Int["default"];
                _ = sp.Int["foo", def];
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "base: Spec",
                    "properties:",
                    "  default: Int",
                    $"  foo: Int, {dynamicDefault}"),
                mold.ToString());
        }

        /// <summary>
        /// MoldSpecメソッドによる、アクセスキーと型の取得をテスト。
        /// ただし、アクセス時にif分岐を含み、分岐は別キーのフラグで行われる。
        /// Test the MoldSpec method with if-clause.
        /// The flag will be gotten from another key.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        /// <param name="dynamicSwitch">The dynamic switching flag.</param>
        [Theory]
        [InlineData("foo.spec", true)]
        [InlineData("foo.spec", false)]
        [InlineData("dir/bar.spec", true)]
        [InlineData("dir/bar.spec", false)]
        [InlineData("dir1/dir2/baz.spec", true)]
        [InlineData("dir1/dir2/baz.spec", false)]
        public void MoldSpecWithDynamicSwitchTest(string path, bool dynamicSwitch)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            this.SetupSpecFile(proxy, path, Utility.JoinLines(
                "properties:",
                $"  flag: {dynamicSwitch}"));

            // act
            var spec = new SpecRoot(proxy, agnosticPath, true);
            Action<Spec> action = (Spec sp) =>
            {
                var flag = sp.Bool["flag"];
                if (flag)
                {
                    _ = sp.Int["foo"];
                }
                else
                {
                    _ = sp.Double["bar"];
                }
            };
            var mold = spec.MoldSpec(action);

            // assert
            if (dynamicSwitch)
            {
                Assert.Equal(
                    Utility.JoinLines(
                        "base: Spec",
                        "properties:",
                        "  flag: Bool",
                        "  foo: Int"),
                    mold.ToString());
            }
            else
            {
                Assert.Equal(
                    Utility.JoinLines(
                        "base: Spec",
                        "properties:",
                        "  flag: Bool",
                        "  bar: Double"),
                    mold.ToString());
            }
        }

        /// <summary>
        /// MoldSpecメソッドのBase Specによる挙動の変化をテスト。
        /// Test the MoldSpec method with base spec.
        /// </summary>
        /// <param name="derivedPathStr">The derived spec's path.</param>
        /// <param name="basePathStr">The base spec's path.</param>
        /// <param name="relativeBasePathStr">The base spec's relative path.</param>
        /// <param name="derivedValue">The derived spec's value.</param>
        /// <param name="baseValue">The base spec's value.</param>
        [Theory]
        [InlineData("foo.spec", "bar.spec", "bar.spec", 0, 1)]
        [InlineData("foo.spec", "bar.spec", "bar.spec", 100, 200)]
        [InlineData("foo.spec", "dir/bar.spec", "dir/bar.spec", 100, 200)]
        [InlineData("dir/foo.spec", "bar.spec", "../bar.spec", 100, 200)]
        public void MoldSpecWithBaseSpecTest(
            string derivedPathStr,
            string basePathStr,
            string relativeBasePathStr,
            int derivedValue,
            int baseValue)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var derivedPath =
                AgnosticPath.FromAgnosticPathString(derivedPathStr);
            this.SetupSpecFile(proxy, derivedPathStr, Utility.JoinLines(
                $"base: {relativeBasePathStr}",
                "properties:",
                $"  derived: {derivedValue}"));
            this.SetupSpecFile(proxy, basePathStr, Utility.JoinLines(
                "properties:",
                $"  base: {baseValue}"));

            // act
            var derivedSpec = new SpecRoot(proxy, derivedPath, true);
            Action<Spec> action = (Spec sp) =>
            {
                _ = sp.Int["derived"];
                var referredBase = sp.Int["base"];
                _ = sp.Int["dynamic", referredBase];
            };
            var mold = derivedSpec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "base: Spec",
                    "properties:",
                    "  derived: Int",
                    "  base: Int",
                    $"  dynamic: Int, {baseValue}"),
                mold.ToString());
        }

        private SpecRoot SetupSpecFile(
            OutSideProxy proxy,
            string layerName,
            string layeredPathStr,
            string body)
        {
            var layer = new Layer(proxy, layerName, true);
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

        private SpecRoot SetupSpecFile(
            OutSideProxy proxy, string layeredPathStr, string body)
        {
            return this.SetupSpecFile(proxy, "layer", layeredPathStr, body);
        }
    }
}
