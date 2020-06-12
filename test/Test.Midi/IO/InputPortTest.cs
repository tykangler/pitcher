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

        [Fact]
        /// <remark>require that at least one device is connected</remark>
        public void ConstructFillsProperties() {
            InputPort inputPort = new InputPort(0u);
            Assert.True(inputPort.Device.DeviceId == 0u);
            Assert.True(inputPort.Device.ProductName.Length != 0);
            output.WriteLine("Product Name: " + inputPort.Device.ProductName);
            output.WriteLine("Product ID: " + inputPort.Device.ProductId);
        }

        [Fact]
        public void StartHandlesSingleNoteEvent() {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            void handleMessageReceived(object _, MessageEventArgs m) {
                Assert.IsAssignableFrom<NoteEvent>(m.Event);
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
                Assert.IsAssignableFrom<NoteEvent>(m.Event);
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
