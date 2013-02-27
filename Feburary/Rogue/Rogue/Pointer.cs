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
    public class Pointer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        MouseState mouseState;
        SpriteBatch spritebatch;
        Texture2D pointerSprite;

        public Pointer(Game game)
            : base(game)
        {
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spritebatch = new SpriteBatch(Game.GraphicsDevice);
            pointerSprite = Game.Content.Load<Texture2D>(@"images/Pointer");
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spritebatch.Begin();
            spritebatch.Draw(pointerSprite,
                    new Vector2(mouseState.X, mouseState.Y),
                    null,
                    Color.White,
                    0,
                    new Vector2((pointerSprite.Width / 2), (pointerSprite.Height / 2)),
                    1,
                    SpriteEffects.None,
                    1);
            spritebatch.End();
            base.Draw(gameTime);
        }
    }
}
