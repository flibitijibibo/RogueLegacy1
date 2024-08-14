using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class JukeboxBonusRoom : BonusRoomObj
    {
        private GameObj m_jukeBox;
        private string[] m_songList;
        private string[] m_songTitleList;
        private int m_songIndex;
        private bool m_rotatedLeft;
        private TextObj m_nowPlayingText;
        private TextObj m_songTitle;

        private SpriteObj m_speechBubble;

        public JukeboxBonusRoom()
        {
            m_songList = new string[]{"CastleBossSong", "GardenSong", "GardenBossSong", "TowerSong", "TowerBossSong",
                "DungeonSong", "DungeonBoss", "CastleSong", "PooyanSong", "LegacySong", "SkillTreeSong", "TitleScreenSong",
                "CreditsSong", "LastBossSong", "EndSong", "EndSongDrums"};

            m_songTitleList = new string[] {"Pistol Shrimp", "The Grim Outdoors", "Skin Off My Teeth", "Narwhal", "Lamprey",
                "Broadside of the Broadsword", "Mincemeat", "Trilobyte", "Poot-yan", "SeaSawHorse (Legacy)", "SeaSawHorse (Manor)", "Rogue Legacy",
                "The Fish and the Whale", "Rotten Legacy", "Whale. Shark.", "Whale. Shark. (Drums)"};
        }

        public override void Initialize()
        {
            m_speechBubble = new SpriteObj("UpArrowSquare_Sprite");
            m_speechBubble.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            m_speechBubble.Visible = false;
            GameObjList.Add(m_speechBubble);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "Jukebox")
                {
                    m_jukeBox = obj;
                    break;
                }
            }

            (m_jukeBox as SpriteObj).OutlineWidth = 2;
            m_jukeBox.Y -= 2;
            m_speechBubble.Position = new Vector2(m_jukeBox.X, m_jukeBox.Y - m_speechBubble.Height - 20);

            base.Initialize();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            m_songTitle = new TextObj();
            m_songTitle.Font = Game.JunicodeLargeFont;
            m_songTitle.Align = Types.TextAlign.Right;
            m_songTitle.Text = "Song name here";
            m_songTitle.Opacity = 0;
            m_songTitle.FontSize = 40;
            m_songTitle.Position = new Vector2(1320 - 50, 720 - 150);
            m_songTitle.OutlineWidth = 2;

            m_nowPlayingText = m_songTitle.Clone() as TextObj;
            m_nowPlayingText.Text = LocaleBuilder.getString("LOC_ID_JUKEBOX_BONUS_ROOM_1", m_nowPlayingText); // "Now Playing"
            m_nowPlayingText.FontSize = 24;
            m_nowPlayingText.Y -= 50;

            base.LoadContent(graphics);
        }

        public override void OnEnter()
        {
            m_jukeBox.Scale = new Vector2(3, 3);
            m_jukeBox.Rotation = 0;
            base.OnEnter();
        }

        public override void Update(GameTime gameTime)
        {
            if (CollisionMath.Intersects(Player.Bounds, m_jukeBox.Bounds))
            {
                m_speechBubble.Visible = true;
                m_speechBubble.Y = m_jukeBox.Y - m_speechBubble.Height - 110 + (float)Math.Sin(Game.TotalGameTime * 20) * 2;

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    Tween.StopAllContaining(m_jukeBox, false);
                    m_jukeBox.Scale = new Vector2(3, 3);
                    m_jukeBox.Rotation = 0;

                    Tween.StopAllContaining(m_nowPlayingText, false);
                    Tween.StopAllContaining(m_songTitle, false);
                    m_songTitle.Opacity = 0;
                    m_nowPlayingText.Opacity = 0;

                    Tween.To(m_songTitle, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "1");
                    m_songTitle.Opacity = 1; // This is necessary because the tweener stores the initial value of the property when it is called.
                    Tween.To(m_songTitle, 0.5f, Linear.EaseNone, "delay", "2.2", "Opacity", "0");
                    m_songTitle.Opacity = 0;

                    Tween.To(m_nowPlayingText, 0.5f, Linear.EaseNone, "Opacity", "1");
                    m_nowPlayingText.Opacity = 1; // This is necessary because the tweener stores the initial value of the property when it is called.
                    Tween.To(m_nowPlayingText, 0.5f, Linear.EaseNone, "delay", "2", "Opacity", "0");
                    m_nowPlayingText.Opacity = 0;

                    // Set the lineage screen song back to regular volume. If you enter this room after selecting a new lineage, its volume is set to 0.
                    if (m_songList[m_songIndex] == "LegacySong" && SoundManager.AudioEngine != null)
                        SoundManager.AudioEngine.GetCategory("Legacy").SetVolume(SoundManager.GlobalMusicVolume);

                    SoundManager.PlayMusic(m_songList[m_songIndex], true, 1);
                    m_songTitle.Text = m_songTitleList[m_songIndex];
                    m_songIndex++;
                    if (m_songIndex > m_songList.Length - 1)
                        m_songIndex = 0;
                    AnimateJukebox();

                    CheckForSongRepeat();
                }
            }
            else
                m_speechBubble.Visible = false;

            base.Update(gameTime);
        }

        // Checks to see if the song should keep playing or not when exiting the room.
        private void CheckForSongRepeat()
        {
            //Console.WriteLine(this.LevelType);
            //if (this.LevelType == GameTypes.LevelType.CASTLE && m_songList[m_songIndex] == "CastleSong" ||
            //    this.LevelType == GameTypes.LevelType.DUNGEON && m_songList[m_songIndex] == "DungeonSong" ||
            //    this.LevelType == GameTypes.LevelType.GARDEN && m_songList[m_songIndex] == "GardenSong" ||
            //    this.LevelType == GameTypes.LevelType.TOWER && m_songList[m_songIndex] == "TowerSong")
            //    Game.ScreenManager.GetLevelScreen().DisableSongUpdating = false;
            //else
                Game.ScreenManager.GetLevelScreen().JukeboxEnabled = true;
        }

        public void AnimateJukebox()
        {
            Tween.To(m_jukeBox, 0.2f, Tween.EaseNone, "ScaleY", "2.9", "ScaleX", "3.1", "Rotation", "0");
            Tween.AddEndHandlerToLastTween(this, "AnimateJukebox2");
            Player.AttachedLevel.ImpactEffectPool.DisplayMusicNote(new Vector2(m_jukeBox.Bounds.Center.X + CDGMath.RandomInt(-20, 20), m_jukeBox.Bounds.Top + CDGMath.RandomInt(0, 20)));
        }

        public void AnimateJukebox2()
        {
            if (m_rotatedLeft == false)
            {
                Tween.To(m_jukeBox, 0.2f, Tween.EaseNone, "Rotation", "-2");
                m_rotatedLeft = true;
            }
            else
            {
                Tween.To(m_jukeBox, 0.2f, Tween.EaseNone, "Rotation", "2");
                m_rotatedLeft = false;
            }

            Tween.To(m_jukeBox, 0.2f, Tween.EaseNone, "ScaleY", "3.1", "ScaleX", "2.9");
            Tween.AddEndHandlerToLastTween(this, "AnimateJukebox");
        }

        public override void Draw(Camera2D camera)
        {
            m_songTitle.Position = new Vector2(this.X + 1320 - 50, this.Y + 720 - 150);
            m_nowPlayingText.Position = m_songTitle.Position;
            m_nowPlayingText.Y -= 50;

            base.Draw(camera);

            SamplerState storedState = camera.GraphicsDevice.SamplerStates[0];
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_songTitle.Draw(camera);
            m_nowPlayingText.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = storedState;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_songTitle.Dispose();
                m_songTitle = null;
                m_nowPlayingText.Dispose();
                m_nowPlayingText = null;
                m_jukeBox = null;
                Array.Clear(m_songList, 0, m_songList.Length);
                Array.Clear(m_songTitleList, 0, m_songTitleList.Length);
                m_songTitleList = null;
                m_songList = null;
                m_speechBubble = null;
                base.Dispose();
            }
        }
    }
}
