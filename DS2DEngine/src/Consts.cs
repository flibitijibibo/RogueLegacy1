using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class Consts
    {
        public const bool DEBUG_ShowSafeZones = true;

        public const float FRAMERATE_CAP = 1 / 30f;

        // Hitbox Consts.
        public const int NULL_HITBOX = -1;
        public const int TERRAIN_HITBOX = 0;
		public const int WEAPON_HITBOX = 1;
		public const int BODY_HITBOX = 2;

        public const int COLLISIONRESPONSE_NULL = 0;
        public const int COLLISIONRESPONSE_TERRAIN = 1;
        public const int COLLISIONRESPONSE_FIRSTBOXHIT = 2;
        public const int COLLISIONRESPONSE_SECONDBOXHIT = 3;
    }
}
