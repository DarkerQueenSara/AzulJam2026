using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BuzzControllerSystem;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public static class GameInputUtils
{
    public class WaitForInput : CustomYieldInstruction
    {
        public int player, button;
        
        public override bool keepWaiting
        {
            get
            {
                var device = BuzzInputDevice.current;
                bool anyPressed;
                (player, button, anyPressed) = new[] { device.player1, device.player2, device.player3, device.player4 }
                    .SelectMany((buttons, p) =>
                        buttons.Select((control, i) => (p, i, control.wasPressedThisFrame)))
                    .FirstOrDefault(pb => pb.wasPressedThisFrame);
                return anyPressed;
            }
        }
    }
}