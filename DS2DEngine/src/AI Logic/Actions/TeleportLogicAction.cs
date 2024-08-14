using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class TeleportLogicAction : LogicAction
    {
        GameObj m_target = null;
        Vector2 m_newPosition;
        Vector2 m_minPosition;
        Vector2 m_maxPosition;

        public TeleportLogicAction(GameObj target, Vector2 relativePos)
        {
            m_target = target;
            m_newPosition = relativePos;
        }

        public TeleportLogicAction(GameObj target, Vector2 minPos, Vector2 maxPos)
        {
            m_target = target;
            m_minPosition = minPos;
            m_maxPosition = maxPos;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                if (m_minPosition != m_maxPosition)
                    m_newPosition = new Vector2(CDGMath.RandomInt((int)m_minPosition.X, (int)m_maxPosition.X), CDGMath.RandomInt((int)m_minPosition.Y, (int)m_maxPosition.Y));

                if (m_target == null)
                    ParentLogicSet.ParentGameObj.Position = m_newPosition;
                else
                    ParentLogicSet.ParentGameObj.Position = m_target.Position + m_newPosition;
                base.Execute();
            }
        }

        public override object Clone()
        {
            if (m_minPosition != m_maxPosition)
                return new TeleportLogicAction(m_target, m_minPosition, m_maxPosition);
            else
                return new TeleportLogicAction(m_target, m_newPosition);
        }

        public override void Dispose()
        {
            if (IsDisposed == true)
            {
                m_target = null;
                base.Dispose();
            }
        }
    }
}
