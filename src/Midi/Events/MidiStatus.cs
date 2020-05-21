namespace Pitcher.Midi.Events {
   public enum MidiStatus : byte {
      NoteOff = 8,
      NoteOn,
      PolyphonicPressure,
      Controller,
      ProgramChange,
      ChannelPressure,
      PitchBend
   }
}