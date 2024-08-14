using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public abstract class LogicAction : IDisposableObj, ICloneable
    {
        // A linked list of trigger actions. 
        public LogicSet ParentLogicSet = null;
        public LogicAction PreviousLogicAction = null;
        public LogicAction NextLogicAction = null;

        public string Tag { get; set; }
        public Types.Sequence SequenceType = Types.Sequence.Serial;

        private bool m_isDisposed = false;
        public bool IsDisposed { get { return m_isDisposed; } }

        // All classes that override Execute() must be contained in the following if statement:
        // if (this.ParentLogicSet.IsActive == true) { } 
        public virtual void Execute()
        {
            ParentLogicSet.ActiveLogicAction = this;

            if (SequenceType == Types.Sequence.Parallel && NextLogicAction != null)
                NextLogicAction.Execute();
            else if (SequenceType == Types.Sequence.Serial)
                ExecuteNext();
            else if (SequenceType == Types.Sequence.Parallel && NextLogicAction == null)
                ParentLogicSet.ExecuteComplete();
        }

        public virtual void ExecuteNext()
        {
            if (NextLogicAction != null)
                NextLogicAction.Execute();
            else
                ParentLogicSet.ExecuteComplete();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Stop()
        {
            //Runs through all the logic actions and runs their Stop() method.
            if (NextLogicAction != null)
                NextLogicAction.Stop();
        }

        public virtual void Dispose()
        {
            ParentLogicSet = null;
            NextLogicAction = null;
            PreviousLogicAction = null;
            m_isDisposed = true;
        }

        public abstract object Clone();

    }
}
