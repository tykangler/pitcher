namespace Pitcher.Midi.Events {
   
   /// <summary>
   /// A general midi event
   /// </summary>
   public abstract class MidiEvent {

      byte channel;

      /// <summary> 
      /// midi event in a raw message format that can be sent and received.
      /// </summary>
      public abstract uint RawMessage { get; }
      
      /// <summary>
      /// channel associated with the event
      /// </summary>
      public byte Channel { 
         get => channel; 
         protected set {
            if (value > 15 || value < 0) {
               throw new System.ArgumentException("channel must be in range [0, 16)");
            }
            channel = value;
         }
      }
      
   }

}