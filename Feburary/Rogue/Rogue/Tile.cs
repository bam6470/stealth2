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
    class Tile
    {
        public int visibleTile;
        public bool solid;
        public Vector2 position;
        public Tile(int VisibleTile, Vector2 Position)
        {
            position = Position;
            visibleTile = VisibleTile;
        }
        public void SetSolid(bool Solid)
        {
            solid = Solid;
        }
    }
}
