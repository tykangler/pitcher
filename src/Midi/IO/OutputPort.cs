using System;
using System.Collections.Generic;
using System.IO;
using Pitcher.Midi.Events;
using Pitcher.Midi.Interop;

namespace Pitcher.Midi.IO {
   public class OutputPort : IDisposable {

      public class DeviceInformation {
         public uint DeviceId { get; }
         public string ProductName { get; }
         public ushort ProductId { get; }
         public uint Version { get; }
         public string Technology { get; }
         public ushort NumVoices { get; }
         public ushort NumSimultaneousNotes { get; }
         public IList<int> SupportedChannels { get; }
         public OptSupport SupportedFunctionality { get; }

         public DeviceInformation(uint deviceId) {
            uint capsSize = NativeOutputOps.midiOutCapsSize;
            var devCapsCode = NativeOutputOps.midiOutGetDevCaps(
               deviceId, out NativeOutputOps.MidiOutCaps caps, capsSize);
            if (IsError(devCapsCode)) {
               throw new IOException($"{devCapsCode} returned with device id {deviceId}");
            }
            this.DeviceId = deviceId;
            this.ProductName = caps.productName;
            this.ProductId = caps.productId;
            this.Version = caps.version;
            this.Technology = caps.technology.ToString();
            this.NumVoices = caps.voices;
            this.NumSimultaneousNotes = caps.notes;
            this.SupportedChannels = ExtractChannels(caps.channelMask);
            this.SupportedFunctionality = caps.optSupport;
         }

         static IList<int> ExtractChannels(ushort channelMask) {
            IList<int> channels = new List<int>();
            for (int i = 0; i < 16; ++i) {
               if (((channelMask >> i) & 1) == 1) {
                  channels.Add(i);
               }
            }
            return channels;
         }
      }

      bool disposed;
      MidiOutSafeHandle handle;
      public DeviceInformation Device { get; }

      public OutputPort(uint deviceId) {
         this.Device = new DeviceInformation(deviceId);
         this.disposed = false;
         var openCode = NativeOutputOps.midiOutOpen(
            out this.handle, this.Device.DeviceId, null, UIntPtr.Zero, CallbackFlag.CallbackNull);
         if (IsError(openCode)) {
            throw new IOException($"{openCode} thrown with device id {this.Device.DeviceId}");
         }
      }

      public bool SendShortMessage(MidiEvent midiEvent) {
         var sendCode = NativeOutputOps.midiOutShortMessage(this.handle, midiEvent.RawMessage);
         return !IsError(sendCode);
      }

      static bool IsError(MessageResult code) => code != MessageResult.MMSYSERR_NOERROR;
   
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
   }
}