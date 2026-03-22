using UnityEngine;
using BuzzControllerSystem;
using UnityEngine.Assertions;

[RequireComponent(typeof(BuzzInput))]
public class TitleScreenManager : MonoBehaviour
{
    const int NUM_BUZZ_PLAYERS = 4;
    const int NUM_BUZZ_BUTTONS = 5;

    public BuzzInput buzzInput;
    public BuzzInput.BuzzButton startButton = BuzzInput.BuzzButton.Buzz;

    void Start()
    {
        buzzInput = GetComponent<BuzzInput>();
        buzzInput.StartLightSequence(BuzzInput.BuzzLightSequence.FastMove);
    }

    void Update()
    {
        for (int player = 0; player < NUM_BUZZ_PLAYERS; player++)
        {
            if (buzzInput.GetButtonDown(player, startButton))
            {
                //Load the next scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        }
    }
}
