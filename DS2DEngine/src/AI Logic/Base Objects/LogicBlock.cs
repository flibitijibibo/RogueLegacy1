using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class LogicBlock : IDisposableObj
    {
        List<LogicSet> m_logicSetList;
        private bool m_isActive;
        private int[] m_percentParams;

        private LogicSet m_activeLS = null;

        private bool m_isDisposed = false;

        public LogicBlock()
        {
            m_logicSetList = new List<LogicSet>();
            m_isActive = false;
        }

        public void RunLogicBlock(params int[] percentParams)
        {
            m_percentParams = percentParams;

            if (percentParams.Length != m_logicSetList.Count)
                throw new Exception("Number of percentage parameters (" + percentParams.Length + ") does not match the number of logic sets in this block (" + m_logicSetList.Count + ").");

            m_isActive = true;

            int chance = CDGMath.RandomInt(1, 100);
            int totalChance = 0;

            for (int i = 0; i < m_logicSetList.Count; i++)
            {
                // Automatically execute this logic set if it is the last one in the list.
                if (i == m_logicSetList.Count - 1)
                {
                    m_activeLS = m_logicSetList[i];
                    m_logicSetList[i].Execute();
                    break;
                }
                else
                {
                    totalChance += percentParams[i];
                    if (chance <= totalChance)
                    {
                        m_activeLS = m_logicSetList[i];
                        m_logicSetList[i].Execute();
                        break;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsActive == true)
            {
                foreach (LogicSet set in m_logicSetList)
                {
                    if (set.IsActive == true)
                        set.Update(gameTime);
                }
            }
        }

        public void StopLogicBlock()
        {
            foreach (LogicSet set in m_logicSetList)
            {
                set.Stop();
            }
            LogicBlockComplete();
        }

        public void LogicBlockComplete()
        {
            m_isActive = false;
        }

        public void AddLogicSet(params LogicSet[] logicSet)
        {
            foreach (LogicSet set in logicSet)
            {
                LogicSet setToAdd = set.Clone();
                m_logicSetList.Add(setToAdd);
                setToAdd.ParentLogicBlock = this;
            }
        }

        // Doesn't work because logicsets are cloned so the comparison will never be made.
        //public void RemoveLogicSet(params LogicSet[] logicSet)
        //{
        //    foreach (LogicSet set in logicSet)
        //    {
        //        m_logicSetList.Remove(set);
        //        set.ParentLogicBlock = null;
        //    }
        //}

        public void ClearAllLogicSets()
        {
            foreach (LogicSet set in m_logicSetList)
            {
                set.ParentLogicBlock = null;
            }
            m_logicSetList.Clear();
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;
                foreach (LogicSet set in m_logicSetList)
                    set.Dispose();
                m_logicSetList.Clear();
                m_logicSetList = null;
                m_activeLS = null; // No need to dispose since it should already be disposed at this point.
                m_isActive = false;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        public List<LogicSet> LogicSetList
        {
            get { return m_logicSetList; }
        }

        public LogicSet ActiveLS
        {
            get { return m_activeLS; }
        }
    }
}
