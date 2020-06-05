namespace Pitcher.Midi.Events {
   
   public interface IMidiEvent {
      MidiStatus Status { get; }
      byte Channel { get; }

      uint Pack();
   }

}