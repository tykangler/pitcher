namespace Pitcher.Midi.Events {
   public class PitchBend : IMidiEvent {
      public byte Channel { get; }
      public byte Bend { get; }

      public PitchBend(byte channel, byte bend) {
         this.Channel = channel;
         this.Bend = bend;
      }
   }
}