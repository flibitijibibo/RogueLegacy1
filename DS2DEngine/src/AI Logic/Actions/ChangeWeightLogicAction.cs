using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class ChangeWeightLogicAction : LogicAction
    {
        private bool m_isWeighted = false;

        public ChangeWeightLogicAction(bool isWeighted)
        {
            m_isWeighted = isWeighted;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                IPhysicsObj physicsObj = ParentLogicSet.ParentGameObj as IPhysicsObj;
                if (physicsObj != null)
                    physicsObj.IsWeighted = m_isWeighted;
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new ChangeWeightLogicAction(m_isWeighted);
        }
    }
}
