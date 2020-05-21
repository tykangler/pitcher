namespace Pitcher.Midi.Events {
   public class Controller : IMidiEvent {
      public byte Channel { get; }
      public byte Device { get; }
      public byte Which { get; }

      public Controller(byte channel, byte device, byte which) {
         this.Channel = channel;
         this.Device = device;
         this.Which = which;
      }
   }
}