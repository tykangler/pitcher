using System.Runtime.InteropServices;
using System;

namespace Pitcher.Midi.Interop {

   public static class NativeInputOps {

      const string midiLib = "winmm.dll";

      #region native methods

      // use IntPtr for the handleMidiIn because marshaling error
      // "can't marshal SafeHandle from unmanaged to managed
      public delegate void MidiInProc(IntPtr handleMidiIn, 
                                      MidiMessage message, 
                                      UIntPtr instance, 
                                      uint messageParam1, 
                                      uint messageParam2);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInClose(IntPtr handleMidiIn);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInOpen(out MidiInSafeHandle pHandleMidiIn, 
                                                    uint deviceId,
                                                    MidiInProc callback, 
                                                    UIntPtr instance, 
                                                    CallbackFlag callbackFlag);
      
      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInStart(MidiInSafeHandle handleMidiIn);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInStop(MidiInSafeHandle handleMidiIn);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInReset(MidiInSafeHandle handleMidiIn);

      [StructLayout(LayoutKind.Sequential)]
      public struct MidiInCaps {
         public ushort manufacturerId;
         public ushort productId;
         public uint driverVersion;
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
         public string productName;
         public uint dwSupport;
      }

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInGetDevCaps(uint deviceId, 
                                                          out MidiInCaps caps, 
                                                          uint capsSize);

      [DllImport(midiLib, SetLastError = true)]
      public static extern uint midiInGetNumDevs();

      #endregion

      #region public helpers

      public static uint midiInCapsSize() => (uint) Marshal.SizeOf<MidiInCaps>();

      #endregion
   }
}
