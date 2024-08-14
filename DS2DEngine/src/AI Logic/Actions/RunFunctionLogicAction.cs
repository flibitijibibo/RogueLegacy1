using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DS2DEngine
{
    public class RunFunctionLogicAction : LogicAction
    {
        MethodInfo m_methodInfo;
        object m_methodObject;
        object[] m_args;
        string m_functionName;
        Type m_objectType;

        public RunFunctionLogicAction(object methodObject, string functionName, params object[] args)
        {
            if (methodObject == null)
                throw new Exception("methodObject cannot be null");

            m_methodInfo = methodObject.GetType().GetMethod(functionName);

            if (m_methodInfo == null)
                throw new Exception("Function " + functionName + " not found in class " + methodObject.GetType().ToString());

            m_methodObject = methodObject;
            m_args = args;
            m_functionName = functionName;
        }

        public RunFunctionLogicAction(Type objectType, string functionName, params object[] args)
        {
            Type[] argList = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
                argList[i] = args[i].GetType();

            m_methodInfo = objectType.GetMethod(functionName, argList);
            m_args = args;

            if (m_methodInfo == null)
            {
                m_methodInfo = objectType.GetMethod(functionName, new Type[] { args[0].GetType().MakeArrayType() });
                m_args = new object[1];
                m_args[0] = args;
            }

            if (m_methodInfo == null)
                throw new Exception("Function " + functionName + " not found in class " + objectType.ToString());

            m_methodObject = null;
            m_functionName = functionName;
            m_objectType = objectType;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                m_methodInfo.Invoke(m_methodObject, m_args);
                base.Execute();
            }
        }

        public override object Clone()
        {
            if (m_methodObject != null)
                return new RunFunctionLogicAction(m_methodObject, m_functionName, m_args);
            else
                return new RunFunctionLogicAction(m_objectType, m_functionName, m_args);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_methodInfo = null;
                m_methodObject = null;
                if (m_args != null)
                    Array.Clear(m_args, 0, m_args.Length);
                m_args = null;
                m_objectType = null;
                base.Dispose();
            }
        }
    }
}
