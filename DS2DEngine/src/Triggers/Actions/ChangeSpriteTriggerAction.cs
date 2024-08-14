using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class ChangeSpriteTriggerAction : TriggerAction
    {
        private IDrawableObj m_objToChange;
        private string m_spriteName;

        public ChangeSpriteTriggerAction(IDrawableObj objToChange, string spriteName)
        {
            m_objToChange = objToChange;
            m_spriteName = spriteName;
        }

        public override void Execute()
        {
            if (ParentTriggerSet.IsActive)
            {
                m_objToChange.ChangeSprite(m_spriteName);
                base.Execute();
            }
        }

        public override void Stop()
        {
            throw new NotImplementedException();
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
