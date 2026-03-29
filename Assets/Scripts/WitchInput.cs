using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static WitchGameplay;
using static BuzzControllerSystem.BuzzInput;

public class WitchInput : MonoBehaviour
{
    public const int ButtonCount = 5;

    #region --- Controls configuration go here ---

    private static ButtonControl[,] _GetBuzzButtonMatrix(BuzzInputDevice buzz) =>
        new ButtonControl[PlayerCount, ButtonCount] // new[,]
        {
            // Confirm button,      vote in P1,      vote in P2,      vote in P3,      vote in P4
            { buzz.player1[0], buzz.player1[1], buzz.player1[2], buzz.player1[3], buzz.player1[4] }, // P1
            { buzz.player2[0], buzz.player2[1], buzz.player2[2], buzz.player2[3], buzz.player2[4] }, // P2
            { buzz.player3[0], buzz.player3[1], buzz.player3[2], buzz.player3[3], buzz.player3[4] }, // P3
            { buzz.player4[0], buzz.player4[1], buzz.player4[2], buzz.player4[3], buzz.player4[4] }, // P4
        };

    private static KeyControl[,] _GetKeyboardKeyMatrix(Keyboard kb) =>
        new KeyControl[PlayerCount, ButtonCount] // new[,]
        {
            //     Confirm,      vote P1,      vote P2,      vote P3,      vote P4
            { kb.digit5Key, kb.digit1Key, kb.digit2Key, kb.digit3Key, kb.digit4Key }, // P1
            { kb.tKey, kb.qKey, kb.wKey, kb.eKey, kb.rKey }, // P2
            { kb.gKey, kb.aKey, kb.sKey, kb.dKey, kb.fKey }, // P3
            { kb.bKey, kb.zKey, kb.xKey, kb.cKey, kb.vKey }, // P4
            //Confirm, vote P1, vote P2, vote P3, vote P4
        };

    #endregion --- end of control matrices ---

    #region State

    private ButtonControl[,] _buzzButtons;
    private KeyControl[,] _keys;
    private readonly bool[,] _buttonsPressedThisFrame = new bool[PlayerCount, ButtonCount];

    // Called on Update to read new presses for Buzz controller (if one is connected) and keyboard
    // for every matrix position, caching the result for public GetButtonDown.
    private bool _GetAndCacheButtonDown(int player, int button) =>
        _buttonsPressedThisFrame[player, button] =
            (_buzzButtons != null && _buzzButtons[player, button].wasPressedThisFrame) ||
            _keys[player, button].wasPressedThisFrame;

    #endregion

    #region Public interface

    public static WitchInput current { get; private set; }

    // UnityEvents are conveniently await-able
    public UnityEvent<Player> onConfirmation;
    public UnityEvent<(Player from, Player to)> onVote;
    public UnityEvent<(Player from, Bet bet)> onBet;

    public bool GetButtonDown(int player, int button) => _buttonsPressedThisFrame[player, button];
    public bool GetButtonDown(int player, BuzzButton button) => _buttonsPressedThisFrame[player, (int)button];
    public bool GetButtonDown(Player player, int button) => _buttonsPressedThisFrame[(int)player, button];
    public bool GetButtonDown(Player player, BuzzButton button) => _buttonsPressedThisFrame[(int)player, (int)button];

    #endregion

    #region MonoBehaviour lifecycle

    // Enforce singleton (maybe?)
    private void Awake()
    {
        if (FindObjectsByType<WitchInput>().Length >= 2)
        {
            Debug.LogWarning("Found " + FindObjectsByType<WitchInput>().Length + " WitchInputs");
            // Destroy(this)
        }
        else
        {
            Debug.Log($"{nameof(WitchInput)}.current = {this}");
            current = this;
        }
    }

    // Initialize Buzz and keyboard buttons matrices
    // (BuzzInput accesses BuzzInputDevice.current on Start, so I'm doing the same, but this could be cargo culting)
    private void Start()
    {
        // Buzz
        var buzz = BuzzInputDevice.current;
        if (buzz != null)
            _buzzButtons = _GetBuzzButtonMatrix(buzz);
        else
            Debug.LogWarning($"BuzzInputDevice is null. {nameof(WitchInput)} will read from keyboard only.");

        // keyboard
        var kb = Keyboard.current;
        Assert.IsNotNull(kb, "I assume there's always a keyboard present here");
        _keys = _GetKeyboardKeyMatrix(kb);
    }

    // Refresh the whole _buttonsPressedThisFrame matrix,
    // and dispatch some interesting events for driving gameplay logic while we are at it.
    private void Update()
    {
        // Check confirmations (Buzz (red) or 5/T/G/B)
        for (var player = 0; player < PlayerCount; player++)
            if (_GetAndCacheButtonDown(player, 0))
                onConfirmation?.Invoke((Player)player);

        // Check votes and bets
        for (var player = 0; player < PlayerCount; player++)
        for (var button = 1; button < ButtonCount; button++)
            if (_GetAndCacheButtonDown(player, button))
            {
                var vote = ((Player from, Player to))(player, button - 1);
                if (vote.from != vote.to)
                    onVote?.Invoke(vote);

                var betType = (button < 3) ? Bet.Survival : Bet.Demise;
                onBet?.Invoke((from: vote.from, betType));
            }
    }

    #endregion
}