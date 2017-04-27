using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothData
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

        DeviceWatcher deviceWatcher;
        ObservableCollection<DeviceInformation> deviceList = new ObservableCollection<DeviceInformation>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = deviceList;

            deviceWatcher = DeviceInformation.CreateWatcher(
                "System.ItemNameDisplay:~~\"BlueNRG\"",
                new string[] {
                    "System.Devices.Aep.DeviceAddress",
                    "System.Devices.Aep.IsConnected" },
                DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            deviceWatcher.Stop();
            base.OnNavigatedFrom(e);
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var toRemove = (from a in deviceList where a.Id == args.Id select a).FirstOrDefault();
            if (toRemove != null)
                await this.Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal, 
                    () => { deviceList.Remove(toRemove); });
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await this.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, 
                () => { deviceList.Add(args); });
            
            /*var device = await BluetoothLEDevice.FromIdAsync(args.Id);

            var services=await device.GetGattServicesAsync();
            
            foreach(var service in services.Services)
            {
                Debug.WriteLine($"Service: {service.Uuid}");

                var characteristics=await service.GetCharacteristicsAsync();

                foreach (var character in characteristics.Characteristics)
                {
                    Debug.WriteLine($"Characteristic: {character.Uuid}");                       
                }
            }*/
        }

        private void deviceListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(DevicePage), e.ClickedItem);
        }
    }
}
