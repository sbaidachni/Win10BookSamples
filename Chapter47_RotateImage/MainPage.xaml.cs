using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Haptics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Chapter47_RotateImage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        RadialControllerConfiguration config;
        RadialController controller;
        RadialControllerMenuItem customItem;

        public MainPage()
        {
            this.InitializeComponent();

            controller = RadialController.CreateForCurrentView();
            controller.RotationChanged += Controller_RotationChanged;
            controller.ButtonHolding += Controller_ButtonHolding;

            controller.UseAutomaticHapticFeedback = false;
            
            controller.RotationResolutionInDegrees = 1;

            customItem = RadialControllerMenuItem.CreateFromKnownIcon("Rotate", RadialControllerMenuKnownIcon.Ruler);
            controller.Menu.Items.Add(customItem);

            controller.Menu.SelectMenuItem(customItem);

            config = RadialControllerConfiguration.GetForCurrentView();
            config.ActiveControllerWhenMenuIsSuppressed = controller;
            config.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, RadialControllerSystemMenuItemKind.Scroll });

            //uncomment it to supress the default menu
            //config.IsMenuSuppressed = true;
        }

        private void Controller_ButtonHolding(RadialController sender, RadialControllerButtonHoldingEventArgs args)
        {
            if (rotationControl.Visibility == Visibility.Visible)
                rotationControl.Visibility = Visibility.Collapsed;
            else
                rotationControl.Visibility = Visibility.Visible;
        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            //rotateTransform.Angle += args.RotationDeltaInDegrees;
            rotationControl.Angle += (float)args.RotationDeltaInDegrees;
        }
    }
}
