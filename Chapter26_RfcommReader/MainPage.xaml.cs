using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chapter26_RfcommReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            App.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            StopStream();
        }

        DeviceWatcher deviceWatcher;
        DataWriter tx;
        DataReader rx;
        StreamSocket stream;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceWatcher = DeviceInformation.CreateWatcher(
                "System.ItemNameDisplay:~~\"rfcommhc\"",
                null,
                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Start();

            base.OnNavigatedTo(e);
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var device = await BluetoothDevice.FromIdAsync(args.Id);         

            var services = await device.GetRfcommServicesAsync();
            if (services.Services.Count>0)
            {
                var service = services.Services[0];

                stream = new StreamSocket();
                await stream.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName);

                rx = new DataReader(stream.InputStream);
                tx = new DataWriter(stream.OutputStream);

                await this.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal, 
                    () => { switchLed.IsEnabled = true; });

                deviceWatcher.Stop();
            }

        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            byte data = 2;
            if (switchLed.IsOn)
            {
                data = 3;
            }

            tx.WriteByte(data);
            await tx.StoreAsync();

            uint buf;

            buf = await rx.LoadAsync(1);

            var symbol = rx.ReadByte();

            Debug.WriteLine(data);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopStream();
            base.OnNavigatedFrom(e);
        }

        private void StopStream()
        {
            deviceWatcher.Stop();
            if (stream != null)
            {
                tx.Dispose();
                rx.Dispose();
                stream.Dispose();
            }
        }
    }
}
