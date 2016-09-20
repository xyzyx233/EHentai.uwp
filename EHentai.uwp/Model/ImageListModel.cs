using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using EHentai.uwp.Common;
using Uwp.Common;

namespace EHentai.uwp.Model
{
    public class ImageListModel : NotifyPropertyChanged
    {
        public int Id { get; set; }//ID
        public string Title { get; set; }//标题
        public string Index { get; set; }//图片坐标

        private string _imageUrl;//图片Url
        public string ImageUrl
        {
            get
            {
                //if (ImageCache.HasCache(CacheName).Result)
                //{
                //    return _imageUrl = ImageCache.GetImageBase64(CacheName);
                //}
                return _imageUrl;
            }
            set { _imageUrl = value; GetImageUrl?.Invoke(this, EventArgs.Empty); }
        }
        public string Herf { get; set; }//链接
        public string Src { get; set; }//图片Base64值
        public EnumLoadState ImageLoadState { get; set; }

        private double? _height;//高度
        public double? Height
        {
            get { return _height; }
            set
            {
                SetProperty(ref _height, value, "Height");
            }
        }

        private BitmapImage _image;//图片
        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                SetProperty(ref _image, value, "Image");
            }
        }

        private string _cacheName;//图片本地缓存名称
        public string CacheName
        {
            get { return _cacheName; }
            set
            {
                _cacheName = value;
                //SetProperty(ref _cacheName, value);
            }
        }

        private Visibility _visibility;//是否显示
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                SetProperty(ref _visibility, value, "Visibility");
            }
        }

        public EnumOrder Order { get; set; }

        public event EventHandler GetImageUrl; //获取到图片Url事件
        public event EventHandler Loaded; //加载完成事件

        public ImageListModel()
        {
            Order = EnumOrder.Next;
            Visibility = Visibility.Collapsed;
        }

        public void OnLoaded()
        {
            Loaded?.Invoke(this, EventArgs.Empty);
        }
    }

    public enum EnumOrder
    {
        Prev,
        Center,
        Next
    }
    public enum EnumLoadState
    {
        NotLoaded,
        Loading,
        Loaded
    }
}
