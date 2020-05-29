using System.Runtime.InteropServices;
using System;

namespace Pitcher.Midi.Interop {

   internal static class NativeInputOps {

      const string midiLib = "winmm.dll";

      #region native methods

      public enum MidiMessage : uint {
         Open = 0x3c1,
         Close,
         Data,
         LongData,
         Error,
         LongError,
         MoreData = 0x3cc
      }

      public enum CallbackFlag : uint {
         CallbackNull = 0,
         MidiIoStatus = 0x20,
         CallbackWindows = 0x10000,
         CallbackThread = 0x20000,
         CallbackFunction = 0x30000,
      }

      public enum MessageResult : uint {
         MMSYSERR_NOERROR = 0,
         MMSYSERR_BADDEVICEID = 2,
         MMSYSERR_ALLOCATED = 4,
         MMSYSERR_INVALHANDLE = 5,
         MMSYSERR_NOMEM = 7,
         MMSYSERR_INVALFLAG = 0xa,
         MMSYSERR_INVALPARAM = 0xb,
         MIDIERR_STILLPLAYING = 0x29
      }

      public delegate void MidiInProc(MidiInSafeHandle handleMidiIn, 
                                      MidiMessage message, 
                                      UIntPtr instance, 
                                      uint messageParam1, 
                                      uint messageParam2);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInClose(IntPtr handleMidiIn);

      [DllImport(midiLib, SetLastError = true)]
      public static extern MessageResult midiInOpen(out IntPtr pHandleMidiIn, 
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
                                                          uint cbmic);

      [DllImport(midiLib, SetLastError = true)]
      public static extern uint midiInGetNumDevs();

      #endregion

      #region public helpers

      public static uint midiInCapsSize() => (uint) Marshal.SizeOf<MidiInCaps>();

      #endregion
   }
}
