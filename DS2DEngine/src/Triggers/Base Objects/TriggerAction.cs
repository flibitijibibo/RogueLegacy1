using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public abstract class TriggerAction
    {
        // A linked list of trigger actions. 
        public TriggerSet ParentTriggerSet = null;
        public TriggerAction PreviousTrigger = null;
        public TriggerAction NextTrigger = null;

        public string Tag { get; set; }
        public Types.Sequence SequenceType = Types.Sequence.Serial;

        public virtual void Execute()
        {
            ParentTriggerSet.ActiveTriggerAction = this;

            if (SequenceType == Types.Sequence.Parallel && NextTrigger != null)
                NextTrigger.Execute();
            else if (SequenceType == Types.Sequence.Serial)
                ExecuteNext();
        }

        public virtual void Update() { }

        public virtual void ExecuteNext()
        {
            if (NextTrigger != null)
                NextTrigger.Execute();
            else
                ParentTriggerSet.TriggerComplete();
        }

        public abstract void Stop();

        public abstract void Dispose();

        public abstract TriggerAction Clone();
    }
}
