namespace Pitcher.Midi.Events {
   public class PitchBend : MidiEvent {
      public override uint RawMessage { get; }
      public byte Bend { get; }

      public PitchBend(byte channel, byte bend) {
         this.Channel = channel;
         this.Bend = bend;
      }

      public PitchBend(uint raw) {
         this.RawMessage = raw;
         var (channel, bend) = ParseMessage(raw);
         this.Channel = channel;
         this.Bend = bend;
      }

      (byte, byte) ParseMessage(uint raw) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1]);
      }

      uint Pack(int channel, int bend) {
         int statusByte = (((byte) MidiStatus.PitchBend) << 4) | channel;
         int bendByte = bend << 8;
         return (uint) (bendByte | statusByte);
      }

      public override string ToString() {
         return $"PitchBend<Channel={this.Channel}, Bend={this.Bend}>";
      }
   }
}