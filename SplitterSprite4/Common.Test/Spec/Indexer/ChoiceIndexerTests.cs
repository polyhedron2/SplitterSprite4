// -----------------------------------------------------------------------
// <copyright file="ChoiceIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using System;
    using System.Collections.Generic;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the ChoiceIndexer class.
    /// </summary>
    public class ChoiceIndexerTests
    {
        /// <summary>
        /// Test ChoiceIndexer class with list and func definition.
        /// </summary>
        public class ChoiceIndexerWithListAndFuncTests : LiteralIndexerTests
        {
            /// <summary>
            /// Test the choice accessor.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            public override void LiteralAccessTest(string path)
            {
                // arrange
                var proxy = Utility.TestOutSideProxy();
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"黄色\"",
                    "  \"あか\": \"赤\"",
                    "  \"みどり\": \"緑\"",
                    "  \"あお\": \"青\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                Func<Color, string> func = (Color color) =>
                {
                    switch (color.Name)
                    {
                        case "RED":
                            return "赤";
                        case "GREEN":
                            return "緑";
                        case "BLUE":
                            return "青";
                        default:
                            // YELLOW is not supported.
                            throw new Exception($"{color.Name} is not supported.");
                    }
                };
                var choices = new List<Color>
                {
                    Color.RED,
                    Color.GREEN,
                    Color.BLUE,
                };

                // assert
                // get value without default value.

                // Duplicated string.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    spec.Choice(
                        new List<Color>()
                        {
                            Color.RED,
                            Color.RED2,
                            Color.GREEN,
                            Color.BLUE,
                        },
                        func);
                });

                // null choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(null, func);
                });

                // empty choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(new List<Color>(), func);
                });

                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices, func)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(choices, func)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(choices, func)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(choices, func)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices, func)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(choices, func)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(choices, func)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(choices, func)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(choices, func)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(choices, func)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(choices, func)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(choices, func)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(choices, func)["べつのあか"]);
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"黄色\"",
                        "  \"あか\": \"赤\"",
                        "  \"みどり\": \"緑\"",
                        "  \"あお\": \"青\"",
                        "  \"べつのあか\": \"赤\""),
                    spec.ToString());

                // act
                spec.Save();
                proxy = Utility.PoolClearedProxy(proxy);
                var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"黄色\"",
                        "  \"あか\": \"赤\"",
                        "  \"みどり\": \"緑\"",
                        "  \"あお\": \"青\"",
                        "  \"べつのあか\": \"赤\""),
                    reloadedSpec.ToString());
            }
        }

        /// <summary>
        /// Test ChoiceIndexer class with list definition.
        /// </summary>
        public class ChoiceIndexerWithListTests : LiteralIndexerTests
        {
            /// <summary>
            /// Test the choice accessor.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            public override void LiteralAccessTest(string path)
            {
                // arrange
                var proxy = Utility.TestOutSideProxy();
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"YELLOW\"",
                    "  \"あか\": \"RED\"",
                    "  \"みどり\": \"GREEN\"",
                    "  \"あお\": \"BLUE\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var choices = new List<Color>
                {
                    Color.RED,
                    Color.GREEN,
                    Color.BLUE,
                };

                // assert
                // get value without default value.

                // Duplicated string.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    spec.Choice(
                        new List<Color>()
                        {
                            Color.RED,
                            Color.RED2,
                            Color.GREEN,
                            Color.BLUE,
                        });
                });

                // null choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(null as IEnumerable<Color>);
                });

                // empty choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(new List<Color>());
                });

                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(choices)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(choices)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(choices)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(choices)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(choices)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(choices)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(choices)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(choices)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(choices)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(choices)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(choices)["べつのあか"]);
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"YELLOW\"",
                        "  \"あか\": \"RED\"",
                        "  \"みどり\": \"GREEN\"",
                        "  \"あお\": \"BLUE\"",
                        "  \"べつのあか\": \"RED\""),
                    spec.ToString());

                // act
                spec.Save();
                proxy = Utility.PoolClearedProxy(proxy);
                var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"YELLOW\"",
                        "  \"あか\": \"RED\"",
                        "  \"みどり\": \"GREEN\"",
                        "  \"あお\": \"BLUE\"",
                        "  \"べつのあか\": \"RED\""),
                    reloadedSpec.ToString());
            }
        }

        /// <summary>
        /// Test ChoiceIndexer class with dict definition.
        /// </summary>
        public class ChoiceIndexerWithDictTests : LiteralIndexerTests
        {
            /// <summary>
            /// Test the choice accessor.
            /// </summary>
            /// <param name="path">The os-agnostic path of the spec file.</param>
            public override void LiteralAccessTest(string path)
            {
                // arrange
                var proxy = Utility.TestOutSideProxy();
                var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
                Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                    "\"properties\":",
                    "  \"invalid\": \"GIALLO\"",
                    "  \"あか\": \"ROSSO\"",
                    "  \"みどり\": \"VERDE\"",
                    "  \"あお\": \"AZZURRO\""));

                // act
                var spec = SpecRoot.Fetch(proxy, agnosticPath);
                var choices = new Dictionary<Color, string>
                {
                    { Color.RED, "ROSSO" },
                    { Color.GREEN, "VERDE" },
                    { Color.BLUE, "AZZURRO" },
                };

                // assert
                // get value without default value.

                // Duplicated string.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    spec.Choice(
                        new Dictionary<Color, string>()
                        {
                            { Color.RED, "ROSSO" },
                            { Color.GREEN, "VERDE" },
                            { Color.BLUE, "AZZURRO" },
                            { Color.YELLOW, "ROSSO" },
                        });
                });

                // null choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(null as Dictionary<Color, string>);
                });

                // empty choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(new Dictionary<Color, string>());
                });

                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(choices)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(choices)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(choices)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(choices)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(choices)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(choices)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(choices)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(choices)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(choices)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(choices)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(choices)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(choices)["べつのあか"]);
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"GIALLO\"",
                        "  \"あか\": \"ROSSO\"",
                        "  \"みどり\": \"VERDE\"",
                        "  \"あお\": \"AZZURRO\"",
                        "  \"べつのあか\": \"ROSSO\""),
                    spec.ToString());

                // act
                spec.Save();
                proxy = Utility.PoolClearedProxy(proxy);
                var reloadedSpec = SpecRoot.Fetch(proxy, agnosticPath);

                // assert
                Assert.Equal(
                    Utility.JoinLines(
                        "\"properties\":",
                        "  \"invalid\": \"GIALLO\"",
                        "  \"あか\": \"ROSSO\"",
                        "  \"みどり\": \"VERDE\"",
                        "  \"あお\": \"AZZURRO\"",
                        "  \"べつのあか\": \"ROSSO\""),
                    reloadedSpec.ToString());
            }
        }

        /// <summary>
        /// Dummy color class for testing ChoiceIndexer.
        /// </summary>
        public class Color
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Color"/> class.
            /// </summary>
            /// <param name="name">The name of color.</param>
            private Color(string name)
            {
                this.Name = name;
            }

            /// <summary>
            /// Gets red color.
            /// </summary>
            public static Color RED
            {
                get => new Color("RED");
            }

            /// <summary>
            /// Gets duplicated red color.
            /// </summary>
            public static Color RED2
            {
                get => new Color("RED");
            }

            /// <summary>
            /// Gets green color.
            /// </summary>
            public static Color GREEN
            {
                get => new Color("GREEN");
            }

            /// <summary>
            /// Gets blue color.
            /// </summary>
            public static Color BLUE
            {
                get => new Color("BLUE");
            }

            /// <summary>
            /// Gets yellow color.
            /// </summary>
            public static Color YELLOW
            {
                get => new Color("YELLOW");
            }

            /// <summary>
            /// Gets the name of this color.
            /// </summary>
            public string Name { get; }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                if (obj is Color)
                {
                    var that = obj as Color;
                    return this.Name == that.Name;
                }
                else
                {
                    return false;
                }
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return this.Name.GetHashCode();
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
