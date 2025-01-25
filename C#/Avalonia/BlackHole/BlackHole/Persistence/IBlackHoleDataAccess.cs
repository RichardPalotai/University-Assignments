using System;
using System.IO;
using System.Threading.Tasks;

namespace BlackHole.Persistence
{
    public interface IBlackHoleDataAccess
    {
        Task<BlackHoleMap> LoadAsync(String path);
        Task<BlackHoleMap> LoadAsync(Stream stream);
        Task SaveAsync(String path, BlackHoleMap map);
        Task SaveAsync(Stream stream, BlackHoleMap map);
    }
}
