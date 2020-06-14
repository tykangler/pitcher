using System;

namespace Pitcher.Midi.Events {

   public abstract class NoteEvent : MidiEvent {

      protected const int lowestOctave = -1;
      protected const int highestOctave = 9;

      public override uint RawMessage { get; }
      public byte Note { get; }
      public byte Velocity { get; }
      
      protected NoteEvent(MidiStatus code, byte channel, byte note, byte velocity) {
         if (velocity < 0 || velocity > 127) {
            throw new ArgumentException($"{velocity} not within [0, 127]");
         } else if (note < 0 || note > 127) {
            throw new ArgumentException($"{note} not within range [0, 127]");
         }
         this.Note = note;
         this.Velocity = velocity;
         this.Channel = channel;
         this.RawMessage = PackIntoRawMessage(code, channel, note, velocity);
      }

      protected NoteEvent(MidiStatus code, byte channel, string note, byte velocity) {
         if (velocity < 0 || velocity > 127) {
            throw new ArgumentException($"{velocity} not within [0, 127]");
         }
         this.Note = ParseMidiNote(note);
         this.Channel = channel;
         this.Velocity = velocity;
         this.RawMessage = PackIntoRawMessage(code, channel, this.Note, velocity);
      }

      protected NoteEvent(uint raw) {
         this.RawMessage = raw;
         var (channel, note, velocity) = ParseMessage(raw);
         this.Channel = channel;
         this.Note = note;
         this.Velocity = velocity;
      }

      protected uint PackIntoRawMessage(MidiStatus code, int channel, int note, int velocity) {
         int statusByte = (((byte) code) << 4) | channel;
         int noteByte = note << 8;
         int velocityByte = velocity << 16;
         return (uint) (velocityByte | noteByte | statusByte);
      }

      protected (byte, byte, byte) ParseMessage(uint raw, bool littleEndian = true) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1], rawBytes[2]);
      }

      protected byte ParseMidiNote(string note) {
         if (note.Length > 4 || note.Length < 2) {
            throw new ArgumentException("note string must have length == 2 or 3");
         } 
         int octave = ParseOctave(note);
         char baseNote = note[0];
         if (octave > highestOctave || octave < lowestOctave) {
            throw new ArgumentException($"{octave} is not in range [-1, 9]");
         }
         byte midiNote = (byte) (Char.ToLower(baseNote) switch {
            'c' => 0, 'd' => 2, 'e' => 4, 'f' => 5, 'g' => 7, 'a' => 9, 'b' => 11, 
            _ => throw new ArgumentException("not a valid note")
         } + (octave + 1) * 12);
         if (note.Length > 2) {
            if (note[1] == '#') {
               ++midiNote;
            } else if (note[1] == 'b') {
               --midiNote;
            }
         }
         if (midiNote > 127 || midiNote < 0) {
            throw new ArgumentException($"parsed note {midiNote} not within range [0, 127]");
         }
         return midiNote;
      }

      int ParseOctave(string note) {
         if (!Char.IsDigit(note[^1])) {
            throw new ArgumentException($"{note[^1]} is not an octave");
         } 
         int octave = 0;
         if (note.Length > 2) {
            if (note.Length == 3) {
               octave = note[^2] switch {
                  '-' => Convert.ToInt32(note[^2..]),
                  'b' => note[^1] - '0', '#' => note[^1] - '0',
                  _ => throw new ArgumentException($"got {note[^2]} instead of sign/accidental")
               };
            } else if (note.Length == 4) {
               if (note[^2] != '-') {
                  throw new ArgumentException($"got {note[^2]} instead of sign");
               }
               octave = Convert.ToInt32(note[^2..]);
            }
         } else {
            octave = note[^1] - '0';
         }
         return octave;
      }
   }

   public class NoteOff : NoteEvent {

      public NoteOff(byte channel, byte note, byte velocity) 
      : base(MidiStatus.NoteOff, channel, note, velocity) {}

      public NoteOff(byte channel, string note, byte velocity) 
      : base(MidiStatus.NoteOff, channel, note, velocity) {}

      public NoteOff(uint raw) : base(raw) {}

      public override string ToString() {
         return $"NoteOff<Channel={this.Channel}, Note={this.Note}, Velocity={this.Velocity}\n" +
                new string(' ', this.Note) + '|';
      }
   }

   public class NoteOn : NoteEvent {

      public NoteOn(byte channel, byte note, byte velocity)
      : base(MidiStatus.NoteOn, channel, note, velocity) {}

      public NoteOn(uint raw) : base(raw) {}

      public NoteOn(byte channel, string note, byte velocity)
      : base(MidiStatus.NoteOn, channel, note, velocity) {}

      public override string ToString() {
         return $"NoteOn<Channel={this.Channel}, Note={this.Note}, Velocity={this.Velocity}\n" +
                new string(' ', this.Note) + '|';
      }
   }
}