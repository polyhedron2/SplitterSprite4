// -----------------------------------------------------------------------
// <copyright file="CycleTests.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Test.Cycle
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using MagicKitchen.SplitterSprite4.Common.Clock;
    using MagicKitchen.SplitterSprite4.Common.Cycle;
    using MagicKitchen.SplitterSprite4.Common.Effect;
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
            var questClock = new QuestClock(status, 5);
            var battleClock = new BattleClock(status, 10);
            var gameOverClock = new GameOverClock();

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
            var applyResults = new List<string>();

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
                new QuestClock(status, enemyInterval: 3),
                new List<Effect>
                {
                    new XEffect(status, applyResults),
                },
                new Dictionary<string, string>
                {
                    { "Continue", "探索" },
                    { "EncounterEnemy", "戦闘" },
                });
            var battleMode = new Mode(
                new BattleClock(status, damage: 10),
                new List<Effect>
                {
                    new HPEffect(status, applyResults),
                    new EnemyHPEffect(status, applyResults),
                },
                new Dictionary<string, string>
                {
                    { "Continue", "戦闘" },
                    { "勝利時", "探索" },
                    { "敗北時", "ゲームオーバー" },
                });
            var gameOverMode = new Mode(
                new GameOverClock(),
                new List<Effect>
                {
                    new GameOverEffect(status, applyResults),
                },
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
                string.Join("\n", applyResults));
        }

        public class XEffect : Effect
        {
            private Status status;
            private List<string> applyResults;

            public XEffect(Status status, List<string> applyResults)
            {
                this.status = status;
                this.applyResults = applyResults;
            }

            public override void Apply()
            {
                this.applyResults.Add($"X is {this.status.X}");
            }
        }

        public class Status : IStatus
        {
            public Status(int x, int hp)
            {
                this.X = x;
                this.HP = hp;
                this.EnemyHP = 0;
            }

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

        public class HPEffect : Effect
        {
            private Status status;
            private List<string> applyResults;

            public HPEffect(Status status, List<string> applyResults)
            {
                this.status = status;
                this.applyResults = applyResults;
            }

            public override void Apply()
            {
                this.applyResults.Add($"HP is {this.status.HP}");
            }
        }

        public class EnemyHPEffect : Effect
        {
            private Status status;
            private List<string> applyResults;

            public EnemyHPEffect(Status status, List<string> applyResults)
            {
                this.status = status;
                this.applyResults = applyResults;
            }

            public override void Apply()
            {
                this.applyResults.Add($"Enemy HP is {this.status.EnemyHP}");
            }
        }

        public class GameOverEffect : Effect
        {
            private Status status;
            private List<string> applyResults;

            public GameOverEffect(Status status, List<string> applyResults)
            {
                this.status = status;
                this.applyResults = applyResults;
            }

            public override void Apply()
            {
                this.applyResults.Add("game over");
            }
        }

        public class QuestClock : Clock<QuestResultEnum>
        {
            private Status status;
            private int enemyInterval;

            public QuestClock(Status status, int enemyInterval)
            {
                this.status = status;
                this.enemyInterval = enemyInterval;
            }

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

            public BattleClock(Status status, int damage)
            {
                this.status = status;
                this.damage = damage;
            }

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
            protected override GameOverResultEnum Tick()
            {
                return GameOverResultEnum.Continue;
            }
        }
    }
}
