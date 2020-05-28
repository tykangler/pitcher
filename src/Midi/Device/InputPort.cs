using System;
using System.IO;
using Pitcher.Midi.Events;
using Pitcher.Midi.Interop;
using System.Runtime.InteropServices;

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
         if (devCapsCode != NativeInputOps.MessageResult.MMSYSERR_NOERROR) {
            throw new IOException(devCapsCode + " returned with device id " + deviceId);
         }
         this.DeviceId = deviceId;
         this.ProductName = caps.productName;
         this.ProductId = caps.productId;
         this.disposed = false;
      }

      public bool Open() {
         var eCode = NativeInputOps.midiInOpen(out this.handle, this.DeviceId, 
                                               this.handleMidiDeviceInput, UIntPtr.Zero, 
                                               NativeInputOps.CallbackFlag.CallbackFunction);
         return eCode == NativeInputOps.MessageResult.MMSYSERR_NOERROR;
      }

      public void Dispose() {
         this.Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected void Dispose(bool disposing) {
         if (!disposed) {
            this.disposed = true;
            if (disposing) {
               handle.Dispose();
            }
            // dispose unmanaged (no unmanaged)
         }
      }

      private void handleMidiDeviceInput(MidiInSafeHandle handleMidiIn, 
                                         NativeInputOps.MidiMessage message, 
                                         UIntPtr instance, 
                                         uint messageParam1, 
                                         uint messageParam2) {
         
      }
      
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