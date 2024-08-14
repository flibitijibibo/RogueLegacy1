using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public class FrameSoundObj : IDisposableObj
    {
        private int m_frame;
        private IAnimateableObj m_obj;
        private GameObj m_listener;
        private string[] m_sounds;
        private bool m_soundPlaying = false;
        private bool m_isDisposed = false;

        public FrameSoundObj(IAnimateableObj obj, int frame, params string[] sounds)
        {
            m_obj = obj;
            m_frame = frame;
            m_sounds = sounds;
            m_listener = null;
        }

        public FrameSoundObj(IAnimateableObj obj, GameObj listener, int frame, params string[] sounds)
        {
            m_obj = obj;
            m_frame = frame;
            m_sounds = sounds;
            m_listener = listener;
        }

        public void Update()
        {
            if (m_soundPlaying == false && m_obj.CurrentFrame == m_frame)
            {
                m_soundPlaying = true;
                if (m_listener == null)
                    SoundManager.PlaySound(m_sounds);
                else
                    SoundManager.Play3DSound((m_obj as GameObj), m_listener, m_sounds);
            }

            if (m_obj.CurrentFrame != m_frame)
                m_soundPlaying = false;
        }

        public void Dispose()
        {
            if (m_isDisposed == false)
            {
                m_isDisposed = true;
                Array.Clear(m_sounds, 0, m_sounds.Length);
                m_sounds = null;
                m_obj = null;
                m_listener = null;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
