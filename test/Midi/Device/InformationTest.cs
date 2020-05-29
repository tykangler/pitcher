using System;
using Xunit;
using Pitcher.Midi.Device;

namespace Pitcher.Test.Midi.Device {

   /// <remark>These tests require that at least one device is connected.</remark>
   public class InformationTest {

      [Fact]
      public void NumInputDevicesShouldReturnOne() {
         uint numInputDevices = Information.numInputDevices;
      }


   }

}