using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class MoveDirectionLogicAction : LogicAction
    {
        private Vector2 m_direction;
        private float m_speed;
        private bool m_moveBasedOnFlip = false;

        //To stop an object from moving, call this method and set overrideSpeed to 0.
        public MoveDirectionLogicAction(Vector2 direction, float overrideSpeed = -1)
        {
            m_direction = direction;
            m_speed = overrideSpeed;
            m_direction.Normalize();
            m_moveBasedOnFlip = false;
        }

        // Move an object based on its flip.
        public MoveDirectionLogicAction(float overrideSpeed = -1)
        {
            m_moveBasedOnFlip = true;
            m_speed = overrideSpeed;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                if (m_speed == -1)
                    m_speed = this.ParentLogicSet.ParentGameObj.Speed;

                if (m_moveBasedOnFlip == false)
                {
                    this.ParentLogicSet.ParentGameObj.CurrentSpeed = m_speed;
                    this.ParentLogicSet.ParentGameObj.Heading = m_direction;
                }
                else
                {
                    this.ParentLogicSet.ParentGameObj.CurrentSpeed = m_speed;
                    if (this.ParentLogicSet.ParentGameObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                        this.ParentLogicSet.ParentGameObj.Heading = new Vector2(1, 0);
                    else
                        this.ParentLogicSet.ParentGameObj.Heading = new Vector2(-1, 0);
                }

                base.Execute();
            }
           /* Vector2 seekPosition;

            if (m_moveTowards == true)
                seekPosition = m_target.Position;
            else
                seekPosition = 2 * this.ParentLogicSet.ParentGameObj.Position - m_target.Position;

            TurnToFace(seekPosition, this.ParentLogicSet.ParentGameObj.TurnSpeed);

            this.ParentLogicSet.ParentGameObj.HeadingX = (float)Math.Cos(this.ParentLogicSet.ParentGameObj.Orientation);
            this.ParentLogicSet.ParentGameObj.HeadingY = (float)Math.Sin(this.ParentLogicSet.ParentGameObj.Orientation);
            */

        }

        public override void Stop()
        {
            this.ParentLogicSet.ParentGameObj.CurrentSpeed = 0;
            base.Stop();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                base.Dispose();
            }
        }

        public override object Clone()
        {
            if (m_direction == Vector2.Zero)
                return new MoveDirectionLogicAction(m_speed);
            return new MoveDirectionLogicAction(m_direction, m_speed);
        }
    }
}
