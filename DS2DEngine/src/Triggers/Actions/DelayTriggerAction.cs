using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweener;

namespace DS2DEngine
{
    public class DelayTriggerAction : TriggerAction
    {
        private float m_duration;
        private BlankObj m_blankObj;
        private TweenObject m_tween;

        public DelayTriggerAction(float delayInSeconds)
        {
            m_duration = delayInSeconds;
            m_blankObj = new BlankObj(1, 1);
        }

        public override void Execute()
        {
            if (ParentTriggerSet.IsActive)
            {
                // Delays can only be run in serial.
                SequenceType = Types.Sequence.Serial;
                m_tween = Tween.To(m_blankObj, m_duration, Tweener.Ease.Linear.EaseNone, "X", "1");

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
           // m_tween.StopTween(false);
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
