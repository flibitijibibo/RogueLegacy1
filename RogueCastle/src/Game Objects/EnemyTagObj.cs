using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    // Represent a tag that tells the game where enemies should be placed once they are procedurally added to a level.
    public class EnemyTagObj : GameObj
    {
        public string EnemyType { get; set; }

        protected override GameObj CreateCloneInstance()
        {
            return new EnemyTagObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            EnemyTagObj clone = obj as EnemyTagObj;
            clone.EnemyType = this.EnemyType; 
        }
    }
}
