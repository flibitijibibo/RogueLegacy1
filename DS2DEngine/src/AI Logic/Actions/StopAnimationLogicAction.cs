using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweener;
using Tweener.Ease;

namespace DS2DEngine
{
    public class StopAnimationLogicAction : LogicAction
    {
        public StopAnimationLogicAction()
        {
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                IAnimateableObj obj = this.ParentLogicSet.ParentGameObj as IAnimateableObj;
                if (obj != null)
                    obj.StopAnimation();

                base.Execute();
            }
        }

        public override object Clone()
        {
            return new StopAnimationLogicAction();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
                base.Dispose();
        }
    }
}
