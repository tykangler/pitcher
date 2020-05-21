using Device.Net;
using System;
using System.Threading.Tasks;
using Pitcher.Midi.Events;

namespace Pitcher.Midi {

   public class InPort {

      IDevice inputDevice;

      public event EventHandler<MessageEventArgs> MessageReceived;

      protected virtual void OnMessageReceived(MessageEventArgs e) {
         MessageReceived?.Invoke(this, e);
      }

      public static async Task<InPort> CreateInPort(IDevice inputDevice) {
         InPort inPort = new InPort(inputDevice);
         await inPort.inputDevice.InitializeAsync();
         return inPort;
      }

      private InPort(IDevice inputDevice) => this.inputDevice = inputDevice;

      
   }

   public class MessageEventArgs : EventArgs {
      
      const byte hexDigitBits = 4;
      const byte topBits = 0XF0;
      public MidiStatus EventStatus { get; }
      IMidiEvent Event { get; }

      (MidiStatus, byte) SplitStatusByte(byte val) {
         return ((MidiStatus) (val >> hexDigitBits), (byte) (val & topBits));
      }

      public MessageEventArgs(byte[] midiMessage) {
         var (status, channel) = SplitStatusByte(midiMessage[0]);
         switch (status) {
            case MidiStatus.NoteOff:
               Event = new NoteEvent(channel, midiMessage[1], midiMessage[2], 
                                     NoteEvent.Sound.Off);
               break;
            case MidiStatus.NoteOn:
               Event = new NoteEvent(channel, midiMessage[1], midiMessage[2], 
                                     NoteEvent.Sound.On);
               break;
            case MidiStatus.ChannelPressure:
               Event = new ChannelPressure(channel, midiMessage[1]);
               break;
            case MidiStatus.Controller:
               Event = new Controller(channel, midiMessage[1], midiMessage[2]);
               break;
            case MidiStatus.PitchBend:
               Event = new PitchBend(channel, midiMessage[1]);
               break;
            case MidiStatus.PolyphonicPressure:
               Event = new PolyphonicPressure(channel, midiMessage[1], midiMessage[2]);
               break;
            case MidiStatus.ProgramChange:
               Event = new ProgramChange(channel, midiMessage[1]);
               break;
            default:
               break;
         }
      }
   }
}