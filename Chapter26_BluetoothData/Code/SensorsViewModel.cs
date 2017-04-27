using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;

namespace BluetoothData.Code
{
    public class SensorsViewModel : INotifyPropertyChanged, IDisposable
    {
        DispatcherTimer timer = new DispatcherTimer();
        DeviceInformation device;
        BluetoothLEDevice leDevice;

        public SensorsViewModel(DeviceInformation info)
        {
            this.device = info;
        }

        public async void StartReceivingData()
        {
            leDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
            string selector = "(System.DeviceInterface.Bluetooth.DeviceAddress:=\"" + leDevice.BluetoothAddress.ToString("X") + "\")";

            var services = await leDevice.GetGattServicesAsync();

            foreach (var service in services.Services)
            {
                switch (service.Uuid.ToString())
                {
                    case SensorUUIDs.UUID_ENV_SERV:
                        InitializeTemperatureSensor(service);
                        InitializePressureSensor(service);
                        break;
                    case SensorUUIDs.UUID_ACC_SERV:
                        InitializeAccelerationSensor(service);
                        break;
                }
            }

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            UpdateAllData();
        }

        public void StopReceivingData()
        {
            timer.Stop();
        }

        private TemperatureSensor tempSensor = null;

        protected void InitializeTemperatureSensor(GattDeviceService service)
        {
            tempSensor = new TemperatureSensor(service);
        }

        private string temperature="";

        public string Temperature
        {
            get
            {
                return temperature;
            }
        }

        private PressureSensor presSensor = null;

        protected void InitializePressureSensor(GattDeviceService service)
        {
            presSensor = new PressureSensor(service);
        }

        private string pressure = "";

        public string Pressure
        {
            get
            {
                return pressure;
            }
        }

        private AccelerationSensor accSensor = null;

        protected async void InitializeAccelerationSensor(GattDeviceService service)
        {
            accSensor = new AccelerationSensor(service);
            await accSensor.EnableNotifications();
        }

        private double angleX;
        private double angleY;
        private double angleZ;

        public double AngleX
        {
            get
            {
                return angleX;
            }
        }

        public double AngleY
        {
            get
            {
                return angleY;
            }
        }

        public double AngleZ
        {
            get
            {
                return angleZ;
            }
        }


        public async void UpdateAllData()
        {
            if (tempSensor != null)
            {
                temperature = String.Format($"{(await tempSensor.GetTemperature())} Celsius");
                if (PropertyChanged!=null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Temperature"));
            }
            if (presSensor != null)
            {
                pressure = String.Format($"{(await presSensor.GetPressure()).ToString()} milliBar");
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Pressure"));
            }
            if (accSensor != null)
            {
                var data = await accSensor.GetAcceleration();

                angleX = data[0];
                angleY = data[1];
                angleZ = data[2];
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AngleX"));
                    PropertyChanged(this, new PropertyChangedEventArgs("AngleY"));
                    PropertyChanged(this, new PropertyChangedEventArgs("AngleZ"));
                }
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            StopReceivingData();
            if (tempSensor != null) tempSensor.Dispose();
            if (presSensor != null) presSensor.Dispose();
            if (accSensor != null) accSensor.Dispose();
        }
    }
}
