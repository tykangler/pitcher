# Roadmap

## Current 

- Input/Output Functionality finished
- Separated device opening and device information retrieval
- 

### TODO

*Readability*
- [ ] Separate NoteEvent into NoteOff and NoteOn
- [ ] Create constructors for all midi events taking in `byte[] message` parameter
- [ ] Make `uint Pitcher.Midi.Events.IMidiEvent.Pack()` a property with backing field
- [ ] Populate `Pitcher.Midi.IO.InputPort.MessageEventArgs.eventMap`

## Future

Move items into [*Current*](#Current) if implementing.

- Main
- Console vs GUI (using ASP/Blazor/Electron or WPF)
- Sending signals not supported by device (central ui for message sending)
- Audio Transformer