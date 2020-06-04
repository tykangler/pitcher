using System;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Pitcher.Midi.IO;
using Pitcher.Midi.Events;

namespace Pitcher.Test.Midi.IO {

    public class InputPortTest {

        readonly ITestOutputHelper output;

        public InputPortTest(ITestOutputHelper output) => this.output = output;

        void PrintMessageData(MessageEventArgs m) {
            output.WriteLine(m.Event?.ToString());
            output.WriteLine($"\tRaw Message: {string.Join(", ", m.Message)}");
            output.WriteLine($"\tTimeStamp: {m.TimeStamp / 1000.0}");
        }

        // void PrintNoteMessageData(MessageEventArgs m) {
        //     NoteEvent noteEvent = m.Event as NoteEvent;
        //     output.WriteLine($"Type: Note {noteEvent.Type}");
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"Note: {noteEvent.Note}");
        //     output.WriteLine($"Velocity: {noteEvent.Velocity}");
        // }

        // void PrintProgramChangeMessageData(MessageEventArgs m) {
        //     ProgramChange programChange = m.Event as ProgramChange;
        //     output.WriteLine("Type: ProgramChange")
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"Program: {programChange.Program}");
        // }

        // void PrintChannelPressureMessageData(MessageEventArgs m) {
        //     ChannelPressure channelPressure = m.Event as ChannelPressure;
        //     output.WriteLine("Type: ChannelPressure");
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"Pressure: {channelPressure.Pressure}");
        // }

        // void PrintControllerMessageData(MessageEventArgs m) {
        //     Controller controller = m.Event as Controller;
        //     output.WriteLine("Type: Controller");
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"ControlDevice: {controller.Device}");
        //     output.WriteLine($"Which: {controller.Which}");
        // }
        
        // void PrintPitchBendMessageData(MessageEventArgs m) {
        //     PitchBend pitchBend = m.Event as PitchBend;
        //     output.WriteLine("Type: PitchBend");
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"Bend: {pitchBend.Bend}");
        // }

        // void PrintPolyphonicPressureMessageData(MessageEventArgs m) {
        //     PolyphonicPressure polyphonicPressure = m.Event as PolyphonicPressure;
        //     output.WriteLine("Type: PolyphonicPressure");
        //     PrintBasicMessageData(m);
        //     output.WriteLine($"Note: {polyphonicPressure.Note}");
        //     output.WriteLine($"Pressure: {polyphonicPressure.Pressure}");
        // }


        [Fact]
        /// <remark>require that at least one device is connected</remark>
        public void ConstructFillsProperties() {
            InputPort inputPort = new InputPort(0u);
            Assert.True(inputPort.DeviceId == 0u);
            Assert.True(inputPort.ProductName.Length != 0);
            output.WriteLine("Product Name: " + inputPort.ProductName);
            output.WriteLine("Product ID: " + inputPort.ProductId);
        }

        [Fact]
        public void StartHandlesSingleNoteEvent() {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            void handleMessageReceived(object _, MessageEventArgs m) {
                Assert.IsType<NoteEvent>(m.Event);
                PrintMessageData(m);
                resetEvent.Set();
            }
            using (InputPort inputPort = new InputPort(0u)) {
                inputPort.MessageReceived += handleMessageReceived;
                inputPort.Start();
                resetEvent.WaitOne();
            }
        }

        [Fact]
        public void StartHandlesMultipleNoteEvents() {
            CountdownEvent countdown = new CountdownEvent(5);
            void handleMessageReceived(object _, MessageEventArgs m) {
                Assert.IsType<NoteEvent>(m.Event);
                PrintMessageData(m);
                countdown.Signal();
            }
            using (InputPort inputPort = new InputPort(0u)) {
                inputPort.MessageReceived += handleMessageReceived;
                inputPort.Start();
                countdown.Wait();
            }
        }
    }
}
