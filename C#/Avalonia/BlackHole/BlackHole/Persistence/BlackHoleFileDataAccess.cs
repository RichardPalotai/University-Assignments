using System;
using System.IO;
using System.Threading.Tasks;

namespace BlackHole.Persistence
{
    public class BlackHoleFileDataAccess : IBlackHoleDataAccess
    {
        public async Task<BlackHoleMap> LoadAsync(String path)
        {
            return await LoadAsync(File.OpenRead(path));
        }

        public async Task<BlackHoleMap> LoadAsync(Stream stream)
        {
            try
            {
                using (StreamReader reader = new StreamReader(stream)) // fájl megnyitása
                {
                    String line = await reader.ReadLineAsync() ?? String.Empty;
                    String[] elements = line.Split(' ');
                    Int32 mapSize = Int32.Parse(elements[0]); // beolvassuk a tábla méretét
                    Int32 redShipsIn = Int32.Parse(elements[1]); // Pirosm hajo
                    Int32 blueShipsIn = Int32.Parse(elements[2]); // kek hajo
                    BlackHoleMap map = new BlackHoleMap(mapSize); // létrehozzuk a táblát
                    map.RedShipsIn = redShipsIn;
                    map.BlueShipsIn = blueShipsIn;

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
            await SaveAsync(File.OpenWrite(path), map);
        }

        public async Task SaveAsync(Stream stream, BlackHoleMap map)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(stream)) // fájl megnyitása
                {
                    writer.WriteLine(map.MapSize + " " + map.RedShipsIn + " " + map.BlueShipsIn); // kiírjuk a méreteket
                    for (Int32 i = 0; i < map.MapSize; i++)
                    {
                        for (Int32 j = 0; j < map.MapSize; j++)
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
