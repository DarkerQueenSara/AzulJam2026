using System.Collections;
using TMPro;
using UnityEngine;

public class JoinSceneManager : MonoBehaviour
{
    public string msgPlayerPress = "Player {0}, press the red button!";
    public string msgCountdown = "Starting in {0}...";

    public TextMeshProUGUI text;
    public CheckReady checkReady;


    private async Awaitable Start()
    {
        await foreach (var player in checkReady.PlayersBuzzInOrderAsync())
        {
            text.text = string.Format(msgPlayerPress, player + 1);
        }
        StartCoroutine(AllReadyCountdownCoro());
    }
    
    

    private void Start1()
    {
        checkReady.onAskForPlayerPress.AddListener(UpdateTextNextPlayer);
        checkReady.RequestAllPlayersPress(inOrder: true, whenAllHavePressed: StartCountdown);
        return;

        void UpdateTextNextPlayer(int player) =>
            text.text = string.Format(msgPlayerPress, player + 1);
        
        void StartCountdown() =>
            StartCoroutine(AllReadyCountdownCoro());
    }
    
    private static readonly YieldInstruction WaitOneSecond = new WaitForSeconds(1.0f);

    private IEnumerator AllReadyCountdownCoro()
    {
        for (var sec = 3; sec > 0; sec--)
        {
            text.text = string.Format(msgCountdown, sec);
            yield return WaitOneSecond;
        }

        text.text = "Starting now...";

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}