using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using BuzzControllerSystem;

public class ControllerExample : MonoBehaviour
{

    public Image[] player1;
    public Image[] player2;
    public Image[] player3;
    public Image[] player4;

    BuzzInput buzzInput;
    Color[] imageColors = new Color[5];

    // Start is called before the first frame update
    void Start()
    {
        buzzInput = GetComponent<BuzzInput>();
        buzzInput.StartLightSequence(BuzzInput.BuzzLightSequence.SlowFlash);

        for(int i = 0; i < 5; i++)
        {
            imageColors[i] = player1[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update Player 1
        for(int i = 0; i < player1.Length; i++)
        {
            if (buzzInput.GetButtonDown(0, (BuzzInput.BuzzButton) i ))
            {
                player1[i].color = Color.white;
            }
            if (buzzInput.GetButtonUp(0,(BuzzInput.BuzzButton) i))
            {
                player1[i].color = imageColors[i];
            }
        }

        //Update Player 2
        for (int i = 0; i < player2.Length; i++)
        {
            if (buzzInput.GetButtonDown(1, (BuzzInput.BuzzButton)i))
            {
                player2[i].color = Color.white;
            }
            if (buzzInput.GetButtonUp(1, (BuzzInput.BuzzButton)i))
            {
                player2[i].color = imageColors[i];
            }
        }

        //Update Player 3
        for (int i = 0; i < player3.Length; i++)
        {
            if (buzzInput.GetButtonDown(2, (BuzzInput.BuzzButton)i))
            {
                player3[i].color = Color.white;
            }
            if (buzzInput.GetButtonUp(2, (BuzzInput.BuzzButton)i))
            {
                player3[i].color = imageColors[i];
            }
        }

        //Update Player 4
        for (int i = 0; i < player4.Length; i++)
        {
            if (buzzInput.GetButtonDown(3, (BuzzInput.BuzzButton)i))
            {
                player4[i].color = Color.white;
            }
            if (buzzInput.GetButtonUp(3, (BuzzInput.BuzzButton)i))
            {
                player4[i].color = imageColors[i];
            }
        }
    }

    public void ChangeSequence(Int32 change)
    {
        buzzInput.StopLightSequence();
        buzzInput.StartLightSequence((BuzzInput.BuzzLightSequence)change);
    }
}
