using System;
using System.Threading;
using Pitcher.Midi.IO;
using Pitcher.Midi.Events;

namespace Pitcher.MessageReciever {

   public class InputPortReceiver {

      public static void Main(string[] args) {
         AutoResetEvent resetEvent = new AutoResetEvent(false);
         const byte c6 = 72;
         void handleMessageReceived(object _, MessageEventArgs m) {
            if (!(m.Event is NoteEvent noteEvent && noteEvent.Type == NoteEvent.Sound.Off)) {
               PrintMessageData(m);
            }
            if (m.Event is NoteEvent note && note.Type == NoteEvent.Sound.On) {
               if (note.Note == c6) {
                  resetEvent.Set();
               }
            }
         }
         using (InputPort inputPort = new InputPort(0u)) {
            Console.WriteLine($"Found Device {inputPort.ProductName}");
            inputPort.MessageReceived += handleMessageReceived;
            Console.WriteLine("Added Message Handler");
            inputPort.Start();
            Console.WriteLine("Reading...");
            resetEvent.WaitOne();
         }
      }

      static void PrintMessageData(MessageEventArgs m) {
         if (m.Event is NoteEvent noteEvent) {
            if (noteEvent.Type == NoteEvent.Sound.On) {
               Console.ForegroundColor = ConsoleColor.Green;
            } else {
               Console.ForegroundColor = ConsoleColor.Red;
            }
         } 
         Console.WriteLine(m.Event?.ToString());
         Console.ForegroundColor = ConsoleColor.White;
         Console.WriteLine($"\tRaw Message: {string.Join(", ", m.Message)}");
         Console.WriteLine($"\tTimeStamp: {m.TimeStamp / 1000.0}");
      }
   }

}