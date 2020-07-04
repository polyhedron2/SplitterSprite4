// -----------------------------------------------------------------------
// <copyright file="InteriorIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.YAML;

    /// <summary>
    /// Indexer class for SpawnerChild instances.
    /// </summary>
    /// <typeparam name="T">Expected SpawnerChild type.</typeparam>
    public class InteriorIndexer<T>
        : IIndexerWithDefault<T, Type>
        where T : ISpawnerChild<object>
    {
        private Spec parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteriorIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        internal InteriorIndexer(Spec parent)
        {
            // molding default process validates type parameter T.
            parent.MoldingDefault<T>().GetType();

            this.parent = parent;
        }

        /// <summary>
        /// Indexer for SpecChild.
        /// </summary>
        /// <param name="key">The string key for the SpecChild.</param>
        /// <returns>The SpecChild instance.</returns>
        public T this[string key]
        {
            get => this.IndexGet(
                key,
                (specChild) => specChild.SpawnerType,
                $"Spawner, {Spec.EncodeType(typeof(T))}");

            set
            {
                lock (this.parent.Properties)
                {
                    try
                    {
                        var specChild = this.parent.Child[key, typeof(T)];

                        // valueのTypeがDefaultで取得された場合、
                        // yamlにSpawnerTypeが無いため明示的に書き込む。
                        // Write spawner type explicitly,
                        // because the its spawner type may not be in yaml
                        // in case of default type.
                        specChild.SpawnerType = value.GetType();

                        specChild.Body["properties"] =
                            value.Spec.Properties.Clone(
                                $"{this.parent.Properties.ID}[{key}]");
                    }
                    catch (Exception ex)
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            typeof(T).Name,
                            ex);
                    }
                }
            }
        }

        /// <summary>
        /// Indexer for SpecChild with default spawner type.
        /// </summary>
        /// <param name="key">The string key for the SpecChild.</param>
        /// <param name="defaultType">Default spawner type.</param>
        /// <returns>The SpecChild instance.</returns>
        public T this[string key, Type defaultType]
        {
            get
            {
                try
                {
                    Spawner.ValidateSpawnerType<T>(defaultType);
                }
                catch (Exception ex)
                {
                    throw new Spec.InvalidSpecDefinitionException(
                        $"型{defaultType.Name}は" +
                        $"Interior<{typeof(T).Name}>の" +
                        $"デフォルト値として不正です。",
                        ex);
                }

                var moldingAccessCode =
                    $"Spawner, {Spec.EncodeType(typeof(T))}, " +
                    $"{Spec.EncodeType(defaultType)}";

                return this.IndexGet(
                    key,
                    (specChild) =>
                    {
                        try
                        {
                            return specChild.SpawnerType;
                        }
                        catch (YAML.YAMLKeyUndefinedException)
                        {
                            return defaultType;
                        }
                    },
                    moldingAccessCode);
            }
        }

        private T IndexGet(
            string key,
            Func<SpecChild, Type> getSpawnerType,
            string moldingAccessCode)
        {
            lock (this.parent.Properties)
            {
                var childSpec = this.parent.Child[key, typeof(T)];

                try
                {
                    if (this.parent.IsMolding)
                    {
                        _ = childSpec.Mold;
                        this.parent.Mold[key]["spawner"] =
                            new ScalarYAML(moldingAccessCode);
                    }

                    var type = getSpawnerType(childSpec);
                    var spawner = (T)Activator.CreateInstance(type);
                    spawner.Spec = childSpec;

                    if (this.parent.IsMolding)
                    {
                        // 正当なSpawner型を得られた場合には、
                        // ダミーSpecによるSpawnを実行することで
                        // SpawnwerChild.Spawn()をmoldに記録する。
                        // If valid spawner type is defined in spec,
                        // call DummySpawn with dummy spec to mold calls in
                        // SpawnerChild.Spawn().
                        Spawner.DummySpawn<object>(spawner);
                    }

                    return spawner;
                }
                catch (Exception ex)
                {
                    if (this.parent.IsMolding)
                    {
                        // 正当なSpawner型を得られなかった場合には、
                        // ダミーのSpecChildを設定した
                        // moldingDefaultを返すことで、
                        // moldingDefault.Spawn()の値をmoldしない。
                        // If valid spawner type is not defined in spec,
                        // return molding default instance
                        // with dummy SpecChild not to mold calls in
                        // moldingDefault.Spawn().
                        var moldingDefault =
                            this.parent.MoldingDefault<T>();
                        moldingDefault.Spec =
                            SpecRoot.CreateDummy(this.parent.Proxy)
                            .Child[key, typeof(T)];
                        return moldingDefault;
                    }
                    else
                    {
                        throw new Spec.InvalidSpecAccessException(
                            $"{this.parent.Properties.ID}[{key}]",
                            typeof(T).Name,
                            ex);
                    }
                }
            }
        }
    }
}
