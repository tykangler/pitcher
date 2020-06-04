using System;
using Microsoft.Win32.SafeHandles;

namespace Pitcher.Midi.Interop {

   public class MidiOutSafeHandle : SafeHandleMinusOneIsInvalid {

      public MidiOutSafeHandle() : base(true) {}

      public MidiOutSafeHandle(IntPtr handle) : base(true) {
         SetHandle(handle);
      }

      protected override bool ReleaseHandle() {
         return NativeOutputOps.midiOutClose(this.handle) 
                == MessageResult.MMSYSERR_NOERROR;
      }
   }

}