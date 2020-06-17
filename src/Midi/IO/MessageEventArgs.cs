using System;
using Pitcher.Midi.Events;

namespace Pitcher.Midi.IO {

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