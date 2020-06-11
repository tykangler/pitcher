namespace Pitcher.Midi.Events {

   // FIXME: Inheritance is funky, Fix raw message and codes.
   /// <summary></summary>
   public abstract class NoteEvent : MidiEvent {

      public override uint RawMessage { get; }
      public byte Note { get; }
      public byte Velocity { get; }
      
      protected NoteEvent(MidiStatus code, byte channel, byte note, byte velocity) {
         this.Note = note;
         this.Velocity = velocity;
         this.Channel = channel;
         this.RawMessage = PackIntoRawMessage(code, channel, note, velocity);
      }

      protected NoteEvent(uint raw) {
         this.RawMessage = raw;
         var (channel, note, velocity) = ParseMessage(raw);
         this.Channel = channel;
         this.Note = note;
         this.Velocity = velocity;
      }

      protected uint PackIntoRawMessage(MidiStatus code, int channel, int note, int velocity) {
         int statusByte = (((byte) code) << 2) | channel;
         int noteByte = note << 8;
         int velocityByte = velocity << 16;
         return (uint) (velocityByte | noteByte | statusByte);
      }

      protected (byte, byte, byte) ParseMessage(uint raw, bool littleEndian = true) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1], rawBytes[2]);
      }
   }

   public class NoteOff : NoteEvent {

      public NoteOff(byte channel, byte note, byte velocity) 
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

      public override string ToString() {
         return $"NoteOn<Channel={this.Channel}, Note={this.Note}, Velocity={this.Velocity}\n" +
                new string(' ', this.Note) + '|';
      }
   }
}