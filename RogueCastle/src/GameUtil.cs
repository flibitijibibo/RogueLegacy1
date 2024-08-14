using System;
using SteamWorksWrapper;

namespace RogueCastle
{
    public static class GameUtil
    {
        public static void UnlockAchievement(string achievementName)
        {
            Steamworks.UnlockAchievement(achievementName);
        }

        public static bool IsAchievementUnlocked(string achievementName)
        {
            return Steamworks.IsAchievementUnlocked(achievementName);
        }
    }
}
