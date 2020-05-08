using Device.Net;
using System;

namespace Pitcher.Device {
   
   /// <summary>watches for connected devices</summary>
   class Watcher {
      const int pollMilliseconds = 3000;

      public event EventHandler DeviceAdded;
      public event EventHandler DeviceRemoved;

      DeviceListener listener;
      
      public Watcher() {
         listener = new DeviceListener(null, pollMilliseconds);
      }
   
   }
}