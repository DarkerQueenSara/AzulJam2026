using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct BuzzOutputReport : IInputDeviceCommandInfo
    {

        public static FourCC Type { get { return new FourCC("HIDO"); } }
        internal const int id = 0;
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 8;

        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize)] 
        public int zero;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] 
        public int one;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)]
        public byte p1Light;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 3)]
        public byte p2Light;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)]
        public byte p3Light;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 5)]
        public byte p4Light;

        public FourCC typeStatic
        {
            get { return Type; }
        }

        public static BuzzOutputReport Create(bool p1Lightp, bool p2Lightp, bool p3Lightp, bool p4Lightp)
        {
            return new BuzzOutputReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                p1Light = (byte)(p1Lightp ? 0xff : 0),
                p2Light = (byte)(p2Lightp ? 0xff : 0),
                p3Light = (byte)(p3Lightp ? 0xff : 0),
                p4Light = (byte)(p4Lightp ? 0xff : 0),
            };
        }
    }
}
