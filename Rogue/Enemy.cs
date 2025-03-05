using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;
using static System.Net.Mime.MediaTypeNames;

namespace Rogue
{
    public class Enemy
    {
      

        public string name;
        public Vector2 position;
        private Color color;
        int imagesPerRow;
        Texture image1;
        int imagePixelX;
        int imagePixelY;


        public int tileId;
        public int hp;
        public Enemy() { }
        public Enemy(Enemy copyFrom)
        {
            //Copies enemy variables for the editor
            this.name = copyFrom.name;
            this.position = copyFrom.position;
            this.color = copyFrom.color;
            this.imagesPerRow = copyFrom.imagesPerRow;
            this.image1 = copyFrom.image1;
            this.hp = copyFrom.hp;
            this.tileId = copyFrom.tileId;
           

        }
        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            //Sets up the tilemap for the enemy layer
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Game.tileSize;
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }
        public Enemy(string name, Vector2 position, Color color)
        {
            //Idk no refrences
            this.name = name;
            this.position = position;
            this.color = color;
        }

        public Enemy(string name, int tileID, int hitPoints)
        {
            //Adds a new enemy
            this.name = name;
            this.tileId = tileID;
            this.hp = hitPoints;
        }

        public void DrawEnemy()
        {
            //Drawn an enemy once called
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);

            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
