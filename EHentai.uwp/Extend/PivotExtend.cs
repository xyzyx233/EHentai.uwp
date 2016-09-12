using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EHentai.uwp.Extend
{
    public static class PivotExtend
    {
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">内容</param>
        /// <param name="selectIndex">要选择的item</param>
        public static void Add(this Pivot pivot, PivotItem item, int? selectIndex = null)
        {
            pivot.Items.Add(item);
            if (selectIndex != null && selectIndex < pivot.Items.Count)
            {
                pivot.SelectedIndex = selectIndex.Value;
            }
        }

        /// <summary>
        /// 添加项并选择最后一项
        /// </summary>
        /// <param name="item">内容</param>
        public static void AddAndSelectLast(this Pivot pivot, PivotItem item)
        {
            pivot.Items.Add(item);
            pivot.SelectedIndex = pivot.Items.Count - 1;
        }

    }
}
