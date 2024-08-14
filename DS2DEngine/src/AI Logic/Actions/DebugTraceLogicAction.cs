using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class DebugTraceLogicAction : LogicAction
    {
        private string m_debugText;

        public DebugTraceLogicAction(string debugText)
        {
            m_debugText = debugText;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                Console.WriteLine(m_debugText);
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new DebugTraceLogicAction(m_debugText);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
                base.Dispose();
        }
    }
}
