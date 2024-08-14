using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    class InputMapType
    {
        // Menu Input
        public const byte MENU_CONFIRM1 = 0;
        public const byte MENU_CONFIRM2 = 1;
        public const byte MENU_CANCEL1 = 2;
        public const byte MENU_CANCEL2 = 3;
        public const byte MENU_OPTIONS = 4;
        public const byte MENU_ROGUEMODE = 5;
        public const byte MENU_CREDITS = 6;
        public const byte MENU_PROFILECARD = 7;
        public const byte MENU_PAUSE = 8;
        public const byte MENU_MAP = 9;

        // Player Input
        public const byte PLAYER_JUMP1 = 10;
        public const byte PLAYER_JUMP2 = 11;
        public const byte PLAYER_ATTACK = 12;
        public const byte PLAYER_BLOCK = 13;
        public const byte PLAYER_DASHLEFT = 14;
        public const byte PLAYER_DASHRIGHT = 15;
        public const byte PLAYER_UP1 = 16;
        public const byte PLAYER_UP2 = 17;
        public const byte PLAYER_DOWN1 = 18;
        public const byte PLAYER_DOWN2 = 19;
        public const byte PLAYER_LEFT1 = 20;
        public const byte PLAYER_LEFT2 = 21;
        public const byte PLAYER_RIGHT1 = 22;
        public const byte PLAYER_RIGHT2 = 23;
        public const byte PLAYER_SPELL1 = 24;

        public const byte MENU_PROFILESELECT = 25;
        public const byte MENU_DELETEPROFILE = 26;

        public const byte MENU_CONFIRM3 = 27;
        public const byte MENU_CANCEL3 = 28;

        public const byte Total = 29;
    }
}
