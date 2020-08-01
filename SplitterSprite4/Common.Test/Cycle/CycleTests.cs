// -----------------------------------------------------------------------
// <copyright file="CycleTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Cycle
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Clock;
    using MagicKitchen.SplitterSprite4.Common.Cycle;
    using MagicKitchen.SplitterSprite4.Common.Effect;
    using MagicKitchen.SplitterSprite4.Common.Spawner;
    using MagicKitchen.SplitterSprite4.Common.Spec;
    using MagicKitchen.SplitterSprite4.Common.Status;
    using Xunit;

    /// <summary>
    /// Test the Cycle class.
    /// </summary>
    public class CycleTests
    {
        public enum QuestResultEnum
        {
            /// <summary>
            /// Continue quest.
            /// </summary>
            Continue,

            /// <summary>
            /// Enconter a monster.
            /// </summary>
            EncounterEnemy,
        }

        public enum BattleResultEnum
        {
            /// <summary>
            /// Continue the battle.
            /// </summary>
            Continue,

            /// <summary>
            /// Win the battle.
            /// </summary>
            [Description("勝利時")]
            Win,

            /// <summary>
            /// Lose the battle.
            /// </summary>
            [Description("敗北時")]
            Lose,
        }

        public enum GameOverResultEnum
        {
            /// <summary>
            /// Continue game over display.
            /// </summary>
            Continue,
        }

        /// <summary>
        /// Test clock's result candidate enum enumerator.
        /// </summary>
        [Fact]
        public void ResultCandidatesTest()
        {
            // arrange
            var status = new Status(0, 100);
            var questClock = new QuestClock(status, new List<Effect>(), 5);
            var battleClock = new BattleClock(status, new List<Effect>(), 10);
            var gameOverClock = new GameOverClock(new List<Effect>());

            // act
            var questCandidates = questClock.ResultCandidates().ToHashSet();
            var battleCandidates = battleClock.ResultCandidates().ToHashSet();
            var gameOverCandidates = gameOverClock.ResultCandidates().ToHashSet();

            // assert
            Assert.Equal(
                new HashSet<string>
                {
                    "Continue",
                    "EncounterEnemy",
                },
                questCandidates);
            Assert.Equal(
                new HashSet<string>
                {
                    "Continue",
                    "勝利時",
                    "敗北時",
                },
                battleCandidates);
            Assert.Equal(
                new HashSet<string>
                {
                    "Continue",
                },
                gameOverCandidates);
        }

        /// <summary>
        /// Test Cycle class's Update and Draw method.
        /// </summary>
        [Fact]
        public void CycleTest()
        {
            // arrange
            // hp (80) is more than enemy's hp (50)
            var status = new Status(x: 0, hp: 80);

            /* 状態遷移図は下記の通り
             *
             *                  +-----------------------+
             *                  |                       |
             *     [EncounterEnemy]                     |
             *                  |                       |
             *           +---+  |                 +---+ |                    +---+
             *           |   |  |                 |   | |                    |   |
             *    [Continue] |  |          [Continue] | |             [Continue] |
             *           |   V  |                 |   V V                    |   V
             * [開始]-> ( 探  索 ) <--[勝利時]-- ( 戦  闘 ) --[敗北時]--> (ゲームオーバー)
             *
             */
            var questMode = new Mode(
                new QuestClock(
                    status,
                    new List<Effect>
                    {
                        new XEffect(status),
                    },
                    enemyInterval: 3),
                new Dictionary<string, string>
                {
                    { "Continue", "探索" },
                    { "EncounterEnemy", "戦闘" },
                });
            var battleMode = new Mode(
                new BattleClock(
                    status,
                    new List<Effect>
                    {
                        new HPEffect(status),
                        new EnemyHPEffect(status),
                    },
                    damage: 10),
                new Dictionary<string, string>
                {
                    { "Continue", "戦闘" },
                    { "勝利時", "探索" },
                    { "敗北時", "ゲームオーバー" },
                });
            var gameOverMode = new Mode(
                new GameOverClock(
                    new List<Effect>
                    {
                        new GameOverEffect(status),
                    }),
                new Dictionary<string, string>
                {
                    { "Continue", "ゲームオーバー" },
                });
            var cycle = new Cycle(
                "タイトル",
                60,
                "探索",
                new Dictionary<string, Mode>
                {
                    { "探索", questMode },
                    { "戦闘", battleMode },
                    { "ゲームオーバー", gameOverMode },
                });

            // act
            int loopNum = 16;
            for (int i = 0; i < loopNum; i++)
            {
                cycle.Update();
                cycle.Draw();
            }

            // assert
            Assert.Equal(
                string.Join("\n", new List<string>
                {
                    // 探索開始
                    "X is 1",
                    "X is 2",

                    // X == 3で敵と遭遇
                    "HP is 80",
                    "Enemy HP is 50",
                    "HP is 70",
                    "Enemy HP is 40",
                    "HP is 60",
                    "Enemy HP is 30",
                    "HP is 50",
                    "Enemy HP is 20",
                    "HP is 40",
                    "Enemy HP is 10",

                    // EnemyHP == 0で勝利し探索に戻る
                    "X is 0",
                    "X is 1",
                    "X is 2",

                    // X == 3で再度敵と遭遇
                    "HP is 30",
                    "Enemy HP is 50",
                    "HP is 20",
                    "Enemy HP is 40",
                    "HP is 10",
                    "Enemy HP is 30",

                    // HP == 0でゲームオーバー
                    "game over",
                    "game over",
                    "game over",
                }),
                string.Join("\n", status.ApplyResults));
        }

        /// <summary>
        /// Test CycleSpawner's Spawn method.
        /// </summary>
        [Fact]
        public void SpawnCycleTest()
        {
            // arrange
            /* 状態遷移図は下記の通り
             *
             *                  +-----------------------+
             *                  |                       |
             *     [EncounterEnemy]                     |
             *                  |                       |
             *           +---+  |                 +---+ |                    +---+
             *           |   |  |                 |   | |                    |   |
             *    [Continue] |  |          [Continue] | |             [Continue] |
             *           |   V  |                 |   V V                    |   V
             * [開始]-> ( 探  索 ) <--[勝利時]-- ( 戦  闘 ) --[敗北時]--> (ゲームオーバー)
             *
             */
            var proxy = Utility.TestOutSideProxy();
            Utility.SetupSpecFile(proxy, "status.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(StatusSpawner))}\"",
                "\"properties\":",
                "  \"X\": \"0\"",
                "  \"HP\": \"80\""));
            Utility.SetupSpecFile(proxy, "quest_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(QuestClockSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\"",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"x_effect.spec\"",
                "  \"敵の出現間隔\": \"3\""));
            Utility.SetupSpecFile(proxy, "battle_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(BattleClockSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\"",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"hp_effect.spec\"",
                "      \"1\": \"enemy_hp_effect.spec\"",
                "  \"ダメージ量\": \"10\""));
            Utility.SetupSpecFile(proxy, "game_over_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(GameOverClockSpawner))}\"",
                "\"properties\":",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"game_over_effect.spec\""));
            Utility.SetupSpecFile(proxy, "x_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(XEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "hp_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(HPEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "enemy_hp_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(EnemyHPEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "game_over_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(GameOverEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "cycle.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(CycleSpawner))}\"",
                "\"properties\":",
                "  \"タイトル\": \"タイトル\"",
                "  \"FPS\": \"60\"",
                "  \"開始モード\": \"探索\"",
                "  \"モード一覧\":",
                "    \"DictBody\":",
                "      \"探索\":",
                "        \"状態更新\": \"quest_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"探索\"",
                "          \"EncounterEnemy\": \"戦闘\"",
                "      \"戦闘\":",
                "        \"状態更新\": \"battle_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"戦闘\"",
                "          \"勝利時\": \"探索\"",
                "          \"敗北時\": \"ゲームオーバー\"",
                "      \"ゲームオーバー\":",
                "        \"状態更新\": \"game_over_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"ゲームオーバー\""));
            Utility.SetupSpecFile(proxy, "entry_point.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"ゲームサイクル\": \"cycle.spec\"",
                "  \"ステータス\": \"status.spec\""));

            // act
            var entry_point = SpecRoot.Fetch(proxy, AgnosticPath.FromAgnosticPathString("entry_point.spec"));
            var cycle = entry_point.Exterior<CycleSpawner>()["ゲームサイクル"].Spawn();
            var status = entry_point.Exterior<StatusSpawner>()["ステータス"].Spawn();
            int loopNum = 16;
            for (int i = 0; i < loopNum; i++)
            {
                cycle.Update();
                cycle.Draw();
            }

            // assert
            Assert.Equal(
                string.Join("\n", new List<string>
                {
                    // 探索開始
                    "X is 1",
                    "X is 2",

                    // X == 3で敵と遭遇
                    "HP is 80",
                    "Enemy HP is 50",
                    "HP is 70",
                    "Enemy HP is 40",
                    "HP is 60",
                    "Enemy HP is 30",
                    "HP is 50",
                    "Enemy HP is 20",
                    "HP is 40",
                    "Enemy HP is 10",

                    // EnemyHP == 0で勝利し探索に戻る
                    "X is 0",
                    "X is 1",
                    "X is 2",

                    // X == 3で再度敵と遭遇
                    "HP is 30",
                    "Enemy HP is 50",
                    "HP is 20",
                    "Enemy HP is 40",
                    "HP is 10",
                    "Enemy HP is 30",

                    // HP == 0でゲームオーバー
                    "game over",
                    "game over",
                    "game over",
                }),
                string.Join("\n", status.ApplyResults));
        }

        /// <summary>
        /// CycleSpawnerのMold結果をテスト。
        /// Test the MoldSpec method on CycleSpawner.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldCycleSpawnerTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<SpecRoot> action = (sp) =>
            {
                var cycleSpawner = new CycleSpawner();
                cycleSpawner.Spec = sp;
                _ = cycleSpawner.Spawn();
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"タイトル\": \"Keyword\"",
                    "  \"FPS\": \"Int\"",
                    "  \"開始モード\": \"Keyword\"",
                    "  \"モード一覧\":",
                    "    \"MoldingType\": \"Dict, Keyword\"",
                    "    \"DictBody\":",
                    "      \"\":",
                    $"        \"状態更新\": \"Exterior, {Spec.EncodeType(typeof(ISpawnerRootWithoutArgs<IClock>))}\"",
                    "        \"遷移先\":",
                    "          \"Continue\": \"Choice, \"",
                    "          \"EncounterEnemy\": \"Choice, \""),
                mold.ToString(true));
        }

        /// <summary>
        /// CycleSpawnerのMold結果をテスト。 ただし、Specは開始モードの設定を含む。
        /// Test the MoldSpec method on CycleSpawner. Spec contains "開始モード" for CycleSpawner.
        /// </summary>
        /// <param name="path">The os-agnostic path of the spec file.</param>
        [Theory]
        [InlineData("foo.spec")]
        [InlineData("dir/bar.spec")]
        [InlineData("dir1/dir2/baz.spec")]
        public void MoldCycleSpawnerWithInitialModeNameOnSpecTest(string path)
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            var agnosticPath = AgnosticPath.FromAgnosticPathString(path);
            Utility.SetupSpecFile(proxy, path, Utility.JoinLines(
                "\"properties\":",
                "  \"開始モード\": \"INIT\""));

            // act
            var spec = SpecRoot.Fetch(proxy, agnosticPath, true);
            Action<SpecRoot> action = (sp) =>
            {
                var cycleSpawner = new CycleSpawner();
                cycleSpawner.Spec = sp;
                _ = cycleSpawner.Spawn();
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"タイトル\": \"Keyword\"",
                    "  \"FPS\": \"Int\"",
                    "  \"開始モード\": \"Keyword\"",
                    "  \"モード一覧\":",
                    "    \"MoldingType\": \"Dict, Keyword\"",
                    "    \"DictBody\":",
                    "      \"INIT\":",
                    $"        \"状態更新\": \"Exterior, {Spec.EncodeType(typeof(ISpawnerRootWithoutArgs<IClock>))}\"",
                    "        \"遷移先\":",
                    "          \"Continue\": \"Choice, INIT\"",
                    "          \"EncounterEnemy\": \"Choice, INIT\""),
                mold.ToString(true));
        }

        /// <summary>
        /// CycleSpawnerのMold結果をテスト。 ただし、Specは一通りの設定を含む。
        /// Test the MoldSpec method on CycleSpawner. Spec contains total values for CycleSpawner.
        /// </summary>
        [Fact]
        public void MoldCycleSpawnerWithTotalValueOnSpecTest()
        {
            // arrange
            var proxy = Utility.TestOutSideProxy();
            Utility.SetupSpecFile(proxy, "status.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(StatusSpawner))}\"",
                "\"properties\":",
                "  \"X\": \"0\"",
                "  \"HP\": \"80\""));
            Utility.SetupSpecFile(proxy, "quest_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(QuestClockSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\"",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"x_effect.spec\"",
                "  \"敵の出現間隔\": \"3\""));
            Utility.SetupSpecFile(proxy, "battle_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(BattleClockSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\"",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"hp_effect.spec\"",
                "      \"1\": \"enemy_hp_effect.spec\"",
                "  \"ダメージ量\": \"10\""));
            Utility.SetupSpecFile(proxy, "game_over_clock.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(GameOverClockSpawner))}\"",
                "\"properties\":",
                "  \"状態描画\":",
                "    \"DictBody\":",
                "      \"0\": \"game_over_effect.spec\""));
            Utility.SetupSpecFile(proxy, "x_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(XEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "hp_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(HPEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "enemy_hp_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(EnemyHPEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "game_over_effect.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(GameOverEffectSpawner))}\"",
                "\"properties\":",
                "  \"状態管理\": \"status.spec\""));
            Utility.SetupSpecFile(proxy, "cycle.spec", Utility.JoinLines(
                $"\"spawner\": \"{Spec.EncodeType(typeof(CycleSpawner))}\"",
                "\"properties\":",
                "  \"タイトル\": \"タイトル\"",
                "  \"FPS\": \"60\"",
                "  \"開始モード\": \"探索\"",
                "  \"モード一覧\":",
                "    \"DictBody\":",
                "      \"探索\":",
                "        \"状態更新\": \"quest_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"探索\"",
                "          \"EncounterEnemy\": \"戦闘\"",
                "      \"戦闘\":",
                "        \"状態更新\": \"battle_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"戦闘\"",
                "          \"勝利時\": \"探索\"",
                "          \"敗北時\": \"ゲームオーバー\"",
                "      \"ゲームオーバー\":",
                "        \"状態更新\": \"game_over_clock.spec\"",
                "        \"遷移先\":",
                "          \"Continue\": \"ゲームオーバー\""));
            Utility.SetupSpecFile(proxy, "entry_point.spec", Utility.JoinLines(
                "\"properties\":",
                "  \"ゲームサイクル\": \"cycle.spec\"",
                "  \"ステータス\": \"status.spec\""));

            // act
            var spec = SpecRoot.Fetch(proxy, AgnosticPath.FromAgnosticPathString("cycle.spec"), true);
            Action<SpecRoot> action = (sp) =>
            {
                var cycleSpawner = new CycleSpawner();
                cycleSpawner.Spec = sp;
                _ = cycleSpawner.Spawn();
            };
            var mold = spec.MoldSpec(action);

            // assert
            Assert.Equal(
                Utility.JoinLines(
                    "\"base\": \"Spec\"",
                    $"\"spawner\": \"Spawner, {Spec.EncodeType(typeof(ISpawnerRoot<object>))}\"",
                    "\"properties\":",
                    "  \"タイトル\": \"Keyword\"",
                    "  \"FPS\": \"Int\"",
                    "  \"開始モード\": \"Keyword\"",
                    "  \"モード一覧\":",
                    "    \"MoldingType\": \"Dict, Keyword\"",
                    "    \"DictBody\":",
                    "      \"探索\":",
                    $"        \"状態更新\": \"Exterior, {Spec.EncodeType(typeof(ISpawnerRootWithoutArgs<IClock>))}\"",
                    "        \"遷移先\":",
                    "          \"Continue\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\"",
                    "          \"EncounterEnemy\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\"",
                    "      \"戦闘\":",
                    $"        \"状態更新\": \"Exterior, {Spec.EncodeType(typeof(ISpawnerRootWithoutArgs<IClock>))}\"",
                    "        \"遷移先\":",
                    "          \"Continue\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\"",
                    "          \"勝利時\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\"",
                    "          \"敗北時\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\"",
                    "      \"ゲームオーバー\":",
                    $"        \"状態更新\": \"Exterior, {Spec.EncodeType(typeof(ISpawnerRootWithoutArgs<IClock>))}\"",
                    "        \"遷移先\":",
                    "          \"Continue\": \"Choice, 探索\\\\c 戦闘\\\\c ゲームオーバー\""),
                mold.ToString(true));
        }

        public class Status : IStatus
        {
            public Status(int x, int hp)
            {
                this.ApplyResults = new List<string>();
                this.X = x;
                this.HP = hp;
                this.EnemyHP = 0;
            }

            /// <summary>
            /// Gets apply result strings.
            /// </summary>
            public List<string> ApplyResults { get; }

            /// <summary>
            /// Gets or sets x coordinate.
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Gets or sets hit point.
            /// </summary>
            public int HP { get; set; }

            /// <summary>
            /// Gets or sets hit point.
            /// </summary>
            public int EnemyHP { get; set; }
        }

        public class XEffect : Effect
        {
            private Status status;

            public XEffect(Status status)
            {
                this.status = status;
            }

            public override void Apply()
            {
                this.status.ApplyResults.Add($"X is {this.status.X}");
            }
        }

        public class HPEffect : Effect
        {
            private Status status;

            public HPEffect(Status status)
            {
                this.status = status;
            }

            public override void Apply()
            {
                this.status.ApplyResults.Add($"HP is {this.status.HP}");
            }
        }

        public class EnemyHPEffect : Effect
        {
            private Status status;

            public EnemyHPEffect(Status status)
            {
                this.status = status;
            }

            public override void Apply()
            {
                this.status.ApplyResults.Add($"Enemy HP is {this.status.EnemyHP}");
            }
        }

        public class GameOverEffect : Effect
        {
            private Status status;

            public GameOverEffect(Status status)
            {
                this.status = status;
            }

            public override void Apply()
            {
                this.status.ApplyResults.Add("game over");
            }
        }

        public class QuestClock : Clock<QuestResultEnum>
        {
            private Status status;
            private int enemyInterval;

            public QuestClock(Status status, List<Effect> effects, int enemyInterval)
            {
                this.status = status;
                this.Effects = effects;
                this.enemyInterval = enemyInterval;
            }

            public override List<Effect> Effects { get; }

            public override void Enter()
            {
                this.status.X = 0;
            }

            protected override QuestResultEnum Tick()
            {
                this.status.X++;
                if (this.status.X % this.enemyInterval == 0)
                {
                    return QuestResultEnum.EncounterEnemy;
                }
                else
                {
                    return QuestResultEnum.Continue;
                }
            }
        }

        public class BattleClock : Clock<BattleResultEnum>
        {
            private Status status;
            private int damage;

            public BattleClock(Status status, List<Effect> effects, int damage)
            {
                this.status = status;
                this.Effects = effects;
                this.damage = damage;
            }

            public override List<Effect> Effects { get; }

            public override void Enter()
            {
                this.status.EnemyHP = 50;
            }

            protected override BattleResultEnum Tick()
            {
                this.status.HP -= this.damage;
                this.status.EnemyHP -= this.damage;

                if (this.status.HP <= 0)
                {
                    return BattleResultEnum.Lose;
                }
                else if (this.status.EnemyHP <= 0)
                {
                    return BattleResultEnum.Win;
                }
                else
                {
                    return BattleResultEnum.Continue;
                }
            }
        }

        public class GameOverClock : Clock<GameOverResultEnum>
        {
            public GameOverClock(List<Effect> effects)
            {
                this.Effects = effects;
            }

            public override List<Effect> Effects { get; }

            protected override GameOverResultEnum Tick()
            {
                return GameOverResultEnum.Continue;
            }
        }

        public class StatusSpawner : StatusSpawner<Status>
        {
            /// <inheritdoc/>
            public override string Note { get => "状態管理"; }

            /// <inheritdoc/>
            protected override Status SpawnStatus()
            {
                return new Status(this.Spec.Int["X"], this.Spec.Int["HP"]);
            }
        }

        public class XEffectSpawner : ISpawnerRootWithoutArgs<XEffect>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "X座標描画"; }

            /// <inheritdoc/>
            public XEffect Spawn()
            {
                return new XEffect(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn());
            }
        }

        public class HPEffectSpawner : ISpawnerRootWithoutArgs<HPEffect>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "HP描画"; }

            /// <inheritdoc/>
            public HPEffect Spawn()
            {
                return new HPEffect(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn());
            }
        }

        public class EnemyHPEffectSpawner : ISpawnerRootWithoutArgs<EnemyHPEffect>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "EnemyHP描画"; }

            /// <inheritdoc/>
            public EnemyHPEffect Spawn()
            {
                return new EnemyHPEffect(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn());
            }
        }

        public class GameOverEffectSpawner : ISpawnerRootWithoutArgs<GameOverEffect>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "ゲームオーバー描画"; }

            /// <inheritdoc/>
            public GameOverEffect Spawn()
            {
                return new GameOverEffect(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn());
            }
        }

        public class QuestClockSpawner : ISpawnerRootWithoutArgs<QuestClock>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "探索状態更新"; }

            /// <inheritdoc/>
            public QuestClock Spawn()
            {
                return new QuestClock(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn(),
                    this.Spec.List.Exterior<ISpawnerRootWithoutArgs<Effect>>()["状態描画"].Select(x => x.Spawn()).ToList(),
                    this.Spec.Int["敵の出現間隔"]);
            }
        }

        public class BattleClockSpawner : ISpawnerRootWithoutArgs<BattleClock>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "戦闘状態更新"; }

            /// <inheritdoc/>
            public BattleClock Spawn()
            {
                return new BattleClock(
                    this.Spec.Exterior<StatusSpawner<Status>>()["状態管理"].Spawn(),
                    this.Spec.List.Exterior<ISpawnerRootWithoutArgs<Effect>>()["状態描画"].Select(x => x.Spawn()).ToList(),
                    this.Spec.Int["ダメージ量"]);
            }
        }

        public class GameOverClockSpawner : ISpawnerRootWithoutArgs<GameOverClock>
        {
            /// <inheritdoc/>
            public SpecRoot Spec { get; set; }

            /// <inheritdoc/>
            public string Note { get => "ゲームオーバー状態更新"; }

            /// <inheritdoc/>
            public GameOverClock Spawn()
            {
                return new GameOverClock(
                    this.Spec.List.Exterior<ISpawnerRootWithoutArgs<Effect>>()["状態描画"].Select(x => x.Spawn()).ToList());
            }
        }
    }
}
