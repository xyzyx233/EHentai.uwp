using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Uwp.Common;

namespace Uwp.Control.Model
{
    public class PivotViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<PivotContent> Contents { get; set; }

        private int _selectedIndex;//当前选中
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                SetProperty(ref _selectedIndex, value, "SelectedIndex");
            }
        }

        public PivotViewModel()
        {
            Contents = new ObservableCollection<PivotContent>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="head">标题</param>
        /// <param name="content">内容</param>
        public void Add(string head, object content)
        {
            PivotContent PivotContent = new PivotContent(head, content, SelectedIndex);
            Contents.Add(PivotContent);
            SelectedIndex = Contents.Count - 1;
        }

        /// <summary>
        /// 移除某个Item
        /// </summary>
        /// <param name="index">TabItem坐标</param>
        public void RemoveAt(int index)
        {
            var content = Contents[index];
            if (SelectedIndex == index)
            {
                SelectedIndex = content.CreateIndex;
            }
            Contents.RemoveAt(index);
        }

        /// <summary>
        /// 移除某个TabItem
        /// </summary>
        /// <param name="content">TabItem对象</param>
        public void Remove(PivotContent content)
        {
            if (SelectedIndex == Contents.IndexOf(content))
            {
                SelectedIndex = content.CreateIndex;
            }
            Contents.Remove(content);
        }


        ///// <summary>
        ///// 关闭事件
        ///// </summary>        
        //private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        //{
        //    var viewModel = args.DragablzItem.DataContext as Pivot;
        //    if (SelectedIndex == Contents.IndexOf(viewModel))
        //    {
        //        SelectedIndex = viewModel.CreateIndex;
        //    }
        //}
    }
}
