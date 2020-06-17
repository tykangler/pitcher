using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Pitcher.Midi.IO;
using Pitcher.Midi.Events;

namespace Pitcher.Test.Midi.IO {

   public class OutputPortTest {

      readonly ITestOutputHelper output;

      public OutputPortTest(ITestOutputHelper output) {
         this.output = output;
      }

      [Fact]
      public void SendNoteOnNoteOffMakesSound() {
         byte channel = 0;
         string note = "c4";
         byte velocity = 127;
         using (OutputPort outputPort = new OutputPort(0)) {
            Assert.True(outputPort.SendShortMessage(new NoteOn(channel, note, velocity)));
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Assert.True(outputPort.SendShortMessage(new NoteOff(channel, note, velocity)));
         }
      }

      [Fact]
      public void ProgramChangeChangesInstrument() {
         byte note = 48; // c3
         byte channel = 0;
         byte velocity = 100;
         using (OutputPort outputPort = new OutputPort(0)) {
            for (int i = 0; i < 8; ++i) {
               Assert.True(outputPort.SendShortMessage(new ProgramChange(channel, (byte) i)));
               Assert.True(outputPort.SendShortMessage(new NoteOn(channel, note, velocity)));
               Thread.Sleep(TimeSpan.FromSeconds(1.5));
               Assert.True(outputPort.SendShortMessage(new NoteOff(channel, note, 0)));
               ++note;
            }
         }
      }

      [Fact]
      public void CanMakeChords() {
         byte channel = 0;
         byte velocity = 100;
         byte[] chord = {48, 52, 55};
         using (OutputPort outputPort = new OutputPort(0)) {
            for (int i = 0; i < chord.Length; ++i) {
               outputPort.SendShortMessage(new NoteOn(channel, chord[i], velocity));
               Thread.Sleep(TimeSpan.FromSeconds(1.5));
            }
            Thread.Sleep(TimeSpan.FromSeconds(1.5));
            for (int i = chord.Length - 1; i >= 0; --i) {
               outputPort.SendShortMessage(new NoteOff(channel, chord[i], velocity));
               Thread.Sleep(TimeSpan.FromSeconds(1.5));
            }
         }
      }

      [Fact]
      public void ControllerDoesSomething() {
         byte channel = 0;
         byte controller = 64;
         using (OutputPort outputPort = new OutputPort(0)) {
            outputPort.SendShortMessage(new NoteOn(channel, "a2", 120));
            for (int i = 0; i < 127; ++i) {
               outputPort.SendShortMessage(new Controller(channel, controller, (byte) i));
            }
            Thread.Sleep(TimeSpan.FromSeconds(1.5));
         }
      }
   }

}