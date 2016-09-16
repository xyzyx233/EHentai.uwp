using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Uwp.Control.Model;

namespace EHentai.uwp.Model
{
    public static class ControlExtension
    {
        public static void AddSelect(this PivotViewModel pivot, string head, object content)
        {
            MainPage.HideButton.Focus(FocusState.Pointer);
            pivot.Add(head, content);
        }
    }
}
