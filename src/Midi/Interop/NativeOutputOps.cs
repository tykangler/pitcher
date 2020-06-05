using System;
using System.Runtime.InteropServices;

namespace Pitcher.Midi.Interop {

   public static class NativeOutputOps {

      const string midiLib = "winmm.dll";

      #region native methods

      public delegate void MidiOutProc(IntPtr handleMidiOut,
                                       MidiMessage message,
                                       UIntPtr instance,
                                       uint messageParam1,
                                       uint messageParam2);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiOutOpen(out MidiOutSafeHandle pHandleMidiOut,
                                                     uint deviceId,
                                                     MidiOutProc callback,
                                                     UIntPtr instance,
                                                     CallbackFlag callbackFlag);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiOutClose(IntPtr handleMidiOut);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiOutShortMessage(MidiOutSafeHandle handleMidiOut,
                                                             uint message);
      
      [StructLayout(LayoutKind.Sequential)]
      public struct MidiOutCaps {
         public ushort manufacturerId;
         public ushort productId;
         public uint version;
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
         public string productName;
         public Technology technology;
         public ushort voices;
         public ushort notes;
         public ushort channelMask;
         public OptSupport optSupport;
      }
      
      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiOutGetDevCaps(uint deviceId, 
                                                           out MidiOutCaps caps, 
                                                           uint capsSize);

      [DllImport(midiLib, SetLastError = true)]
      public static extern uint midiOutGetNumDevs();

      #endregion

      #region helper methods

      public static uint midiOutCapsSize { get => (uint) Marshal.SizeOf<MidiOutCaps>(); }
   }
}