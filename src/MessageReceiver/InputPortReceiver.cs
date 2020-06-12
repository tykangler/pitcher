using System;
using System.Threading;
using Pitcher.Midi.IO;
using Pitcher.Midi.Events;

namespace Pitcher.MessageReciever {

   public class InputPortReceiver {

      public static void Main(string[] args) {
         AutoResetEvent resetEvent = new AutoResetEvent(false);
         const byte c6 = 72;
         void handleMessageReceived(object? _, MessageEventArgs m) {
            if (!(m.Event is NoteOff noteOff)) {
               PrintMessageData(m);
            }
            if (m.Event is NoteOn noteOn) {
               if (noteOn.Note == c6) {
                  resetEvent.Set();
               }
            }
         }
         using (InputPort inputPort = new InputPort(0u)) {
            Console.WriteLine($"Found Device {inputPort.Device.ProductName}");
            inputPort.MessageReceived += handleMessageReceived;
            Console.WriteLine("Added Message Handler");
            inputPort.Start();
            Console.WriteLine("Reading...");
            resetEvent.WaitOne();
         }
      }

      static void PrintMessageData(MessageEventArgs m) {
         Console.ForegroundColor = m.Event switch {
            NoteOn _ => ConsoleColor.Green,
            NoteOff _ => ConsoleColor.Red,
            _ => ConsoleColor.White
         };
         Console.WriteLine(m.Event?.ToString());
         Console.ForegroundColor = ConsoleColor.White;
         Console.WriteLine($"\tRaw Message: {string.Join(", ", m.Message)}");
         Console.WriteLine($"\tTimeStamp: {m.TimeStamp / 1000.0}");
      }
   }

}