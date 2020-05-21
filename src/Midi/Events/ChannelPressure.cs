namespace Pitcher.Midi.Events {
   public class ChannelPressure : IMidiEvent {
      public byte Channel { get; }
      public byte Pressure { get; }

      public ChannelPressure(byte channel, byte pressure) {
         this.Channel = channel;
         this.Pressure = pressure;
      }
   }
}