using System;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using static WitchGameplay;

public class WitchInput : MonoBehaviour
{
    public static WitchInput current { get; private set; }

    public UnityEvent<Player> onConfirmation;
    public UnityEvent<(Player from, Player to)> onVote;
    public UnityEvent<(Player from, Bet bet)> onBet;

    private bool _buzzConnected;
    private ButtonControl[,] _buzzButtons;
    private KeyControl[,] _keys;

    public bool GetButtonDown(int player, int button) =>
        (_buzzButtons != null && _buzzButtons[player, button].wasPressedThisFrame) ||
        _keys[player, button].wasPressedThisFrame;

    public bool GetButtonDown(Player player, BuzzInput.BuzzButton button) =>
        GetButtonDown((int)player, (int)button);

    private void Awake()
    {
        // Enforce singleton
        if (FindObjectsByType<WitchInput>().Length >= 2)
        {
            Debug.LogWarning("Found " + FindObjectsByType<WitchInput>().Length + " WitchInputs");
        }
        else
        {
            Debug.Log($"WitchInput.current = {this}");
            current = this;
        }
    }

    private void Start()
    {
        // Initialize buzz buttons matrix
        var buzz = BuzzInputDevice.current;
        _buzzConnected = buzz != null;
        if (_buzzConnected)
        {
            _buzzButtons = new ButtonControl[4, 5]
            {
                // Confirm button,      vote in P1,      vote in P2,      vote in P3,      vote in P4
                { buzz.player1[0], buzz.player1[1], buzz.player1[2], buzz.player1[3], buzz.player1[4] },
                { buzz.player2[0], buzz.player2[1], buzz.player2[2], buzz.player2[3], buzz.player2[4] },
                { buzz.player3[0], buzz.player3[1], buzz.player3[2], buzz.player3[3], buzz.player3[4] },
                { buzz.player4[0], buzz.player4[1], buzz.player4[2], buzz.player4[3], buzz.player4[4] },
            };
        }
        else
        {
            Debug.LogWarning("BuzzInputDevice not found");
        }

        // Initialize keyboard keys matrix
        var kb = Keyboard.current;
        _keys = new KeyControl[4, 5]
        {
            { kb.digit5Key, kb.digit1Key, kb.digit2Key, kb.digit3Key, kb.digit4Key },
            //Confirm, vote P1, vote P2, vote P3, vote P4
            { kb.tKey, kb.qKey, kb.wKey, kb.eKey, kb.rKey },
            { kb.gKey, kb.aKey, kb.sKey, kb.dKey, kb.fKey },
            { kb.bKey, kb.zKey, kb.xKey, kb.cKey, kb.vKey },
        };
    }

    private void Update()
    {
        // Check confirmations (Buzz (red) or 5/T/G/B)
        for (var player = Player.P1; player <= Player.P4; player++)
            if (GetButtonDown(player, BuzzInput.BuzzButton.Buzz))
                onConfirmation?.Invoke(player);

        // Check votes and bets
        for (var player = 0; player < PlayerCount; player++)
        for (var button = 1; button < 5; button++)
            if (GetButtonDown(player, button))
            {
                var vote = ((Player from, Player to))(player, button - 1);
                if (vote.from != vote.to)
                    onVote?.Invoke(vote);

                var betType = (button < 3) ? Bet.Survival : Bet.Demise;
                onBet?.Invoke((from: vote.from, betType));
            }
    }
}