using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public abstract class Screen : IDisposableObj
    {
        public bool UpdateIfCovered { get; set; }
        public bool DrawIfCovered { get; set; }
        public bool HandleInputIfCovered { get; set; }
        public bool HasFocus {get;set;}
        public PlayerIndex? ControllingPlayer { get; set; }
        public ScreenManager ScreenManager { get; set; }
        private bool m_isInitialized = false;
        private bool m_isPaused = false;
        private bool m_isContentLoaded = false;
        private bool m_isDisposed = false;

        public Screen()
        {
            UpdateIfCovered = false;
            DrawIfCovered = false;
            HasFocus = false;
        }

        public virtual void Initialize()
        {
            m_isInitialized = true;
        }

        public virtual void ReinitializeRTs()
        {
        }

        public virtual void DisposeRTs()
        {
        }

        public virtual void RefreshTextObjs() {}

        public virtual void LoadContent()
        {
            m_isContentLoaded = true;
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput() { }

        /// <summary>
        /// Allows the screen to accept data from other screens.
        /// </summary>
        /// <param name="objList">The list of data to be passed in.</param>
        public virtual void PassInData(List<object> objList)
        {
            objList.Clear(); // Just in case.
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gametime) { }

        /// <summary>
        /// Called right after is a screen is added to the ScreenManager's screen list.
        /// In case some initializing needs to take place on the as it enters.
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// Called right after the screen is removed from the ScreenManager's screen list.
        /// In case some de-initializing needs to take place on the screen as it leaves.
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// Pauses the screen.
        /// </summary>
        public virtual void PauseScreen() { m_isPaused = true; }

        /// <summary>
        /// Unpauses the screen.
        /// </summary>
        public virtual void UnpauseScreen() { m_isPaused = false; }

        public virtual void Dispose()
        {
            m_isDisposed = true;
        }

        public bool IsInitialized
        {
            get { return m_isInitialized; }
        }

        public bool IsPaused
        {
            get { return m_isPaused; }
        }

        public bool IsContentLoaded
        {
            get { return m_isContentLoaded; }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        public Camera2D Camera
        {
            get { return ScreenManager.Camera; }
        }
    }
}
