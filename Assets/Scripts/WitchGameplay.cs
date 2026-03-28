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

    [Flags]
    public enum PMask
    {
        P1,
        P2,
        P3,
        P4
    }

    public enum Bet
    {
        Survival,
        Demise,
    }

}
