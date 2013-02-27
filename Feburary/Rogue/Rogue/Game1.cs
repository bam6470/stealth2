using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Rogue
{
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public DrawableGameComponent screen;
        public Pointer pointer;
        ScreenType screenSelect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Initialize screen slect
            screenSelect = ScreenType.MainMenu;
            screen = new MainMenu(this);
            Components.Add(screen);
            ((MainMenu)screen).moveToPlayGame += changeScreen;

            // Initialize the pointer
            pointer = new Pointer(this);
            Components.Add(pointer);
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected void changeScreen(object sender, ChangeScreenEventArgs args)
        {
            if (args.screenType == ScreenType.InGame)
            {
                Components.Remove(screen);
                screen = new PlayGame(this);
                Components.Add(screen);
                ((PlayGame)screen).Initialize(this);
            }
        }
    }
    public enum ScreenType
    {
        MainMenu,
        InGame
    }

    public class ChangeScreenEventArgs : EventArgs
    {
        public ScreenType screenType { get; set; }

        public ChangeScreenEventArgs(ScreenType type)
        {
            screenType = type;
        }
    }
}
