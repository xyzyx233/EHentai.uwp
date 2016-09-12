using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace EHentai.uwp.Common
{
    public static class ImageDecoder
    {
        public static readonly DependencyProperty SourceProperty;

        static ImageDecoder()
        {
            SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(BitmapImage), typeof(ImageDecoder), new PropertyMetadata(null, OnComplate));
        }

        public static string GetSource(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("Image");
            return (string)image.GetValue(SourceProperty);
        }

        public static void SetSource(Image image, BitmapImage value)
        {
            if (image == null)
                throw new ArgumentNullException("Image");
            image.SetValue(SourceProperty, value);
        }
        private static void OnComplate(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Image img = o as Image;
            img.Source = e.NewValue as BitmapImage;
            Storyboard storyboard = new Storyboard();
            //DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(500.0)));
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = 1;
            doubleAnimation.BeginTime = TimeSpan.FromMilliseconds(500);
            Storyboard.SetTarget(doubleAnimation, img);
            Storyboard.SetTargetProperty(doubleAnimation, "Opacity");
            storyboard.Children.Add(doubleAnimation);
            storyboard.Begin();
        }
    }
}
