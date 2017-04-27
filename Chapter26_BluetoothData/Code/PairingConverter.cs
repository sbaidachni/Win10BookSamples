using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Data;

namespace BluetoothData.Code
{
    public class PairingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pairing = value as DeviceInformationPairing;
            string res = "False";
            if (pairing.IsPaired) res = "Already paired";
            else if (pairing.CanPair) res = "True";
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
