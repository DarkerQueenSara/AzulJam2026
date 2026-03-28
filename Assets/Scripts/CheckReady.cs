using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;
using static WitchGameplay;

[RequireComponent(typeof(BuzzInput))]
public class CheckReady : MonoBehaviour
{
    private enum PollingState
    {
        OrderingFrom1To4,
        OrderingArbitrarily,
        Idle
    }

    public delegate void AllPlayersPressedCallback();

    public BuzzInput input;
    public Image[] images;
    public Color[] playerColors;

    public UnityEvent<int> onAskForPlayerPress;
    public UnityEvent onAllPlayersHavePressed;

    // player presses bookkeeping
    private PollingState _pollingState;
    private int _currentChecking;
    private readonly bool[] _playerPressed = new bool[4];
    private AllPlayersPressedCallback _callback;
    private readonly AwaitableCompletionSource _awaitableCompletionSource = new AwaitableCompletionSource();


    public void RequestAllPlayersPress(bool inOrder, AllPlayersPressedCallback whenAllHavePressed = null)
    {
        _callback = whenAllHavePressed;

        if (inOrder)
        {
            _pollingState = PollingState.OrderingFrom1To4;
            _currentChecking = 0;
            onAskForPlayerPress.Invoke(0);
        }
        else
        {
            _pollingState = PollingState.OrderingArbitrarily;
            Array.Fill(_playerPressed, false);
        }

        HideAllImages();
        TurnOffAllLights();
    }

    #region playing with async/await and Awaitables

    public Awaitable RequestAllPlayersPressAsync(bool inOrder)
    {
        _awaitableCompletionSource.Reset();
        RequestAllPlayersPress(inOrder, whenAllHavePressed: _awaitableCompletionSource.SetResult);
        return _awaitableCompletionSource.Awaitable;
    }

    public async IAsyncEnumerable<Player> PlayersBuzzInOrderAsync()
    {
        for (var playerToConfirm = Player.P1; playerToConfirm <= Player.P4; playerToConfirm++)
        {
            yield return playerToConfirm;
            await Utils.SubscribeUntil(WitchInput.current.onConfirmation,
                confirmingPlayer => confirmingPlayer == playerToConfirm);
        }
    }

    public async Awaitable PlayersBuzzAnyOrderAsync()
    {
        var confirmations = new BitVector32();
        do
        {
            var confirmingPlayer = await WitchInput.current.onConfirmation;
            confirmations[(int)confirmingPlayer] = true;
        } while (confirmations.Data != 0b1111);
    }

    public void TurnOffAllLights()
    {
        for (var player = 0; player < 4; player++)
            input.SetLight(player, false);
    }

    public void HideAllImages()
    {
        for (var player = 0; player < 4; player++)
            SetImageVisibility(player, false);
    }

    #endregion

    private void Update()
    {
        if (_pollingFunctions[(int)_pollingState]())
        {
            _pollingState = PollingState.Idle;
            AllPlayersHavePressed();
        }
    }

    private readonly Func<bool>[] _pollingFunctions = new Func<bool>[4];

    private void Awake()
    {
        Array.Fill(_pollingFunctions, () => false);
        _pollingFunctions[(int)PollingState.OrderingFrom1To4] = CheckFrom1To4;
        _pollingFunctions[(int)PollingState.OrderingArbitrarily] = CheckArbitrarily;
    }


    #region Polling player readiness in selected order

    private void PollPlayersInSelectedOrder()
    {
        switch (_pollingState)
        {
            case PollingState.OrderingFrom1To4 when CheckFrom1To4():
            case PollingState.OrderingArbitrarily when CheckArbitrarily():
                _pollingState = PollingState.Idle;
                AllPlayersHavePressed();
                break;
        }
    }

    private bool CheckArbitrarily()
    {
        for (var player = 0; player < 4; player++)
        {
            if (input.GetButtonDown(player, BuzzInput.BuzzButton.Buzz))
            {
                input.SetLight(player, true);
                SetImageVisibility(player, true);
                _playerPressed[player] = true;
            }
        }

        return _playerPressed.All(p => p);
    }

    private bool CheckFrom1To4()
    {
        Assert.IsTrue(_currentChecking is >= 0 and < 4,
            $"Current checking index {_currentChecking} is out of bounds!");

        if (input.GetButtonDown(_currentChecking, BuzzInput.BuzzButton.Buzz))
        {
            input.SetLight(_currentChecking, true);
            SetImageVisibility(_currentChecking, true);
            if (++_currentChecking < 4)
                onAskForPlayerPress.Invoke(_currentChecking);
        }

        return _currentChecking == 4;
    }

    private void AllPlayersHavePressed()
    {
        TurnOffAllLights();

        onAllPlayersHavePressed.Invoke();
        if (_callback != null)
        {
            _callback();
            _callback = null;
        }
    }

    #endregion

    private void SetImageVisibility(int playerIndex, bool visible)
    {
        // images[playerIndex].gameObject.SetActive(visible);

        var currColor = images[playerIndex].color;
        currColor.a = visible ? 1.0f : 0.0f;
        images[playerIndex].color = currColor;
    }
}