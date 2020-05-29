using System.Collections.Generic;
using Pitcher.Midi.Interop;

namespace Pitcher.Midi.Device
{

   /// <summary>methods to retrieve device information</summary>
   public class Information {
      public static uint numInputDevices { get => NativeInputOps.midiInGetNumDevs(); }
      public static uint numOutputDevices { get; }

      public static IEnumerable<InputPort> InputDevices { 
         get {
            for (uint i = 0; i < numInputDevices; ++i) {
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