using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

namespace BuzzControllerSystem
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    struct BuzzHIDInputReport : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'I', 'D');

        [FieldOffset(0)] public byte reportId;

        [InputControl(name = "P1Buzz", displayName = "Player 1 Buzz", layout = "Button", bit = 0)]
        [InputControl(name = "P1Blue", displayName = "Player 1 Blue", layout = "Button", bit = 4)]
        [InputControl(name = "P1Orange", displayName = "Player 1 Orange", layout = "Button", bit = 3)]
        [InputControl(name = "P1Green", displayName = "Player 1 Green", layout = "Button", bit = 2)]
        [InputControl(name = "P1Yellow", displayName = "Player 1 Yellow", layout = "Button", bit = 1)]
        [InputControl(name = "P2Buzz", displayName = "Player 2 Buzz", layout = "Button", bit = 5)]
        [InputControl(name = "P2Yellow", displayName = "Player 2 Yellow", layout = "Button", bit = 6)]
        [InputControl(name = "P2Green", displayName = "Player 2 Green", layout = "Button", bit = 7)]

        [FieldOffset(3)] public byte Buttons1;

        [InputControl(name = "P2Orange", displayName = "Player 2 Orange", layout = "Button", bit = 0)]
        [InputControl(name = "P2Blue", displayName = "Player 2 Blue", layout = "Button", bit = 1)]
        [InputControl(name = "P3Buzz", displayName = "Player 3 Buzz", layout = "Button", bit = 2)]
        [InputControl(name = "P3Yellow", displayName = "Player 3 Yellow", layout = "Button", bit = 3)]
        [InputControl(name = "P3Green", displayName = "Player 3 Green", layout = "Button", bit = 4)]
        [InputControl(name = "P3Orange", displayName = "Player 3 Orange", layout = "Button", bit = 5)]
        [InputControl(name = "P3Blue", displayName = "Player 3 Blue", layout = "Button", bit = 6)]
        [InputControl(name = "P4Buzz", displayName = "Player 4 Buzz", layout = "Button", bit = 7)]

        [FieldOffset(4)] public byte Buttons2;

        [InputControl(name = "P4Yellow", displayName = "Player 4 Yellow", layout = "Button", bit = 0)]
        [InputControl(name = "P4Green", displayName = "Player 4 Green", layout = "Button", bit = 1)]
        [InputControl(name = "P4Orange", displayName = "Player 4 Orange", layout = "Button", bit = 2)]
        [InputControl(name = "P4Blue", displayName = "Player 4 Blue", layout = "Button", bit = 3)]

        [FieldOffset(5)] public byte Buttons3;
    }
}
