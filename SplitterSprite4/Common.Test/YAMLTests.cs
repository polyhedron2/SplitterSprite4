// -----------------------------------------------------------------------
// <copyright file="YAMLTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using MagicKitchen.SplitterSprite4.Common.YAML;
    using Xunit;

    /// <summary>
    /// Test the YAML class.
    /// </summary>
    public class YAMLTests
    {
        /// <summary>
        /// Test acceptAbsence option.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void AcceptAbsenceTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var yaml = new RootYAML(proxy, agnosticPath, acceptAbsence: true);

            // assert
            Assert.Equal("{}", yaml.ToString());
        }

        /// <summary>
        /// Test the ID of the RootYAML instance.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void SimpleIDTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            Assert.Equal(path, yaml.ID);
        }

        /// <summary>
        /// Test the load and overwrite of the RootYAML instance.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void OverwriteTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // act
            yaml.Overwrite();
            var readLines = proxy.FileIO.WithTextReader(
                AgnosticPath.FromAgnosticPathString(path),
                (reader) => reader.ReadToEnd());

            // assert
            Assert.Equal(this.TestYAMLBody(), readLines);
        }

        /// <summary>
        /// Test the load and rename of the RootYAML instance.
        /// </summary>
        /// <param name="fromPath">Path string which the yaml is renamed from.</param>
        /// <param name="toPath">Path string which the yaml is renamed to.</param>
        [Theory]
        [InlineData("foo.yaml", "foo2.yaml")]
        [InlineData("dir/bar.yaml", "dir/bar2.yaml")]
        [InlineData("dir1/dir2/baz.yaml", "dir3/dir4/baz.yaml")]
        public void SaveAsTest(string fromPath, string toPath)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, fromPath);

            // act
            yaml.SaveAs(AgnosticPath.FromAgnosticPathString(toPath));
            var readLines = proxy.FileIO.WithTextReader(
                AgnosticPath.FromAgnosticPathString(toPath),
                (reader) => reader.ReadToEnd());

            // assert
            Assert.Equal(this.TestYAMLBody(), readLines);
        }

        /// <summary>
        /// Test the load and overwrite of the RootYAML instance.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void IgnoreEmptyMappingChildSaveTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var loadBody = Utility.JoinLines(
                "\"scalar_key\": \"scalar_value\"",
                "\"list_key\":",
                "  - \"foo\"",
                "  - []",
                "  - {}",
                "  -",
                "    - []",
                "  -",
                "    - {}",
                "  - \"first\": []",
                "  - \"second\": {}",
                "\"mapping_key\":",
                "  \"first\": \"bar\"",
                "  \"second\": []",
                "  \"third\": {}",
                "  \"fourth\":",
                "    - []",
                "  \"fifth\":",
                "    - {}",
                "  \"sixth\":",
                "    \"first\": []",
                "  \"seventh\":",
                "    \"first\": {}");
            var saveBody = Utility.JoinLines(
                "\"scalar_key\": \"scalar_value\"",
                "\"list_key\":",
                "  - \"foo\"",
                "  - []",
                "  - {}",
                "  -",
                "    - []",
                "  -",
                "    - {}",
                "  - {}",
                "  - {}",
                "\"mapping_key\":",
                "  \"first\": \"bar\"",
                "  \"fourth\":",
                "    - []",
                "  \"fifth\":",
                "    - {}");
            RootYAML yaml = this.SetupYamlFile(proxy, path, loadBody);

            // act
            yaml.Overwrite(ignoreEmptyMappingChild: true);
            var readLines = proxy.FileIO.WithTextReader(
                AgnosticPath.FromAgnosticPathString(path),
                (reader) => reader.ReadToEnd());

            // assert
            Assert.Equal(saveBody, readLines);
        }

        /// <summary>
        /// Test the remove method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void RemoveTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);
            Assert.Equal(
                Utility.JoinLines(
                    "- \"sqeuence_member_0\"",
                    "- \"sqeuence_member_1\"",
                    "- \"sqeuence_member_2\""),
                yaml["simple_sequence_field"].ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"simple_mapping_key_0\": \"simple_mapping_value_0\"",
                    "\"simple_mapping_key_1\": \"simple_mapping_value_1\"",
                    "\"simple_mapping_key_2\": \"simple_mapping_value_2\""),
                yaml["simple_mapping_field"].ToString());

            // act
            yaml["simple_sequence_field"].Remove(1);
            yaml["simple_mapping_field"].Remove("simple_mapping_key_1");

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "- \"sqeuence_member_0\"",
                    "- \"sqeuence_member_2\""),
                yaml["simple_sequence_field"].ToString());
            Assert.Equal(
                Utility.JoinLines(
                    "\"simple_mapping_key_0\": \"simple_mapping_value_0\"",
                    "\"simple_mapping_key_2\": \"simple_mapping_value_2\""),
                yaml["simple_mapping_field"].ToString());
        }

        /// <summary>
        /// Test the scalar getter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void ScalarGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            Assert.Equal(
                $"{path}[scalar_field]",
                yaml.Scalar["scalar_field"].ID);

            Assert.Equal(
                "\"scalar_value\"",
                yaml.Scalar["scalar_field"].ToString());

            Assert.Equal(
                $"{path}[multiline_text_field]",
                yaml.Scalar["multiline_text_field"].ID);

            Assert.Equal(
                Utility.JoinLines("\"line 0\"", "\"line 1\"", "\"line 2\""),
                yaml.Scalar["multiline_text_field"].ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<ScalarYAML>>(() =>
            {
                var seq = yaml.Scalar["simple_sequence_field"];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<ScalarYAML>>(() =>
            {
                var seq = yaml.Scalar["simple_mapping_field"];
            });

            Assert.Throws<YAML.YAMLKeyUndefinedException>(() =>
            {
                var map = yaml.Scalar["undefined_field"];
            });
        }

        /// <summary>
        /// Test the sequence getter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void SequenceGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            Assert.Equal(
                $"{path}[simple_sequence_field]",
                yaml.Sequence["simple_sequence_field"].ID);

            Assert.Equal(
                Utility.JoinLines(
                    "- \"sqeuence_member_0\"",
                    "- \"sqeuence_member_1\"",
                    "- \"sqeuence_member_2\""),
                yaml.Sequence["simple_sequence_field"].ToString());

            Assert.Equal(
                "\"sqeuence_member_0\"",
                yaml.Sequence["simple_sequence_field"][0].ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml.Sequence["scalar_field"];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml.Sequence["simple_mapping_field"];
            });

            Assert.Throws<YAML.YAMLKeyUndefinedException>(() =>
            {
                var map = yaml.Sequence["undefined_field"];
            });
        }

        /// <summary>
        /// Test the mapping getter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void MappingGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            Assert.Equal(
                $"{path}[simple_mapping_field]",
                yaml.Mapping["simple_mapping_field"].ID);

            Assert.Equal(
                Utility.JoinLines(
                    "\"simple_mapping_key_0\": \"simple_mapping_value_0\"",
                    "\"simple_mapping_key_1\": \"simple_mapping_value_1\"",
                    "\"simple_mapping_key_2\": \"simple_mapping_value_2\""),
                yaml.Mapping["simple_mapping_field"].ToString());

            Assert.Equal(
                "\"simple_mapping_value_0\"",
                yaml.Mapping[
                    "simple_mapping_field"][
                    "simple_mapping_key_0"].ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml.Mapping["scalar_field"];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml.Mapping["simple_sequence_field"];
            });

            Assert.Throws<YAML.YAMLKeyUndefinedException>(() =>
            {
                var map = yaml.Mapping["undefined_field"];
            });
        }

        /// <summary>
        /// Test the scalar getter with default value method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void ScalarDefaultGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            Assert.Equal(
                $"{path}[scalar_field]",
                yaml["scalar_field", new ScalarYAML("dummy")].ID);

            Assert.Equal(
                "\"scalar_value\"",
                yaml["scalar_field", new ScalarYAML("dummy")].ToString());

            var multilineDefaultResult = yaml.Scalar[
                "multiline_text_field",
                new ScalarYAML("dummy line 0", "dummy line 1")];
            Assert.Equal(
                $"{path}[multiline_text_field]",
                multilineDefaultResult.ID);

            Assert.Equal(
                Utility.JoinLines("\"line 0\"", "\"line 1\"", "\"line 2\""),
                multilineDefaultResult.ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<ScalarYAML>>(() =>
            {
                var seq = yaml.Scalar[
                    "simple_sequence_field",
                    new ScalarYAML("dummy")];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<ScalarYAML>>(() =>
            {
                var seq = yaml.Scalar[
                    "simple_mapping_field",
                    new ScalarYAML("dummy")];
            });

            Assert.Equal(
                "\"dummy\"",
                yaml[
                    "undefined_field",
                    new ScalarYAML("dummy")].ToString());
        }

        /// <summary>
        /// Test the sequence getter with default value method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void SequenceDefaultGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            var defaultValue = new SequenceYAML()
                {
                    new ScalarYAML("dummy"),
                };
            var sequenceDefaultResult = yaml.Sequence[
                "simple_sequence_field", defaultValue];

            Assert.Equal(
                $"{path}[simple_sequence_field]",
                sequenceDefaultResult.ID);

            Assert.Equal(
                Utility.JoinLines(
                    "- \"sqeuence_member_0\"",
                    "- \"sqeuence_member_1\"",
                    "- \"sqeuence_member_2\""),
                sequenceDefaultResult.ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml.Sequence[
                    "scalar_field", defaultValue];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml.Sequence[
                    "simple_mapping_field", defaultValue];
            });

            Assert.Equal(
                "- \"dummy\"",
                yaml["undefined_field", defaultValue].ToString());
        }

        /// <summary>
        /// Test the mapping getter with default value method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void MappingDefaultGetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            var defaultValue = new MappingYAML()
                {
                    { "dummy_key", new ScalarYAML("dummy_value") },
                };
            Assert.Equal(
                $"{path}[simple_mapping_field]",
                yaml.Mapping["simple_mapping_field", defaultValue].ID);

            Assert.Equal(
                Utility.JoinLines(
                    "\"simple_mapping_key_0\": \"simple_mapping_value_0\"",
                    "\"simple_mapping_key_1\": \"simple_mapping_value_1\"",
                    "\"simple_mapping_key_2\": \"simple_mapping_value_2\""),
                yaml.Mapping["simple_mapping_field", defaultValue].ToString());

            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml.Mapping["scalar_field", defaultValue];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml.Mapping[
                    "simple_sequence_field", defaultValue];
            });

            Assert.Equal(
                "\"dummy_key\": \"dummy_value\"",
                yaml["undefined_field", defaultValue].ToString());
        }

        /// <summary>
        /// Test the scalar setter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void ScalarSetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML originalYaml = this.SetupYamlFile(proxy, path);

            // act
            originalYaml.Scalar["scalar_field"] =
                new ScalarYAML("new_value_1");
            originalYaml.Scalar["undefined_field"] =
                new ScalarYAML("new_value_2");
            originalYaml.Scalar["multiline_text_field"] =
                new ScalarYAML("new_line_1", "new_line_2");
            originalYaml.Scalar["multiline_undefined_field"] =
                new ScalarYAML("new_line_3", "new_line_4");
            originalYaml.Overwrite();
            var reloadYaml =
                new RootYAML(proxy, AgnosticPath.FromAgnosticPathString(path));

            // assert
            void InternalAssert(RootYAML yaml)
            {
                Assert.Equal(
                    $"{path}[scalar_field]",
                    yaml.Scalar["scalar_field"].ID);
                Assert.Equal(
                    $"{path}[undefined_field]",
                    yaml.Scalar["undefined_field"].ID);
                Assert.Equal(
                    $"{path}[multiline_text_field]",
                    yaml.Scalar["multiline_text_field"].ID);
                Assert.Equal(
                    $"{path}[multiline_undefined_field]",
                    yaml.Scalar["multiline_undefined_field"].ID);

                Assert.Equal(
                    "\"new_value_1\"",
                    yaml.Scalar["scalar_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines("\"new_line_1\"", "\"new_line_2\""),
                    yaml.Scalar["multiline_text_field"].ToString());
                Assert.Equal(
                    "\"new_value_2\"",
                    yaml.Scalar["undefined_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines("\"new_line_3\"", "\"new_line_4\""),
                    yaml.Scalar["multiline_undefined_field"].ToString());
            }

            InternalAssert(originalYaml);
            InternalAssert(reloadYaml);
        }

        /// <summary>
        /// Test the sequence setter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void SequenceSetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML originalYaml = this.SetupYamlFile(proxy, path);

            // act
            originalYaml.Sequence["simple_sequence_field"] =
                new SequenceYAML()
                {
                        new ScalarYAML("new_value_1_1"),
                        new ScalarYAML("new_value_1_2"),
                };
            originalYaml.Sequence["undefined_field"] =
                new SequenceYAML()
                {
                        new ScalarYAML("new_value_2_1"),
                        new ScalarYAML("new_value_2_2"),
                };
            originalYaml.Overwrite();
            var reloadYaml =
                new RootYAML(proxy, AgnosticPath.FromAgnosticPathString(path));

            // assert
            void InternalAssert(RootYAML yaml)
            {
                Assert.Equal(
                    $"{path}[simple_sequence_field]",
                    yaml.Sequence["simple_sequence_field"].ID);
                Assert.Equal(
                    $"{path}[undefined_field]",
                    yaml.Sequence["undefined_field"].ID);

                Assert.Equal(
                    Utility.JoinLines("- \"new_value_1_1\"", "- \"new_value_1_2\""),
                    yaml.Sequence["simple_sequence_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines("- \"new_value_2_1\"", "- \"new_value_2_2\""),
                    yaml.Sequence["undefined_field"].ToString());
            }

            InternalAssert(originalYaml);
            InternalAssert(reloadYaml);
        }

        /// <summary>
        /// Test mapping setter method.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void MappingSetterTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML originalYaml = this.SetupYamlFile(proxy, path);

            // act
            originalYaml.Mapping["simple_mapping_field"] =
                new MappingYAML()
                {
                        { "new_key_1_1", new ScalarYAML("new_value_1_1") },
                        { "new_key_1_2", new ScalarYAML("new_value_1_2") },
                };
            originalYaml.Mapping["undefined_field"] =
                new MappingYAML()
                {
                        { "new_key_2_1", new ScalarYAML("new_value_2_1") },
                        { "new_key_2_2", new ScalarYAML("new_value_2_2") },
                };
            originalYaml.Overwrite();
            var reloadYaml =
                new RootYAML(proxy, AgnosticPath.FromAgnosticPathString(path));

            // assert
            void InternalAssert(RootYAML yaml)
            {
                Assert.Equal(
                    $"{path}[simple_mapping_field]",
                    yaml.Mapping["simple_mapping_field"].ID);
                Assert.Equal(
                    $"{path}[undefined_field]",
                    yaml.Mapping["undefined_field"].ID);

                Assert.Equal(
                    Utility.JoinLines(
                        "\"new_key_1_1\": \"new_value_1_1\"",
                        "\"new_key_1_2\": \"new_value_1_2\""),
                    yaml.Mapping["simple_mapping_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines(
                        "\"new_key_2_1\": \"new_value_2_1\"",
                        "\"new_key_2_2\": \"new_value_2_2\""),
                    yaml.Mapping["undefined_field"].ToString());
            }

            InternalAssert(originalYaml);
            InternalAssert(reloadYaml);
        }

        /// <summary>
        /// Test the keys and values with espaced characters.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void DoubleQuoatationTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML originalYaml = this.SetupYamlFile(proxy, path);

            // act
            originalYaml.Scalar["double_\"quotation\"_field"] =
                new ScalarYAML("new_\"value\"_1");
            originalYaml.Scalar["undefined_\"quotation\"_field"] =
                new ScalarYAML("new_\"value\"_2");
            originalYaml.Scalar["multiline_double_\"quotation\"_field"] =
                new ScalarYAML("new_\"line\"_1", "new_\"line\"_2");
            originalYaml.Scalar["multiline_undefined_\"quotation\"_field"] =
                new ScalarYAML("new_\"line\"_3", "new_\"line\"_4");
            originalYaml.Overwrite();
            var reloadYaml =
                new RootYAML(proxy, AgnosticPath.FromAgnosticPathString(path));

            // assert
            void InternalAssert(RootYAML yaml)
            {
                Assert.Equal(
                    $"{path}[double_\"quotation\"_field]",
                    yaml.Scalar["double_\"quotation\"_field"].ID);
                Assert.Equal(
                    $"{path}[undefined_\"quotation\"_field]",
                    yaml.Scalar["undefined_\"quotation\"_field"].ID);
                Assert.Equal(
                    $"{path}[multiline_double_\"quotation\"_field]",
                    yaml.Scalar["multiline_double_\"quotation\"_field"].ID);
                Assert.Equal(
                    $"{path}[multiline_undefined_\"quotation\"_field]",
                    yaml.Scalar["multiline_undefined_\"quotation\"_field"].ID);

                Assert.Equal(
                    "new_\"value\"_1",
                    yaml.Scalar["double_\"quotation\"_field"].Value);
                Assert.Equal(
                    Utility.JoinLines("new_\"line\"_1", "new_\"line\"_2"),
                    yaml.Scalar["multiline_double_\"quotation\"_field"].Value);
                Assert.Equal(
                    "new_\"value\"_2",
                    yaml.Scalar["undefined_\"quotation\"_field"].Value);
                Assert.Equal(
                    Utility.JoinLines("new_\"line\"_3", "new_\"line\"_4"),
                    yaml.Scalar["multiline_undefined_\"quotation\"_field"].Value);

                Assert.Equal(
                    "\"new_\\\"value\\\"_1\"",
                    yaml.Scalar["double_\"quotation\"_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines("\"new_\\\"line\\\"_1\"", "\"new_\\\"line\\\"_2\""),
                    yaml.Scalar["multiline_double_\"quotation\"_field"].ToString());
                Assert.Equal(
                    "\"new_\\\"value\\\"_2\"",
                    yaml.Scalar["undefined_\"quotation\"_field"].ToString());
                Assert.Equal(
                    Utility.JoinLines("\"new_\\\"line\\\"_3\"", "\"new_\\\"line\\\"_4\""),
                    yaml.Scalar["multiline_undefined_\"quotation\"_field"].ToString());
            }

            InternalAssert(originalYaml);
            InternalAssert(reloadYaml);
        }

        /// <summary>
        /// Test accessor methods that automatically selected.
        /// </summary>
        /// <param name="path">The os-agnostic path string.</param>
        [Theory]
        [InlineData("foo.yaml")]
        [InlineData("dir/bar.yaml")]
        [InlineData("dir1/dir2/baz.yaml")]
        public void AutoAccessorTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            RootYAML yaml = this.SetupYamlFile(proxy, path);

            // assert
            // アクセス名無しでインデクサをつなげれば
            // 型に合わせてYAMLを読み進める

            // Scalarは自動でScalarとして読まれる
            Assert.Equal("\"scalar_value\"", yaml["scalar_field"].ToString());

            // 複数行でも同様
            Assert.Equal(
                Utility.JoinLines("\"line 0\"", "\"line 1\"", "\"line 2\""),
                yaml["multiline_text_field"].ToString());

            // Sequenceは自動でSequenceとして読まれる
            Assert.Equal(
                Utility.JoinLines(
                    "- \"sqeuence_member_0\"",
                    "- \"sqeuence_member_1\"",
                    "- \"sqeuence_member_2\""),
                yaml["simple_sequence_field"].ToString());

            // 整数インデクサでの個別アクセスも可能
            Assert.Equal(
                "\"sqeuence_member_0\"",
                yaml["simple_sequence_field"][0].ToString());

            // Mappingは自動でMappingとして読まれる
            Assert.Equal(
                Utility.JoinLines(
                    "\"simple_mapping_key_0\": \"simple_mapping_value_0\"",
                    "\"simple_mapping_key_1\": \"simple_mapping_value_1\"",
                    "\"simple_mapping_key_2\": \"simple_mapping_value_2\""),
                yaml["simple_mapping_field"].ToString());

            // 文字列インデクサでの個別アクセスも可能
            Assert.Equal(
                "\"simple_mapping_value_0\"",
                yaml[
                    "simple_mapping_field"][
                    "simple_mapping_key_0"].ToString());

            // SequenceとMappingのネスト構造へのアクセスも可能
            Assert.Equal(
                "\"nested_mapping_value_0\"",
                yaml[
                    "nested_sequence_field"][1][
                    "nested_mapping_key_0"].ToString());
            Assert.Equal(
                "\"nested_sequence_member_0\"",
                yaml["nested_sequence_field"][2][0].ToString());
            Assert.Equal(
                "\"nested_mapping_value_3\"",
                yaml[
                    "nested_mapping_field"][
                    "deep_nested_mapping_field"][
                    "nested_mapping_key_3"].ToString());
            Assert.Equal(
                "\"nested_sequence_member_3\"",
                yaml[
                    "nested_mapping_field"][
                    "deep_nested_sequence_field"][0].ToString());

            // 整数インデクサでのデフォルト値使用も可能
            Assert.Equal(
                "\"default\"",
                yaml[
                    "simple_sequence_field"][
                    100, new ScalarYAML("default")].ToString());

            // 文字列インデクサでのデフォルト値使用も可能
            Assert.Equal(
                "\"default\"",
                yaml[
                    "simple_mapping_field"][
                    "simple_mapping_key_100", new ScalarYAML("default")]
                .ToString());

            // 整数インデクサでの上書きも可能
            yaml["simple_sequence_field"][0] =
                new MappingYAML()
                {
                        { "new key", new ScalarYAML("new value") },
                };
            Assert.Equal(
                "\"new key\": \"new value\"",
                yaml["simple_sequence_field"][0].ToString());

            // 文字列インデクサでの上書きも可能
            yaml["simple_mapping_field"]["simple_mapping_key_0"] =
                new SequenceYAML()
                {
                        new ScalarYAML("new value"),
                };
            Assert.Equal(
                "- \"new value\"",
                yaml[
                    "simple_mapping_field"][
                    "simple_mapping_key_0"].ToString());

            // 定義された内容が配列でない場合、整数インデクサを用いれば例外
            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml["scalar_field"][0];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<SequenceYAML>>(() =>
            {
                var seq = yaml["simple_mapping_field"][0];
            });

            // 定義された内容が辞書でない場合、文字列インデクサを用いれば例外
            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml["scalar_field"]["foo"];
            });
            Assert.Throws<YAML.YAMLTypeSlipException<MappingYAML>>(() =>
            {
                var seq = yaml["simple_sequence_field"]["foo"];
            });

            // 未定義の値にアクセスすると例外
            Assert.Throws<YAML.YAMLKeyUndefinedException>(() =>
            {
                var map = yaml["undefined_field"];
            });
        }

        private string TestYAMLBody() => Utility.JoinLines(
            "\"scalar_field\": \"scalar_value\"",
            "\"multiline_text_field\": |+",
            "  \"line 0\"",
            "  \"line 1\"",
            "  \"line 2\"",
            "\"simple_sequence_field\":",
            "  - \"sqeuence_member_0\"",
            "  - \"sqeuence_member_1\"",
            "  - \"sqeuence_member_2\"",
            "\"simple_mapping_field\":",
            "  \"simple_mapping_key_0\": \"simple_mapping_value_0\"",
            "  \"simple_mapping_key_1\": \"simple_mapping_value_1\"",
            "  \"simple_mapping_key_2\": \"simple_mapping_value_2\"",
            "\"nested_sequence_field\":",
            "  - |+",
            "    \"nested line 0\"",
            "    \"nested line 1\"",
            "    \"nested line 2\"",
            "  - \"nested_mapping_key_0\": \"nested_mapping_value_0\"",
            "    \"nested_mapping_key_1\": \"nested_mapping_value_1\"",
            "    \"nested_mapping_key_2\": \"nested_mapping_value_2\"",
            "  -",
            "    - \"nested_sequence_member_0\"",
            "    - \"nested_sequence_member_1\"",
            "    - \"nested_sequence_member_2\"",
            "  - []",
            "  - {}",
            "\"nested_mapping_field\":",
            "  \"deep_nested_multiline_text_field\": |+",
            "    \"nested line 3\"",
            "    \"nested line 4\"",
            "    \"nested line 5\"",
            "  \"deep_nested_mapping_field\":",
            "    \"nested_mapping_key_3\": \"nested_mapping_value_3\"",
            "    \"nested_mapping_key_4\": \"nested_mapping_value_4\"",
            "    \"nested_mapping_key_5\": \"nested_mapping_value_5\"",
            "  \"deep_nested_sequence_field\":",
            "    - \"nested_sequence_member_3\"",
            "    - \"nested_sequence_member_4\"",
            "    - \"nested_sequence_member_5\"",
            "  \"deep_nested_empty_sequence\": []",
            "  \"deep_nested_empty_mapping\": {}",
            "\" key:with[some}spec'ialtokens{ \": \" value:with[some}special]tokens{ \"");

        private RootYAML SetupYamlFile(OutSideProxy proxy, string path)
        {
            return this.SetupYamlFile(proxy, path, this.TestYAMLBody());
        }

        private RootYAML SetupYamlFile(OutSideProxy proxy, string path, string body)
        {
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            proxy.FileIO.CreateDirectory(agnosticPath.Parent);

            proxy.FileIO.WithTextWriter(agnosticPath, false, (writer) =>
            {
                writer.Write(body);
            });

            return new RootYAML(proxy, agnosticPath);
        }
    }
}
