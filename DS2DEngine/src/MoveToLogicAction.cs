using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class MoveToLogicAction : LogicAction
    {
        private Vector2 m_targetPos;
        private float m_speed;

        //To stop an object from moving, call this method and set overrideSpeed to 0.
        public MoveToLogicAction(Vector2 targetPos, float overrideSpeed = -1)
        {
            m_targetPos = targetPos;
            m_speed = overrideSpeed;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                if (m_speed == -1)
                    m_speed = this.ParentLogicSet.ParentGameObj.Speed;

                this.ParentLogicSet.ParentGameObj.CurrentSpeed = m_speed;

                GameObj mover = ParentLogicSet.ParentGameObj;
                mover.Heading = new Vector2(m_targetPos.X - mover.X, m_targetPos.Y - mover.Y);

                if (this.ParentLogicSet.ParentGameObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    this.ParentLogicSet.ParentGameObj.Heading = new Vector2(1, 0);
                else
                    this.ParentLogicSet.ParentGameObj.Heading = new Vector2(-1, 0);
                base.Execute();
            }
        }

        public override void ExecuteNext()
        {
            if (CDGMath.DistanceBetweenPts(ParentLogicSet.ParentGameObj.Position, m_targetPos) <= 10)
            {
                ParentLogicSet.ParentGameObj.Position = m_targetPos;
                base.ExecuteNext();
            }
        }


        public override void Stop()
        {
            this.ParentLogicSet.ParentGameObj.CurrentSpeed = 0;
            base.Stop();
        }

        public override object Clone()
        {
            return new MoveToLogicAction(m_targetPos, m_speed);
        }
    }
}
