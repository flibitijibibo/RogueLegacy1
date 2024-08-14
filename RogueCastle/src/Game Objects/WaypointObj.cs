using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class WaypointObj : GameObj
    {
        public int OrbType = 0;

        protected override GameObj CreateCloneInstance()
        {
            return new WaypointObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            WaypointObj clone = obj as WaypointObj;
            clone.OrbType = this.OrbType;
        }
    }
}
