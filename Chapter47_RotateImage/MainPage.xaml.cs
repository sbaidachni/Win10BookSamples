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

            controller.UseAutomaticHapticFeedback = false;
            
            controller.RotationResolutionInDegrees = 1;

            customItem = RadialControllerMenuItem.CreateFromKnownIcon("Rotate", RadialControllerMenuKnownIcon.Ruler);
            controller.Menu.Items.Add(customItem);

            controller.Menu.SelectMenuItem(customItem);

            config = RadialControllerConfiguration.GetForCurrentView();
            config.ActiveControllerWhenMenuIsSuppressed = controller;
            config.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume, RadialControllerSystemMenuItemKind.Scroll });

            //comment these three lines to supress menu scenario
            controller.ControlLost += Controller_ControlLost;
            controller.ControlAcquired += Controller_ControlAcquired;
            controller.RotationChanged += Controller_RotationChanged;

            //uncomment it to supress the default menu
            //config.IsMenuSuppressed = true;
            //controller.ButtonHolding += Controller_ButtonHolding;

        }

        private void Controller_ControlAcquired(RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            rotationControl.Visibility = Visibility.Visible;
        }

        private void Controller_ControlLost(RadialController sender, object args)
        {
            rotationControl.Visibility = Visibility.Collapsed;
        }

        private void Controller_ButtonHolding(RadialController sender, RadialControllerButtonHoldingEventArgs args)
        {
            if (rotationControl.Visibility == Visibility.Visible)
            {
                controller.RotationChanged -= Controller_RotationChanged;
                rotationControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                controller.RotationChanged += Controller_RotationChanged;
                rotationControl.Visibility = Visibility.Visible;
            }
        }

        private void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            //rotateTransform.Angle += args.RotationDeltaInDegrees;
            rotationControl.Angle += (float)args.RotationDeltaInDegrees;
        }
    }
}
