using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Uwp.Common.Extend
{
    public static class ControlExtension
    {

        /// <summary>
        /// 查找父控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="name">元素的名称</param>
        /// <returns></returns>
        public static T GetParentControl<T>(this DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// 查找子控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="name">元素的名称</param>
        /// <returns></returns>
        public static T GetChildControl<T>(this DependencyObject obj, string name = "") where T : FrameworkElement
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    var grandChild = child.GetChildControl<T>(name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;

        }

        /// <summary>
        /// 查找所有子控件
        /// </summary>
        /// <param name="obj">当前控件</param>
        /// <param name="name">元素的名称</param>
        /// <returns></returns>
        public static List<T> GetChildControls<T>(this DependencyObject obj, string name = "") where T : FrameworkElement
        {
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }

                childList.AddRange(GetChildControls<T>(child, ""));
            }

            return childList;

        }
    }
}
