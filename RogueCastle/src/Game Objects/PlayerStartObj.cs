using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class PlayerStartObj : GameObj
    {
        protected override GameObj CreateCloneInstance()
        {
            return new PlayerStartObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }
    }
}
