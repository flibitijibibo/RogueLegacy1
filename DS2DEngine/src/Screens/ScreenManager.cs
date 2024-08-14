using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DS2DEngine
{
    public class ScreenManager
    {
        protected List<Screen> m_screenArray;
        private List<Screen> m_screensToUpdate;
        protected Camera2D m_camera;
        private Game m_game;
        private bool m_isInitialized = false;
        private bool m_isContentLoaded = false;

        public ScreenManager(Game game)
        {
            m_screenArray = new List<Screen>();
            m_screensToUpdate = new List<Screen>();
            m_game = game;
        }

        public virtual void Initialize()
        {
            if (IsInitialized == false)
            {
                ContentManager content = m_game.Content;
                m_camera = new Camera2D(m_game.GraphicsDevice, EngineEV.ScreenWidth, EngineEV.ScreenHeight);
                m_isInitialized = true;
            }
        }

        public virtual void LoadContent()
        {
            m_isContentLoaded = true;
        }

        public virtual void AddScreen(Screen screen, PlayerIndex? controllingPlayer)
        {
            AddScreenAt(screen, controllingPlayer, m_screenArray.Count);
        }

        public virtual void AddScreenAt(Screen screen, PlayerIndex? controllingPlayer, int index)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;

            // If we have a graphics device, tell the screen to load content.
            if (IsInitialized && screen.IsContentLoaded == false)
                screen.LoadContent();

            m_screenArray.Insert(index, screen);
            screen.OnEnter();
        }

        public virtual void RemoveScreen(Screen screen, bool disposeScreen)
        {
            screen.OnExit();
            if (disposeScreen == true)
                screen.Dispose();

            m_screenArray.Remove(screen);
            screen.ScreenManager = null;
        }

        public virtual void Update(GameTime gameTime)
        {
            m_screensToUpdate.Clear();
            foreach (Screen screen in m_screenArray)
                m_screensToUpdate.Add(screen);

            Camera.GameTime = gameTime;
            Camera.ElapsedTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            int screensToUpdateCount = m_screensToUpdate.Count;
            for (int i = 0; i < screensToUpdateCount; i++)
            {
                Screen screen = m_screensToUpdate[i];
                if (i < screensToUpdateCount - 1)
                {
                    if (screen.UpdateIfCovered == false)
                        continue;
                    screen.Update(gameTime);
                    if (screen.HandleInputIfCovered == false)
                        continue;
                    screen.HandleInput();
                }
                else
                {
                    screen.Update(gameTime);
                    screen.HandleInput();
                }
            }

            //foreach (Screen screen in m_screensToUpdate)
            //{
            //    if (screen.UpdateIfCovered == false && screen != CurrentScreen)
            //        continue;
            //    screen.Update(gameTime);
            //    if (screen.HandleInputIfCovered == false && screen != CurrentScreen)
            //    screen.HandleInput();
            //}
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (Screen screen in m_screenArray)
            {
                //if (screen.ScreenState == ScreenState.Hidden)
                //    continue;

                if (screen.DrawIfCovered == false && screen != CurrentScreen)
                    continue;

                screen.Draw(gameTime);
            }
        }

        public Screen CurrentScreen
        {
            get
            {
                if (m_screenArray.Count < 1) return null;
                return m_screenArray[m_screenArray.Count - 1];
            }
        }

        public bool Contains(Screen screen)
        {
            return m_screenArray.Contains(screen);
        }

        public Screen[] GetScreens()
        {
            return m_screenArray.ToArray();
        }

        public Game Game
        {
            get { return m_game; }
        }

        public Camera2D Camera
        {
            get { return m_camera; }
        }

        public bool IsInitialized
        {
            get { return m_isInitialized; }
        }

        public bool IsContentLoaded
        {
            get { return m_isContentLoaded; }
        }
    }
}
