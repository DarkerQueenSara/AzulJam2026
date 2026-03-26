using BuzzControllerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

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
    private int _currentChecking = 0;
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

    public Awaitable RequestAllPlayersPressAsync(bool inOrder)
    {
        _awaitableCompletionSource.Reset();
        RequestAllPlayersPress(inOrder, whenAllHavePressed: _awaitableCompletionSource.SetResult);
        return _awaitableCompletionSource.Awaitable;
    }

    public async System.Collections.Generic.IAsyncEnumerable<int> PlayersBuzzInOrderAsync()
    {
        for (var player = 0; player < 4; player++)
        {
            yield return player;
            while (!input.GetButtonDown(player, BuzzInput.BuzzButton.Buzz))
                await Awaitable.NextFrameAsync();
        }
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

    private void Update()
    {
        PollPlayersInSelectedOrder();
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
        if (_callback != null) {
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