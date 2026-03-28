using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSceneManager : MonoBehaviour
{
    public string msgPlayerPress = "{0}, press the red button!";
    public string msgCountdown = "Starting in {0}...";

    public TextMeshProUGUI text;
    public CheckReady checkReady;

    private async Awaitable Start()
    {
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
}