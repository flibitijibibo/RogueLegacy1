using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public class SoundManager
    {
        public static bool IsDisposed { get { return m_isDisposed; } }
        private static bool m_isDisposed = false;

        private static WaveBank m_musicWaveBank;
        private static SoundBank m_musicSoundBank;
        private static AudioEngine m_audioEngine;
        private static SoundBank m_sfxSoundBank;
        private static WaveBank m_sfxWaveBank;

        // Variables for dealing with music fading.
        private static Cue m_currentMusicCue;
        private static Cue m_nextMusicCue;
        private static string m_songName;
        private static bool m_loopMusic = false;
        private static float m_musicFadeIn = 0;
        private static float m_musicFadeInCounter = 0;
        private static float m_musicFadeOut = 0;
        private static float m_musicFadeOutCounter = 0;
        private static float m_musicVolume = 1;
        private static bool m_musicFadingIn = false;
        private static bool m_musicFadingOut = false;

        private static List<PositionalSoundObj> m_positionalSoundList;
        private static AudioEmitter m_emitter;
        private static AudioListener m_listener;

        private static float m_globalMusicVolume = 1;
        private static float m_globalSFXVolume = 1;

        private static List<PositionalSoundObj> m_queuedSoundList;

        public static void Initialize(string settingsFile)
        {
            try
            {
                m_audioEngine = new AudioEngine(settingsFile);
                m_positionalSoundList = new List<PositionalSoundObj>();
                m_emitter = new AudioEmitter();
                m_listener = new AudioListener();

                m_queuedSoundList = new List<PositionalSoundObj>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to initialize audio engine. Original Error:" + e.ToString());
            }
        }

        public static void LoadWaveBank(string wbFilename, bool isMusicWB = false)
        {
            if (m_audioEngine != null)
            {
                if (isMusicWB == true)
                {
                    if (m_musicWaveBank != null)
                        m_musicWaveBank.Dispose();
                    m_musicWaveBank = new WaveBank(m_audioEngine, wbFilename);
                }
                else
                {
                    if (m_sfxWaveBank != null)
                        m_sfxWaveBank.Dispose();
                    m_sfxWaveBank = new WaveBank(m_audioEngine, wbFilename);
                }
            }
        }

        public static void LoadSoundBank(string sbFileName, bool isMusicSB = false)
        {
            if (m_audioEngine != null)
            {
                if (isMusicSB == true)
                {
                    if (m_musicSoundBank != null)
                        m_musicSoundBank.Dispose();
                    m_musicSoundBank = new SoundBank(m_audioEngine, sbFileName);
                }
                else
                {
                    if (m_sfxSoundBank != null)
                        m_sfxSoundBank.Dispose();
                    m_sfxSoundBank = new SoundBank(m_audioEngine, sbFileName);
                }
            }
        }

        public static Cue PlayMusic(string songName, bool loopSong, float fade = 0)
        {
            if (m_audioEngine != null)
            {
                m_songName = songName;
                m_loopMusic = loopSong;

                if (fade == 0)
                {
                    if (m_currentMusicCue != null)
                    {
                        m_currentMusicCue.Stop(AudioStopOptions.Immediate);
                        m_currentMusicCue.Dispose();
                    }
                    m_currentMusicCue = m_musicSoundBank.GetCue(songName);
                    m_currentMusicCue.Play();

                    m_audioEngine.GetCategory("Music").SetVolume(m_musicVolume * m_globalMusicVolume);
                    m_musicFadeOutCounter = 0;
                    return m_currentMusicCue;
                }
                else
                {
                    // A song is already playing. So fade it out by the set time then play the next song.
                    if (IsMusicPlaying == true)
                    {
                        if (IsMusicFadingIn == true)
                        {
                            m_musicFadeOut = m_musicFadeIn;
                            m_musicFadeOutCounter = m_musicFadeIn - m_musicFadeInCounter;
                            m_musicFadeIn = m_musicFadeInCounter = fade;
                        }
                        else if (IsMusicFadingOut == true)
                            m_musicFadeIn = m_musicFadeInCounter = fade;
                        else
                            m_musicFadeIn = m_musicFadeInCounter = m_musicFadeOut = m_musicFadeOutCounter = fade;

                        m_nextMusicCue = m_musicSoundBank.GetCue(songName);

                        return m_nextMusicCue;
                    }
                    else
                    {
                        m_musicFadeOut = m_musicFadeOutCounter = 0;
                        m_musicFadeIn = m_musicFadeInCounter = fade;

                        m_audioEngine.GetCategory("Music").SetVolume(0);

                        if (m_currentMusicCue != null)
                            m_currentMusicCue.Dispose();
                        m_currentMusicCue = m_musicSoundBank.GetCue(songName);
                        m_currentMusicCue.Play();
                        return m_currentMusicCue;
                    }
                }
            }
            return null;
        }

        public static void StopMusic(float fade = 0)
        {
            if (m_audioEngine != null)
            {
                if (fade == 0)
                {
                    m_musicFadeIn = m_musicFadeInCounter = m_musicFadeOut = m_musicFadeOutCounter = 0;
                    if (m_currentMusicCue != null)
                        m_currentMusicCue.Stop(AudioStopOptions.Immediate);
                }
                else
                {
                    if (IsMusicFadingOut == false) // If music is already fading out, just let it fade out. Otherwise do the fade.
                        m_musicFadeOut = m_musicFadeOutCounter = fade;
                }

                if (m_nextMusicCue != null)
                {
                    m_nextMusicCue.Dispose();
                    m_nextMusicCue = null;
                }

                m_loopMusic = false;
            }
        }

        public static void PauseMusic()
        {
            if (m_audioEngine != null)
            {
                if (m_currentMusicCue != null && m_currentMusicCue.IsPaused == false)
                    m_currentMusicCue.Pause();
            }
        }

        public static void ResumeMusic()
        {
            if (m_audioEngine != null)
            {
                if (m_currentMusicCue != null && m_currentMusicCue.IsPaused == true)
                    m_currentMusicCue.Resume();
            }
        }

        public static Cue PlaySound(string soundName)
        {
            if (m_audioEngine != null)
            {
                Cue cue = m_sfxSoundBank.GetCue(soundName);
                cue.Play();
                return cue;
            }
            return null;
        }

        public static Cue PlaySound(params string[] soundName)
        {
            if (m_audioEngine != null)
            {
            Cue cue = m_sfxSoundBank.GetCue(soundName[CDGMath.RandomInt(0, soundName.Length - 1)]);
            cue.Play();
            return cue;
            }
            return null;
        }

        public static Cue Play3DSound(GameObj emitter, GameObj listener, string soundName)
        {
            if (m_audioEngine != null)
            {
                Cue cue = m_sfxSoundBank.GetCue(soundName);
                PositionalSoundObj positionalSound = new PositionalSoundObj() { Emitter = emitter, Listener = listener, AttachedCue = cue };
                positionalSound.Update(m_listener, m_emitter);

                // This code checks to see if a sound is played multiple times, and if so, find the one closest to the player and play that one.
                PositionalSoundObj soundObjToRemove = null;
                foreach (PositionalSoundObj soundObj in m_queuedSoundList)
                {
                    Cue cueCheck = soundObj.AttachedCue;
                    if (cueCheck.Name == cue.Name)
                    {
                        soundObj.Update(m_listener, m_emitter);
                        // The stored cue is further away and needs to be replaced.
                        if (cueCheck.GetVariable("Distance") > cue.GetVariable("Distance"))
                            soundObjToRemove = soundObj;
                        else // The stored cue is closer, therefore break out of this method and just return the created cue..
                            return cue;
                    }
                }

                if (soundObjToRemove != null)
                {
                    // This code replaces the stored sound with the new sound.
                    m_queuedSoundList.Remove(soundObjToRemove);
                    soundObjToRemove.Dispose();
                    m_queuedSoundList.Add(positionalSound);
                }
                else
                    // This happens if this is the only instance of the sound found in the positional sound list.
                    m_queuedSoundList.Add(positionalSound);

                return cue;
            }
            return null;
        }

        public static Cue Play3DSound(GameObj emitter, GameObj listener, params string[] soundName)
        {
            if (m_audioEngine != null)
            {
                Cue cue = m_sfxSoundBank.GetCue(soundName[CDGMath.RandomInt(0, soundName.Length - 1)]);
                PositionalSoundObj positionalSound = new PositionalSoundObj() { Emitter = emitter, Listener = listener, AttachedCue = cue };
                positionalSound.Update(m_listener, m_emitter);

                // This code checks to see if a sound is played multiple times, and if so, find the one closest to the player and play that one.
                PositionalSoundObj soundObjToRemove = null;
                foreach (PositionalSoundObj soundObj in m_queuedSoundList)
                {
                    Cue cueCheck = soundObj.AttachedCue;
                    if (cueCheck.Name == cue.Name)
                    {
                        soundObj.Update(m_listener, m_emitter);
                        // The stored cue is further away and needs to be replaced.
                        if (cueCheck.GetVariable("Distance") > cue.GetVariable("Distance"))
                            soundObjToRemove = soundObj;
                        else // The stored cue is closer, therefore break out of this method and just return the created cue..
                            return cue;
                    }
                }

                if (soundObjToRemove != null)
                {
                    // This code replaces the stored sound with the new sound.
                    m_queuedSoundList.Remove(soundObjToRemove);
                    soundObjToRemove.Dispose();
                    m_queuedSoundList.Add(positionalSound);
                }
                else
                    // This happens if this is the only instance of the sound found in the positional sound list.
                    m_queuedSoundList.Add(positionalSound);

                return cue;
            }
            return null;
        }

        public static void StopAllSounds(string category)
        {
            if (m_audioEngine != null)
                m_audioEngine.GetCategory(category).Stop(AudioStopOptions.Immediate);
        }

        public static void PauseAllSounds(string category)
        {
            if (m_audioEngine != null)
                m_audioEngine.GetCategory(category).Pause();
        }

        public static void ResumeAllSounds(string category)
        {
            if (m_audioEngine != null)
                m_audioEngine.GetCategory(category).Resume();
        }

        public static void Update(GameTime gameTime)
        {
            if (m_audioEngine != null)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                m_audioEngine.Update();
                // Makes music loop if IsLooping is set to true.
                if (IsMusicLooping == true && IsMusicPlaying == false && m_musicFadeInCounter <= 0 && m_musicFadeOutCounter <= 0)
                    PlayMusic(m_songName, true, 0);
                //PlayMusic(m_songName, true, m_musicFadeIn);

                if (IsMusicPlaying == true && IsMusicPaused == false)
                {
                    if (m_musicFadeOutCounter > 0) // If there is a fade out, fade out the volume.
                    {
                        m_musicFadingIn = false;
                        m_musicFadingOut = true;

                        m_musicFadeOutCounter -= elapsedTime;
                        float reductionPercent = m_musicFadeOutCounter / m_musicFadeOut;
                        if (reductionPercent < 0) reductionPercent = 0;
                        m_audioEngine.GetCategory("Music").SetVolume(m_musicVolume * reductionPercent * m_globalMusicVolume);

                        if (m_musicFadeOutCounter <= 0 && m_nextMusicCue == null)
                            m_currentMusicCue.Stop(AudioStopOptions.Immediate);
                    }
                    else if (m_musicFadeOutCounter <= 0 && m_nextMusicCue != null) // If fade out is 0 but a song is queued, switch songs and play the next song.
                    {
                        m_currentMusicCue.Stop(AudioStopOptions.Immediate);
                        m_currentMusicCue.Dispose();
                        m_currentMusicCue = m_nextMusicCue;
                        m_nextMusicCue = null;
                        m_currentMusicCue.Play();
                    }

                    if (m_musicFadeOutCounter <= 0 && m_musicFadeInCounter > 0) // If there is a fade in, fade in the volume.
                    {
                        m_musicFadingIn = true;
                        m_musicFadingOut = false;

                        m_musicFadeInCounter -= elapsedTime;
                        float increasePercent = 1 - m_musicFadeInCounter / m_musicFadeIn;
                        if (increasePercent < 0) increasePercent = 0;
                        m_audioEngine.GetCategory("Music").SetVolume(m_musicVolume * increasePercent * m_globalMusicVolume);
                    }
                    else
                    {
                        m_musicFadingIn = false;
                        m_musicFadingOut = false;
                    }
                }
            }
        }

        public static void Update3DSounds()
        {
            if (m_audioEngine != null)
            {
                foreach (PositionalSoundObj soundObj in m_queuedSoundList)
                {
                    soundObj.AttachedCue.Play();
                    m_positionalSoundList.Add(soundObj);
                }
                m_queuedSoundList.Clear();

                for (int i = 0; i < m_positionalSoundList.Count; i++)
                {
                    PositionalSoundObj sound = m_positionalSoundList[i];

                    if (sound.AttachedCue.IsPlaying == false)
                    {
                        if (sound.IsDisposed == false)
                            sound.Dispose();
                        m_positionalSoundList.Remove(sound);
                        i--;
                    }
                    else
                    {
                        sound.Update(m_listener, m_emitter);
                    }
                }
            }
        }

        public static void Dispose()
        {
            if (IsDisposed == false)
            {
                if (m_audioEngine != null)
                {
                    if (m_currentMusicCue != null)
                        m_currentMusicCue.Dispose();
                    m_currentMusicCue = null;
                    if (m_nextMusicCue != null)
                        m_nextMusicCue.Dispose();
                    m_nextMusicCue = null;

                    m_sfxSoundBank.Dispose();
                    m_musicSoundBank.Dispose();
                    m_sfxWaveBank.Dispose();
                    m_musicWaveBank.Dispose();
                    m_audioEngine.Dispose();

                    foreach (PositionalSoundObj soundObj in m_positionalSoundList)
                        soundObj.Dispose();
                    m_positionalSoundList.Clear();
                    m_positionalSoundList = null;

                    m_emitter = null;
                    m_listener = null;
                }
                m_isDisposed = true;
            }
        }

        public static bool IsMusicFadingIn
        { get { return m_musicFadingIn; } }

        public static bool IsMusicFadingOut
        { get { return m_musicFadingOut; } }

        public static bool IsMusicPlaying
        {
            get 
            {
                if (m_currentMusicCue == null)
                    return false;
                return m_currentMusicCue.IsPlaying; 
            }
        }

        public static bool IsMusicLooping
        {
            get { return m_loopMusic; }
        }

        public static bool IsMusicPaused
        {
            get 
            {
                if (m_currentMusicCue == null)
                    return false;
                return m_currentMusicCue.IsPaused; 
            }
        }

        public static Cue GetCue(string cueName)
        {
            if (m_audioEngine == null)
                return null;
            return m_sfxSoundBank.GetCue(cueName);
        }

        public static Cue GetMusicCue(string cueName)
        {
            if (m_audioEngine == null)
                return null;
            return m_musicSoundBank.GetCue(cueName);
        }

        public static Cue GetCurrentMusicCue()
        {
            return m_currentMusicCue;
        }

        public static string GetCurrentMusicName()
        {
            return m_songName;
        }

        public static float GlobalMusicVolume
        {
            get { return m_globalMusicVolume; }
            set
            {
                if (m_audioEngine == null)
                    return;
                m_globalMusicVolume = value;
                if (m_globalMusicVolume < 0) m_globalMusicVolume = 0;
                if (m_globalMusicVolume > 1) m_globalMusicVolume = 1;
                m_audioEngine.GetCategory("Music").SetVolume(m_musicVolume * m_globalMusicVolume);
                //m_audioEngine.GetCategory("Legacy").SetVolume(m_musicVolume * m_globalMusicVolume); // Hack
            }
        }

        public static float GlobalSFXVolume 
        {
            get { return m_globalSFXVolume; }
            set
            {
                if (m_audioEngine == null)
                    return;
                m_globalSFXVolume = value;
                if (m_globalSFXVolume < 0) m_globalSFXVolume = 0;
                if (m_globalSFXVolume > 1) m_globalSFXVolume = 1;
                m_audioEngine.GetCategory("Default").SetVolume(m_globalSFXVolume);
                m_audioEngine.GetCategory("Pauseable").SetVolume(m_globalSFXVolume); // Hack
            }
        }

        public static AudioEngine AudioEngine
        {
            get { return m_audioEngine; }
        }

    }

    public class PositionalSoundObj : IDisposableObj
    {
        private bool m_isDisposed = false;
        public bool IsDisposed { get { return m_isDisposed; } }

        public GameObj Emitter;
        public GameObj Listener;
        public Cue AttachedCue;

        public void Update(AudioListener listener, AudioEmitter emitter)
        {
            emitter.Position = new Vector3(0, 0, CDGMath.DistanceBetweenPts(this.Listener.AbsPosition, this.Emitter.AbsPosition)); // Only modify the Z value to turn off stereo mixing.
            AttachedCue.Apply3D(listener, emitter);
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                m_isDisposed = true;
                Emitter = null;
                Listener = null;
                AttachedCue = null;
            }
        }
    }
}
