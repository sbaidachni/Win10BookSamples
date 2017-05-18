using Chapter26_BluetoothGattService.Code;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
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

namespace Chapter26_BluetoothGattService
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

        GattServiceProvider accService;
        GattServiceProvider envService;
        GattLocalCharacteristic accData;
        GattLocalCharacteristic tempData;
        GattLocalCharacteristic pressData;

        DispatcherTimer timer;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var adapter = await BluetoothAdapter.GetDefaultAsync();

            var serviceResult = 
                await GattServiceProvider.CreateAsync(Guid.Parse(SensorUUIDs.UUID_ACC_SERV));
            accService = serviceResult.ServiceProvider;
            accService.AdvertisementStatusChanged += AccService_AdvertisementStatusChanged;

            var param = new GattLocalCharacteristicParameters();
            param.CharacteristicProperties =  
                GattCharacteristicProperties.Indicate | 
                GattCharacteristicProperties.Read;

            param.WriteProtectionLevel = GattProtectionLevel.Plain;

            param.UserDescription = "accelerometer";

            var charResult=
                await accService.Service.CreateCharacteristicAsync(Guid.Parse(SensorUUIDs.UUID_ACC_DATA),param);
            accData = charResult.Characteristic;
            accData.ReadRequested += AccData_ReadRequested;
            accService.StartAdvertising(new GattServiceProviderAdvertisingParameters() { IsDiscoverable = true, IsConnectable = true });

            serviceResult =
                await GattServiceProvider.CreateAsync(Guid.Parse(SensorUUIDs.UUID_ENV_SERV));
            envService = serviceResult.ServiceProvider;

            param = new GattLocalCharacteristicParameters();
            param.CharacteristicProperties = 
                GattCharacteristicProperties.Indicate |
                GattCharacteristicProperties.Read;

            param.UserDescription = "temperature";

            charResult =
                await envService.Service.CreateCharacteristicAsync(Guid.Parse(SensorUUIDs.UUID_ENV_TEMP), param);
            tempData = charResult.Characteristic;
            tempData.ReadRequested += TempData_ReadRequested;

            param = new GattLocalCharacteristicParameters();
            param.CharacteristicProperties = 
                GattCharacteristicProperties.Indicate |
                GattCharacteristicProperties.Read;

            param.UserDescription = "pressure";

            charResult =
                await envService.Service.CreateCharacteristicAsync(Guid.Parse(SensorUUIDs.UUID_ENV_PRES), param);
            pressData = charResult.Characteristic;
            pressData.ReadRequested += PressData_ReadRequested;
            envService.StartAdvertising(new GattServiceProviderAdvertisingParameters() { IsDiscoverable = true, IsConnectable = true });

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            //timer.Start();

            base.OnNavigatedTo(e);
        }

        private async void PressData_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();
            var writer = new DataWriter();
            writer.WriteBytes(new byte[3] { 0x12, 0x12, 0x12});
            request.RespondWithValue(writer.DetachBuffer());
            deferral.Complete();
        }

        private async void TempData_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var request = await args.GetRequestAsync();
            var writer = new DataWriter();
            writer.WriteBytes(new byte[2] { 0x12, 0x12 });
            request.RespondWithValue(writer.DetachBuffer());
            deferral.Complete();
        }

        private void AccService_AdvertisementStatusChanged(GattServiceProvider sender, GattServiceProviderAdvertisementStatusChangedEventArgs args)
        {

            Debug.WriteLine(args.Status);
        }

        private async void Timer_Tick(object sender, object e)
        {
            var writer = new DataWriter();
            writer.WriteBytes(new byte[6] { 0x12, 0x12, 0x12, 0x12, 0x12, 0x12 });
            await accData.NotifyValueAsync(writer.DetachBuffer());
        }

        private async void AccData_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral=args.GetDeferral();
            var request = await args.GetRequestAsync();
            var writer = new DataWriter();
            writer.WriteBytes(new byte[6] { 0x12, 0x12, 0x12, 0x12, 0x12, 0x12 });
            request.RespondWithValue(writer.DetachBuffer());
            deferral.Complete();
        }
    }
}
