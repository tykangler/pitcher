using System;
using Microsoft.Win32.SafeHandles;

namespace Pitcher.Midi.Interop {

   public class MidiInSafeHandle : SafeHandleMinusOneIsInvalid {

      public MidiInSafeHandle() : base(true) {}

      public MidiInSafeHandle(IntPtr handle) : base(true) {
         SetHandle(handle);
      }

      protected override bool ReleaseHandle() {
         return NativeInputOps.midiInClose(this.handle) 
                == NativeInputOps.MessageResult.MMSYSERR_NOERROR;
      }
   }
}