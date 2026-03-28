using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreenManager : MonoBehaviour
{
    public List<string> tutorialTextLines;
    public TextMeshProUGUI tutorialText;
    public CheckReady checkReady;

    private async Awaitable Start()
    {
        foreach (var line in tutorialTextLines)
        {
            tutorialText.text = line;
            await checkReady.PlayersBuzzAnyOrderAsync();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}