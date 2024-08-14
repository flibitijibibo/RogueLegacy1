using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class DS2DPool<OBJ_T> : IDisposableObj
    {
        private int m_poolSize;
        private int m_numActiveObjs;

        private Stack<OBJ_T> m_ObjStack;
        private List<OBJ_T> m_activeObjsList; // Since objects are constantly being added and removed, it might be a good idea to turn this into a linked list.

        private bool m_isDisposed = false;

        public DS2DPool()
        {
            m_ObjStack = new Stack<OBJ_T>();
            m_activeObjsList = new List<OBJ_T>();

            m_poolSize = 0;
            m_numActiveObjs = 0;
        }

        public void AddToPool(OBJ_T obj)
        {
            m_ObjStack.Push(obj);
            m_poolSize++;

            //if (!(obj is IPoolableObj))
            //    throw new Exception("Cannot add object to pool. Please ensure that the object inherits from the IPoolableObj interface");
        }

        public OBJ_T CheckOut()
        {
            if (m_poolSize - m_numActiveObjs < 1)
                throw new Exception("Resource pool out of items");

            m_numActiveObjs++;
            OBJ_T objToReturn = m_ObjStack.Pop();
            //(objToReturn as IPoolableObj).IsCheckedOut = true;
            m_activeObjsList.Add(objToReturn);
            return objToReturn;
        }


        public OBJ_T CheckOutReturnNull()
        {
            if (m_poolSize - m_numActiveObjs < 1)
                return default(OBJ_T);

            m_numActiveObjs++;
            OBJ_T objToReturn = m_ObjStack.Pop();
            //(objToReturn as IPoolableObj).IsCheckedOut = true;
            m_activeObjsList.Add(objToReturn);
            return objToReturn;
        }


        public void CheckIn(OBJ_T obj)
        {
            // Make sure to clear loc ID when TextObj is freed. This is because
            // next use of this TextObj might not have loc ID and should not be
            // refreshed on language change.
            if (obj is TextObj)
                (obj as TextObj).locStringID = "";

            //This pool has been tested and the check below is unnecessary, so for performance purposes it has been removed.
            //if ((obj as IPoolableObj).IsCheckedOut == true)
            {
                //(obj as IPoolableObj).IsCheckedOut = false;
                m_numActiveObjs--;
                m_ObjStack.Push(obj);
                m_activeObjsList.Remove(obj);
            }
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                // Checkins all active objects before disposing of the pool.
                while (m_activeObjsList.Count > 0)
                {
                    CheckIn(m_activeObjsList[m_activeObjsList.Count-1]);
                }
                m_activeObjsList.Clear();

                foreach (OBJ_T obj in m_ObjStack)
                {
                    IDisposableObj disposableObj = obj as IDisposableObj;
                    if (disposableObj != null)
                        disposableObj.Dispose();
                }
                m_ObjStack.Clear();
                m_isDisposed = true;
            }
        }

        public int NumActiveObjs
        {
            get { return m_numActiveObjs; }
        }

        public int TotalPoolSize
        {
            get { return m_poolSize; }
        }

        public int CurrentPoolSize
        {
            get { return TotalPoolSize - NumActiveObjs; }
        }

        public List<OBJ_T> ActiveObjsList
        {
            get { return m_activeObjsList; }
        }

        public Stack<OBJ_T> Stack
        {
            get { return m_ObjStack; }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
