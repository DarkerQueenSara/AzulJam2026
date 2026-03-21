using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;



namespace BuzzControllerSystem
{
    #if UNITY_EDITOR
    [InitializeOnLoad] // Make sure static constructor is called during startup.
    #endif

    [InputControlLayout(stateType = typeof(BuzzHIDInputReport))]
    public class BuzzInputDevice : InputDevice
    {
        public ButtonControl[] player1 { get; private set; } = new ButtonControl[5];
        public ButtonControl[] player2 { get; private set; } = new ButtonControl[5];
        public ButtonControl[] player3 { get; private set; } = new ButtonControl[5];
        public ButtonControl[] player4 { get; private set; } = new ButtonControl[5];
        public static BuzzInputDevice current { get; private set; }

        static BuzzInputDevice()
        {
            InputSystem.RegisterLayout<BuzzInputDevice>(
                "Buzz Controller",
                new InputDeviceMatcher()
                    .WithInterface("HID")
                    .WithCapability("vendorId", 0x054c)
                    .WithCapability("productId", 0x1000));
            InputSystem.RegisterLayout<BuzzInputDevice>(
                "Wireless Buzz Controller",
                new InputDeviceMatcher()
                    .WithInterface("HID")
                    .WithCapability("vendorId", 0x054c)
                    .WithCapability("productId", 2));
        }

        // In the Player, to trigger the calling of the static constructor,
        // create an empty method annotated with RuntimeInitializeOnLoadMethod.
        [RuntimeInitializeOnLoadMethod]
        static void Init() { }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            Debug.Log("Buzz Controller Set-Up");

            player1[0] = GetChildControl<ButtonControl>("P1Buzz");
            player1[1] = GetChildControl<ButtonControl>("P1Blue");
            player1[2] = GetChildControl<ButtonControl>("P1Orange");
            player1[3] = GetChildControl<ButtonControl>("P1Green");
            player1[4] = GetChildControl<ButtonControl>("P1Yellow");

            player2[0] = GetChildControl<ButtonControl>("P2Buzz");
            player2[1] = GetChildControl<ButtonControl>("P2Blue");
            player2[2] = GetChildControl<ButtonControl>("P2Orange");
            player2[3] = GetChildControl<ButtonControl>("P2Green");
            player2[4] = GetChildControl<ButtonControl>("P2Yellow");

            player3[0] = GetChildControl<ButtonControl>("P3Buzz");
            player3[1] = GetChildControl<ButtonControl>("P3Blue");
            player3[2] = GetChildControl<ButtonControl>("P3Orange");
            player3[3] = GetChildControl<ButtonControl>("P3Green");
            player3[4] = GetChildControl<ButtonControl>("P3Yellow");

            player4[0] = GetChildControl<ButtonControl>("P4Buzz");
            player4[1] = GetChildControl<ButtonControl>("P4Blue");
            player4[2] = GetChildControl<ButtonControl>("P4Orange");
            player4[3] = GetChildControl<ButtonControl>("P4Green");
            player4[4] = GetChildControl<ButtonControl>("P4Yellow");
        }

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }
    }
}
