using System;
using System.IO;
using System.Threading.Tasks;

namespace BlackHole.Persistence
{
    public class BlackHoleFileDataAccess : IBlackHoleDataAccess
    {
        public async Task<BlackHoleMap> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path)) // fájl megnyitása
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] elements;
                    Int32 mapSize = Int32.Parse(line); // beolvassuk a tábla méretét
                    BlackHoleMap map = new BlackHoleMap(mapSize); // létrehozzuk a táblát

                    for (Int32 i = 0; i < mapSize; i++)
                    {
                        line = await reader.ReadLineAsync() ?? String.Empty;
                        elements = line.Split(' ');

                        for (Int32 j = 0; j < mapSize; j++)
                        {
                            switch (int.Parse(elements[j]))
                            {
                                case 1:
                                    map.SetFieldValue(i, j, BlackHoleMap.Field.RED);
                                    break;
                                case 2:
                                    map.SetFieldValue(i, j, BlackHoleMap.Field.BLUE);
                                    break;
                                case 3:
                                    map.SetFieldValue(i, j, BlackHoleMap.Field.BLACKHOLE);
                                    break;
                                default:
                                    map.SetFieldValue(i, j, BlackHoleMap.Field.EMPTY);
                                    break;
                            }
                        }
                    }

                    return map;
                }
            }
            catch
            {
                throw new BlackHoleDataException();
            }
        }

        public async Task SaveAsync(String path, BlackHoleMap map)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    writer.WriteLine(map.GetMapSize()); // kiírjuk a méreteket
                    for (Int32 i = 0; i < map.GetMapSize(); i++)
                    {
                        for (Int32 j = 0; j < map.GetMapSize(); j++)
                        {
                            switch (map[i, j])
                            {
                                case BlackHoleMap.Field.RED:
                                    await writer.WriteAsync(1 + " ");
                                    break;
                                case BlackHoleMap.Field.BLUE:
                                    await writer.WriteAsync(2 + " ");
                                    break;
                                case BlackHoleMap.Field.BLACKHOLE:
                                    await writer.WriteAsync(3 + " ");
                                    break;
                                default:
                                    await writer.WriteAsync(0 + " ");
                                    break;
                            }
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new BlackHoleDataException();
            }
        }
    }
}
