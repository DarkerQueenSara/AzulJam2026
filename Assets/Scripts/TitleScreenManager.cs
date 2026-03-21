using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using BuzzControllerSystem;

public class TitleScreenManager : MonoBehaviour
{
    const int NUM_BUZZ_PLAYERS = 4;
    const int NUM_BUZZ_BUTTONS = 5;

    public BuzzInput buzzInput;

    void Start()
    {
        buzzInput = GetComponent<BuzzInput>();
    }

    void Update()
    {
        for (int player = 0; player < NUM_BUZZ_PLAYERS; player++)
        {
            for (int button = 0; button < NUM_BUZZ_BUTTONS; button++)
            {
                var buzzButton = (BuzzInput.BuzzButton)button;
                if (buzzInput.GetButtonDown(player, buzzButton))
                {
                    //Load the next scene
                    UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                }
            }
        }
    }
}
