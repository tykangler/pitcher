using System;

namespace Pitcher {
   
namespace Music {
      
class Note {
   // array of tuples (noteString, frequency)
   public static readonly (string, double)[] baseNotes = 
      {("A", 27.50), ("A#", 29.14), ("B", 30.87), ("C", 16.35), ("C#", 17.32), ("D", 18.35), 
       ("D#", 19.45), ("E", 20.60), ("F", 21.83), ("F#", 23.12), ("G", 24.50), ("G#", 25.96)};

   public int NoteValue { get; }
   public double Frequency { get; }
   public string NoteString { get; }

   /// <summary>
   /// Initializes <c>Note</c> with noteValue and its corresponding frequency
   /// </summary>
   /// <param name="noteValue">
   /// Number of semitones after A0, with A0 at 0, B0 at 2, C1 at 3, ...
   /// </param>
   public Note(int noteValue) {
      this.NoteValue = noteValue;
      int octave = noteValue / baseNotes.Length;
      int baseNoteIndex = noteValue % baseNotes.Length;
      (string baseNoteString, double baseFrequency) = baseNotes[baseNoteIndex];
      this.Frequency = baseFrequency * (uint) (1 << octave);
      this.NoteString = baseNoteString + (baseNoteIndex < 3 ? octave : octave + 1);
   }

   /// <returns>
   /// NoteString, the conventional representation of a note with octave included
   /// </returns>
   public override string ToString() {
      return NoteString;
   }
}
}
}