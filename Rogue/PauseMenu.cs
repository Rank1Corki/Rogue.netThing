using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;

namespace Rogue
{
    class PauseMenu
    {
        public event EventHandler<Game.GameState> BackButtonPressedEvent;
        public void DrawMenu()
        {
            // Tyhjennä ruutu ja aloita piirtäminen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Laske ylimmän napin paikka ruudulla.
            int button_width = 200;
            int button_height = 40;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            // Piirrä pelin nimi nappien yläpuolelle
            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            if (RayGui.GuiButton(new Rectangle(button_x, button_y
                , button_width, button_height), "Go Back") == 1)
            {
                BackButtonPressedEvent.Invoke(this, Game.GameState.GameLoop);

            }

            button_y += button_height;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y
            , button_width, button_height), "Main menu") == 1)
            {
                BackButtonPressedEvent.Invoke(this, Game.GameState.MainMenu);

            }

            Raylib.EndDrawing();
        }
    }
}
