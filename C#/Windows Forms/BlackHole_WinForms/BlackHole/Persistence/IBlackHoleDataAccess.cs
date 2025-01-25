using System;
using System.Threading.Tasks;

namespace BlackHole.Persistence
{
    public interface IBlackHoleDataAccess
    {
        Task<BlackHoleMap> LoadAsync(String path);
        Task SaveAsync(String path, BlackHoleMap map);
    }
}
