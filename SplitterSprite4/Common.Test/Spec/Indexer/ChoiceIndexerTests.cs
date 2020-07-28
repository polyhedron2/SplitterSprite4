// -----------------------------------------------------------------------
// <copyright file="ChoiceIndexerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Spec.Indexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using Xunit;

    /// <summary>
    /// Test the ChoiceIndexer class.
    /// </summary>
    public class ChoiceIndexerTests
    {
        /// <summary>
        /// Gets choices list for testing.
        /// </summary>
        public static List<Color> ListChoices
        {
            get => new List<Color>
            {
                Color.RED,
                Color.GREEN,
                Color.BLUE,
            };
        }

        /// <summary>
        /// Gets choices dictionary for testing.
        /// </summary>
        public static IEnumerable<KeyValuePair<Color, string>> DictChoices
        {
            get => new Dictionary<Color, string>
            {
                { Color.RED, "ROSSO" },
                { Color.GREEN, "VERDE" },
                { Color.BLUE, "AZZURRO" },
            }.OrderBy(kv => kv.Value);
        }

        /// <summary>
        /// Func for testing ChoiceIndexer.
        /// </summary>
        /// <param name="color">Choice color.</param>
        /// <returns>String value for spec.</returns>
        public static string ChoiceToSpecStr(Color color)
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
        }

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
                        ChoiceToSpecStr);
                });

                // null choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice<Color>(null, ChoiceToSpecStr);
                });

                // empty choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(new List<Color>(), ChoiceToSpecStr);
                });

                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(ListChoices, ChoiceToSpecStr)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(ListChoices, ChoiceToSpecStr)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(ListChoices, ChoiceToSpecStr)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(ListChoices, ChoiceToSpecStr)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(ListChoices, ChoiceToSpecStr)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(ListChoices, ChoiceToSpecStr)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(ListChoices, ChoiceToSpecStr)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(ListChoices, ChoiceToSpecStr)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(ListChoices, ChoiceToSpecStr)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(ListChoices, ChoiceToSpecStr)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(ListChoices, ChoiceToSpecStr)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(ListChoices, ChoiceToSpecStr)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(ListChoices, ChoiceToSpecStr)["べつのあか"]);
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
                    _ = spec.Choice(ListChoices)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(ListChoices)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(ListChoices)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(ListChoices)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(ListChoices)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(ListChoices)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(ListChoices)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(ListChoices)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(ListChoices)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(ListChoices)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(ListChoices)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(ListChoices)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(ListChoices)["べつのあか"]);
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
                    _ = spec.Choice(DictChoices)["invalid"];
                });

                Assert.Equal(Color.RED, spec.Choice(DictChoices)["あか"]);
                Assert.Equal(Color.GREEN, spec.Choice(DictChoices)["みどり"]);
                Assert.Equal(Color.BLUE, spec.Choice(DictChoices)["あお"]);

                // get value with default value.
                // invalid choice.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    _ = spec.Choice(DictChoices)["invalid", Color.RED];
                });

                // Default value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecDefinitionException>(() =>
                {
                    _ = spec.Choice(DictChoices)["なし", Color.YELLOW];
                });

                Assert.Equal(Color.RED, spec.Choice(DictChoices)["なし", Color.RED]);
                Assert.Equal(Color.RED, spec.Choice(DictChoices)["あか", Color.RED]);
                Assert.Equal(Color.GREEN, spec.Choice(DictChoices)["みどり", Color.RED]);
                Assert.Equal(Color.BLUE, spec.Choice(DictChoices)["あお", Color.RED]);

                // act
                // Valid setter access.
                spec.Choice(DictChoices)["べつのあか"] = Color.RED;

                // Set value which is not contained in the choices.
                Assert.Throws<Spec.InvalidSpecAccessException>(() =>
                {
                    spec.Choice(DictChoices)["きいろ"] = Color.YELLOW;
                });

                // assert
                Assert.Equal(Color.RED, spec.Choice(DictChoices)["べつのあか"]);
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
