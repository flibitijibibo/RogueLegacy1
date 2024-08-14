using System;
using System.IO;
using SteamWorksWrapper;
using SDL3;

namespace RogueCastle
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("FNA_PLATFORM_BACKEND", "SDL3");

            bool loadGame = true;

            if (LevelEV.CREATE_RETAIL_VERSION == true)// && LevelEV.CREATE_INSTALLABLE == false)
            {
                Steamworks.Init();
                loadGame = Steamworks.WasInit;
            }

            // Don't really need this anymore... -flibit
            //if (loadGame == true)
            {
#if true
                // Dave's custom EV settings for localization testing
                //LevelEV.RUN_TESTROOM = true;// false; // true; // false;
                //LevelEV.LOAD_SPLASH_SCREEN = false; // true; // false;
                //LevelEV.CREATE_RETAIL_VERSION = false;
                //LevelEV.SHOW_DEBUG_TEXT = false; // true;
#endif

                if (LevelEV.CREATE_RETAIL_VERSION == true)
                {
                    LevelEV.SHOW_ENEMY_RADII = false;
                    LevelEV.ENABLE_DEBUG_INPUT = false;
                    LevelEV.UNLOCK_ALL_ABILITIES = false;
                    LevelEV.TESTROOM_LEVELTYPE = GameTypes.LevelType.CASTLE;
                    LevelEV.TESTROOM_REVERSE = false;
                    LevelEV.RUN_TESTROOM = false;
                    LevelEV.SHOW_DEBUG_TEXT = false;
                    LevelEV.LOAD_TITLE_SCREEN = false;
                    LevelEV.LOAD_SPLASH_SCREEN = true;
                    LevelEV.SHOW_SAVELOAD_DEBUG_TEXT = false;
                    LevelEV.DELETE_SAVEFILE = false;
                    LevelEV.CLOSE_TESTROOM_DOORS = false;
                    LevelEV.RUN_TUTORIAL = false;
                    LevelEV.RUN_DEMO_VERSION = false;
                    LevelEV.DISABLE_SAVING = false;
                    LevelEV.RUN_CRASH_LOGS = true;
                    LevelEV.WEAKEN_BOSSES = false;
                    LevelEV.ENABLE_BACKUP_SAVING = true;
                    LevelEV.ENABLE_OFFSCREEN_CONTROL = false;
                    LevelEV.SHOW_FPS = false;
                    LevelEV.SAVE_FRAMES = false;
                    LevelEV.UNLOCK_ALL_DIARY_ENTRIES = false;
                    LevelEV.ENABLE_BLITWORKS_SPLASH = false;
                }

                if (args.Length == 1 && LevelEV.CREATE_RETAIL_VERSION == false)
                {
                    using (Game game = new Game(args[0]))
                    {
                        LevelEV.RUN_TESTROOM = true;
                        LevelEV.DISABLE_SAVING = true;
                        game.Run();
                    }
                }
                else
                {
                    if (LevelEV.RUN_CRASH_LOGS == true)
                    {
                        try
                        {
                            using (Game game = new Game())
                            {
                                game.Run();
                            }
                        }
                        catch (Exception e)
                        {
                            string date = DateTime.Now.ToString("dd-mm-yyyy_HH-mm-ss");
                            if (!Directory.Exists(Program.OSDir))
                                Directory.CreateDirectory(Program.OSDir);
                            string configFilePath = Path.Combine(Program.OSDir, "CrashLog_" + date + ".log");

                            //using (StreamWriter writer = new StreamWriter("CrashLog_" + date + ".log", false))
                            using (StreamWriter writer = new StreamWriter(configFilePath, false))
                            {
                                writer.WriteLine(e.ToString());
                            }

                            Console.WriteLine(e.ToString());
                            SDL.SDL_ShowSimpleMessageBox(
                                SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
                                "SAVE THIS MESSAGE!",
                                e.ToString(),
                                IntPtr.Zero
                            );
                        }
                    }
                    else
                    {
                        using (Game game = new Game())
                        {
                            game.Run();
                        }
                    }
                }
            }
            //else
            //{
            //    #if STEAM
            //    SDL.SDL_ShowSimpleMessageBox(
            //        SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
            //        "Launch Error",
            //        "Please load Rogue Legacy from the Steam client",
            //        IntPtr.Zero
            //    );
            //    #endif
            //}
            Steamworks.Shutdown();
        }

        public static readonly string OSDir = GetOSDir();
        private static string GetOSDir()
        {
            string os = SDL.SDL_GetPlatform();
            if (    os.Equals("Linux") ||
                    os.Equals("FreeBSD") ||
                    os.Equals("OpenBSD") ||
                    os.Equals("NetBSD") )
            {
                string osDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                if (string.IsNullOrEmpty(osDir))
                {
                    osDir = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(osDir))
                    {
                        return "."; // Oh well.
                    }
                    else
                    {
                        return Path.Combine(osDir, ".config", "RogueLegacy");
                    }
                }
                return Path.Combine(osDir, "RogueLegacy");
            }
            else if (os.Equals("Mac OS X"))
            {
                string osDir = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(osDir))
                {
                    return "."; // Oh well.
                }
                return Path.Combine(osDir, "Library/Application Support/RogueLegacy");
            }
            else if (!os.Equals("Windows"))
            {
                throw new NotSupportedException("Unhandled SDL3 platform!");
            }
            else
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appdata, "Rogue Legacy");
            }
        }
    }
}

