namespace Pitcher.Midi.Events {
   public class ProgramChange : IMidiEvent {
      public byte Channel { get; }
      public byte Program { get; }

      public ProgramChange(byte channel, byte program) {
         this.Channel = channel;
         this.Program = program;
      }

      public override string ToString() {
         return $"ProgramChange<Channel={this.Channel}, Program={this.Program}>";
      }
   }
}