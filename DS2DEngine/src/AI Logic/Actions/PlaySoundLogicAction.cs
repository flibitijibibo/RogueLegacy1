using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class PlaySoundLogicAction : LogicAction
    {
        private string[] m_sounds;

        public PlaySoundLogicAction(params string[] sounds)
        {
            m_sounds = sounds;
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                SoundManager.PlaySound(m_sounds);
                base.Execute();
            }
        }

        public override object Clone()
        {
            return new PlaySoundLogicAction(m_sounds);
        }
    }
}
