using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothData.Code
{
    public class PressureSensor:SensorBase
    {
        public PressureSensor(GattDeviceService dataService) : base(dataService, SensorUUIDs.UUID_ENV_PRES) { }

        public async Task<double> GetPressure()
        {
            byte[] data = await ReadValue();
            return ((double)(data[0] + 256 * data[1]+256*256*data[2]))/100;
        }
    }
}
