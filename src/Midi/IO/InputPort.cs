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
namespace Pitcher.Midi.IO {

   public class InputPort : IDisposable {
      
      public class DeviceInformation {
         public uint DeviceId { get; }
         public string ProductName { get; }
         public ushort ProductId { get; }
         public uint Version { get; }

         public DeviceInformation(uint deviceId) {
            uint capsSize = NativeInputOps.midiInCapsSize;
            var devCapsCode = NativeInputOps.midiInGetDevCaps(deviceId, 
                                                              out NativeInputOps.MidiInCaps caps, 
                                                              capsSize);
            if (IsError(devCapsCode)) {
               throw new IOException(devCapsCode + " returned with device id " + deviceId);
            }
            this.DeviceId = deviceId;
            this.ProductName = caps.productName;
            this.ProductId = caps.productId;
         }
      }

      // fields
      bool disposed;
      MidiInSafeHandle handle;
      /* necessary because only passing handleMidiDeviceInput into midiInOpen
       * causes an error. The delegate is garbage collected because it is passed into an
       * unmanaged function and is not managed.
       * Full Doc from MSDOC:
       * The delegate from which the function pointer was created and exposed to 
       * unmanaged code was garbage collected. When the unmanaged component tries to 
       * call on the function pointer, it generates an access violation.
       * https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/callbackoncollecteddelegate-mda 
       */ 
      NativeInputOps.MidiInProc midiInProc;

      // properties
      public DeviceInformation Device { get; }
      // events
      public event EventHandler<MessageEventArgs> MessageReceived;

      protected virtual void OnMessageReceived(MessageEventArgs e) {
         MessageReceived?.Invoke(this, e);
      }

      public InputPort(uint deviceId) {
         // get device information
         this.midiInProc += handleMidiDeviceInput;
         this.disposed = false;
         var openCode = NativeInputOps.midiInOpen(out this.handle, this.DeviceId, 
                                                  this.midiInProc, UIntPtr.Zero, 
                                                  NativeInputOps.CallbackFlag.CallbackFunction);
         if (IsError(openCode)) {
            throw new IOException($"{openCode} returned with device id {this.DeviceId}");
         }
         // this.handle = new MidiInSafeHandle(ptrHandle);
      }

      public void Start() {
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
               this.midiInProc = null;
               handle?.Dispose();
            }
            // dispose unmanaged (no unmanaged)
         }
      }

      static bool IsError(MessageResult code) => code != MessageResult.MMSYSERR_NOERROR;

      void handleMidiDeviceInput(IntPtr handleMidiIn, 
                                 MidiMessage messageType, 
                                 UIntPtr instance, 
                                 uint message, 
                                 uint timestamp) {
         switch (messageType) {
            case MidiMessage.Open: // params reserved
            case MidiMessage.Close: // params reserved
               break;
            case MidiMessage.Data: // message received
               // little endian correct
               byte[] data = BitConverter.GetBytes(message);
               OnMessageReceived(new MessageEventArgs(data, timestamp));
               break;
            case MidiMessage.LongData: // ptr to midihdr struct (input buffer)
               byte[] longData = BitConverter.GetBytes(message);
               OnMessageReceived(new MessageEventArgs(longData, timestamp));
               break;
            case MidiMessage.MoreData: // message received
               // midi_io_status flag must be used in midiinopen
               byte[] moreData = BitConverter.GetBytes(message);
               OnMessageReceived(new MessageEventArgs(moreData, timestamp));
               break;
            case MidiMessage.Error: // invalid midi message
               byte[] errorData = BitConverter.GetBytes(message);
               OnMessageReceived(new MessageEventArgs(errorData, timestamp));
               break;
            case MidiMessage.LongError:
               // pointer to midihdr struct (input buffer with invalid message)
               byte[] longErrorData = BitConverter.GetBytes(message);
               OnMessageReceived(new MessageEventArgs(longErrorData, timestamp));
               break;
            default:
               OnMessageReceived(new MessageEventArgs());
               break;
         }
      }
      
   }

   public class MessageEventArgs : EventArgs {
      
      const byte hexDigitBits = 4;
      const byte botBits = 0X0F;

      public IMidiEvent Event { get; }
      public byte[] Message { get; }
      public uint TimeStamp { get; }

      public MessageEventArgs() {
         Event = null;
         Message = null;
         TimeStamp = 0;
      }

      public MessageEventArgs(byte[] midiMessage, uint timeStamp) {
         if (midiMessage == null) {
            throw new IOException("midiMessage can't be null. Use default ctor");
         }
         this.Message = midiMessage;
         MidiStatus status = (MidiStatus) (midiMessage[0] >> hexDigitBits);
         byte channel = (byte) (midiMessage[0] & botBits);
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
               Event = null;
               break;
         }
         this.TimeStamp = timeStamp;
      }
   }

}