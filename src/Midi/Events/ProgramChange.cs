namespace Pitcher.Midi.Events {
   public class ProgramChange : IMidiEvent {
      public MidiStatus Status { get => MidiStatus.ProgramChange; }
      public byte Channel { get; }
      public byte Program { get; }

      public ProgramChange(byte channel, byte program) {
         this.Channel = channel;
         this.Program = program;
      }

      public uint Pack() {
         int statusByte = (((byte) Status) << 2) | Channel;
         int programByte = Program << 8;
         return (uint) (programByte | statusByte);
      }

      public override string ToString() {
         return $"ProgramChange<Channel={this.Channel}, Program={this.Program}>";
      }
   }
}