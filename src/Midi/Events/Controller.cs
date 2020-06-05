namespace Pitcher.Midi.Events {
   public class Controller : IMidiEvent {
      public MidiStatus Status { get => MidiStatus.Controller; }
      public byte Channel { get; }
      public byte ControlDevice { get; }
      public byte Which { get; }

      public Controller(byte channel, byte controlDevice, byte which) {
         this.Channel = channel;
         this.ControlDevice = controlDevice;
         this.Which = which;
      }

      public uint Pack() {
         int statusByte = (((byte) Status) << 2) | Channel;
         int controllerByte = ControlDevice << 8;
         int whichByte = Which << 16;
         return (uint) (whichByte | controllerByte | statusByte);
      }

      public override string ToString() {
         return $"Controller<Channel={this.Channel}, " + 
                $"ControlDevice={this.ControlDevice}, Which={this.Which}>";
      }
   }
}