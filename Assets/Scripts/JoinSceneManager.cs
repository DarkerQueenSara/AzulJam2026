using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class JoinSceneManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string msgPlayerPress = "Player {0}, press the red button!";
    public string msgCountdown = "Starting in {0}...";

    public CheckReady checkReady;

    void Start()
    {
        checkReady.onAskForPlayerPress.AddListener((int player) =>
        {
            text.text = string.Format(msgPlayerPress, player + 1);
        });

        checkReady.onAllPlayersHavePressed.AddListener(() =>
        {
            StartCoroutine(AllReadyCountdownCoro());
        });

        checkReady.RequestAllPlayerPress(CheckReady.Order.From1To4);
    }

    IEnumerator AllReadyCountdownCoro()
    {
        for (int sec = 3; sec > 0; sec--)
        {
            text.text = string.Format(msgCountdown, sec);
            yield return new WaitForSeconds(1.0f);
        }

        text.text = "Starting now...";

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
