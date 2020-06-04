using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Pitcher.Midi.IO;

namespace Pitcher.Test.Midi.IO {

   public class DeviceManagerTest {

      // readonly ITestOutputHelper output;

      // public DeviceManagerTest(ITestOutputHelper output) => this.output = output; 

      [Theory]
      [InlineData(1)]
      /// <param name="connectedDevices">
      /// number of connected devices. must be known before running
      /// </param>
      public void NumInputDevicesReturnsNumConnectedDevices(uint connectedDevices) {
         uint numInputDevices = DeviceManager.NumInputDevices;
         Assert.True(numInputDevices == connectedDevices);
      }

      [Fact]
      public void InputDevicesReturnsCorrectNumberDevices() {
         uint numInputDevices = DeviceManager.NumInputDevices;
         IEnumerable<InputPort> devices = DeviceManager.InputDevices;
         Assert.True(numInputDevices == devices.Count());
      }

   }

}