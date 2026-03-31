using System;
using System.Collections;
using BuzzControllerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class JoinSceneManager : MonoBehaviour
{
    public string msgPlayerPress = "{0}, press the red button!";
    public string msgCountdown = "Starting in {0}...";

    public TextMeshProUGUI text;
    public CheckReady checkReady;

    private async Awaitable Start()
    {
        await Awaitable.EndOfFrameAsync();
        // ^-- Without an await like this, execution mysteriously derails on the next line --v
        checkReady.witchInput.onConfirmation.AddListener(player => Debug.Log($"{player} confirmed"));
        await foreach (var player in checkReady.PlayersBuzzInOrderAsync())
        {
            text.text = string.Format(msgPlayerPress, player.DisplayName());
        }

        for (var sec = 3; sec > 0; sec--)
        {
            text.text = string.Format(msgCountdown, sec);
            await Awaitable.WaitForSecondsAsync(1.0f);
        }
        text.text = "Starting now...";
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Update()
    {
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            Debug.Log($"5 was pressed in frame {Time.frameCount}");
        }

        if (checkReady.witchInput.GetButtonDown(WitchGameplay.Player.P1, BuzzInput.BuzzButton.Buzz))
        {
            Debug.Log($"P1's confirmation button registered at frame {Time.frameCount}");
        }
    }
}