using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace BuzzControllerSystem
{
    public class BuzzInput : MonoBehaviour
    {

        public enum BuzzButton
        {
            Buzz,
            Blue,
            Orange,
            Green,
            Yellow
        }

        public enum BuzzLightSequence
        {
            SlowFlash,
            Flash,
            FastFlash,
            SlowMove,
            Move,
            FastMove,
            PingPong
        }

        BuzzInputDevice device;
        bool[] lights = new bool[4];
        int position = 0;
        bool direction = false;

        public float slowSequenceTime = 1f;
        public float normalSequenceTime = 0.4f;
        public float fastSequenceTime = 0.1f;

        void Start()
        {
            device = BuzzInputDevice.current;
        }

        //
        // Button Inputs
        //

        public bool GetButton(int player, BuzzButton button)
        {
            switch (player)
            {
                case 0:
                    return device.player1[(int)button].isPressed;
                case 1:
                    return device.player2[(int)button].isPressed;
                case 2:
                    return device.player3[(int)button].isPressed;
                case 3:
                    return device.player4[(int)button].isPressed;
                default:
                    return false;
            }
        }

        public bool GetButtonDown(int player, BuzzButton button)
        {
            switch (player)
            {
                case 0:
                    return device.player1[(int)button].wasPressedThisFrame;
                case 1:
                    return device.player2[(int)button].wasPressedThisFrame;
                case 2:
                    return device.player3[(int)button].wasPressedThisFrame;
                case 3:
                    return device.player4[(int)button].wasPressedThisFrame;
                default:
                    return false;
            }
        }

        public bool GetButtonUp(int player, BuzzButton button)
        {
            switch (player)
            {
                case 0:
                    return device.player1[(int)button].wasReleasedThisFrame;
                case 1:
                    return device.player2[(int)button].wasReleasedThisFrame;
                case 2:
                    return device.player3[(int)button].wasReleasedThisFrame;
                case 3:
                    return device.player4[(int)button].wasReleasedThisFrame;
                default:
                    return false;
            }
        }


        //
        // Lights
        //

        public void SetLight(int player, bool toggle)
        {
            BuzzOutputReport outputReport;
            switch (player)
            {
                case 0:
                    outputReport = BuzzOutputReport.Create(toggle, lights[1], lights[2], lights[3]);
                    break;
                case 1:
                    outputReport = BuzzOutputReport.Create(lights[0], toggle, lights[2], lights[3]);
                    break;
                case 2:
                    outputReport = BuzzOutputReport.Create(lights[0], lights[1], toggle, lights[3]);
                    break;
                case 3:
                    outputReport = BuzzOutputReport.Create(lights[0], lights[1], lights[2], toggle);
                    break;
                default:
                    outputReport = BuzzOutputReport.Create(false, false, false, false);
                    break;
            }
            lights[player] = toggle;
            device?.ExecuteCommand(ref outputReport);
        }

        public void StartLightSequence(BuzzLightSequence lightSequence)
        {
            switch (lightSequence)
            {
                case BuzzLightSequence.SlowFlash:
                    InvokeRepeating("Flash", 0, slowSequenceTime);
                    break;
                case BuzzLightSequence.Flash:
                    InvokeRepeating("Flash", 0, normalSequenceTime);
                    break;
                case BuzzLightSequence.FastFlash:
                    InvokeRepeating("Flash", 0, fastSequenceTime);
                    break;
                case BuzzLightSequence.SlowMove:
                    InvokeRepeating("Move", 0, slowSequenceTime);
                    break;
                case BuzzLightSequence.Move:
                    InvokeRepeating("Move", 0, normalSequenceTime);
                    break;
                case BuzzLightSequence.FastMove:
                    InvokeRepeating("Move", 0, fastSequenceTime);
                    break;
                case BuzzLightSequence.PingPong:
                    InvokeRepeating("PingPong", 0, normalSequenceTime);
                    break;
            }
        }

        public void StopLightSequence()
        {
            CancelInvoke();
            var outputReport = BuzzOutputReport.Create(false, false, false, false);
            device?.ExecuteCommand(ref outputReport);

            for (int i = 0; i < 4; i++)
            {
                lights[i] = false;
            }
        }

        void Flash()
        {
            var outputReport = BuzzOutputReport.Create(!lights[0], !lights[1], !lights[2], !lights[3]);
            device?.ExecuteCommand(ref outputReport);

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i] = !lights[i];
            }
        }

        void Move()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (i != position)
                {
                    lights[i] = false;
                }

            }

            SetLight(position, true);

            position++;

            if (position == 4)
            {
                position = 0;
            }
        }

        void PingPong()
        {
            for (int i = 0; i < lights.Length; i++)
            {
                if (i != position)
                {
                    lights[i] = false;
                }

            }

            SetLight(position, true);

            if (!direction)
            {
                position++;
            }
            else
            {
                position--;
            }

            if (position == 4)
            {
                position = 3;
                direction = true;
            }
            if (position == -1)
            {
                position = 0;
                direction = false;
            }
        }

        private void OnApplicationQuit()
        {
            var outputReport = BuzzOutputReport.Create(false, false, false, false);
            device?.ExecuteCommand(ref outputReport);
        }
    }
}
