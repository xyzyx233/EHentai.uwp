using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Uwp.Common.Extend;
using Uwp.Control.Model;

namespace Uwp.Control
{
    public partial class PivotTemplate : ResourceDictionary
    {
        public PivotTemplate()
        {
            InitializeComponent();
        }

        private static async void ClosePiovt(FrameworkElement sources)
        {
            try
            {
                PivotHeaderItem item = sources.GetParentControl<PivotHeaderItem>();
                PivotHeaderPanel panel = sources.GetParentControl<PivotHeaderPanel>();

                for (int i = 0; i < panel.Children.Count; i++)
                {
                    var child = panel.Children[i];
                    if (child == item)
                    {
                        PivotViewModel pivot = sources.GetParentControl<Pivot>().DataContext as PivotViewModel;
                        pivot.RemoveAt(i);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClosePiovt(sender as Windows.UI.Xaml.Controls.Control);
        }

        private void ContentPresenter_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var sources = sender as FrameworkElement;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                var properties = e.GetCurrentPoint(sources).Properties;
                if (properties.IsMiddleButtonPressed)
                {
                    ClosePiovt(sources);
                }
            }

        }
    }
}
