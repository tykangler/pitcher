namespace Pitcher.Midi.Events {
   public class ChannelPressure : IMidiEvent {
      public MidiStatus Status { get => MidiStatus.ChannelPressure; }
      public byte Channel { get; }
      public byte Pressure { get; }

      public ChannelPressure(byte channel, byte pressure) {
         this.Channel = channel;
         this.Pressure = pressure;
      }

      public ChannelPressure(byte[] rawMessage) {
         
      }

      public uint Pack() {
         int statusByte = (((byte) Status) << 2) | Channel;
         int pressureByte = Pressure << 8;
         return (uint) (pressureByte | statusByte);
      }

      public override string ToString() {
         return $"ChannelPressure<Channel={this.Channel}, Pressure={this.Pressure}>";
      }
   }
}