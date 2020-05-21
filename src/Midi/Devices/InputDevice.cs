using System;
using System.Runtime.InteropServices;

namespace Pitcher.Midi.Devices {

   public class InputDevice {
      
      private IntPtr handle;

      public uint DeviceId { get; }
      public ushort ProductId { get; }
      public ushort ManufacturerId { get; }
      public string ProductName { get; }

      public InputDevice(uint deviceId) {
         var caps = new NativeInputOperations.MidiInCaps();
         uint capsSize = (uint) Marshal.SizeOf<NativeInputOperations.MidiInCaps>();
         if (NativeInputOperations.midiInGetDevCaps(deviceId, ref caps, capsSize) ==
             NativeInputOperations.MessageResult.MMSYSERR_NOERROR) {
            
         }

         this.DeviceId = deviceId;
         this.ProductId = caps.productId;
         this.ManufacturerId = caps.manufacturerId;
         this.ProductName = caps.productName;
         this.handle =  IntPtr.Zero;
      }
   }

   internal static class NativeInputOperations {

      const string midiLib = "winmm.dll";

      public enum MidiMessage : uint {
         Open = 0x3c1,
         Close,
         Data,
         LongData,
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

      public delegate void MidiInCallback(IntPtr handleMidiIn, 
                                          MidiMessage message, 
                                          IntPtr instance, 
                                          uint messageParam1, 
                                          uint messageParam2);

      [DllImport(midiLib)]
      public static extern MessageResult midiInClose(IntPtr handleMidiIn);

      [DllImport(midiLib)]
      public static extern MessageResult midiInOpen(ref IntPtr ptrHandleMidiIn, 
                                                    uint deviceId,
                                                    MidiInCallback callback, 
                                                    UIntPtr instance, 
                                                    CallbackFlag callbackFlag);
      
      [DllImport(midiLib)]
      public static extern MessageResult midiInStart(IntPtr handleMidiIn);

      [DllImport(midiLib)]
      public static extern MessageResult midiInStop(IntPtr handleMidiIn);

      [StructLayout(LayoutKind.Sequential)]
      public struct MidiInCaps {
         public ushort manufacturerId;
         public ushort productId;
         public uint driverVersion;
         [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
         public string productName;
         public uint dwSupport;
      }
      
      [DllImport(midiLib)]
      public static extern MessageResult midiInGetDevCaps(uint deviceId, 
                                                          ref MidiInCaps caps, 
                                                          uint cbmic);
      // Use Marshal.SizeOf(typeof(MidiInCaps)) for parameter cbmic
   }

}