using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class GroundCheckLogicAction : LogicAction
    {
        CharacterObj m_obj = null;

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                m_obj = ParentLogicSet.ParentGameObj as CharacterObj;
                SequenceType = Types.Sequence.Serial;
                base.Execute();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.ExecuteNext();
            base.Update(gameTime);
        }

        public override void ExecuteNext()
        {
            if (m_obj.IsTouchingGround == true)
                base.ExecuteNext();
        }

        public override object Clone()
        {
            return new GroundCheckLogicAction();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_obj = null;
                base.Dispose();
            }
        }
    }
}
