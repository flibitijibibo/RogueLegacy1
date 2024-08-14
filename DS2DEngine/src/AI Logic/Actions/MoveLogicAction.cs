using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class MoveLogicAction : LogicAction
    {
        private bool m_moveTowards;
        private GameObj m_target;
        private float m_speed;

        //To stop an object from moving, call this method and set overrideSpeed to 0.
        public MoveLogicAction(GameObj target, bool moveTowards, float overrideSpeed = -1)
        {
            m_moveTowards = moveTowards;
            m_target = target;
            m_speed = overrideSpeed;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                if (m_speed == -1)
                    m_speed = this.ParentLogicSet.ParentGameObj.Speed;

                this.ParentLogicSet.ParentGameObj.CurrentSpeed = m_speed;

                if (m_target != null)
                {
                    Vector2 seekPosition;

                    if (m_moveTowards == true)
                        seekPosition = m_target.Position;
                    else
                        seekPosition = 2 * this.ParentLogicSet.ParentGameObj.Position - m_target.Position;

                    TurnToFace(seekPosition, this.ParentLogicSet.ParentGameObj.TurnSpeed);

                    this.ParentLogicSet.ParentGameObj.HeadingX = (float)Math.Cos(this.ParentLogicSet.ParentGameObj.Orientation);
                    this.ParentLogicSet.ParentGameObj.HeadingY = (float)Math.Sin(this.ParentLogicSet.ParentGameObj.Orientation);

                    if (ParentLogicSet.ParentGameObj.LockFlip == false)
                    {
                        if (ParentLogicSet.ParentGameObj.X > m_target.X)
                            ParentLogicSet.ParentGameObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                        else
                            ParentLogicSet.ParentGameObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                    }
                }
                base.Execute();
            }
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
                m_target = null; // This class is not the class that should be disposing the target.
                base.Dispose();
            }
        }

        public override object Clone()
        {
            return new MoveLogicAction(m_target, m_moveTowards, m_speed);
        }

        public void TurnToFace(float angle, float turnSpeed)
        {
            float desiredAngle = angle;
            float difference = MathHelper.WrapAngle(desiredAngle - this.ParentLogicSet.ParentGameObj.Orientation);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            this.ParentLogicSet.ParentGameObj.Orientation = MathHelper.WrapAngle(this.ParentLogicSet.ParentGameObj.Orientation + difference);
        }

        public void TurnToFace(Vector2 facePosition, float turnSpeed)
        {
            float x = facePosition.X - this.ParentLogicSet.ParentGameObj.Position.X;
            float y = facePosition.Y - this.ParentLogicSet.ParentGameObj.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = MathHelper.WrapAngle(desiredAngle - this.ParentLogicSet.ParentGameObj.Orientation);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            this.ParentLogicSet.ParentGameObj.Orientation = MathHelper.WrapAngle(this.ParentLogicSet.ParentGameObj.Orientation + difference);
        }
    }
}
