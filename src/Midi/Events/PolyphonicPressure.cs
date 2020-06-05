namespace Pitcher.Midi.Events {
   public class PolyphonicPressure : IMidiEvent {
      public MidiStatus Status { get => MidiStatus.PolyphonicPressure; }
      public byte Channel { get; }
      public byte Note { get; }
      public byte Pressure { get; }

      public PolyphonicPressure(byte channel, byte note, byte pressure) {
         this.Channel = channel;
         this.Note = note;
         this.Pressure = pressure;
      }

      public uint Pack() {
         int statusByte = (((byte) Status) << 2) | Channel;
         int noteByte = Note << 8;
         int pressureByte = Note << 16;
         return (uint) (pressureByte | noteByte | statusByte);
      }

      public override string ToString() {
         return $"PolyphonicPressure<Channel={this.Channel}, Note={this.Note}, " + 
                $"Pressure={this.Pressure}>";
      }
   }
}