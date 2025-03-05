using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZeroElectric.Vinculum;
using TurboMapReader;
using RayGuiCreator;
using System.Diagnostics;


namespace Rogue
{
    internal class Game
    {
        //Player
        PlayerCharacter player;

        //Player name
        TextBoxEntry playerNameEntry = new TextBoxEntry(10);

        //Map
        Map lvl1;

        //Class selection
        MultipleChoiceEntry characterClass = new MultipleChoiceEntry(
            new string[] { "A", "B", "C" });

        //Race selection
        MultipleChoiceEntry race = new MultipleChoiceEntry(
       new string[] { "A", "B", "C" });

        //Games state
        public enum GameState
        {
            //State stack 
            MainMenu,
            CharacterCreation,
            GameLoop,
            PauseMenu,
            OptionsMenu,
            Quit
        }

        //Creates the stack
        Stack<GameState> stateStack = new Stack<GameState>();

        //Options menu
        OptionsMenu myOptionsMenu;

        //PauseMenu
        PauseMenu myPauseMenu;

        //Current game state
        GameState currentGameState;

        //Enemies
        Enemy enemy;

        //Soun effect
        Sound soundToPlay;

        //Tile size
        public static readonly int tileSize = 16;

        //Screen width and height
        const int screen_width = 1280;
        const int screen_height = 720;
        public void Run()
        {
            //Game starts in Main menu
            stateStack.Push(GameState.MainMenu);

            //Creates Options and Pause menus
            myOptionsMenu = new OptionsMenu();
            myPauseMenu = new PauseMenu();

       
            //Makes it possible to exit pause and options menus
            myOptionsMenu.BackButtonPressedEvent += this.OnOptionsBackButtonPressed;
            myPauseMenu.BackButtonPressedEvent += this.OnPauseBackButtonPressed;

            // Prepare to show game
            Console.CursorVisible = false;

            // A small window
            Console.WindowWidth = 60;
            Console.WindowHeight = 26;

            // Create player
            player = new PlayerCharacter('@', Raylib.RED, ConsoleColor.Green);

            // Creates the map reader
            MapReader reader = new MapReader();

            //Set the player position
            player.position = new Vector2(3, 3);

         
            Console.Clear();

            //Game starts

            //Sets up the graphics and audio
            Raylib.InitWindow(screen_width, screen_height, "Raylib");
            Raylib.InitAudioDevice();

            Sound basicSound = Raylib.LoadSound("Sound/Bass Drum.wav");
            Texture imageTexture = Raylib.LoadTexture("Images/tilemap.png");


            //Reads the test map files
            lvl1 = reader.ReadMapFromFile("mapfile.json");
            //Reads the map files
            lvl1 = reader.ReadTiledMapFromFile("tilded/Rogue.tmj");

            lvl1.SetImageAndIndex(imageTexture, 12, 109);
            //Load EnemyEditor
            lvl1.LoadEnemiesAndItems();
            player.SetImageAndIndex(imageTexture, 12, 109);

            soundToPlay = basicSound;
           
            //Game loops until its closed
                GameLoop();
            
            Raylib.CloseWindow();

        }
        //Main menu
        private void DrawMainMenu()
        {
            // Tyhjennä ruutu ja aloita piirtäminen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Laske ylimmän napin paikka ruudulla.
            int button_width = 100;
            int button_height = 20;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            // Piirrä pelin nimi nappien yläpuolelle
            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            //Start game
            if (RayGui.GuiButton(new Rectangle(button_x, button_y
                , button_width, button_height), "Start Game") == 1)
            {
                // Start the game
                stateStack.Push(GameState.CharacterCreation);
            }
            // Piirrä seuraava nappula edellisen alapuolelle
            button_y += button_height * 2;

            //Options
            if (RayGui.GuiButton(new Rectangle(button_x,
                button_y,
                button_width, button_height), "Options") == 1)
            {
                // Go to options
                stateStack.Push(GameState.OptionsMenu);
            }

            button_y += button_height * 2;

            //Pause
            if (RayGui.GuiButton(new Rectangle(button_x,
                button_y,
                button_width, button_height), "Pause") == 1)
            {
                //currentGameState = GameState.PauseMenu;
                stateStack.Push(GameState.PauseMenu);
            }

            button_y += button_height * 2;

            //Quit
            if (RayGui.GuiButton(new Rectangle(button_x,
                button_y,
                button_width, button_height), "Quit") == 1)
            {
                stateStack.Push(GameState.Quit);
            }
            Raylib.EndDrawing();
        }
        private void DrawRay()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            // Draw rest of the game here

            Raylib.EndDrawing();
        }

        void OnOptionsBackButtonPressed(object sender, EventArgs args)
        {
            //Go back when back button is pressed
            stateStack.Pop();
        }

        void OnPauseBackButtonPressed(object sender, Game.GameState newState)
        {
            if (newState == Game.GameState.GameLoop)
            {
                //Go back if new state is Game loop
                stateStack.Pop();
            }
            else
            {
                //If not go to Main menu
                stateStack.Push(GameState.MainMenu);
            }
        }

        void GameLoop()
        {
            bool gameIsOn = true;
            //Check if game should run
            while (Raylib.WindowShouldClose() == false && gameIsOn)
            {
                //Set up for state stack
                switch (stateStack.Peek())
                {
                    //Main menu
                    case GameState.MainMenu:
                        DrawMainMenu();
                        break;
                        //Character creator
                    case GameState.CharacterCreation:
                        int x = 0;
                        int y = 40;
                        int withd = 300;
                        DrawCharacterMenu(x +Raylib.GetScreenWidth()/2 - withd, y, withd);
                        break;
                        //Pause menu
                    case GameState.PauseMenu:
                        myPauseMenu.DrawMenu();
                        break;
                        //Options menu
                    case GameState.OptionsMenu:
                        myOptionsMenu.DrawMenu();
                        break;
                        //Game loop
                    case  GameState.GameLoop:
                        //This is where the game voids are called
                        DrawRay();
                        Update();
                        Draw();
                        break;
                        //Quit the state stack
                    case GameState.Quit:
                        gameIsOn = false;
                        break;
                }

            }
            //Draw the game
            void Draw()
            {
                //Clear picture
                Console.Clear();
                //Draw map and object layers
                lvl1.DrawMap();
                //Draw player
                player.Draw();

            }

            //Character creator
            void DrawCharacterMenu(int x, int y, int width)
            {
                //Set up character creator usin Raylib UI
                Raylib.ClearBackground(Raylib.GetColor(((uint)RayGui.GuiGetStyle(((int)GuiControl.DEFAULT), ((int)GuiDefaultProperty.BACKGROUND_COLOR)))));
                MenuCreator c = new MenuCreator(x, y, Raylib.GetScreenHeight() / 20, width);
                c.Label("Create character");
                c.Label("Name Character");

                c.TextBox(playerNameEntry);
                c.Label("Select class");
                c.DropDown(characterClass);
                c.Label("Select race");
                c.DropDown(race);

                //Check if character is ok
                if (c.Button("Start Game"))
                {
                    bool nameOk = true;
                    var nimi = playerNameEntry.ToString();
                    if (string.IsNullOrEmpty(nimi))
                    {
                        //Nope if name is empty
                        Console.WriteLine("Ei kelpaa");
                        nameOk = false;
                    }


                    //Check if name contains numbers
                    for (int i = 0; i < nimi.Length; i++)
                    {
                        char kirjain = nimi[i];

                        if (char.IsLetter(kirjain))
                        {

                        }

                        else
                        {
                            //Nope if name contains numbers
                            nameOk = false;
                            break;
                        }
                    }

                    //Converts the answer to string
                    string raceAnswer = race.ToString();
                    string classAnswer = characterClass.ToString();

                    bool raceSelect = false;
                    bool classSelect = false;


                    //Check race and class selection
                    if (raceAnswer == "A")
                    {
                        raceSelect = true;
                        player.rotu = Race.A;
                    }
                    if (raceAnswer == "B")
                    {
                        raceSelect = true;
                        player.rotu = Race.B;
                    }
                    if (raceAnswer == "C")
                    {
                        player.rotu = Race.C;
                        raceSelect = true;
                    }
                    if (classAnswer == "A")
                    {
                        player.hahmoluokka = Class.A;
                        classSelect = true;
                    }
                    if (classAnswer == "B")
                    {
                        player.hahmoluokka = Class.B;
                        classSelect = true;

                    }
                    if (classAnswer == "C")
                    {
                        player.hahmoluokka = Class.C;
                        classSelect = true;

                    }
                    //If everything is ok go to game
                    if (nameOk && raceSelect == true && classSelect == true) { stateStack.Push( currentGameState = GameState.GameLoop); }

                }
                c.EndMenu();
                Raylib.EndDrawing();
            }

         


            void Update()
            {
                //Player postition
                Vector2 newPlace = player.position;


                //Move player and make sound
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                {
                    Raylib.PlaySound(soundToPlay);
                    newPlace.Y -= 1;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                {
                    Raylib.PlaySound(soundToPlay);
                    newPlace.Y += 1;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT))
                {
                    Raylib.PlaySound(soundToPlay);
                    newPlace.X -= 1;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT))
                {
                    Raylib.PlaySound(soundToPlay);
                    newPlace.X += 1;
                }
                //Pause
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB))
                {
                    stateStack.Push(GameState.PauseMenu);
                    
                }

                //Check if the tile player is moving to is ground
                if (lvl1.GetTileAtGround((int)newPlace.X, (int)newPlace.Y) == Map.MapTile.Floor)
                {
                    player.position = newPlace;

                    //Check if player hits an enemy
                    foreach (var item in lvl1.enemies)
                    {
                        if (item.position == player.position)
                        {
                            Console.WriteLine($"You hit enemy {item.name}");
                        }
                    }

                    //Check if player hits an item
                    foreach (var item in lvl1.items)
                    {
                        if (item.position == player.position)
                        {
                            Console.WriteLine($"You hit item {item.name}");
                        }
                    }
                }
            }
        }
    }
}
