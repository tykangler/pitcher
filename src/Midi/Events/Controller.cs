namespace Pitcher.Midi.Events {
   public class Controller : MidiEvent {
      public override uint RawMessage { get; }
      public byte ControlDevice { get; }
      public byte Which { get; }

      public Controller(byte channel, byte controlDevice, byte which) {
         this.Channel = channel;
         this.ControlDevice = controlDevice;
         this.Which = which;
         this.RawMessage = Pack(channel, controlDevice, which);
      }

      public Controller(uint raw) {
         this.RawMessage = raw;
         var (channel, controller, which) = ParseMessage(raw);
         this.Channel = channel;
         this.ControlDevice = controller;
         this.Which = which;
      }

      (byte, byte, byte) ParseMessage(uint raw) {
         byte[] rawBytes = System.BitConverter.GetBytes(raw);
         return (rawBytes[0], rawBytes[1], rawBytes[2]);
      } 

      uint Pack(int channel, int controlDevice, int which) {
         int statusByte = (((byte) MidiStatus.Controller) << 2) | channel;
         int controllerByte = controlDevice << 8;
         int whichByte = which << 16;
         return (uint) (whichByte | controllerByte | statusByte);
      }

      public override string ToString() {
         return $"Controller<Channel={this.Channel}, " + 
                $"ControlDevice={this.ControlDevice}, Which={this.Which}>";
      }
   }
}