using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class LogicSet : IDisposableObj
    {
        private LogicAction m_firstLogicNode;
        private LogicAction m_currentLogicNode;
        private LogicAction m_lastLogicNode;

        private bool m_isActive = false;
        private GameObj m_parentObject;
        public LogicBlock ParentLogicBlock { get; set; }

        private bool m_isDisposed = false;
        public int Tag = 0;

        public LogicSet(GameObj parentObj)
        {
            m_parentObject = parentObj;
        }

        public void AddAction(LogicAction logicAction, Types.Sequence sequenceType = Types.Sequence.Serial)
        {
            logicAction.SequenceType = sequenceType;
            logicAction.ParentLogicSet = this;

            if (m_firstLogicNode == null)
                m_firstLogicNode = logicAction;
            else
                m_currentLogicNode.NextLogicAction = logicAction;

            m_currentLogicNode = logicAction;
            m_lastLogicNode = m_currentLogicNode;
        }

        public void Execute()
        {
            if (m_lastLogicNode == null)
                throw new Exception("Cannot execute logic set. Call CompleteAddAction() first.");

            m_isActive = true;
            m_firstLogicNode.Execute();
        }

        public void ExecuteComplete()
        {
            m_isActive = false;
            //Console.WriteLine("logic set complete");
            if (ParentLogicBlock != null && ParentLogicBlock.ActiveLS == this)
            {
                ParentLogicBlock.LogicBlockComplete();
            }
        }

        public void Stop()
        {
            m_isActive = false;
            m_firstLogicNode.Stop();
        }

        public void Update(GameTime gameTime)
        {
            ActiveLogicAction.Update(gameTime);
        }

        public LogicSet Clone()
        {
            LogicSet lsToReturn = new LogicSet(m_parentObject);
            lsToReturn.AddAction(m_firstLogicNode.Clone() as LogicAction, m_firstLogicNode.SequenceType);
            m_currentLogicNode = m_firstLogicNode;
            while (m_currentLogicNode.NextLogicAction != null)
            {
                m_currentLogicNode = m_currentLogicNode.NextLogicAction;
                lsToReturn.AddAction(m_currentLogicNode.Clone() as LogicAction, m_currentLogicNode.SequenceType);
            }
            m_currentLogicNode = m_lastLogicNode;
            lsToReturn.Tag = this.Tag;
            return lsToReturn;
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }

        public GameObj ParentGameObj
        {
            get { return m_parentObject; }
        }

        public LogicAction ActiveLogicAction
        {
            get
            {
                if (IsActive == false)
                    return null;
                else
                    return m_currentLogicNode;
            }
            set { m_currentLogicNode = value; }
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;
                m_currentLogicNode = m_firstLogicNode;
                while (m_currentLogicNode != null)
                {
                    LogicAction nextNode = m_currentLogicNode.NextLogicAction;
                    m_currentLogicNode.Dispose();
                    m_currentLogicNode = nextNode;
                }
                m_isActive = false;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
