using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class EnemyOrbObj : GameObj
    {
        public int OrbType = 0;
        public bool ForceFlying { get; set; }

        protected override GameObj CreateCloneInstance()
        {
            return new EnemyOrbObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            EnemyOrbObj clone = obj as EnemyOrbObj;
            clone.OrbType = this.OrbType;
            clone.ForceFlying = this.ForceFlying;
        }
    }
}
