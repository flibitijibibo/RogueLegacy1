using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class ChaseLogicAction : LogicAction
    {
        private bool m_moveTowards;
        private GameObj m_target;
        private float m_speed;
        private float m_duration;
        private float m_durationCounter;
        private Vector2 m_minRelativePos = Vector2.Zero;
        private Vector2 m_maxRelativePos = Vector2.Zero;
        private Vector2 m_storedRelativePos;

        //To stop an object from moving, call this method and set overrideSpeed to 0.
        public ChaseLogicAction(GameObj target, bool moveTowards, float duration, float overrideSpeed = -1)
        {
            m_moveTowards = moveTowards;
            m_target = target;
            m_speed = overrideSpeed;
            m_duration = duration;
        }

        public ChaseLogicAction(GameObj target, Vector2 minRelativePos, Vector2 maxRelativePos, bool moveTowards, float duration, float overrideSpeed = -1)
        {
            m_minRelativePos = minRelativePos;
            m_maxRelativePos = maxRelativePos;
            m_moveTowards = moveTowards;
            m_target = target;
            m_speed = overrideSpeed;
            m_duration = duration;
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
                    m_durationCounter = m_duration;

                    m_storedRelativePos = new Vector2(CDGMath.RandomInt((int)m_minRelativePos.X, (int)m_maxRelativePos.X), CDGMath.RandomInt((int)m_minRelativePos.Y, (int)m_maxRelativePos.Y));

                    Vector2 seekPosition;

                    if (m_moveTowards == true)
                        seekPosition = m_target.Position + m_storedRelativePos;
                    else
                        seekPosition = 1000 * this.ParentLogicSet.ParentGameObj.Position - m_target.Position;

                    TurnToFace(seekPosition, this.ParentLogicSet.ParentGameObj.TurnSpeed, 1/60f);

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

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_durationCounter -= elapsedSeconds;
            if (m_target != null && m_durationCounter > 0)
            {
                Vector2 seekPosition;
                if (m_moveTowards == true)
                    seekPosition = m_target.Position + m_storedRelativePos;
                else
                    seekPosition = 2 * this.ParentLogicSet.ParentGameObj.Position - m_target.Position;

                TurnToFace(seekPosition, this.ParentLogicSet.ParentGameObj.TurnSpeed, elapsedSeconds);

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
            this.ExecuteNext();
            base.Update(gameTime);
        }

        public override void ExecuteNext()
        {
            if (m_durationCounter <= 0)
                base.ExecuteNext();
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
            return new ChaseLogicAction(m_target, m_minRelativePos, m_maxRelativePos, m_moveTowards, m_duration, m_speed);
        }

        public void TurnToFace(float angle, float turnSpeed, float elapsedSeconds)
        {
            float desiredAngle = angle;
            float difference = MathHelper.WrapAngle(desiredAngle - this.ParentLogicSet.ParentGameObj.Orientation);

            float elapsedTurnSpeed = (turnSpeed * 60) * elapsedSeconds;

            difference = MathHelper.Clamp(difference, -elapsedTurnSpeed, elapsedTurnSpeed);
            this.ParentLogicSet.ParentGameObj.Orientation = MathHelper.WrapAngle(this.ParentLogicSet.ParentGameObj.Orientation + difference);
        }

        public void TurnToFace(Vector2 facePosition, float turnSpeed, float elapsedSeconds)
        {
            float x = facePosition.X - this.ParentLogicSet.ParentGameObj.Position.X;
            float y = facePosition.Y - this.ParentLogicSet.ParentGameObj.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = MathHelper.WrapAngle(desiredAngle - this.ParentLogicSet.ParentGameObj.Orientation);

            float elapsedTurnSpeed = (turnSpeed * 60) * elapsedSeconds;

            difference = MathHelper.Clamp(difference, -elapsedTurnSpeed, elapsedTurnSpeed);
            this.ParentLogicSet.ParentGameObj.Orientation = MathHelper.WrapAngle(this.ParentLogicSet.ParentGameObj.Orientation + difference);
        }
    }
}
