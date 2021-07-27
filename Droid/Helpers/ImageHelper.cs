using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Java.IO;
using WarehouseHandheld.Droid.Helpers;
using WarehouseHandheld.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageHelper))]
namespace WarehouseHandheld.Droid.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> AddImage(byte[] image)
        {
            var filename = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var f1 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            filename = System.IO.Path.Combine(filename, DateTime.Now.ToString("ddMMyyyymmss") + ".png");
             
            MemoryStream stream = new MemoryStream(image);
            var bitmap = BitmapFactory.DecodeStream(stream);
            if (!System.IO.File.Exists(filename))
            {
                using (var filestream = new FileStream(filename, FileMode.Create))
                {
                    if (bitmap.Compress(Bitmap.CompressFormat.Png, 100, filestream))
                    {
                        filestream.Flush();
                    }
                    else { } // handle failure case...
                }
            }
            bitmap.Recycle();
            bitmap.Dispose();
            return filename;
        }
        public void DeleteImage(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public byte[] GetImageBytes(string path)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return bytes;
        }
    }
}
