using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class LockFaceDirectionLogicAction : LogicAction
    {
        private int m_forceDirection = 0;
        private bool m_lockFace = false;

        public LockFaceDirectionLogicAction(bool lockFace, int forceDirection = 0)
        {
            m_lockFace = lockFace;
            m_forceDirection = forceDirection;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                CharacterObj obj = ParentLogicSet.ParentGameObj as CharacterObj;
                if (obj != null)
                {
                    obj.LockFlip = m_lockFace;

                    if (m_forceDirection > 0)
                        obj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                    else if (m_forceDirection < 0)
                        obj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                }
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new LockFaceDirectionLogicAction(m_lockFace, m_forceDirection);
        }
    }
}
