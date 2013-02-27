using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Rogue
{
    class Player
    {
        public Vector2 position,velocity, offset, drawPosition; //position, offset, position to draw to
        //Stamina stuff
        Texture2D staminaBack, staminaFront;
        Vector2 staminaPosition;
        int stamina = 400;
        bool isSprinting;
        int speed;
        //roll
        int roll;
        int rollCD;

        Tile[,] tiles; // 9 nearest tiles

        Texture2D texture; //texture
        float rotation; // rotation to mouse
        MouseState mouse, mousePrevious; // mouse position
        KeyboardState keyState, keyStatePrevious; // current key state, and one from previous update(Used for single tap events)
        
        
       
        

        public Player(Texture2D Texture, Vector2 Position, Vector2 DrawPosition, Texture2D StaminaB, Texture2D StaminaF)
        {
            drawPosition = DrawPosition;
            position = Position;
            texture = Texture;
            speed = 3;
            mouse = Mouse.GetState();
            mousePrevious = mouse;
            //Stamina
            staminaBack = StaminaB;
            staminaFront = StaminaF;
            staminaPosition = new Vector2(0,DrawPosition.Y*2 - 64);
        }

        public void Update(Tile[,] Tiles)
        {

            mouse = Mouse.GetState();//update mouse 
            keyState = Keyboard.GetState();
            velocity = Vector2.Zero; //reset velocity
            if (roll == 0)
            {
                UpdateKeyboard();
                rotation = (float)(Math.Atan2(mouse.Y - position.Y - offset.Y, mouse.X - position.X - offset.X) + .5 * Math.PI); // update rotation
            }
            else
            {
                roll--;
                velocity = new Vector2(-(float)Math.Cos(rotation + (.5 * Math.PI)) * 10, -(float)Math.Sin(rotation + (.5 * Math.PI)) * 10);
            }
            if (rollCD < 100)
                rollCD++;
            tiles = Tiles; //update nearby tiles
            CollisionWithTiles(tiles);
            position += velocity;

            offset.X = drawPosition.X - position.X;//update X offset
            offset.Y = drawPosition.Y - position.Y;//update Y offset


            mousePrevious = mouse;
            keyStatePrevious = keyState;
        }

        // Draws the player
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, drawPosition, null, Color.White, rotation, new Vector2(16, 16), 1f, SpriteEffects.None, 1);
            // Draws the display
            DrawHUD(spriteBatch);
        }

        private void UpdateKeyboard()
        {
            if (mouse.RightButton == ButtonState.Pressed && rollCD == 100)
            {
                roll = 10;
                rollCD = 0;
            }
            //update position
            if (keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.A))
            {
                velocity.Y -= (int)speed/(float)Math.Sqrt(2);
                velocity.X -= (int)speed / (float)Math.Sqrt(2);
            }
            else if (keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.D))
            {
                velocity.Y -= speed / (float)Math.Sqrt(2);
                velocity.X += speed / (float)Math.Sqrt(2);
            }
            else if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.A))
            {
                velocity.Y += speed / (float)Math.Sqrt(2);
                velocity.X -= speed / (float)Math.Sqrt(2);
            }
            else if (keyState.IsKeyDown(Keys.S) && keyState.IsKeyDown(Keys.D))
            {
                velocity.Y += speed / (float)Math.Sqrt(2);
                velocity.X += speed / (float)Math.Sqrt(2);
            }
            else if (keyState.IsKeyDown(Keys.W))
                velocity.Y -= speed;
            else if (keyState.IsKeyDown(Keys.S))
                velocity.Y += speed;
            else if (keyState.IsKeyDown(Keys.D))
                velocity.X += speed;
            else if (keyState.IsKeyDown(Keys.A))
                velocity.X -= speed;

            

        }

        private void CollisionWithTiles(Tile[,] Tiles)
        {
            Vector2 Closest;
            for (int i = 0; i < 3; i++)
            {
                for (int x = 0; x < 3; x++)
                {

                    if (Tiles[i, x].solid)
                    {
                        //cases for non diagonal
                        //if overlap horizontally
                        if (position.Y > Tiles[i, x].position.Y && position.Y < Tiles[i, x].position.Y + 32)
                        {
                            if (position.X > Tiles[i, x].position.X && position.X < Tiles[i, x].position.X + 32)
                            {
                                // coinside
                            }
                            if (position.X > Tiles[i, x].position.X + 32 && position.X - 16 + velocity.X < Tiles[i, x].position.X + 32)
                            {
                                //to the right
                                velocity.X = 0;
                                position.X = Tiles[i, x].position.X + 48;
                            }
                            if (position.X < Tiles[i, x].position.X && position.X + 16 + velocity.X > Tiles[i, x].position.X)
                            {
                                // to the left
                                velocity.X = 0;
                                position.X = Tiles[i, x].position.X -16;
                            }
                        }
                        //if overlap horizontally
                        if (position.X > Tiles[i, x].position.X && position.X < Tiles[i, x].position.X + 32)
                        {

                            if (position.Y > Tiles[i, x].position.Y + 32 && position.Y - 16 + velocity.Y < Tiles[i, x].position.Y + 32)
                            {
                                //to the bottom
                                velocity.Y = 0;
                                position.Y= Tiles[i, x].position.Y + 48;
                            }
                            if (position.Y < Tiles[i, x].position.Y && position.Y + 16 + velocity.Y > Tiles[i, x].position.Y)
                            {
                                //to the top
                                velocity.Y = 0;
                                position.Y = Tiles[i, x].position.Y- 16;
                            }
                        }


                    }
                    //test position

                }
            }
                //corners
            for (int i = 0; i < 3; i++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (Tiles[i, x].solid)
                    {
                        if (position.X > Tiles[i, x].position.X + 32)
                        {
                            if (position.Y > Tiles[i, x].position.Y + 32)
                            {
                                //bottom right
                                Closest = Tiles[i, x].position + new Vector2(32, 32);

                                float distance = (float)Math.Sqrt((position.Y + velocity.Y - Closest.Y) * (position.Y + velocity.Y - Closest.Y) + (position.X + velocity.X - Closest.X) * (position.X + velocity.X - Closest.X));
                                if (distance < 16)
                                {
                                    float Currentrotation = (float)(Math.Atan2(Math.Abs(position.Y + velocity.Y - Closest.Y), Math.Abs(position.X + velocity.X - Closest.X))); // update rotation
                                    velocity = Vector2.Zero;
                                    position = Closest + new Vector2((float)Math.Cos(Currentrotation) * 16, (float)Math.Sin(Currentrotation) * 16);

                                }

                            }

                            if (position.Y < Tiles[i, x].position.Y)
                            {
                                Closest = Tiles[i, x].position + new Vector2(32, 0);

                                float distance = (float)Math.Sqrt((position.Y + velocity.Y - Closest.Y) * (position.Y + velocity.Y - Closest.Y) + (position.X + velocity.X - Closest.X) * (position.X + velocity.X - Closest.X));
                                if (distance < 16)
                                {
                                    float Currentrotation = (float)(Math.Atan2(Math.Abs(position.Y + velocity.Y - Closest.Y), Math.Abs(position.X + velocity.X - Closest.X))); // update rotation
                                    velocity = Vector2.Zero;
                                    position = Closest + new Vector2((float)Math.Cos(Currentrotation) * 16, -(float)Math.Sin(Currentrotation) * 16);

                                }
                            }

                        }
                        else if (position.X < Tiles[i, x].position.X)
                        {
                            if (position.Y > Tiles[i, x].position.Y + 32)
                            {
                                //below left
                                Closest = Tiles[i, x].position + new Vector2(0, 32);

                                float distance = (float)Math.Sqrt((position.Y + velocity.Y - Closest.Y) * (position.Y + velocity.Y - Closest.Y) + (position.X + velocity.X - Closest.X) * (position.X + velocity.X - Closest.X));
                                if (distance < 16)
                                {
                                    float Currentrotation = (float)(Math.Atan2(Math.Abs(position.Y + velocity.Y - Closest.Y), Math.Abs(position.X + velocity.X - Closest.X))); // update rotation
                                    velocity = Vector2.Zero;
                                    position = Closest + new Vector2(-(float)Math.Cos(Currentrotation) * 16, (float)Math.Sin(Currentrotation) * 16);

                                }
                            }

                            if (position.Y < Tiles[i, x].position.Y)
                            {
                                //above left
                                Closest = Tiles[i, x].position + new Vector2(0, 0);

                                float distance = (float)Math.Sqrt((position.Y + velocity.Y - Closest.Y) * (position.Y + velocity.Y - Closest.Y) + (position.X + velocity.X - Closest.X) * (position.X + velocity.X - Closest.X));
                                if (distance < 16)
                                {
                                    float Currentrotation = (float)(Math.Atan2(Math.Abs(position.Y + velocity.Y - Closest.Y), Math.Abs(position.X + velocity.X - Closest.X))); // update rotation
                                    velocity = Vector2.Zero;
                                    position = Closest + new Vector2(-(float)Math.Cos(Currentrotation) * 16, -(float)Math.Sin(Currentrotation) * 16);

                                }
                            }
                        }
                    }
                }
            }
                //endcorners

            
        }

        

        // Draws the display
        public void DrawHUD(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(staminaFront, staminaPosition, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
            spriteBatch.Draw(staminaFront, staminaPosition, new Rectangle(0, 0, rollCD * 256 / 100, 32), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
            //spriteBatch.Draw(healthBack, healthPosition, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
            //spriteBatch.Draw(healthFront, healthPosition, new Rectangle(0, 0,(int)health * 256 / 100, 32), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
        }
        
    }
}
