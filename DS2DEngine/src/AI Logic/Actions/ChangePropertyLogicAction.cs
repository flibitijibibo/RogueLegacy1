using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DS2DEngine
{
    public class ChangePropertyLogicAction : LogicAction
    {
        private string m_propertyName;
        private object m_propertyArg;
        private object m_object;

        public ChangePropertyLogicAction(object propertyObject, string propertyName, object propertyArg)
        {
            m_object = propertyObject;
            m_propertyName = propertyName;
            m_propertyArg = propertyArg;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive)
            {
                PropertyInfo propertyInfo = m_object.GetType().GetProperty(m_propertyName);
                propertyInfo.SetValue(m_object, m_propertyArg, null);
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new ChangePropertyLogicAction(m_object, m_propertyName, m_propertyArg);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_propertyArg = null;
                m_object = null;
                base.Dispose();
            }
        }
    }
}
