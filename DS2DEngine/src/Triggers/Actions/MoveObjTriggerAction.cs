using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tweener;

namespace DS2DEngine
{
    //TODO:  Change to TweenTriggerAction
    public class MoveObjTriggerAction : TriggerAction
    {
        private GameObj m_objToMove;
        private float m_duration;
        private Easing m_easeType;
        private Vector2 m_pos;
        private TweenObject m_tween;
        private bool m_moveTo;

        public MoveObjTriggerAction(GameObj objToMove, bool moveTo, Vector2 newPos, float timeInSeconds, Easing easeType)
        {
            m_objToMove = objToMove;
            m_duration = timeInSeconds;
            m_easeType = easeType;
            m_pos = newPos;
            m_moveTo = moveTo;
        }

        public override void Execute()
        {
            if (ParentTriggerSet.IsActive)
            {
                if (m_moveTo == true)
                    m_tween = Tween.To(m_objToMove, m_duration, m_easeType, "X", m_pos.X.ToString(), "Y", m_pos.Y.ToString());
                else
                    m_tween = Tween.By(m_objToMove, m_duration, m_easeType, "X", m_pos.X.ToString(), "Y", m_pos.Y.ToString());

                base.Execute();
            }
        }

        public override void ExecuteNext()
        {
            if (NextTrigger != null)
                m_tween.EndHandler(NextTrigger, "Execute");
            else
                m_tween.EndHandler(ParentTriggerSet, "TriggerComplete");
        }

        public override void Stop()
        {
            //m_tween.StopTween(false);
        }

        public override TriggerAction Clone()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
