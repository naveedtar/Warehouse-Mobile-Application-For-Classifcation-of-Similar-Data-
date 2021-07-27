using System;
using System.Threading.Tasks;

namespace WarehouseHandheld.Helpers
{
    public interface IImageHelper
    {
        Task<string> AddImage(byte[] image);
        void DeleteImage(string path);
        byte[] GetImageBytes(string path);
    }
}
