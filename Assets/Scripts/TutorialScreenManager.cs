using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialScreenManager : MonoBehaviour
{
    public List<string> tutorialTextLines;
    public GameObject tutorialText;
    public CheckReady checkReady;

    private int _currentIndex = 0;

    private void Start()
    {
        checkReady.onAllPlayersHavePressed.AddListener(NextSentence);
        checkReady.RequestAllPlayersPress(inOrder: false);
    }

    public void NextSentence()
    {
        //advance to next string
        _currentIndex++;
        if (_currentIndex < tutorialTextLines.Count)
        {
            StartCoroutine(NextSentenceCoro());
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    System.Collections.IEnumerator NextSentenceCoro()
    {
        yield return new WaitForSeconds(0.5f);
        tutorialText.GetComponent<TextMeshProUGUI>().text = tutorialTextLines[_currentIndex];
        checkReady.RequestAllPlayersPress(inOrder: false);
    }
}
