using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using TurboMapReader;

namespace Rogue
{
    internal class MapReader
    {
        public Map LoadTestMap()
        {
            Map testi = new Map();
            testi.mapWidth = 8;
            testi.layers = new MapLayer[3];
            testi.layers[0].mapTiles = new int[] {
            2, 2, 2, 2, 2, 2, 2, 2,
            2, 1, 1, 2, 1, 1, 1, 2,
            2, 1, 1, 2, 1, 1, 1, 2,
            2, 1, 1, 1, 1, 1, 2, 2,
            2, 2, 2, 2, 1, 1, 1, 2,
            2, 1, 1, 1, 1, 1, 1, 2,
            2, 2, 2, 2, 2, 2, 2, 2
            };
         
            return testi;
        }

        public Map ReadMapFromFile(string fileName)
        {
            bool exists = File.Exists(fileName);

            if (exists == false)
            {
                Console.WriteLine($"File {fileName} not found");
                return LoadTestMap();
            }

            string fileContents;

            using (StreamReader reader = File.OpenText(fileName))
            {
                fileContents = reader.ReadToEnd();
            }

            Map loadedMap = JsonConvert.DeserializeObject<Map>(fileContents,new JsonSerializerSettings{ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor});




            return loadedMap;
        }

        public Map ReadTiledMapFromFile(string filename)
        {

            
            
            TiledMap loadedTileMap = TurboMapReader.MapReader.LoadMapFromFile(filename);

            int mapWidht = loadedTileMap.width;
            int mapheight = loadedTileMap.height;

            TurboMapReader.MapLayer groundLayer = loadedTileMap.GetLayerByName("Ground");
            TurboMapReader.MapLayer enemyLayer = loadedTileMap.GetLayerByName("Enemy");
            TurboMapReader.MapLayer itemLayer = loadedTileMap.GetLayerByName("Items");

            int howManyTiles = groundLayer.data.Length;
            int[] groundTiles = groundLayer.data;
            string name = groundLayer.name;

            int howManyEnemyTiles = enemyLayer.data.Length;
            int howManyItemTiles = itemLayer.data.Length;
            int[] enemyTiles = enemyLayer.data;

            int[] itemTiles = itemLayer.data;
            string nameEnemy = enemyLayer.name;
            string nameItems = itemLayer.name;



            Map tiledMap = new Map();
            tiledMap.mapWidth = mapWidht;
            tiledMap.layers = new MapLayer[3];
            tiledMap.layers[0] = new MapLayer();
            tiledMap.layers[0].mapTiles = groundTiles;
            tiledMap.layers[2] = new MapLayer();
            tiledMap.layers[2].mapTiles = enemyTiles;
            tiledMap.layers[1] = new MapLayer();
            tiledMap.layers[1].mapTiles = itemTiles;
            return tiledMap;
        }
    }
}
