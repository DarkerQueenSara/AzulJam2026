using BuzzControllerSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(BuzzInput))]
public class CheckReady : MonoBehaviour
{
    public BuzzInput input;
    public Image[] images;
    public Color[] playerColors;

    // assigned by JoinSceneManager in ancestor Canvas on Start()
    public JoinSceneManager joinSceneManager;

    public UnityEvent<int> requestedPlayerPress;
    public UnityEvent allJoined;

    // all-player-request state
    int currentChecking = 0;
    TextMeshProUGUI textField = null;

    public void RequestAllPlayerPress()
    {
        requestedPlayerPress.Invoke(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentChecking < 4)
        {
            if (input.GetButtonDown(currentChecking, BuzzInput.BuzzButton.Buzz))
            {
                images[currentChecking].color = playerColors[currentChecking];
                currentChecking++;
                requestedPlayerPress.Invoke(currentChecking);

                if (currentChecking == 4)
                {
                    allJoined.Invoke();
                }
            }
        }
        // else all players have pressed, do nothing until next RequestAllPlayerPress()
    }

}
