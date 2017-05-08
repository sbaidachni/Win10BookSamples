using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chapter26_BluetoothAdvertisement
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        //comment this code to activate the receiver
        BluetoothLEAdvertisementPublisher publisher;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var manufacturerData = 
                new BluetoothLEManufacturerData();

            var writer = new DataWriter();
            writer.WriteString("Buy our socks for a dollar");
            manufacturerData.CompanyId = 0xFFFE;
            manufacturerData.Data = writer.DetachBuffer();

            publisher =
                new BluetoothLEAdvertisementPublisher();

            publisher.Advertisement.ManufacturerData.Add(manufacturerData);
            publisher.StatusChanged += Publisher_StatusChanged;

            publisher.Start();

            base.OnNavigatedTo(e);
        }

        private void Publisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            Debug.WriteLine(args.Status.ToString());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            publisher.StatusChanged -= Publisher_StatusChanged;
            publisher.Stop();
            base.OnNavigatedFrom(e);
        }


        //uncomment all code below to read data from the publisher
        /*private BluetoothLEAdvertisementWatcher watcher;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            watcher = new BluetoothLEAdvertisementWatcher();

            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;

            var manufacturerData =
                new BluetoothLEManufacturerData();

            manufacturerData.CompanyId = 0xFFFE;
            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);

            watcher.Stopped += Watcher_Stopped;
            watcher.Received += Watcher_Received;
            watcher.Start();

            base.OnNavigatedTo(e);
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {        }

        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            Debug.WriteLine(args.Advertisement.LocalName);
            Debug.Write(args.RawSignalStrengthInDBm);
           
            if ((args.Advertisement.ManufacturerData.Count>0))
            {
                DataReader data = DataReader.FromBuffer(args.Advertisement.ManufacturerData[0].Data);
                Debug.Write(data.ReadString(args.Advertisement.ManufacturerData[0].Data.Length));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            watcher.Stop();
            watcher.Stopped -= Watcher_Stopped;
            watcher.Received -= Watcher_Received;
            base.OnNavigatedFrom(e);
        }*/
    }
}
