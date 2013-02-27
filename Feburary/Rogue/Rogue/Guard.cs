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
    class Guard
    {
        Vector2 position;
        Texture2D guardSheet;
        float rotation;
        double speed = 3;
        Vector2[] rightBalls = new Vector2[9];
        Vector2[] leftBalls = new Vector2[9];
        int actionTimer = 0;

        // Automation stuff
        int pathNumb = 0;
        bool hasPath;
        List<GuardAction> path;

        // Line of sight stuff
        public bool seenPlayer = false;
        int viewDistance = 200;
        int viewAngle = 60;

        // Rectangle specifing the section of the sprite sheet for the guard
        static Rectangle guardRect = new Rectangle(
            32,
            0,
            32,
            32);
        // Rectangle specifing the section of the sprite sheet for the guards line of sight ball
        static Rectangle boundaryRect = new Rectangle(
            0,
            0,
            15,
            15);

        public Guard(Vector2 pos, Texture2D sprites, float radians)
            : this(pos, sprites, radians, false, null)
        {
            // Intentionally empty
        }

        public Guard(Vector2 pos, Texture2D sprites, float radians, bool boolPath, List<GuardAction> pathing)
        {
            position = pos;
            guardSheet = sprites;
            hasPath = boolPath;
            path = pathing;
            rotation = radians;
            double height = (viewDistance / 5) * (Math.Cos(.5 * (viewAngle * (Math.PI / 180))));
            double width = (viewDistance / 5) * (Math.Sin(.5 * (viewAngle * (Math.PI / 180))));
            // Finds the correct positions for the balls on the side of the view area
            for (int i = 1; i <= 5; i++)
            {
                leftBalls[i - 1] = new Vector2((float)(-1 * (width * i)), (float)((height * i)));
                rightBalls[i - 1] = new Vector2((float)(width * i), (float)((height * i)));
            }
            int modangle = viewAngle / 4;
            // Finds the correct positions for the ball on the outer edge of the view area
            for (int i = 1; i <= 4; i++)
            {
                rightBalls[i + 4] = new Vector2((float)(viewDistance * (Math.Sin(.5 * ((viewAngle - (modangle * i)) * Math.PI / 180)))), (float)(viewDistance * (Math.Cos(.5 * ((viewAngle - (modangle * i)) * Math.PI / 180)))));
                leftBalls[i + 4] = new Vector2((float)(-1 * (viewDistance * (Math.Sin(.5 * ((viewAngle - (modangle * i)) * Math.PI / 180))))), (float)(viewDistance * (Math.Cos(.5 * ((viewAngle - (modangle * i)) * Math.PI / 180)))));
            }
        }

        public void Update(Player player, GameTime gameTime)
        {
            // Update position
            if (hasPath && !seenPlayer)
            {
                if (pathNumb >= path.Count)
                {
                    pathNumb = 0;
                }
                // Finds what command
                if (path[pathNumb].command == GuardCommand.MoveTo)
                {
                    Vector2 movement = new Vector2(); // Stores the x and y movement
                    double moveSpeed;
                    GuardMove action = (GuardMove)path[pathNumb]; // Converts the path[pathNumb] to a GuardMove object in order to use the GuardObject members
                    if (action.distance - action.traveled >= speed) // Gtes the correct distance to move
                    {
                        moveSpeed = speed;
                    }
                    else // If te guard needs to move less than the speed it will make the movespeed = to the distance it needs to go
                    {
                        moveSpeed = action.distance - action.traveled;
                    }
                    // Finds the x and the y movements depending on the direction
                    // If the guard is moving in any direction other than down and to the right the x or y movement is multiplied by -1 to acomodate
                    if (rotation < (90 * Math.PI / 180) || ((rotation - (90 * Math.PI / 180)) < .00009))
                    {
                        movement.X = (float)(Math.Sin(rotation) * speed);
                        movement.Y = (float)(-1 * (Math.Cos(rotation) * speed));
                    }
                    else if (rotation < (180 * Math.PI / 180) || ((rotation - (180 * Math.PI / 180)) < .00009))
                    {
                        movement.X = (float)(Math.Cos(rotation - (90 * Math.PI / 180)) * speed);
                        movement.Y = (float)(Math.Sin(rotation - (90 * Math.PI / 180)) * speed);
                    }
                    else if (rotation < (270 * Math.PI / 180) || ((rotation - (270 * Math.PI / 180)) < .00009))
                    {
                        movement.X = (float)(-1 * (Math.Sin(rotation - (180 * Math.PI / 180)) * speed));
                        movement.Y = (float)(Math.Cos(rotation - (180 * Math.PI / 180)) * speed);
                    }
                    else
                    {
                        movement.X = (float)(-1 * (Math.Cos(rotation - (270 * Math.PI / 180))) * speed);
                        movement.Y = (float)(-1 * (Math.Sin(rotation - (270 * Math.PI / 180))) * speed);
                    }
                    // Moves the guard
                    position += movement;
                    // Updates the traveled
                    action.traveled += moveSpeed;
                    // If the guard has moved the specified distance this code is run to rest the object and move to the next object
                    if (action.traveled == action.distance)
                    {
                        pathNumb++;
                        action.traveled = 0;
                        path[pathNumb - 1] = action;
                    }
                    else
                    {
                        path[pathNumb] = action;
                    }
                }
                else if (path[pathNumb].command == GuardCommand.Look)
                {
                    GuardRotate guardRotate = (GuardRotate)path[pathNumb];
                    // If this is the first time running this node reset the originalRot
                    if (actionTimer == 0)
                    {
                        guardRotate.originalRot = rotation;
                    }
                    // Update elapsed time
                    actionTimer += gameTime.ElapsedGameTime.Milliseconds;
                    // Finds the amount of rotation that is needed based on how much time has passed
                    if (guardRotate.time > actionTimer)
                    {
                        rotation = ((guardRotate.radians * actionTimer) / guardRotate.time) + guardRotate.originalRot;
                    }
                    else // Runs this code on the last pass of this node
                    {
                        rotation = guardRotate.radians + guardRotate.originalRot;
                        path[pathNumb] = guardRotate;
                        pathNumb++;
                        actionTimer = 0;
                    }
                }
            }
            // Check if player is in line of sight
            // Check if the player is within distance to be seen
            if ((getDistance(player.position, position) <= viewDistance) && !seenPlayer)
            {
                double playerAngle = getAngle(position, player.position);
                double angleDifferance;
                // If the angle of the playerAngle is below zero and the rotation - half view angle is above 0
                if ((playerAngle + .5 * (viewAngle * Math.PI / 180) >= 360 * Math.PI / 180) &&
                    rotation - .5 * (viewAngle * Math.PI / 180) <= 360 * Math.PI / 180)
                {
                    angleDifferance = 0 - (rotation + (Math.Abs(playerAngle - 360 * Math.PI / 180)));
                }
                // If the angle of the rotation + half view angle is below zero and the playerAngle is above 0
                else if ((rotation + .5 * (viewAngle * Math.PI / 180) >= 360 * Math.PI / 180) &&
                    playerAngle - .5 * (viewAngle * Math.PI / 180) <= 360 * Math.PI / 180)
                {
                    angleDifferance = 0 - (playerAngle + (Math.Abs(rotation - 360 * Math.PI / 180)));
                }
                else
                {
                    angleDifferance = playerAngle - rotation;
                }
                // Find if the player is in the guards view angle
                if (Math.Abs(angleDifferance) < .5 * viewAngle * Math.PI / 180)
                {
                    seenPlayer = true;
                    actionTimer = 0;
                }
            }
            // Logic for hunting down player when seen
            if (seenPlayer)
            {
                actionTimer += gameTime.ElapsedGameTime.Milliseconds;
                // Look in the players direction
                if (actionTimer < 250)
                {
                    rotation = (float)(getAngle(position, player.position));
                }
                // Lunge twords the player
                else if (actionTimer < 750)
                {
                    double distanceX = ((position.X - player.position.X) / 4);
                    double distanceY = ((position.Y - player.position.Y) / 4);
                    position.X -= (float)distanceX;
                    position.Y -= (float)distanceY;
                }
                // Reset the actiontimer for repeating
                else
                {
                    actionTimer = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            //draw the guard
            spriteBatch.Draw(guardSheet, position + player.offset, guardRect, Color.White, rotation, new Vector2(16, 16), 1f, SpriteEffects.None, 1);
            //draw the guards line of sight ball
            foreach (Vector2 ball in rightBalls)
            {
                spriteBatch.Draw(guardSheet, position + player.offset, new Rectangle(0, 0, 16, 16), Color.White, rotation, new Vector2(8 + ball.X, 8 + ball.Y), 1f, SpriteEffects.None, 1);
            }
            foreach (Vector2 ball in leftBalls)
            {
                spriteBatch.Draw(guardSheet, position + player.offset, new Rectangle(0, 0, 16, 16), Color.White, rotation, new Vector2(8 + ball.X, 8 + ball.Y), 1f, SpriteEffects.None, 1);
            }
        }
        // Returns the distance of 2 points
        public double getDistance(Vector2 one, Vector2 two)
        {
            double retDouble;
            retDouble = Math.Sqrt(Math.Pow(one.X - two.X, 2) + Math.Pow(one.Y - two.Y, 2));
            return retDouble;
        }
        public double getAngle(Vector2 one, Vector2 two)
        {
            double retDouble;
            // Finds what quadrent two is in and applys the correct math to rotate it
            if (one.X > two.X)
            {
                if (one.Y > two.Y)
                {
                    retDouble = (Math.Atan2(Math.Abs(one.Y - two.Y), Math.Abs(one.X - two.X)) + 270 * Math.PI / 180);
                }
                else
                {
                    retDouble = (Math.Atan2(Math.Abs(one.X - two.X), Math.Abs(one.Y - two.Y)) + 180 * Math.PI / 180);
                }
            }
            else
            {
                if (one.Y > two.Y)
                {
                    retDouble = (Math.Atan2(Math.Abs(one.X - two.X), Math.Abs(one.Y - two.Y)));

                }
                else
                {
                    retDouble = (Math.Atan2(Math.Abs(one.Y - two.Y), Math.Abs(one.X - two.X)) + 90 * Math.PI / 180);
                }
            }
            return retDouble;
        }
    }

    // base class for guard actions each action has its own derived class
    abstract class GuardAction
    {
        public GuardCommand command;
    }

    // Moves the guard "distance" pixles int the direction the guard is facing
    class GuardMove : GuardAction
    {
        public double distance; // In pixles
        public double traveled;

        public GuardMove(int dist)
        {
            command = GuardCommand.MoveTo;
            distance = dist;
            traveled = 0;
        }
    }

    // Ratates the guard "radians" over the course of "time"
    class GuardRotate : GuardAction
    {
        public float radians; // Can be negitive
        public int time;
        public float originalRot;

        public GuardRotate(float rad, int t)
        {
            command = GuardCommand.Look;
            radians = rad;
            time = t;
            originalRot = 0;
        }
    }

    // Holds possible guard commands
    enum GuardCommand
    {
        MoveTo, // Need direction and distance
        Look // Need time and direction
    }
}