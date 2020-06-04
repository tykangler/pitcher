namespace Pitcher.Midi.Events {
   public class PolyphonicPressure : IMidiEvent {
      public byte Channel { get; }
      public byte Note { get; }
      public byte Pressure { get; }

      public PolyphonicPressure(byte channel, byte note, byte pressure) {
         this.Channel = channel;
         this.Note = note;
         this.Pressure = pressure;
      }

      public override string ToString() {
         return $"PolyphonicPressure<Channel={this.Channel}, Note={this.Note}, " + 
                $"Pressure={this.Pressure}>";
      }
   }
}