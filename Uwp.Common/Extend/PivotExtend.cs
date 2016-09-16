using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Uwp.Common.Extend
{
    public static class PivotExtend
    {
        public static void Add(this Pivot pivot, string title, Page page)
        {
            PivotItem item = new PivotItem();
            item.Margin = new Thickness(0);
            item.Header = title;
            item.Content = page;
            pivot.Items.Add(item);
            //pivot.SelectedIndex = pivot.Items.Count - 1;
        }

        public static void Remove(this Pivot pivot, object page)
        {
            if (pivot.Items.Any())
            {
                pivot.Items.Remove(page);
            }
        }

        public static void RemoveAt(this Pivot pivot, int index)
        {
            if (index >= 0)
            {
                pivot.Items.RemoveAt(index);
            }
        }

        public static void Select(this Pivot pivot, int? index = null)
        {
            if (index >= 0)
            {
                pivot.SelectedIndex = index.Value;
            }
            else
            {
                pivot.SelectedIndex = pivot.Items.Count - 1;
            }
        }
    }
}
