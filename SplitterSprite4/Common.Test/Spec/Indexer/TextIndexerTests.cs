// -----------------------------------------------------------------------
// <copyright file="TextIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the TextIndexer class.
    /// </summary>
    public class TextIndexerTests : ScalarIndexerTests
    {
        /// <summary>
        /// Test the multi line string accessor.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public override void ScalarAccessTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"without end line\": |+",
                "    \"Live as if you were to die tomorrow.\"",
                "    \"Learn as if you were to live forever.\"",
                "  \"Mohandas Karamchand Gandhi\": |+",
                "    \"Live as if you were to die tomorrow.\"",
                "    \"Learn as if you were to live forever.\"",
                "    \"[End Of Text]\"",
                "  \"Martin Luther King, Jr.\": |+",
                "    \"I have a dream today!\"",
                "    \"[End Of Text]\"",
                "  \"philosophers\":",
                "    \"first\": |+",
                "      \"Thou shouldst eat to live; not live to eat.\"",
                "      \"[End Of Text]\"",
                "    \"second\": |+",
                "      \"A friend to all is a friend to none.\"",
                "      \"[End Of Text]\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            // get value without default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Text["without end line"];
            });
            Assert.Equal(
                Utility.JoinLines(
                    "Live as if you were to die tomorrow.",
                    "Learn as if you were to live forever."),
                spec.Text["Mohandas Karamchand Gandhi"]);
            Assert.Equal(
                "I have a dream today!",
                spec.Text["Martin Luther King, Jr."]);
            Assert.Equal(
                "Thou shouldst eat to live; not live to eat.",
                spec["philosophers"].Text["first"]);
            Assert.Equal(
                "A friend to all is a friend to none.",
                spec["philosophers"].Text["second"]);
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec["philosophers"].Text["third"];
            });

            // get value with default value.
            Assert.Throws<Spec.InvalidSpecAccessException>(() =>
            {
                _ = spec.Text["without end line", "Good morning."];
            });
            Assert.Equal(
                Utility.JoinLines(
                    "Live as if you were to die tomorrow.",
                    "Learn as if you were to live forever."),
                spec.Text["Mohandas Karamchand Gandhi", "Good morning."]);
            Assert.Equal(
                "I have a dream today!",
                spec.Text["Martin Luther King, Jr.", "Good morning."]);
            Assert.Equal(
                "Thou shouldst eat to live; not live to eat.",
                spec["philosophers"].Text["first", "Good morning."]);
            Assert.Equal(
                "A friend to all is a friend to none.",
                spec["philosophers"].Text["second", "Good morning."]);
            Assert.Equal(
                "Good morning.",
                spec["philosophers"].Text["third", "Good morning."]);

            // act
            spec.Text["Martin Luther King, Jr."] = "A lie cannot live.";
            spec["philosophers"].Text["third"] =
                "There are no facts, only interpretations.";
            spec.Text["text with empty lines"] = Utility.JoinLines(
                string.Empty,
                "foo",
                string.Empty,
                "bar",
                string.Empty);

            // assert
            Assert.Equal(
                "A lie cannot live.",
                spec.Text["Martin Luther King, Jr."]);
            Assert.Equal(
                "There are no facts, only interpretations.",
                spec["philosophers"].Text["third"]);
            Assert.Equal(
                Utility.JoinLines(
                    string.Empty,
                    "foo",
                    string.Empty,
                    "bar",
                    string.Empty),
                spec.Text["text with empty lines"]);
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"without end line\": |+",
                    "    \"Live as if you were to die tomorrow.\"",
                    "    \"Learn as if you were to live forever.\"",
                    "  \"Mohandas Karamchand Gandhi\": |+",
                    "    \"Live as if you were to die tomorrow.\"",
                    "    \"Learn as if you were to live forever.\"",
                    "    \"[End Of Text]\"",
                    "  \"Martin Luther King, Jr.\": |+",
                    "    \"A lie cannot live.\"",
                    "    \"[End Of Text]\"",
                    "  \"philosophers\":",
                    "    \"first\": |+",
                    "      \"Thou shouldst eat to live; not live to eat.\"",
                    "      \"[End Of Text]\"",
                    "    \"second\": |+",
                    "      \"A friend to all is a friend to none.\"",
                    "      \"[End Of Text]\"",
                    "    \"third\": |+",
                    "      \"There are no facts, only interpretations.\"",
                    "      \"[End Of Text]\"",
                    "  \"text with empty lines\": |+",
                    "    \"\"",
                    "    \"foo\"",
                    "    \"\"",
                    "    \"bar\"",
                    "    \"\"",
                    "    \"[End Of Text]\""),
                spec.ToString());

            // act
            spec.Save();
            proxy = Utility.PoolClearedProxy(proxy);
            var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"properties\":",
                    "  \"without end line\": |+",
                    "    \"Live as if you were to die tomorrow.\"",
                    "    \"Learn as if you were to live forever.\"",
                    "  \"Mohandas Karamchand Gandhi\": |+",
                    "    \"Live as if you were to die tomorrow.\"",
                    "    \"Learn as if you were to live forever.\"",
                    "    \"[End Of Text]\"",
                    "  \"Martin Luther King, Jr.\": |+",
                    "    \"A lie cannot live.\"",
                    "    \"[End Of Text]\"",
                    "  \"philosophers\":",
                    "    \"first\": |+",
                    "      \"Thou shouldst eat to live; not live to eat.\"",
                    "      \"[End Of Text]\"",
                    "    \"second\": |+",
                    "      \"A friend to all is a friend to none.\"",
                    "      \"[End Of Text]\"",
                    "    \"third\": |+",
                    "      \"There are no facts, only interpretations.\"",
                    "      \"[End Of Text]\"",
                    "  \"text with empty lines\": |+",
                    "    \"\"",
                    "    \"foo\"",
                    "    \"\"",
                    "    \"bar\"",
                    "    \"\"",
                    "    \"[End Of Text]\""),
                reloadedSpec.ToString());
        }
    }
}
