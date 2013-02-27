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
    public class PlayGame : Microsoft.Xna.Framework.DrawableGameComponent
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Map map;
        List<Guard> guards;

        public PlayGame(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public void Initialize(Game game)
        {
            game.IsMouseVisible = true;
            graphics = ((Game1)game).graphics;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
            LoadContent(game);
        }

        protected void LoadContent(Game game)
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Create the player object
            player = new Player(game.Content.Load<Texture2D>(@"images/playerSprite"),
                                new Vector2(100, 100),
                                new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2),
                                game.Content.Load<Texture2D>(@"images/staminaBack"),
                                game.Content.Load<Texture2D>(@"images/staminaFront"));
            // creates the map object and fills the guard object
            map = new Map(game.Content.Load<Texture2D>("MapTest"));
            map.LoadMap(new StreamReader("MapTestText.txt"), game.Content.Load<Texture2D>("MapTest"));
            //testGuard = new Guard(new Vector2(0, 0), Content.Load<Texture2D>(@"images/guard"), 0, false, null);
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            player.Update(map.GetTiles(new Vector2()));
            //foreach (Guard g in guards)
            //{
            //    g.Update(player, gameTime);
            //}
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            // Draw the map
            map.DrawMap(spriteBatch, player.offset, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            // Draw the player
            player.Draw(spriteBatch);
            // Draw the guards
            //foreach (Guard g in guards)
            //{
            //    g.Draw(spriteBatch, player);
            //}
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
