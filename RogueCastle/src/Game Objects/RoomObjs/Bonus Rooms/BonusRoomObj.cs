using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    public class BonusRoomObj : RoomObj
    {
        public bool RoomCompleted { get; set; }

        public BonusRoomObj()
        {
            this.ID = -1;
        }

        public override void Reset()
        {
            RoomCompleted = false;
            base.Reset();
        }
    }
}
