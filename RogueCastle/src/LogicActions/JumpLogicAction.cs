using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class JumpLogicAction : LogicAction
    {
        private float m_overriddenHeight;

        public JumpLogicAction(float overriddenHeight = 0)
        {
            m_overriddenHeight = overriddenHeight;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                CharacterObj character = ParentLogicSet.ParentGameObj as CharacterObj;
                if (character != null)
                {
                    if (m_overriddenHeight > 0)
                        character.AccelerationY = -m_overriddenHeight;
                    else
                        character.AccelerationY = -character.JumpHeight;
                }

                base.Execute();
            }
        }

        public override object Clone()
        {
            return new JumpLogicAction(m_overriddenHeight);
        }
    }
}
