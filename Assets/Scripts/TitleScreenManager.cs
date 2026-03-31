using UnityEngine;
using BuzzControllerSystem;
using UnityEngine.SceneManagement;
using static WitchGameplay;

[RequireComponent(typeof(WitchInput))]
public class TitleScreenManager : MonoBehaviour
{
    private async Awaitable Start()
    {
        // TODO restore hype light sequence
        // buzzInput.StartLightSequence(BuzzInput.BuzzLightSequence.FastMove);
        
        await GetComponent<WitchInput>().onConfirmation;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
