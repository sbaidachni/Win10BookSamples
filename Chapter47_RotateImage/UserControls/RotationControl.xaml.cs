using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Chapter47_RotateImage.UserControls
{
    public sealed partial class RotationControl : UserControl
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle",
            typeof(float),
            typeof(RotationControl),
            new PropertyMetadata(null)
            );

        public float Angle
        {
            get
            {
                return (float)GetValue(AngleProperty);
            }
            set
            {
                SetValue(AngleProperty, value);
                canvasControl.Invalidate();
            }
        }

        public RotationControl()
        {
            this.InitializeComponent();

        }

        private void canvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            int stroke = 3;
            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;
            float radius=Math.Min(width, height)/2-2*stroke;
            float centerX = width / 2;
            float centerY = height / 2;
            float lineEndX = radius * (float)Math.Cos(Math.PI * Angle / 180) + centerX;
            float lineEndY = radius * (float)Math.Sin(Math.PI * Angle / 180) + centerY;

            args.DrawingSession.DrawCircle(centerX, centerY, radius, Colors.Red,stroke);
            args.DrawingSession.DrawLine(centerX, centerY,lineEndX , lineEndY, Colors.Green, stroke);
            args.DrawingSession.DrawText(Angle.ToString(), centerX, centerY, Colors.Black);
        }
    }
}
