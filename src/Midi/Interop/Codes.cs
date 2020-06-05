using System;

namespace Pitcher.Midi.Interop {
      public enum MidiMessage : uint {
      Open = 0x3c1,
      Close,
      Data,
      LongData,
      Error,
      LongError,
      MoreData = 0x3cc
   }

   public enum CallbackFlag : uint {
      CallbackNull = 0,
      MidiIoStatus = 0x20,
      CallbackWindows = 0x10000,
      CallbackThread = 0x20000,
      CallbackFunction = 0x30000,
      CallbackEvent = 0x00050000
   }

   public enum MessageResult : uint {
      MMSYSERR_NOERROR = 0,
      MMSYSERR_BADDEVICEID = 2,
      MMSYSERR_ALLOCATED = 4,
      MMSYSERR_INVALHANDLE = 5,
      MMSYSERR_NODRIVER,
      MMSYSERR_NOMEM = 7,
      MMSYSERR_INVALFLAG = 0xa,
      MMSYSERR_INVALPARAM = 0xb,
      MIDIERR_STILLPLAYING = 0x29,
      MIDIERR_NOTREADY = 0X43,
      MIDIERR_NODEVICE = 0x44,
      MIDIERR_BADOPENMODE = 0x46
   }

   public enum Technology : ushort {
      MidiPort = 1,
      Synth,
      SqSynth,
      FmSynth,
      Mapper,
      WaveTable,
      SwSynth
   }

   [Flags]
   public enum OptSupport : uint {
      Volume = 1,
      LRVolume = 2,
      Cache = 4,
      Stream = 8
   }
}