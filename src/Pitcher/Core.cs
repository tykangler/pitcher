using System;
using System.Linq;
using static System.ConsoleKey;

#nullable enable

namespace Pitcher {

   class Core {
   
      static void Main(string[] args) {
         int inputId = PromptandSelectInputDeviceId();
         int outputId = PromptandSelectOutputDeviceId();
         using (Midi.IO.OutputPort outputPort = new Midi.IO.OutputPort((uint) outputId)) {
            Console.WriteLine("receiving input from keyboard");
            Console.WriteLine($"writing output to {outputPort.Device.ProductName}");
            PlayConsoleKeyboardInput(outputPort);
         }
      }

      static void PlayConsoleKeyboardInput(Midi.IO.OutputPort outputPort) {
         ConsoleKey oldKey = 0;
         ConsoleKeyInfo currKeyInfo = Console.ReadKey(true);
         while (currKeyInfo.Key != ConsoleKey.Q) {
            string note = currKeyInfo.Key switch {
               A => "C4", W => "C#4", S => "D4", E => "D#4", D => "E4", F => "F4", T => "F#4",
               G => "G4", Y => "G#4", H => "A4", U => "A#4", J => "B4", K => "C5", O => "C#5",
               L => "D5", P => "D#5", Oem1 => "E5", _ => string.Empty
            };
            if (note.Length != 0 && oldKey != currKeyInfo.Key) {
               outputPort.SendShortMessage(new Midi.Events.NoteOn(0, note, 120));
            }
            oldKey = currKeyInfo.Key;
            currKeyInfo = Console.ReadKey(true);
         }
      }

      static int PromptandSelectInputDeviceId() {
         Console.WriteLine("Listed Input Devices");
         Console.WriteLine("--------------------------");
         var inputDevices = Midi.IO.DeviceManager.InputDevices;
         if (inputDevices.Count() != 0) {
            foreach (var device in inputDevices) {
               Console.WriteLine($"{device.DeviceId} | {device.ProductName}");
            }
            Console.WriteLine("--------------------------");
            Console.Write("Select ID: ");
            return Convert.ToByte(Console.ReadLine());
         } else {
            Console.WriteLine("None!");
            return -1;
         }
      }

      static int PromptandSelectOutputDeviceId() {
         Console.WriteLine("Listed Output Devices");
         Console.WriteLine("--------------------------");
         var outputDevices = Midi.IO.DeviceManager.OutputDevices;
         if (outputDevices.Count() != 0) {
            foreach (var device in outputDevices) {
               Console.WriteLine($"{device.DeviceId} | {device.ProductName}");
            }
            Console.WriteLine("--------------------------");
            Console.Write("Select ID: ");
            return Convert.ToByte(Console.ReadLine());
         } else {
            Console.WriteLine("None!");
            return -1;
         }
      }
   }
}
