using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothData.Code
{
    public class AccelerationSensor:SensorBase
    {
        public AccelerationSensor(GattDeviceService service) : base(service, SensorUUIDs.UUID_ACC_DATA) { }

        public async Task<double[]> GetAcceleration()
        {
            double[] data = new double[3];
            var byteData=await base.ReadValue();

            data[0] = BitConverter.ToInt16(byteData,0);
            data[1] = BitConverter.ToInt16(byteData, 2);
            data[2] = BitConverter.ToInt16(byteData, 4);

            return data;
        }
    }
}
