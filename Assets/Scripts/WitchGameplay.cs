using System;
using UnityEngine;

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

    public enum Bet
    {
        Survival,
        Demise,
    }
}