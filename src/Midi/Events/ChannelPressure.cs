namespace Pitcher.Midi.Events {
   public class ChannelPressure : MidiEvent {

      public override uint RawMessage { get; }
      public byte Pressure { get; }

      public ChannelPressure(byte channel, byte pressure) {
         this.Channel = channel;
         this.Pressure = pressure;
         this.RawMessage = Pack(channel, pressure);
      }

      public ChannelPressure(uint raw) {
         this.RawMessage = raw;
         var (channel, pressure) = ParseMessage(raw);
         this.Channel = channel;
         this.Pressure = pressure;
      }

      (byte, byte) ParseMessage(uint raw) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1]);
      }

      uint Pack(int channel, int pressure) {
         int statusByte = (((byte) MidiStatus.ChannelPressure) << 2) | channel;
         int pressureByte = pressure << 8;
         return (uint) (pressureByte | statusByte);
      }

      public override string ToString() {
         return $"ChannelPressure<Channel={this.Channel}, Pressure={this.Pressure}>";
      }
   }
}