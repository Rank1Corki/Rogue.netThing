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
    internal class Item
    {
        public string name;
        public Vector2 position;
        private Color color;
        int imagePixelX;
        int imagePixelY;
        Texture image1;

        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Game.tileSize;
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }
        public Item(string name, Vector2 position, Color color)
        {
            this.name = name;
            this.position = position;
            this.color = color;
        }

        public void DrawItem()
        {
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);

            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
