# Program Structure

## Process

1. Watch for devices and display all connected devices, adding/removing/changing devices on display
when devices are added/removed/changed<sub>1</sub>
2. Upon selection of device (either through text/id, or click)<sub>2</sub>, set **input port** 
to the selected input device<sub>3</sub>, and set **output port** to the selected 
output device.<sub>4</sub>
3. If instead of a device, a file is given, then parse the MIDI file and pass the data
to instantiate a **recording** object. 
4. Redirect MIDI events (either from **recording** or device) to a MIDI output device

## Structure

Overall Namespace: `Pitcher`

| System        | Description                             | Namespace         |
|---------------|-----------------------------------------|-------------------|
| UI            | Handles interface                       | `Pitcher.UI`      |
| MIDI Playback | Handles MIDI input and output           | `Pitcher.Midi`    |
| Devices       | Handles device enumeration and watching | `Pitcher.Devices` |

### MIDI Playback

APIs:
* `Device.Net` (**external**): enumerate HID devices and read and write to/from them. Can't find how to filter MIDI devices.
* `NAudio` (**external**): high-level audio programming API, also includes enumeration of MIDI devices

#### Files

MIDI input files can be converted to MIDI objects with a sequence of MIDI events.

#### Devices

MIDI input events must be converted to actions in real time. Can be recorded by converting to recording objects or sent directly to an output device.

Objects:
* `InPort`: Represents MIDI Input Device. Raises event when a message is read, with messages converted to MIDI objects as event arguments
* `OutPort`: Represents MIDI Output Device

#### Devices + Files

Objects:
* `Record`: Can read from a MIDI file and convert into an enumerable. MIDI events from 
input devices should operate on a `Record` if the user wants to record. 
Consists of `Track` and `Header` chunk objects.

#### Playback

Output to audio output devices. `Pitcher.Midi.OutPort` can connect to the system default synthesizer (GS Synthesizer for Microsoft), or
the user can select other output devices.

#### Flow

`Pitcher.Device.Watcher` raises event `DeviceAdded` when a new device is connected, and event `DeviceRemoved` when a device is disconnected. Takes in a list of filters at instantiation. Supports indexing to select a device, and enumeration over connected devices.

The selected input device and output device should be passed to `Pitcher.Midi.InPort` and `Pitcher.Midi.OutPort` respectively. These will implement `Device.Net.IDevice`. `InPort` will convert read messages into MIDI objects and raise an event with these objects as arguments. 

Event handlers should be created for these objects which will either record into a `Pitcher.Midi.Record`, or output to `Pitcher.Midi.Outport`. Conditionally add and remove these event handlers depending on user selection. 