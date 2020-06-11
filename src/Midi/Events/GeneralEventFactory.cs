namespace Pitcher.Midi.Events {

   /// <summary>
   /// Creates general midi events. NOT USED ANYMORE.
   /// </summary>
   /// <remarks>
   /// In the future, this will be one of 2 factories. The other one being <c>MetaEventFactory</c>
   /// </remarks>
   public class GeneralEventFactory {
      readonly System.Func<uint, MidiEvent>[] generalEvents = 
         { m => new NoteOff(m), m => new NoteOn(m),
           m => new PolyphonicPressure(m), m => new Controller(m),
           m => new ProgramChange(m), m => new ChannelPressure(m),
           m => new PitchBend(m) };

      /// <summary>
      /// Creates and returns a new MidiEvent object corresponding to the passed MidiStatus
      /// </summary>
      /// <param name="statusCode">MidiStatus enum value designating the event</param>
      /// <param name="message">The message to be passed into an event</param>
      /// <returns>MidiEvent</returns>
      public MidiEvent? Create(MidiStatus statusCode, uint message) {
         if (!System.Enum.IsDefined(typeof(MidiStatus), statusCode)) {
            return null;
         }
         return generalEvents[(int) statusCode + 8](message);
      }
   }
}