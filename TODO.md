# Roadmap

## Current 

- Input/Output Functionality finished
- Separated device opening and device information retrieval
- Interop Functionality

### TODO

*Readability*
- [x] Separate NoteEvent into NoteOff and NoteOn
- [ ] Create constructors for all midi events taking in `byte[] message` parameter
- [ ] Make `uint Pitcher.Midi.Events.IMidiEvent.Pack()` a property with backing field
- [ ] Populate `Pitcher.Midi.IO.InputPort.MessageEventArgs.eventMap`

*Redundancy*
- [ ] Currently passing `uint rawMessage` to `MidiEvent` constructors based status code, 
      but `rawMessage` already contains status code. Find way to remove this redundancy.

## Future

Move items into [*Current*](#Current) if implementing.

- Main
- Console vs GUI (using ASP/Blazor/Electron or WPF)
- Sending signals not supported by device (central ui for message sending)
- Audio Transformer