using BuzzControllerSystem;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(BuzzInput))]
public class CheckReady : MonoBehaviour
{
    public enum Order
    {
        From1To4,
        Arbitrary,
        None
    }

    public delegate void AllPlayersPressedCallback();

    
    public BuzzInput input;
    public Image[] images;
    public Color[] playerColors;

    public UnityEvent<int> onAskForPlayerPress;
    public UnityEvent onAllPlayersHavePressed;

    // player presses bookkeeping
    private Order _order;
    private int _currentChecking = 0;
    private bool[] _playerPressed = new bool[4];
    private AllPlayersPressedCallback _callback;


    public void RequestAllPlayerPress(Order order, AllPlayersPressedCallback callback = null)
    {
        for (int i = 0; i < 4; i++)
        {
            setImageVisibility(i, false);
        }

        this._order = order;
        _currentChecking = 0;
        _playerPressed = new bool[4];
        _callback = callback;
        onAskForPlayerPress.Invoke(0);
    }


    // Update is called once per frame
    void Update()
    {
        switch (_order)
        {
            case Order.From1To4:
                Assert.IsTrue(_currentChecking >= 0 && _currentChecking < 4,
                    $"Current checking index {_currentChecking} is out of bounds!");

                if (input.GetButtonDown(_currentChecking, BuzzInput.BuzzButton.Buzz))
                {
                    setImageVisibility(_currentChecking, true);

                    _currentChecking++;
                    onAskForPlayerPress.Invoke(_currentChecking);

                    if (_currentChecking == 4)
                    {
                        onAllPlayersHavePressed.Invoke();
                        _callback?.Invoke();
                        _callback = null;
                        _order = Order.None;
                    }
                }
                break;

            case Order.Arbitrary:
                for (int i = 0; i < 4; i++)
                {
                    if (input.GetButtonDown(i, BuzzInput.BuzzButton.Buzz))
                    {
                        _playerPressed[i] = true;
                        setImageVisibility(i, true);
                    }
                }
                if (_playerPressed.All(p => p))
                {
                    onAllPlayersHavePressed.Invoke();
                    _callback?.Invoke();
                    _callback = null;
                    _order = Order.None;
                }
                break;

            default:
                break;
        }
    }


    private void setImageVisibility(int playerIndex, bool visible)
    {
        // images[playerIndex].gameObject.SetActive(visible);

        Color currColor = images[playerIndex].color;
        currColor.a = visible ? 1.0f : 0.0f;
        images[playerIndex].color = currColor;
    }

}
