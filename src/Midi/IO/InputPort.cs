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
      public event EventHandler<MessageEventArgs>? MessageReceived;
      protected virtual void OnMessageReceived(MessageEventArgs e) {
         MessageReceived?.Invoke(this, e);
      }

      public InputPort(uint deviceId) {
         // get device information
         this.Device = new DeviceInformation(deviceId);
         this.midiInProc += handleMidiDeviceInput;
         this.disposed = false;
         var openCode = NativeInputOps.midiInOpen(out this.handle, this.Device.DeviceId, 
                                                  this.midiInProc, UIntPtr.Zero, 
                                                  CallbackFlag.CallbackFunction);
         if (IsError(openCode)) {
            throw new IOException($"{openCode} returned with device id {this.Device.DeviceId}");
         }
      }

      public void Start() {
         var startCode = NativeInputOps.midiInStart(this.handle);
         if (IsError(startCode)) {
            throw new IOException($"{startCode} returned with device id {this.Device.DeviceId}");
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
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
            case MidiMessage.LongData: // ptr to midihdr struct (input buffer)
               // will later send an alternate event args
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
            case MidiMessage.MoreData: // message received
               // midi_io_status flag must be used in midiinopen
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
            case MidiMessage.Error: // invalid midi message
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
            case MidiMessage.LongError:
               // pointer to midihdr struct (input buffer with invalid message)
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
            default:
               OnMessageReceived(new MessageEventArgs(message, timestamp));
               break;
         }
      }
      
   }

   public class MessageEventArgs : EventArgs {
      
      const byte hexDigitBits = 4;
      const byte botBits = 0X0F;

      public MidiEvent? Event { get; }
      public byte[] Message { get; }
      public uint TimeStamp { get; }

      public MessageEventArgs(uint message, uint timeStamp) {
         this.Message = BitConverter.GetBytes(message);
         MidiStatus status = (MidiStatus) (this.Message[0] >> hexDigitBits);
         this.Event = status switch {
            MidiStatus.NoteOff => new NoteOff(message),
            MidiStatus.NoteOn => new NoteOn(message),
            MidiStatus.PolyphonicPressure => new PolyphonicPressure(message),
            MidiStatus.Controller => new Controller(message),
            MidiStatus.PitchBend => new PitchBend(message),
            MidiStatus.ProgramChange => new ProgramChange(message),
            MidiStatus.ChannelPressure => new ChannelPressure(message),
            _ => null
         };
         this.TimeStamp = timeStamp;
      }
   }

}