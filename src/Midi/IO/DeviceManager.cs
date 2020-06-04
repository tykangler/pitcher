using System.Collections.Generic;
using Pitcher.Midi.Interop;

namespace Pitcher.Midi.IO {

   /// <summary>methods to retrieve device information</summary>
   public class DeviceManager {
      public static uint NumInputDevices { get => NativeInputOps.midiInGetNumDevs(); }
      public static uint NumOutputDevices { get; }

      public static IEnumerable<InputPort> InputDevices { 
         get {
            for (uint i = 0; i < NumInputDevices; ++i) {
               yield return new InputPort(i);
            }
         }
      }

      /*
      public static IEnumerable<OutputProt> OutputDevices {
         get;
      }
      */

   }

}