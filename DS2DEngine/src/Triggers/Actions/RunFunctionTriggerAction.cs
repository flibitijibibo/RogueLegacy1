using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DS2DEngine
{
    public class RunFunctionTriggerAction : TriggerAction
    {
        MethodInfo m_methodInfo;
        object m_methodObject;
        object[] m_args;

        public RunFunctionTriggerAction(object methodObject, string functionName, params object[] args)
        {
            if (methodObject == null)
                throw new Exception("methodObject cannot be null");

            m_methodInfo = methodObject.GetType().GetMethod(functionName);

            if (m_methodInfo == null)
                throw new Exception("Function " + functionName + " not found in class " + methodObject.GetType().ToString());

            m_methodObject = methodObject;
            m_args = args;
        }

        // Used for static methods.
        public RunFunctionTriggerAction(Type objectType, string functionName, params object[] args)
        {
            m_methodInfo = objectType.GetMethod(functionName);
            m_methodObject = null;
            m_args = args;
        }

        public override void Execute()
        {
            if (ParentTriggerSet.IsActive)
            {
                m_methodInfo.Invoke(m_methodObject, m_args);
                base.Execute();
            }
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override TriggerAction Clone()
        {
            throw new NotImplementedException();
        }

    }
}
