// -----------------------------------------------------------------------
// <copyright file="LayerTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test
{
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Proxy;
    using Xunit;

    /// <summary>
    /// Test the Layer class.
    /// </summary>
    public class LayerTests
    {
        /// <summary>
        /// Test the layer creation.
        /// </summary>
        /// <param name="name">The layer name.</param>
        /// <param name="dependencies">The dependent layers.</param>
        [Theory]
        [InlineData("layer")]
        [InlineData("foo")]
        [InlineData("bar", "foo")]
        [InlineData("baz", "foo", "bar")]
        public void CreationTest(string name, params string[] dependencies)
        {
            OutSideProxy.WithTestMode(() =>
            {
                // arrange
                var agnosticPath =
                    AgnosticPath.FromAgnosticPathString($"{name}/layer.meta");
                OutSideProxy.FileIO.CreateDirectory(agnosticPath.Parent);
                var yamlBody =
                    dependencies.Length == 0 ?
                    "dependencies: []" :
                    "dependencies:\n" + Utility.JoinLines(
                        dependencies.Select(d => $"  - {d}").ToArray());
                OutSideProxy.FileIO.WithTextWriter(agnosticPath, false, (writer) =>
                {
                    writer.Write(yamlBody);
                });

                // act
                var layer = new Layer(name);

                // assert
                Assert.Equal(name, layer.Name);
                Assert.Equal(
                    dependencies.ToHashSet(),
                    layer.Dependencies.ToHashSet());
            });
        }
    }
}
