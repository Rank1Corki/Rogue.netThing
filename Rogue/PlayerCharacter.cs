using System;
using System.Numerics;
using ZeroElectric.Vinculum;

namespace Rogue
{
    public enum Race
    {
        Human,
        Elf,
        Dwarf
    }

    public enum Class
    {
        Warrior,
        Mage,
        Rogue
    }

    internal class PlayerCharacter
    {
        public string nimi;
        public Race rotu;
        public Class hahmoluokka;

        public Vector2 position;

        // Texture properties for the character
        Texture image1;
        int imagePixelX;
        int imagePixelY;

        private char image;
        ZeroElectric.Vinculum.Color color;
        ConsoleColor color1;

        // Set image and index for the tilemap sprite
        public void SetImageAndIndex(Texture atlasImage, int imagesPerRow, int index)
        {
            image1 = atlasImage;
            imagePixelX = (index % imagesPerRow) * Game.tileSize - 16;  // Shift the image 16 pixels to the left
            imagePixelY = (int)(index / imagesPerRow) * Game.tileSize;
        }

        // Constructor for PlayerCharacter
        public PlayerCharacter(char image, ZeroElectric.Vinculum.Color color, ConsoleColor color1)
        {
            this.image = image;
            this.color1 = color1;
            this.color = color;
        }

        // Move the player on the screen
        public void Move(int x_move, int y_move)
        {
            position.X += x_move;
            position.Y += y_move;

            // Keep the player inside the console window
            position.X = Math.Clamp(position.X, 0, Console.WindowWidth - 1);
            position.Y = Math.Clamp(position.Y, 0, Console.WindowHeight - 1);
        }

        // Draw the player character
        public void Draw()
        {
            // Calculate the pixel position for drawing
            int pixelX = (int)(position.X * Game.tileSize);
            int pixelY = (int)(position.Y * Game.tileSize);

            // Draw the player character at the new position
            Console.ForegroundColor = color1;
            Console.SetCursorPosition((int)position.X, (int)position.Y);
            Console.Write(image);

            // Define the portion of the texture to draw from the atlas
            Rectangle imageRect = new Rectangle(imagePixelX, imagePixelY, Game.tileSize, Game.tileSize);

            // Draw the character texture
            Raylib.DrawTextureRec(image1, imageRect, new Vector2(pixelX, pixelY), Raylib.WHITE);
        }
    }
}
