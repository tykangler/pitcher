using System;
using System.IO;
using Pitcher.Midi.Events;
using Pitcher.Midi.Interop;

/* 
Current Functionality: 
Construct a device with InputPort(), 
open the device and start reading with Start(),
when message is received, will print the message type and received messageParam
TODO:
Redirect to event so that handleMidiDeviceInput() calls OnMessageReceived with 
   MessageEventArgs (constructed from received message) */
namespace Pitcher.Midi.Device {

   public class InputPort : IDisposable {

      // fields
      bool disposed;
      MidiInSafeHandle handle;

      // properties
      public uint DeviceId { get; }
      public string ProductName { get; }
      public ushort ProductId { get; }

      // events
      public event EventHandler<MessageEventArgs> MessageReceived;

      protected virtual void OnMessageReceived(MessageEventArgs e) {
         MessageReceived?.Invoke(this, e);
      }

      public InputPort(uint deviceId) {
         // get device information
         uint capsSize = NativeInputOps.midiInCapsSize();
         var devCapsCode = NativeInputOps.midiInGetDevCaps(deviceId, 
                                                           out NativeInputOps.MidiInCaps caps, 
                                                           capsSize);
         if (IsError(devCapsCode)) {
            throw new IOException(devCapsCode + " returned with device id " + deviceId);
         }
         this.DeviceId = deviceId;
         this.ProductName = caps.productName;
         this.ProductId = caps.productId;
         this.disposed = false;
      }

      public void Start() {
         var openCode = NativeInputOps.midiInOpen(out IntPtr ptrHandle, this.DeviceId, 
                                                  this.handleMidiDeviceInput, UIntPtr.Zero, 
                                                  NativeInputOps.CallbackFlag.CallbackFunction);
         if (IsError(openCode)) {
            throw new IOException($"{openCode} returned with device id {this.DeviceId}");
         }
         this.handle = new MidiInSafeHandle(ptrHandle);
         var startCode = NativeInputOps.midiInStart(this.handle);
         if (IsError(startCode)) {
            throw new IOException($"{startCode} returned with device id {this.DeviceId}");
         }
      }

      public void Dispose() {
         this.Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected void Dispose(bool disposing) {
         if (!disposed) {
            this.disposed = true;
            if (disposing) {
               handle?.Dispose();
            }
            // dispose unmanaged (no unmanaged)
         }
      }

      bool IsError(NativeInputOps.MessageResult code) 
         => code != NativeInputOps.MessageResult.MMSYSERR_NOERROR;

      void handleMidiDeviceInput(MidiInSafeHandle handleMidiIn, 
                                 NativeInputOps.MidiMessage message, 
                                 UIntPtr instance, 
                                 uint messageParam1, 
                                 uint messageParam2) {
         // messageParam2 is timestamp except when reserved in open and close
         switch (message) {
            case NativeInputOps.MidiMessage.Open:
               // reserved
               Console.WriteLine("open ");
               break;
            case NativeInputOps.MidiMessage.Close:
               // reserved
               Console.WriteLine("close ");
               break;
            case NativeInputOps.MidiMessage.Data:
               // message received
               Console.WriteLine("data " + Convert.ToString(messageParam1, 16));
               break;
            case NativeInputOps.MidiMessage.LongData:
               // pointer to midihdr struct (input buffer)
               Console.WriteLine("longdata " + Convert.ToString(messageParam1, 16));
               break;
            case NativeInputOps.MidiMessage.MoreData:
               // midi_io_status flag must be used in midiinopen
               // message received
               Console.WriteLine("moredata " + Convert.ToString(messageParam1, 16));
               break;
            case NativeInputOps.MidiMessage.Error:
               // invalid midi message
               Console.WriteLine("error " + Convert.ToString(messageParam1, 16));
               break;
            case NativeInputOps.MidiMessage.LongError:
               // pointer to midihdr struct (input buffer with invalid message)
               Console.WriteLine("longerror " + Convert.ToString(messageParam1, 16));
               break;
            default:
               Console.WriteLine("no registered message type");
               break;
         }
      }
      
   }

   public class MessageEventArgs : EventArgs {
      
      const byte hexDigitBits = 4;
      const byte topBits = 0XF0;

      public MidiStatus EventStatus { get; }
      IMidiEvent Event { get; }

      (MidiStatus, byte) SplitStatusByte(byte val)
         => ((MidiStatus) (val >> hexDigitBits), (byte) (val & topBits));

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