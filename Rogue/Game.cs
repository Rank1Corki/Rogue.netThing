using System;
using System.Collections.Generic;
using System.Numerics;
using ZeroElectric.Vinculum;
using TurboMapReader;
using RayGuiCreator;

namespace Rogue
{
    internal class Game
    {
        // Player
        private PlayerCharacter player;

        // Player name
        private TextBoxEntry playerNameEntry = new(10);

        // Map
        private Map lvl1;

        //Race selection
        MultipleChoiceEntry race = new MultipleChoiceEntry(
            new string[] { "Human", "Elf", "Dwarf" });  // Change race names

        //Class selection
        MultipleChoiceEntry characterClass = new MultipleChoiceEntry(
            new string[] { "Warrior", "Mage", "Rogue" });  // Change class names


        // Game state
        public enum GameState
        {
            MainMenu,
            CharacterCreation,
            GameLoop,
            PauseMenu,
            OptionsMenu,
            Quit
        }

        // State stack
        private Stack<GameState> stateStack = new();

        // Menus
        private OptionsMenu myOptionsMenu;
        private PauseMenu myPauseMenu;

        // Current game state
        private GameState currentGameState;

        // Enemies
        private Enemy enemy;

        // Sound effect
        private Sound soundToPlay;

        // Tile size
        public static readonly int tileSize = 16;

        // Screen width and height
        private const int screen_width = 1280;
        private const int screen_height = 720;

        public void Run()
        {
            // Start in main menu
            stateStack.Push(GameState.MainMenu);

            // Create menus
            myOptionsMenu = new OptionsMenu();
            myPauseMenu = new PauseMenu();

            // Enable back navigation
            myOptionsMenu.BackButtonPressedEvent += OnOptionsBackButtonPressed;
            myPauseMenu.BackButtonPressedEvent += OnPauseBackButtonPressed;

            // Console setup
            Console.CursorVisible = false;
            Console.WindowWidth = 60;
            Console.WindowHeight = 26;

            // Create player
            player = new PlayerCharacter('@', Raylib.RED, ConsoleColor.Green);
            player.position = new Vector2(3, 3);

            // Initialize map reader
            MapReader reader = new();
            lvl1 = reader.ReadTiledMapFromFile("tilded/Rogue.tmj");

            // Load assets
            Raylib.InitWindow(screen_width, screen_height, "Raylib");
            Raylib.InitAudioDevice();
            soundToPlay = Raylib.LoadSound("Sound/click.mp3");
            Texture imageTexture = Raylib.LoadTexture("Images/tilemap.png");

            // Setup map and player visuals
            lvl1.SetImageAndIndex(imageTexture, 12, 109);
            lvl1.LoadEnemiesAndItems();
            player.SetImageAndIndex(imageTexture, 12, 109);

            // Start game loop
            GameLoop();

            Raylib.CloseWindow();
        }

        private void DrawMainMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            int button_width = 100;
            int button_height = 20;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Start Game") == 1)
                stateStack.Push(GameState.CharacterCreation);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Options") == 1)
                stateStack.Push(GameState.OptionsMenu);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Pause") == 1)
                stateStack.Push(GameState.PauseMenu);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Quit") == 1)
                stateStack.Push(GameState.Quit);

            Raylib.EndDrawing();
        }

        private void DrawRay()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.EndDrawing();
        }

        private void OnOptionsBackButtonPressed(object sender, EventArgs args) => stateStack.Pop();

        private void OnPauseBackButtonPressed(object sender, GameState newState)
        {
            if (newState == GameState.GameLoop)
                stateStack.Pop();
            else
                stateStack.Push(GameState.MainMenu);
        }

        private void GameLoop()
        {
            bool gameIsOn = true;

            while (!Raylib.WindowShouldClose() && gameIsOn)
            {
                switch (stateStack.Peek())
                {
                    case GameState.MainMenu:
                        DrawMainMenu();
                        break;
                    case GameState.CharacterCreation:
                        DrawCharacterMenu(Raylib.GetScreenWidth() / 2 - 150, 40, 300);
                        break;
                    case GameState.PauseMenu:
                        myPauseMenu.DrawMenu();
                        break;
                    case GameState.OptionsMenu:
                        myOptionsMenu.DrawMenu();
                        break;
                    case GameState.GameLoop:
                        DrawRay();
                        Update();
                        Draw();
                        break;
                    case GameState.Quit:
                        gameIsOn = false;
                        break;
                }
            }
        }

        private void Draw()
        {
            Console.Clear();
            lvl1.DrawMap();
            player.Draw();
        }

        private void DrawCharacterMenu(int x, int y, int width)
        {
            Raylib.ClearBackground(Raylib.GetColor((uint)RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.BACKGROUND_COLOR)));

            MenuCreator c = new(x, y, Raylib.GetScreenHeight() / 20, width);
            c.Label("Create Character");
            c.Label("Name Character");
            c.TextBox(playerNameEntry);
            c.Label("Select Class");
            c.DropDown(characterClass); // Dropdown for class selection
            c.Label("Select Race");
            c.DropDown(race); // Dropdown for race selection

            if (c.Button("Start Game"))
            {
                string nimi = playerNameEntry.ToString();
                bool nameOk = !string.IsNullOrEmpty(nimi) && nimi.All(char.IsLetter);  // Name validation

                bool raceSelect = false;
                bool classSelect = false;

                // Race selection logic
                switch (race.ToString())
                {
                    case "Human":
                        player.rotu = Race.Human;
                        raceSelect = true;
                        break;
                    case "Elf":
                        player.rotu = Race.Elf;
                        raceSelect = true;
                        break;
                    case "Dwarf":
                        player.rotu = Race.Dwarf;
                        raceSelect = true;
                        break;
                }

                // Class selection logic
                switch (characterClass.ToString())
                {
                    case "Warrior":
                        player.hahmoluokka = Class.Warrior;
                        classSelect = true;
                        break;
                    case "Mage":
                        player.hahmoluokka = Class.Mage;
                        classSelect = true;
                        break;
                    case "Rogue":
                        player.hahmoluokka = Class.Rogue;
                        classSelect = true;
                        break;
                }

                // Proceed to game if everything is valid
                if (nameOk && raceSelect && classSelect)
                    stateStack.Push(currentGameState = GameState.GameLoop);
            }

            c.EndMenu();
            Raylib.EndDrawing();
        }


        private void Update()
        {
            Vector2 newPlace = player.position;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { newPlace.Y -= 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { newPlace.Y += 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) { newPlace.X -= 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) { newPlace.X += 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB)) stateStack.Push(GameState.PauseMenu);

            if (lvl1.GetTileAtGround((int)newPlace.X, (int)newPlace.Y) == Map.MapTile.Floor)
                player.position = newPlace;
        }
    }
}
