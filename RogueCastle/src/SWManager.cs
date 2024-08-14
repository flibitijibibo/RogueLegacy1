using System;
using System.Runtime.InteropServices;

namespace SteamWorksWrapper
{
    public class Steamworks
    {
        private enum ESteamAPIInitResult
        {
            k_ESteamAPIInitResult_OK = 0,
            k_ESteamAPIInitResult_FailedGeneric = 1,
            k_ESteamAPIInitResult_NoSteamClient = 2,
            k_ESteamAPIInitResult_VersionMismatch = 3,
        }

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern ESteamAPIInitResult SteamAPI_InitFlat(IntPtr errMsg);

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_Shutdown();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SteamAPI_RunCallbacks();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamInternal_CreateInterface(
            [MarshalAs(UnmanagedType.LPStr)]
                string pchVersion
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamUser();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SteamAPI_GetHSteamPipe();

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SteamAPI_ISteamClient_GetISteamUserStats(
            IntPtr steamClient,
            int steamUser,
            int steamPipe,
            [MarshalAs(UnmanagedType.LPStr)]
                string pchVersion
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_RequestCurrentStats(
            IntPtr instance
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_StoreStats(
            IntPtr instance
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_SetAchievement(
            IntPtr instance,
            [MarshalAs(UnmanagedType.LPStr)]
                string name
        );

        [DllImport("steam_api", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool SteamAPI_ISteamUserStats_GetAchievement(
            IntPtr instance,
            [MarshalAs(UnmanagedType.LPStr)]
                string name,
            [MarshalAs(UnmanagedType.I1)]
                out bool achieved
        );

        public static bool WasInit = false;
        private static IntPtr steamUserStats;

        public static void Init()
        {
            try
            {
                WasInit = SteamAPI_InitFlat(IntPtr.Zero) == ESteamAPIInitResult.k_ESteamAPIInitResult_OK;
            }
            catch
            {
                WasInit = false;
            }
            if (WasInit)
            {
                IntPtr steamClient = SteamInternal_CreateInterface(
                    "SteamClient021"
                );
                int steamUser = SteamAPI_GetHSteamUser();
                int steamPipe = SteamAPI_GetHSteamPipe();
                steamUserStats = SteamAPI_ISteamClient_GetISteamUserStats(
                    steamClient,
                    steamUser,
                    steamPipe,
                    "STEAMUSERSTATS_INTERFACE_VERSION012"
                );
                SteamAPI_ISteamUserStats_RequestCurrentStats(steamUserStats);
            }
        }

        public static void Shutdown()
        {
            if (WasInit)
            {
                SteamAPI_Shutdown();
            }
        }

        public static void Update()
        {
            if (WasInit)
            {
                SteamAPI_RunCallbacks();
            }
        }

        public static void UnlockAchievement(string name)
        {
            if (WasInit)
            {
                SteamAPI_ISteamUserStats_SetAchievement(steamUserStats, name);
                SteamAPI_ISteamUserStats_StoreStats(steamUserStats);
                SteamAPI_RunCallbacks();
            }
        }

        public static bool IsAchievementUnlocked(string name)
        {
            bool achieved = false;
            if (WasInit)
            {
                SteamAPI_ISteamUserStats_GetAchievement(steamUserStats, name, out achieved);
            }
            return achieved;
        }
    }
}
