using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace BluetoothData.Code
{
    public class SensorBase:IDisposable
    {
        protected GattDeviceService deviceService;
        protected string sensorDataUuid;
        protected byte[] data;
        protected bool isNotificationSupported = false;

        private GattCharacteristic dataCharacteristic;

        public SensorBase(GattDeviceService dataService, string sensorDataUuid)
        {
            this.deviceService = dataService;
            this.sensorDataUuid = sensorDataUuid;
        }

        public virtual async Task EnableNotifications()
        {
            isNotificationSupported = true;

            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];

            dataCharacteristic.ValueChanged += dataCharacteristic_ValueChanged;

            GattCommunicationStatus status =
                    await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);

        }

        public virtual async Task DisableNotifications()
        {
            isNotificationSupported = false;

            dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                new Guid(sensorDataUuid))).Characteristics[0];

            dataCharacteristic.ValueChanged -= dataCharacteristic_ValueChanged;

            GattCommunicationStatus status =
                await dataCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                GattClientCharacteristicConfigurationDescriptorValue.None);

        }

        protected async Task<byte[]> ReadValue()
        {
            if (!isNotificationSupported)
            {
                if (dataCharacteristic == null)
                    dataCharacteristic = (await deviceService.GetCharacteristicsForUuidAsync(
                        new Guid(sensorDataUuid))).Characteristics[0];

                GattReadResult readResult = await dataCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                data = new byte[readResult.Value.Length];

                DataReader.FromBuffer(readResult.Value).ReadBytes(data);
            }

            return data;
        }

        private void dataCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            data = new byte[args
                .CharacteristicValue.Length];

            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(data);
        }

        public async void Dispose()
        {
            if (isNotificationSupported) await DisableNotifications();
        }
    }
}
