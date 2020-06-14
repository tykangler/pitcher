using System;
using Xunit;
using Xunit.Abstractions;
using Pitcher.Midi.Events;

namespace Pitcher.Test.Midi.Events {
   public class NoteEventTest {

      readonly ITestOutputHelper output;

      public NoteEventTest(ITestOutputHelper output) => this.output = output;

      [Fact]
      public void RegularConstructorPopulatesCorrectly() {
         NoteEvent note = new NoteOff(0, 12, 100);
         Assert.True(note.Channel == 0);
         Assert.True(note.Note == 12);
         Assert.True(note.Velocity == 100);
      }

      [Fact]
      public void StringConstructorPopulatesCorrectly() {
         (string, int)[] midiNotes = {
            ("C", 0), ("C#", 1), ("Db", 1), ("D", 2), ("D#", 3), ("Eb", 3), ("E", 4), 
            ("E#", 5), ("Fb", 4), ("F", 5), ("F#", 6), ("Gb", 6), ("G", 7), ("G#", 8), 
            ("Ab", 8), ("A", 9), ("A#", 10), ("Bb", 10), ("B", 11)
         };
         for (int i = -1; i < 9; ++i) {
            foreach (var (noteString, noteValue) in midiNotes) {
               var newNote = new NoteOff(0, noteString + i, 1);
               Assert.True(newNote.Note == 12 * (i + 1) + noteValue);
            }
         }
         foreach (var (noteString, noteValue) in midiNotes) {
            if (noteString == "G") {
               break;
            }
            var newNote = new NoteOff(0, noteString + '9', 1);
            Assert.True(newNote.Note == 120 + noteValue);
         }
      }

      [Fact]
      public void InvalidNoteStringThrows() {
         Assert.Throws<ArgumentException>(() => new NoteOff(0, "cb-1", 1));
         Assert.Throws<ArgumentException>(() => new NoteOff(0, "g#9", 1));
         Assert.Throws<ArgumentException>(() => new NoteOff(0, "c-2", 1));
         Assert.Throws<ArgumentException>(() => new NoteOff(0, "c10", 1));
      }
   }
}