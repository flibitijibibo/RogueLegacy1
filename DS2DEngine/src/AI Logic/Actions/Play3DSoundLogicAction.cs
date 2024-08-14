using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class Play3DSoundLogicAction : LogicAction
    {
        private string[] m_sounds;
        private GameObj m_listener;
        private GameObj m_emitter;

        public Play3DSoundLogicAction(GameObj emitter, GameObj listener, params string[] sounds)
        {
            m_sounds = sounds;
            m_emitter = emitter;
            m_listener = listener;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                SoundManager.Play3DSound(m_emitter, m_listener, m_sounds);
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new Play3DSoundLogicAction(m_emitter, m_listener, m_sounds);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_listener = null;
                m_emitter = null;
                base.Dispose();
            }
        }
    }
}
