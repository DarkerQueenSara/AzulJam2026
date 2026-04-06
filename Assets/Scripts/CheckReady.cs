using System.Collections.Generic;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static WitchGameplay;
using static BuzzControllerSystem.BuzzInput;

[RequireComponent(typeof(BuzzInput))]
[RequireComponent(typeof(WitchInput))]
public class CheckReady : MonoBehaviour
{
    public BuzzInput buzzInput;
    public WitchInput witchInput;
    public Image[] images;
    public Color[] playerColors;

    public UnityEvent onAllPlayersHavePressed;

    private void Start()
    {
        buzzInput = GetComponent<BuzzInput>();
        witchInput = GetComponent<WitchInput>();
    }

    public async IAsyncEnumerable<Player> PlayersBuzzInOrderAsync()
    {
        await Awaitable.MainThreadAsync();
        HideAllImages();

        for (var playerToConfirm = Player.P1; playerToConfirm <= Player.P4; playerToConfirm++)
        {
            yield return playerToConfirm;
            await Utils.UntilAsync(() => witchInput.GetButtonDown(playerToConfirm, BuzzButton.Buzz));
            // await Utils.SubscribeUntil(witchInput.onConfirmation, playerToConfirm);
            SetImageVisibility((int)playerToConfirm, true);
        }

        AllPlayersHavePressed();
    }

    public async Awaitable PlayersBuzzAnyOrderAsync()
    {
        await Awaitable.MainThreadAsync();
        HideAllImages();

        var confirmations = 0b0000;
        do
        {
            var confirmingPlayer = (int)await witchInput.onConfirmation;
            confirmations |= 1 << confirmingPlayer;
            SetImageVisibility(confirmingPlayer, true);
        } while (confirmations != 0b1111);

        AllPlayersHavePressed();
    }


    private void AllPlayersHavePressed()
    {
        onAllPlayersHavePressed?.Invoke();
    }

    private void SetImageVisibility(int playerIndex, bool visible)
    {
        if (BuzzInputDevice.current != null)
        {
            buzzInput.SetLight(playerIndex, visible);
        }

        var currColor = images[playerIndex].color;
        currColor.a = visible ? 1.0f : 0.0f;
        images[playerIndex].color = currColor;
    }

    public void HideAllImages()
    {
        for (var player = 0; player < PlayerCount; player++)
            SetImageVisibility(player, false);
    }
}