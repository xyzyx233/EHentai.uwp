#region

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Uwp.Common;

#endregion

namespace EHentai.uwp.Common
{
    public class ImageCache
    {
        public static readonly string CachePath = @"ImageCache"; //图片缓存目录

        public static BitmapImage ErrorImage
        {
            get
            {
                var bitmapImage = new BitmapImage(new Uri(@"ms-appx:///Images/sadpanda.jpg"));
                return bitmapImage;
                //return null;
            }
        }

        /// <summary>
        ///     图片缓存是否存在
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> HasCache(string fileName)
        {
            try
            {
                return await FileHelper.ExistsFile(fileName, CachePath);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> ClearCache(string fileName)
        {
            try
            {
                if (await HasCache(fileName))
                    File.Delete(CachePath + fileName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        ///     创建图片缓存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        public static async Task<bool> CreateCache(string fileName, SoftwareBitmap bitmap)
        {
            try
            {
                fileName = fileName.ToLower();

                return await FileHelper.CreateImage(bitmap, fileName, CachePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     覆盖图片缓存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bitmap"></param>
        public static async Task<bool> CoverCache(string fileName, SoftwareBitmap bitmap)
        {
            try
            {
                fileName = fileName.ToLower();

                return await FileHelper.CreateImage(bitmap, fileName, CachePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<BitmapImage> GetImage(string fileName)
        {
            try
            {
                //using (var stream = await FileHelper.GetAccessStreamAsync(fileName, CachePath))
                //{
                //    BitmapImage img = new BitmapImage();
                //    img.SetSource(stream);
                //    return img;
                //}
                return new BitmapImage(new Uri(FileHelper.LoaclFolder.Path + "\\" + CachePath + "\\" + fileName));
            }
            catch (Exception ex)
            {
                File.Delete(fileName);
                return ErrorImage;
            }
        }

        private static BitmapImage ConvertToImage(Stream stream)
        {
            try
            {
                byte[] bitmapArray = new byte[stream.Length];
                stream.Read(bitmapArray, 0, bitmapArray.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);

                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                //将randomAccessStream 转成 IOutputStream
                var outputstream = randomAccessStream.GetOutputStreamAt(0);
                //实例化一个DataWriter
                DataWriter datawriter = new DataWriter(outputstream);
                //将Byte数组数据写进OutputStream
                datawriter.WriteBytes(bitmapArray);
                //在缓冲区提交数据到一个存储区
                datawriter.StoreAsync().GetResults();

                //将InMemoryRandomAccessStream给位图
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(randomAccessStream);

                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }

        //public static async Task<string> GetImageBase64(string fileName)
        //{
        //    var file = await FileHelper.GetFile(fileName, CachePath);
        //    ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
        //    WriteableBitmap bmp = new WriteableBitmap((int)properties.Width, (int)properties.Height);
        //    bmp.SetSource(await file.OpenReadAsync());
        //    string dataStr = await ToBase64(bmp);
        //    string fileType = file.FileType.Substring(1);
        //    string str = "data:image/" + file.FileType + ";base64," + dataStr + "\"";
        //    return str;
        //}

        //private static async Task<string> ToBase64(byte[] image, uint height, uint width, double dpiX = 96, double dpiY = 96)
        //{
        //    var encoded = new InMemoryRandomAccessStream();
        //    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, encoded);
        //    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, height, width, dpiX, dpiY, image);
        //    await encoder.FlushAsync();
        //    encoded.Seek(0);

        //    var bytes = new byte[encoded.Size];
        //    await encoded.AsStream().ReadAsync(bytes, 0, bytes.Length);
        //    return Convert.ToBase64String(bytes);
        //}

        //private static async Task<string> ToBase64(WriteableBitmap bitmap)
        //{
        //    var bytes = bitmap.PixelBuffer.ToArray();
        //    return await ToBase64(bytes, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight);
        //}

        public static string GetImageBase64(string fileName)
        {
            try
            {
                var names = fileName.Split('.');
                string type = names[names.Length - 1];
                return $"data:image/{type};base64," + FileHelper.GetImageBase64(fileName, CachePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetImageBase64(byte[] bytes, string imgType = "jpg")
        {
            try
            {
                return $"data:image/{imgType.Replace(".", "")};base64," + Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetImageBase64(Stream stream, string imgType = "jpg")
        {
            try
            {
                var bytes = FileHelper.StreamToBytes(stream);
                return GetImageBase64(bytes, imgType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> GetImageBase64(BitmapDecoder img, string imgType = "jpg")
        {
            var pixels = await img.GetPixelDataAsync();
            var image = pixels.DetachPixelData();
            // encode image
            var encoded = new InMemoryRandomAccessStream();
            Guid encoderId;
            switch (imgType.Replace(".", ""))
            {
                case "jpg":
                    encoderId = BitmapEncoder.JpegEncoderId;
                    break;
                case "png":
                    encoderId = BitmapEncoder.PngEncoderId;
                    break;
                case "bmp":
                    encoderId = BitmapEncoder.BmpEncoderId;
                    break;
                case "gif":
                    encoderId = BitmapEncoder.GifEncoderId;
                    break;
            }
            var encoder = await BitmapEncoder.CreateAsync(encoderId, encoded);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, img.PixelHeight, img.PixelWidth, img.DpiX, img.DpiY, image);
            await encoder.FlushAsync();
            encoded.Seek(0);

            // read bytes
            var bytes = new byte[encoded.Size];
            await encoded.AsStream().ReadAsync(bytes, 0, bytes.Length);

            // create base64
            return $"data:image/{imgType.Replace(".", "")};base64," + Convert.ToBase64String(bytes);
        }

        public static void SaveImageByBase64(string base64, string fileName)
        {
            FileHelper.SaveBase64(base64, fileName, CachePath);
        }
    }
}