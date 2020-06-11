namespace Pitcher.Midi.Events {
   public class ProgramChange : MidiEvent {
      public override uint RawMessage { get; }
      public byte Program { get; }

      public ProgramChange(byte channel, byte program) {
         this.Channel = channel;
         this.Program = program;
         this.RawMessage = Pack(channel, program);
      }

      public ProgramChange(uint raw) {
         this.RawMessage = raw;
         var (channel, program) = ParseMessage(raw);
         this.Channel = channel;
         this.Program = program;
      }

      (byte, byte) ParseMessage(uint raw) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1]);
      }
      uint Pack(int channel, int program) {
         int statusByte = (((byte) MidiStatus.ProgramChange) << 2) | channel;
         int programByte = program << 8;
         return (uint) (programByte | statusByte);
      }

      public override string ToString() {
         return $"ProgramChange<Channel={this.Channel}, Program={this.Program}>";
      }
   }
}