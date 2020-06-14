# Roadmap

## Current 

- Input/Output Functionality finished
- Separated device opening and device information retrieval
- Interop Functionality

### TODO

*Readability*
- [x] Separate NoteEvent into NoteOff and NoteOn
- [x] Create constructors for all midi events taking in `byte[] message` parameter
- [x] Make `uint Pitcher.Midi.Events.IMidiEvent.Pack()` a property with backing field

*Structure*
- [ ] Restructure classes in `Events` namespace. Most should be fine, but anything dealing with
      notes should be refactored, and may lead to additional refactoring. Look into `NoteEvent`,
      `NoteOff`, `NoteOn`, `PolyphonicPressure`. Possibly create additional helper class for
      dealing with note operations. Maybe a `Note` class 

## Future

Move items into [*Current*](#Current) if implementing.

- Main
- Console vs GUI (using ASP/Blazor/Electron or WPF)
- Sending signals not supported by device (central ui for message sending)
- Audio Transformer