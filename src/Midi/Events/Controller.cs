namespace Pitcher.Midi.Events {
   public class Controller : IMidiEvent {
      public byte Channel { get; }
      public byte ControlDevice { get; }
      public byte Which { get; }

      public Controller(byte channel, byte controlDevice, byte which) {
         this.Channel = channel;
         this.ControlDevice = controlDevice;
         this.Which = which;
      }

      public override string ToString() {
         return $"Controller<Channel={this.Channel}, " + 
                $"ControlDevice={this.ControlDevice}, Which={this.Which}>";
      }
   }
}