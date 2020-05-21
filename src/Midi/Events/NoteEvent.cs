namespace Pitcher.Midi.Events {

   public class NoteEvent : IMidiEvent {

      public enum Sound { On, Off }
      
      public byte Note { get; }
      public byte Velocity { get; }
      public byte Channel { get; }
      public Sound Type { get; }
      
      public NoteEvent(byte channel, byte note, byte velocity, Sound type) {
         this.Channel = channel;
         this.Note = note;
         this.Velocity = velocity;
         this.Type = type;
      }
   }
}