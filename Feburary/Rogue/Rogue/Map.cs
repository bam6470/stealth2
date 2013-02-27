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
    class Map
    {
        Tile[,] MapArray;
        int MapWidth, MapHeight, TileWidth, TileHeight;
        int WidthMap, HeightMap;
        string line;
        StreamReader reader;
        Texture2D tileSet;
        Vector2 playerOffset;
        public Map(Texture2D TileSet)
        {

            tileSet = TileSet;
        }


        public Tile[,] GetTiles(Vector2 position)
        {
            Tile[,] tiles = new Tile[3, 3];
            Rectangle posRect = new Rectangle((int)position.X - 16 - TileWidth / 2, (int)position.Y - 16 - TileWidth / 2, 32 + TileWidth, 32 + TileHeight);
            for (int i = 0; i < MapHeight; i++)
            {
                
                    for (int x = 0; x < MapWidth; x++)
                    {
                        if (posRect.Intersects(new Rectangle(x * TileWidth+(int)playerOffset.X, i * TileHeight+ (int)playerOffset.Y, 32, 32))) 
                        {//writng loop
                            int temp;
                            for (int a = 0; a < 3; a++)
                            {
                                temp = x;
                                for (int b = 0; b < 3; b++)
                                {
                                    tiles[a, b] = MapArray[i, temp];
                                    temp++;
                                }
                                
                                i++;
                            }
                            return tiles;
                            //end of writing loop
                        }
                    }
                
            }
            //if it didn't work
            return tiles;
        }







        public void LoadMap(StreamReader Reader, Texture2D TileSet)
        {
            tileSet = TileSet;
            reader = Reader;
            //get map width
            line = reader.ReadLine();
            MapWidth = Convert.ToInt16(line.Substring(line.Length - 2));
            //get map height
            line = reader.ReadLine();
            MapHeight = Convert.ToInt16(line.Substring(line.Length - 2));
            //get tile width
            line = reader.ReadLine();
            TileWidth = Convert.ToInt16(line.Substring(line.Length - 2));
            //get tile hight
            line = reader.ReadLine();
            //set map array
            TileHeight = Convert.ToInt16(line.Substring(line.Length - 2));
            //skip layer
            line = reader.ReadLine();
            line = reader.ReadLine();


            MapArray = new Tile[MapHeight, MapWidth];
            

            for (int i = 0; i < MapHeight; i++)
            {
                line = reader.ReadLine();
                for (int x = 0; x < MapWidth; x++)
                {

                    MapArray[i, x] = new Tile(Convert.ToInt32(line.Substring(0, line.IndexOf(",", 0))),new Vector2(x*TileWidth,i*TileHeight));
                    line = line.Remove(0, line.IndexOf(",", 0) + 1);

                }
            }
            line = reader.ReadLine();
            for (int i = 0; i < MapHeight; i++)
            {
                line = reader.ReadLine();
                for (int x = 0; x < MapWidth; x++)
                {

                    MapArray[i, x].SetSolid(Convert.ToBoolean(Convert.ToInt32(line.Substring(0, line.IndexOf(",", 0)))));
                    line = line.Remove(0, line.IndexOf(",", 0) + 1);

                }
            }

        }
        public void DrawMap(SpriteBatch spriteBatch, Vector2 PlayerOffset, int WidthMapI, int HeightMapI)
        {
            WidthMap = WidthMapI;
            HeightMap = HeightMapI;

            playerOffset = PlayerOffset;
            for (int i = 0; i < MapHeight; i++)
            {
                if (i * TileHeight + playerOffset.Y > -31 && i * TileHeight + playerOffset.Y < HeightMap)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        if (x * TileWidth + playerOffset.X > -31 && x * TileWidth + playerOffset.X < WidthMap)
                            spriteBatch.Draw(tileSet, new Vector2(x * TileHeight+ (int)playerOffset.X, i * TileWidth+(int)playerOffset.Y), new Rectangle(((MapArray[i, x].visibleTile) % (tileSet.Width / TileWidth)) * TileWidth, (MapArray[i, x].visibleTile / (tileSet.Width / TileWidth)) * TileWidth, TileWidth, TileHeight), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                    }
                }
            }


        }

    }
}

