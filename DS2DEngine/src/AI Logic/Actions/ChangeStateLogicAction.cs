using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class ChangeStateLogicAction : LogicAction
    {
        private int m_state;

        public ChangeStateLogicAction(int state)
        {
            m_state = state;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                if (ParentLogicSet.ParentGameObj is IStateObj)
                    (ParentLogicSet.ParentGameObj as IStateObj).State = m_state;
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new ChangeStateLogicAction(m_state);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
                base.Dispose();
        }
    }
}
