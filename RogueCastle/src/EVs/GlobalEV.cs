using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class GlobalEV
    {
        //Base screen resolution size for the game. 1320x720 == 60x60 tiles
        public const int ScreenWidth = 1320; //1366; 
        public const int ScreenHeight = 720; //768;

        public const float GRAVITY = -1830;//-30.5f;//-28.5f;//-27f; //-15f;
        public static float Camera_XOffset = 0;
        public static float Camera_YOffset = -22;//2; //-23;//50; // The amount the camera is offset by when following the player.
    }
}
