using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Rogue
{
    public class MainMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        GraphicsDeviceManager graphics;
        MouseState mouseState;
        SpriteBatch spritebatch;
        Texture2D playSprite;
        Rectangle playButton;

        public event EventHandler<ChangeScreenEventArgs> moveToPlayGame;

        public MainMenu(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            graphics = ((Game1)Game).graphics;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spritebatch = new SpriteBatch(GraphicsDevice);
            playSprite = Game.Content.Load<Texture2D>(@"images/Play");
            playButton = new Rectangle((graphics.PreferredBackBufferWidth / 2) - (playSprite.Width / 2), 150, playSprite.Width, playSprite.Height);
        }
        public override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (playButton.Intersects(new Rectangle(mouseState.X, mouseState.Y, 1, 1)))
                {
                    moveToPlayGame(this, new ChangeScreenEventArgs(ScreenType.InGame));
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Tan);
            spritebatch.Begin();
            spritebatch.Draw(playSprite, playButton, Color.White);
            spritebatch.End();
            base.Update(gameTime);
        }
    }
}
