using System.Collections.Generic;
using Pitcher.Midi.Interop;

namespace Pitcher.Midi.IO {

   /// <summary>methods to retrieve device information</summary>
   public class DeviceManager {
      public static uint NumInputDevices { get => NativeInputOps.midiInGetNumDevs(); }
      public static uint NumOutputDevices { get => NativeOutputOps.midiOutGetNumDevs(); }

      public static IEnumerable<InputPort.DeviceInformation> InputDevices { 
         get {
            for (uint i = 0; i < NumInputDevices; ++i) {
               yield return new InputPort.DeviceInformation(i);
            }
         }
      }
      
      public static IEnumerable<OutputPort.DeviceInformation> OutputDevices {
         get {
            for (uint i = 0; i < NumOutputDevices; ++i) {
               yield return new OutputPort.DeviceInformation(i);
            }
         }
      }

   }

}