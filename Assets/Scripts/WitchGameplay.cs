using UnityEngine;

public static class WitchGameplay
{
    public const int NumPlayers = (int)Player.Count;
    
    public enum Player
    {
        P1,
        P2,
        P3,
        P4,
        Count,
    }

    public enum Bet
    {
        Survival,
        Demise,
    }

}
