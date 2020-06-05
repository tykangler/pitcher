namespace Pitcher.Midi.Events {
   public class PitchBend : IMidiEvent {
      public MidiStatus Status { get => MidiStatus.PitchBend; }
      public byte Channel { get; }
      public byte Bend { get; }

      public PitchBend(byte channel, byte bend) {
         this.Channel = channel;
         this.Bend = bend;
      }

      public uint Pack() {
         int statusByte = (((byte) Status) << 2) | Channel;
         int bendByte = Bend << 8;
         return (uint) (bendByte | statusByte);
      }

      public override string ToString() {
         return $"PitchBend<Channel={this.Channel}, Bend={this.Bend}>";
      }
   }
}