using System;
using System.Collections.Generic;
using UnityEngine;
using static BuzzControllerSystem.BuzzInput;

public static class WitchGameplay
{
    public const int PlayerCount = (int)Player.Count;

    public enum Player
    {
        P1,
        P2,
        P3,
        P4,

        // handy constants
        Count,
        NoOne = -1,
    }

    public static string DisplayName(this Player player) =>
        player switch
        {
            Player.P1 => "Player 1",
            Player.P2 => "Player 2",
            Player.P3 => "Player 3",
            Player.P4 => "Player 4",
            Player.NoOne => "(no one)",
            _ => throw new ArgumentOutOfRangeException(nameof(player), player, null)
        };

    public class PlayerInfo
    {
        public static readonly PlayerInfo P1 = new PlayerInfo(index: 0)
            { Name = "Player 1", ColorName = "Blue", Button = BuzzButton.Blue };

        public static readonly PlayerInfo P2 = new PlayerInfo(index: 1)
            { Name = "Player 2", ColorName = "Orange", Button = BuzzButton.Orange };

        public static readonly PlayerInfo P3 = new PlayerInfo(index: 2)
            { Name = "Player 3", ColorName = "Green", Button = BuzzButton.Green };

        public static readonly PlayerInfo P4 = new PlayerInfo(index: 3)
            { Name = "Player 4", ColorName = "Yellow", Button = BuzzButton.Yellow };

        public static readonly PlayerInfo[] All = new[] { P1, P2, P3, P4 };

        public int Index { get; }
        public string Name { get; private set; }
        public string ColorName { get; private set; }
        public BuzzButton Button { get; private set; }

        private PlayerInfo(int index) => Index = index;

        public override string ToString() => Name;
        public override int GetHashCode() => Index;

        public static implicit operator Player(PlayerInfo player) => (Player)player.Index;
        public static implicit operator PlayerInfo(Player  player) => All[(int)player];
        public static implicit operator int(PlayerInfo player) => player.Index;
        public static implicit operator PlayerInfo(int player) => All[player];

        public static PlayerInfo operator +(PlayerInfo player, int offset) => player.Index + offset;
    }

    public static Player Succ(this Player player) => ++player;
    public static Player Pred(this Player player) => --player;

    public enum Bet
    {
        Survival,
        Demise,
    }
}