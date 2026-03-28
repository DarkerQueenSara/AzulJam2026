using System;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using static WitchGameplay;

public class WitchInput : MonoBehaviour
{
    public static WitchInput current { get; private set; }

    public UnityEvent<Player> onConfirmation;
    public UnityEvent<(Player from, Player to)> onVote;
    public UnityEvent<(Player from, Bet bet)> onBet;

    private ButtonControl[,] _buttons;
    private KeyControl[,] _keys;

    private void Awake()
    {
        // Enforce singleton
        if (FindObjectsByType<WitchInput>().Length >= 2)
        {
            Debug.Log("Found " + FindObjectsByType<WitchInput>().Length + " WitchInputs. Destroying " + this);
            Destroy(gameObject);
            return;
        }
        else
        {
            current = this;
        }

        // Initialize buzz buttons matrix
        var buzz = BuzzInputDevice.current;
        _buttons = new ButtonControl[4, 5]
        {
            // Confirm button,      vote in P1,      vote in P2,      vote in P3,      vote in P4
            { buzz.player1[0], buzz.player1[1], buzz.player1[2], buzz.player1[3], buzz.player1[4] },
            { buzz.player2[0], buzz.player2[1], buzz.player2[2], buzz.player2[3], buzz.player2[4] },
            { buzz.player3[0], buzz.player3[1], buzz.player3[2], buzz.player3[3], buzz.player3[4] },
            { buzz.player4[0], buzz.player4[1], buzz.player4[2], buzz.player4[3], buzz.player4[4] },
        };

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
        const int confirm = (int)BuzzInput.BuzzButton.Buzz;
        for (var player = 0; player < PlayerCount; player++)
            if (_buttons[player, confirm].wasPressedThisFrame
                || _keys[player, confirm].wasPressedThisFrame)
                onConfirmation?.Invoke((Player)player);

        // Check votes and bets
        for (var player = 0; player < PlayerCount; player++)
        for (var button = 1; button < 5; button++)
            if (_buttons[player, button].wasPressedThisFrame
                || _keys[player, button].wasPressedThisFrame)
            {
                var vote = ((Player from, Player to))(player, button - 1);
                if (vote.from != vote.to)
                    onVote?.Invoke(vote);

                var bet = (button < 3) ? Bet.Survival : Bet.Demise;
                onBet?.Invoke((from: vote.from, bet));
            }
    }
}