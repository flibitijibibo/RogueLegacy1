using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class DialogueObj : IDisposableObj
    {
        public string[] Speakers { get; set; }
        public string[] Dialogue { get; set; }
        private bool m_isDisposed = false;

        public DialogueObj(string[] speakers, string[] dialogue)
        {
            if (speakers.Length != dialogue.Length)
                throw new Exception("Cannot create dialogue obj with mismatching speakers and dialogue");
            Speakers = speakers;
            Dialogue = dialogue;
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                // Done
                Array.Clear(Dialogue, 0, Dialogue.Length);
                Dialogue = null;
                Array.Clear(Speakers, 0, Speakers.Length);
                Speakers = null;
                m_isDisposed = true;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
