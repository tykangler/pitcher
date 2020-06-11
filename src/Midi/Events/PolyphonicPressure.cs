namespace Pitcher.Midi.Events {
   public class PolyphonicPressure : MidiEvent {
      public override uint RawMessage { get; }
      public byte Note { get; }
      public byte Pressure { get; }

      public PolyphonicPressure(byte channel, byte note, byte pressure) {
         this.Channel = channel;
         this.Note = note;
         this.Pressure = pressure;
         this.RawMessage = Pack(channel, note, pressure);
      }

      public PolyphonicPressure(uint raw) {
         this.RawMessage = raw;
         var (channel, note, pressure) = ParseMessage(raw);
         this.Channel = channel;
         this.Note = note;
         this.Pressure = pressure;
      }

      (byte, byte, byte) ParseMessage(uint raw) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1], rawBytes[2]);
      }

      uint Pack(int channel, int note, int pressure) {
         int statusByte = (((byte) MidiStatus.PolyphonicPressure) << 2) | channel;
         int noteByte = note << 8;
         int pressureByte = pressure << 16;
         return (uint) (pressureByte | noteByte | statusByte);
      }

      public override string ToString() {
         return $"PolyphonicPressure<Channel={this.Channel}, Note={this.Note}, " + 
                $"Pressure={this.Pressure}>";
      }
   }
}