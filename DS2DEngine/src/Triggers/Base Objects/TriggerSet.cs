using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class TriggerSet
    {
        //TriggerActions are compiled in a linked list.
        private TriggerAction m_firstActionNode;
        private TriggerAction m_currentActionNode;
        private TriggerAction m_lastActionNode;
        private int m_numActionNodes;

        //private List<ITriggerableObj> m_triggerList; // The GameObj list that is used to check when to activate the trigger.
        private ITriggerableObj[] m_triggerList;
        private bool m_isActive;
        private bool m_triggerComplete;

        public TriggerSet(List<ITriggerableObj> triggerList) :this()
        {
            m_triggerList = new ITriggerableObj[triggerList.Count];
            for (int i = 0; i < triggerList.Count; i++)
            {
                m_triggerList[i] = triggerList[i];
            }
        }

        public TriggerSet(params ITriggerableObj[] triggerList)
            : this()
        {
            m_triggerList = triggerList;
        }

        public TriggerSet(ITriggerableObj triggerObj)
            : this()
        {
            m_triggerList = new ITriggerableObj[1] { triggerObj };
        }

        private TriggerSet()
        {
            m_numActionNodes = 0;
            m_firstActionNode = null;
        }

        public void AddTriggerAction(TriggerAction triggerAction, Types.Sequence sequenceType = Types.Sequence.Serial)
        {
            triggerAction.ParentTriggerSet = this;
            triggerAction.SequenceType = sequenceType;

            // If this is the first trigger added.
            if (m_firstActionNode == null)
            {
                m_firstActionNode = triggerAction;
                m_firstActionNode.PreviousTrigger = null;
                m_currentActionNode = triggerAction;
            }
            else
            {
                // Sets the current action node to the next trigger.
                m_currentActionNode.NextTrigger = triggerAction;
                // Sets the next triggers previous action node to the current node.
                triggerAction.PreviousTrigger = m_currentActionNode;
                // Sets the current node to the next trigger.
                m_currentActionNode = triggerAction;
            }
            //Sets the last node to be the current node.
            m_lastActionNode = m_currentActionNode;
            m_numActionNodes++;
        }

        public void CheckForActivation()
        {
            if (IsActive == false && m_triggerComplete == false)
            {
                foreach (ITriggerableObj obj in m_triggerList)
                {
                    if (obj.IsTriggered == false)
                        return;
                }
                Execute();
            }
        }

        public void Execute()
        {
            if (m_triggerComplete == false)
            {
                if (IsActive == false)
                {
                    if (m_firstActionNode != null)
                    {
                        m_isActive = true;
                        m_currentActionNode = m_firstActionNode;
                        m_firstActionNode.Execute();
                    }
                }
                else
                    Console.WriteLine("Cannot activate already running trigger");
            }
            else
                Console.WriteLine("Cannot activate a completed trigger.");
        }

        public void Stop()
        {
        }

        public void TriggerComplete()
        {
            Console.WriteLine("Trigger Complete");
            m_isActive = false;
            m_triggerComplete = true;
        }

        public void Update()
        {
            if (ActiveTriggerAction != null)
                ActiveTriggerAction.Update();
        }

        public void ResetTrigger()
        {
            foreach (ITriggerableObj obj in m_triggerList)
                obj.IsTriggered = false;

            m_isActive = false;
            m_triggerComplete = false;
        }

        public void DisposeAllTriggers()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }

        public int NumNodes
        {
            get { return m_numActionNodes; }
        }

        public TriggerAction ActiveTriggerAction
        {
            get
            {
                if (m_isActive == false)
                    return null;
                else
                    return m_currentActionNode;
            }
            set { m_currentActionNode = value; }
        }

        public bool AlreadyTriggered
        {
            get { return m_triggerComplete; }
        }
    }
}
