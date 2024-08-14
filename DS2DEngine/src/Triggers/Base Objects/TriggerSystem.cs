using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class TriggerSystem
    {
        private List<TriggerSet> m_triggerList;

        public TriggerSystem()
        {
            m_triggerList = new List<TriggerSet>();
        }

        public void AddTriggerObject(params TriggerSet[] objs)
        {
            foreach (TriggerSet trigger in objs)
                m_triggerList.Add(trigger);
        }

        public void RemoveTriggerObject(params TriggerSet[] objs)
        {
            foreach (TriggerSet trigger in objs)
            {
                if (m_triggerList.Contains(trigger))
                    m_triggerList.Remove(trigger);
            }
        }

        public void Update()
        {
            foreach (TriggerSet trigger in m_triggerList)
            {
                if (trigger.IsActive == false)
                   trigger.CheckForActivation();

                if (trigger.IsActive == true)
                    trigger.Update();
            }
        }
    }
}
