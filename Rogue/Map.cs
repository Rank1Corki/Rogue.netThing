using Newtonsoft.Json;
using System.Numerics;
using System.Text.RegularExpressions;
using ZeroElectric.Vinculum;

namespace Rogue
{

internal class Map
    {
        public int mapWidth;
        public MapLayer[] layers;

        Texture image1;
        int imagePixelX;
        int imagePixelY;

        int imagesPerRow;

        public List<Enemy> enemies;
        public List<Item> items;


        public enum MapTile : int
        {
            Floor = 1,
            Wall = 15,
            Enemy = 110, 
        }

        List<Enemy> enemyTypes;


        public int getTileAt(Vector2 newPlace)
        {
            MapLayer ground = layers[0];
            return (ground.mapTiles[(int)newPlace.X + (int)newPlace.Y * mapWidth]);
            
               
            
        }

        public MapTile GetTileAtGround(int x, int y)
        {
            MapLayer ground = layers[0];
            return (MapTile)(ground.mapTiles[x + y * mapWidth]);
        }

        public MapTile GetTileAtEnemy(int x, int y)
        {
            MapLayer ground = layers[2];
            return (MapTile)(ground.mapTiles[x + y * mapWidth]);
        }

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            this.imagesPerRow = imagesPerRow;
            image1 = atlasImage;
         
          
        }
        public void DrawMap()
        {
            MapLayer ground = layers[0];
            Console.ForegroundColor = ConsoleColor.Gray;
        
            for (int row = 0; row < ground.mapTiles.Length / mapWidth; row++)
            {
                for (int col = 0; col < mapWidth; col++)
                {
                    int tileId = ground.mapTiles[row * mapWidth + col];
                    tileId--;
                    Console.SetCursorPosition(col, row);

                   
                       imagePixelX = (tileId % imagesPerRow) * Game.tileSize;
                            imagePixelY = (int)(tileId / imagesPerRow) * Game.tileSize;
                            int pixelX = (int)(col * Game.tileSize);
                            int pixelY = (int)(row * Game.tileSize);

                            //Console.Write(".");
                            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);
                            // Raylib.DrawRectangle(pixelX, pixelY, Game.tileSize, Game.tileSize, color);
                            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
            
                }

            }

            foreach (var item in enemies)
            {
                item.DrawEnemy();

            }

            foreach (var item in items)
            {
                item.DrawItem();

            }
        }

        public void LoadEnemyTypes(string filename)
        {
            enemyTypes = new List<Enemy>();
            // TODO: Tarkista että tiedosto on olemassa.
            if (File.Exists(filename))
            {
                // TODO: Lue tiedoston sisältö samalla tavalla kuin
                // kentän lataamisessa.
                string fileContents;

                using (StreamReader reader = File.OpenText(filename))
                {
                    fileContents = reader.ReadToEnd();
                }

                // TODO: Käytä NewtonSoft.JSON kirjastoa ja muuta tiedoston
                // sisältö List<Enemy> tai Enemy[] muotoon
                enemyTypes = JsonConvert.DeserializeObject<List<Enemy>>(fileContents);
            }
        }
        private Enemy CreateEnemyBySpriteId(int spriteId)
        {
            foreach (Enemy template in enemyTypes)
            {
                // TODO: Onko tällä enemyllä sama spriteId kuin mitä on saatiin parametrina
                if (template.tileId == spriteId)
                {
                    // Palauta kopio.
                    // Jos palauttaisit suoraan template:n, olisivat
                    // kaikki samanlaiset viholliset yksi ja sama vihollinen
                    return new Enemy(template);
                }
            }
            // TODO: Jos sopivaa ei löytyny, näytä virheilmoitus ja palauta null tai testivihollinen
            Console.WriteLine($"Error, no enemy found with id: {spriteId}");
            return null;
}

        public void LoadEnemiesAndItems()
        {
            LoadEnemyTypes("enemies.json");
            enemies = new List<Enemy>();


            MapLayer enemyLayer = layers[2];

            int[] enemyTiles = enemyLayer.mapTiles;
            int mapHeight = enemyTiles.Length / mapWidth;
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    int index = x + y * mapWidth;
                    int tileId = enemyTiles[index];
                    if (tileId != 0)
                    {
                        Enemy newenemy = CreateEnemyBySpriteId((int)tileId);
                        if (newenemy != null)
                        {
                            newenemy.SetImageAndIndex(image1, imagesPerRow, tileId);
                            newenemy.position = position;
                            enemies.Add(newenemy);
                        }
                   
                    }
                }
            }

            // sama esineille...
            items = new List<Item>();

            MapLayer itemLayers = layers[1];

            int[] itemTiles = itemLayers.mapTiles;
            int mapHeight1 = itemTiles.Length / mapWidth;
            for (int y = 0; y < mapHeight1; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Vector2 position = new Vector2(x, y);
                    int index = x + y * mapWidth;
                    int tileId = itemTiles[index];
                    switch (tileId)
                    {
                        case 0:
                            // ei mitään tässä kohtaa
                            break;
                        case 90:
                            Item i = new Item("chest", position, Raylib.WHITE);
                            i.SetImageAndIndex(image1, imagesPerRow, tileId -= 1);
                            items.Add(i);
                            break;
                        case 119:
                            Item a = new Item("axe", position, Raylib.WHITE);
                            a.SetImageAndIndex(image1, imagesPerRow, tileId -= 1);
                            items.Add(a);
                            break;
                        case 104:
                            Item b = new Item("knife", position, Raylib.WHITE);
                            b.SetImageAndIndex(image1, imagesPerRow, tileId -= 1);
                            items.Add(b);
                            break;
                    }
                }
            }
        }
        
      
    }

 
}