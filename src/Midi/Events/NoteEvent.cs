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

      public override bool Equals(object other) {
         if (other is NoteEvent noteEvent) {
            return this.Note == noteEvent.Note &&
                   this.Channel == noteEvent.Channel &&
                   this.Velocity == noteEvent.Velocity &&
                   this.Type == noteEvent.Type;
         } else {
            return false;
         }
      }

      public override int GetHashCode() {
         unchecked {
            int hash = 17;
            hash = hash * 23 + Note.GetHashCode();
            hash = hash * 23 + Velocity.GetHashCode();
            hash = hash * 23 + Channel.GetHashCode();
            hash = hash * 23 + Type.GetHashCode();
            return hash;
         }
      }

      public override string ToString() {
         // return $"NoteEvent<Type={this.Type}, Channel={this.Channel}, " + 
         //        $"Note={this.Note}, Velocity={this.Velocity}>";
         return $"NoteEvent<Type={this.Type}, Channel={this.Channel}, " + 
                $"Note={this.Note}, Velocity={this.Velocity}>\n" + 
                new string(' ', this.Note) + '|'; 
      }
   }
}