using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using System.IO;
using DS2DEngine;

namespace RogueCastle
{
    public class SaveGameManager
    {
        private StorageContainer m_storageContainer;
        private string m_fileNamePlayer = "RogueLegacyPlayer.rcdat";
        private string m_fileNameUpgrades = "RogueLegacyBP.rcdat";
        private string m_fileNameMap = "RogueLegacyMap.rcdat";
        private string m_fileNameMapData = "RogueLegacyMapDat.rcdat";
        private string m_fileNameLineage = "RogueLegacyLineage.rcdat";
        private Game m_game;

        private int m_saveFailCounter;
        private bool m_autosaveLoaded;

        public SaveGameManager(Game game)
        {
            m_saveFailCounter = 0;
            m_autosaveLoaded = false;
            m_game = game;
        }

        public void Initialize()
        {
            if (LevelEV.RUN_DEMO_VERSION == true)
            {
                m_fileNamePlayer = "RogueLegacyDemoPlayer.rcdat";
                m_fileNameUpgrades = "RogueLegacyDemoBP.rcdat";
                m_fileNameMap = "RogueLegacyDemoMap.rcdat";
                m_fileNameMapData = "RogueLegacyDemoMapDat.rcdat";
                m_fileNameLineage = "RogueLegacyDemoLineage.rcdat";
            }

            if (m_storageContainer != null)
            {
                m_storageContainer.Dispose();
                m_storageContainer = null;
            }

            PerformDirectoryCheck();
        }

        private void GetStorageContainer()
        {
            if (m_storageContainer == null || m_storageContainer.IsDisposed)
            {
                IAsyncResult aSyncResult = StorageDevice.BeginShowSelector(null, null);
                aSyncResult.AsyncWaitHandle.WaitOne();
                StorageDevice storageDevice = StorageDevice.EndShowSelector(aSyncResult);
                aSyncResult.AsyncWaitHandle.Close();
                aSyncResult = storageDevice.BeginOpenContainer("RogueLegacyStorageContainer", null, null);
                aSyncResult.AsyncWaitHandle.WaitOne();
                m_storageContainer = storageDevice.EndOpenContainer(aSyncResult);
                aSyncResult.AsyncWaitHandle.Close();
            }
        }

        // This code was added to create profile directories in case the computer doesn't have them.
        // Older versions of this game does not use directories.
        private void PerformDirectoryCheck()
        {
            GetStorageContainer();

            // Creating the directories.
            if (m_storageContainer.DirectoryExists("Profile1") == false)
            {
                m_storageContainer.CreateDirectory("Profile1");

                // Copying all files from the base directory into Profile1.
                CopyFile(m_storageContainer, m_fileNamePlayer, "Profile1");
                CopyFile(m_storageContainer, "AutoSave_" + m_fileNamePlayer, "Profile1");

                CopyFile(m_storageContainer, m_fileNameUpgrades, "Profile1");
                CopyFile(m_storageContainer, "AutoSave_" + m_fileNameUpgrades, "Profile1");

                CopyFile(m_storageContainer, m_fileNameMap, "Profile1");
                CopyFile(m_storageContainer, "AutoSave_" + m_fileNameMap, "Profile1");

                CopyFile(m_storageContainer, m_fileNameMapData, "Profile1");
                CopyFile(m_storageContainer, "AutoSave_" + m_fileNameMapData, "Profile1");

                CopyFile(m_storageContainer, m_fileNameLineage, "Profile1");
                CopyFile(m_storageContainer, "AutoSave_" + m_fileNameLineage, "Profile1");
            }
            if (m_storageContainer.DirectoryExists("Profile2") == false)
                m_storageContainer.CreateDirectory("Profile2");
            if (m_storageContainer.DirectoryExists("Profile3") == false)
                m_storageContainer.CreateDirectory("Profile3");

            m_storageContainer.Dispose();
            m_storageContainer = null;
        }

        private void CopyFile(StorageContainer storageContainer, string fileName, string profileName)
        {
            if (storageContainer.FileExists(fileName) == true)
            {
                Stream fileToCopy = storageContainer.OpenFile(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                Stream copiedFile = storageContainer.CreateFile(profileName + "/" + fileName);
                fileToCopy.CopyTo(copiedFile);
                fileToCopy.Close();
                copiedFile.Close();
            }
        }

        public void SaveFiles(params SaveType[] saveList)
        {
            if (LevelEV.DISABLE_SAVING == false)
            {
                GetStorageContainer();
                try
                {
                    foreach (SaveType saveType in saveList)
                        SaveData(saveType, false);
                    m_saveFailCounter = 0;
                }
                //catch (IOException e)
                catch
                {
                    if (m_saveFailCounter > 2)
                    {
                        RCScreenManager manager = Game.ScreenManager;
                        manager.DialogueScreen.SetDialogue("Save File Error Antivirus");
                        //manager.DisplayScreen(ScreenType.Dialogue, false, null);
                        Tweener.Tween.RunFunction(0.25f, manager, "DisplayScreen", ScreenType.Dialogue, true, typeof(List<object>));
                        m_saveFailCounter = 0;
                    }
                    else
                    {
                        m_saveFailCounter++;
                    }
                }
                finally
                {
                    if (m_storageContainer != null && m_storageContainer.IsDisposed == false)
                        m_storageContainer.Dispose();
                    m_storageContainer = null;
                }
            }
        }

        public void SaveBackupFiles(params SaveType[] saveList)
        {
            if (LevelEV.DISABLE_SAVING == false)
            {
                GetStorageContainer();
                foreach (SaveType saveType in saveList)
                    SaveData(saveType, true);
                m_storageContainer.Dispose();
                m_storageContainer = null;
            }
        }

        public void SaveAllFileTypes(bool saveBackup)
        {
            if (saveBackup == false)
                SaveFiles(SaveType.PlayerData, SaveType.UpgradeData, SaveType.Map, SaveType.MapData, SaveType.Lineage);
            else
                SaveBackupFiles(SaveType.PlayerData, SaveType.UpgradeData, SaveType.Map, SaveType.MapData, SaveType.Lineage);
        }

        public void LoadFiles(ProceduralLevelScreen level, params SaveType[] loadList)
        {
            if (LevelEV.ENABLE_BACKUP_SAVING == true)
            {
                GetStorageContainer();

                SaveType currentType = SaveType.None;

                try
                {
                    if (LevelEV.DISABLE_SAVING == false)
                    {
                        foreach (SaveType loadType in loadList)
                        {
                            currentType = loadType;
                            LoadData(loadType, level);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Save File Error: " + e.Message);
                    // Only perform autosave loading if you're not loading the map.  This is because the map is loaded on a separate thread, so it needs to
                    // manually call ForceBackup() once the thread has exited.
                    if (currentType != SaveType.Map && currentType != SaveType.MapData && currentType != SaveType.None)
                    {
                        if (m_autosaveLoaded == false)
                        {
                            RCScreenManager manager = Game.ScreenManager;
                            manager.DialogueScreen.SetDialogue("Save File Error");
                            //manager.DialogueScreen.SetDialogue("Save File Error 2");
                            Game.gameIsCorrupt = true;
                            manager.DialogueScreen.SetConfirmEndHandler(this, "LoadAutosave");
                            //manager.DialogueScreen.SetConfirmEndHandler(m_game, "Exit");
                            manager.DisplayScreen(ScreenType.Dialogue, false, null);
                            // Just a small trigger to make sure the game knows the file is corrupt, and to stop doing whatever it's doing.
                            Game.PlayerStats.HeadPiece = 0;

                        }
                        else
                        {
                            m_autosaveLoaded = false;
                            RCScreenManager manager = Game.ScreenManager;
                            manager.DialogueScreen.SetDialogue("Save File Error 2");
                            //manager.DialogueScreen.SetConfirmEndHandler(this, "StartNewGame");
                            Game.gameIsCorrupt = true;
                            manager.DialogueScreen.SetConfirmEndHandler(m_game, "Exit");
                            manager.DisplayScreen(ScreenType.Dialogue, false, null);

                            // Just a small trigger to make sure the game knows the file is corrupt, and to stop doing whatever it's doing.
                            Game.PlayerStats.HeadPiece = 0;
                        }
                    }
                    else throw new Exception(); // This triggers the try/catch block in the loading screen
                }
                finally
                {
                    if (m_storageContainer != null && m_storageContainer.IsDisposed == false)
                        m_storageContainer.Dispose();
                }
            }
            else
            {
                if (LevelEV.DISABLE_SAVING == false)
                {
                    GetStorageContainer();
                    foreach (SaveType loadType in loadList)
                        LoadData(loadType, level);
                    m_storageContainer.Dispose();
                    m_storageContainer = null;
                }
            }
        }

        public void ForceBackup()
        {
            if (m_storageContainer != null && m_storageContainer.IsDisposed == false)
                m_storageContainer.Dispose();
            RCScreenManager manager = Game.ScreenManager;
            manager.DialogueScreen.SetDialogue("Save File Error");
            manager.DialogueScreen.SetConfirmEndHandler(this, "LoadAutosave");
            manager.DisplayScreen(ScreenType.Dialogue, false, null);
        }

        public void LoadAutosave()
        {
            Console.WriteLine("Save file corrupted");
            SkillSystem.ResetAllTraits();
            Game.PlayerStats.Dispose();
            Game.PlayerStats = new PlayerStats();
            Game.ScreenManager.Player.Reset();
            LoadBackups();
            Game.ScreenManager.DisplayScreen(ScreenType.Title, true);
        }

        public void StartNewGame()
        {
            this.ClearAllFileTypes(false);
            this.ClearAllFileTypes(true);
            SkillSystem.ResetAllTraits();
            Game.PlayerStats.Dispose();
            Game.PlayerStats = new PlayerStats();
            Game.ScreenManager.Player.Reset();
            Game.ScreenManager.DisplayScreen(ScreenType.TutorialRoom, true);
        }

        public void ResetAutosave()
        {
            m_autosaveLoaded = false;
        }

        public void LoadAllFileTypes(ProceduralLevelScreen level)
        {
            LoadFiles(level, SaveType.PlayerData, SaveType.UpgradeData, SaveType.Map, SaveType.MapData, SaveType.Lineage);
        }

        public void ClearFiles(params SaveType[] deleteList)
        {
            GetStorageContainer();
            foreach (SaveType deleteType in deleteList)
                DeleteData(deleteType);
            m_storageContainer.Dispose();
            m_storageContainer = null;
        }

        public void ClearBackupFiles(params SaveType[] deleteList)
        {
            GetStorageContainer();
            foreach (SaveType deleteType in deleteList)
                DeleteBackupData(deleteType);
            m_storageContainer.Dispose();
            m_storageContainer = null;
        }

        public void ClearAllFileTypes(bool deleteBackups)
        {
            if (deleteBackups == false)
                ClearFiles(SaveType.PlayerData, SaveType.UpgradeData, SaveType.Map, SaveType.MapData, SaveType.Lineage);
            else
                ClearBackupFiles(SaveType.PlayerData, SaveType.UpgradeData, SaveType.Map, SaveType.MapData, SaveType.Lineage);
        }

        private void DeleteData(SaveType deleteType)
        {
            switch (deleteType)
            {
                case (SaveType.PlayerData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer);
                    break;
                case (SaveType.UpgradeData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades);
                    break;
                case (SaveType.Map):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap);
                    break;
                case (SaveType.MapData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData);
                    break;
                case (SaveType.Lineage):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage);
                    break;
            }

            Console.WriteLine("Save file type " + deleteType + " deleted.");
        }

        private void DeleteBackupData(SaveType deleteType)
        {
            switch (deleteType)
            {
                case (SaveType.PlayerData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNamePlayer))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNamePlayer);
                    break;
                case (SaveType.UpgradeData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameUpgrades))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameUpgrades);
                    break;
                case (SaveType.Map):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMap))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMap);
                    break;
                case (SaveType.MapData):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMapData))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMapData);
                    break;
                case (SaveType.Lineage):
                    if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameLineage))
                        m_storageContainer.DeleteFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameLineage);
                    break;
            }

            Console.WriteLine("Backup save file type " + deleteType + " deleted.");
        }

        private void LoadBackups()
        {
            Console.WriteLine("Replacing save file with back up saves");
            GetStorageContainer();
            if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNamePlayer) && m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer))
            {
                Stream fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNamePlayer, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                
                // Copy the backup prior to corruption revert.  This is in case the loading didn't work, and all data is lost (since backups get overwritten once people start playing again.
                Stream backupCopy = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSaveBACKUP_" + m_fileNamePlayer);
                fileToCopy.CopyTo(backupCopy);
                backupCopy.Close();
                fileToCopy.Close();

                fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNamePlayer, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                Stream fileToOverride = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer); // Create a new file
                fileToCopy.CopyTo(fileToOverride); // Copy the backup to the new file.

                fileToCopy.Close();
                fileToOverride.Close();
            }

            if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameUpgrades) && m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades))
            {
                Stream fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameUpgrades, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup

                // Copy the backup prior to corruption revert.  This is in case the loading didn't work, and all data is lost (since backups get overwritten once people start playing again.
                Stream backupCopy = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSaveBACKUP_" + m_fileNameUpgrades);
                fileToCopy.CopyTo(backupCopy);
                backupCopy.Close();
                fileToCopy.Close();
                
                fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameUpgrades, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                Stream fileToOverride = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades); // Create a new file
                fileToCopy.CopyTo(fileToOverride); // Copy the backup to the new file.

                fileToCopy.Close();
                fileToOverride.Close();
            }

            if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMap) && m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap))
            {
                Stream fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMap, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                
                // Copy the backup prior to corruption revert.  This is in case the loading didn't work, and all data is lost (since backups get overwritten once people start playing again.
                Stream backupCopy = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSaveBACKUP_" + m_fileNameMap);
                fileToCopy.CopyTo(backupCopy);
                backupCopy.Close();
                fileToCopy.Close();

                fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMap, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                Stream fileToOverride = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap); // Create a new file
                fileToCopy.CopyTo(fileToOverride); // Copy the backup to the new file.

                fileToCopy.Close();
                fileToOverride.Close();
            }

            if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMapData) && m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData))
            {
                Stream fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMapData, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup

                // Copy the backup prior to corruption revert.  This is in case the loading didn't work, and all data is lost (since backups get overwritten once people start playing again.
                Stream backupCopy = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSaveBACKUP_" + m_fileNameMapData);
                fileToCopy.CopyTo(backupCopy);
                backupCopy.Close();
                fileToCopy.Close();

                fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameMapData, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                Stream fileToOverride = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData); // Create a new file
                fileToCopy.CopyTo(fileToOverride); // Copy the backup to the new file.

                fileToCopy.Close();
                fileToOverride.Close();
            }

            if (m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameLineage) && m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage))
            {
                Stream fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameLineage, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup

                // Copy the backup prior to corruption revert.  This is in case the loading didn't work, and all data is lost (since backups get overwritten once people start playing again.
                Stream backupCopy = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSaveBACKUP_" + m_fileNameLineage);
                fileToCopy.CopyTo(backupCopy);
                backupCopy.Close();
                fileToCopy.Close();

                fileToCopy = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + "AutoSave_" + m_fileNameLineage, FileMode.Open, FileAccess.Read, FileShare.Read); // Open the backup
                Stream fileToOverride = m_storageContainer.CreateFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage); // Create a new file
                fileToCopy.CopyTo(fileToOverride); // Copy the backup to the new file.

                fileToCopy.Close();
                fileToOverride.Close();
            }
            m_autosaveLoaded = true;
            m_storageContainer.Dispose();
            m_storageContainer = null;
        }

        private void SaveData(SaveType saveType, bool saveBackup)
        {
            switch (saveType)
            {
                case (SaveType.PlayerData):
                    SavePlayerData(saveBackup);
                    break;
                case (SaveType.UpgradeData):
                    SaveUpgradeData(saveBackup);
                    break;
                case (SaveType.Map):
                    SaveMap(saveBackup);
                    break;
                case (SaveType.MapData):
                    SaveMapData(saveBackup);
                    break;
                case (SaveType.Lineage):
                    SaveLineageData(saveBackup);
                    break;
            }
            Console.WriteLine("\nData type " + saveType + " saved!");
        }

        private void SavePlayerData(bool saveBackup)
        {
            string fileName = m_fileNamePlayer;
            if (saveBackup == true)
                fileName = fileName.Insert(0, "AutoSave_");
            fileName = fileName.Insert(0, "Profile" + Game.GameConfig.ProfileSlot + "/");

            // If you have an old version save file, update the name to match the new format before saving.
            if (Game.PlayerStats.RevisionNumber <= 0)
            {
                string playerName = Game.PlayerStats.PlayerName;
                string romanNumeral = "";
                Game.ConvertPlayerNameFormat(ref playerName, ref romanNumeral);
                Game.PlayerStats.PlayerName = playerName;
                Game.PlayerStats.RomanNumeral = romanNumeral;
            }

            using (Stream stream = m_storageContainer.CreateFile(fileName))
            //using (Stream stream = m_storageContainer.OpenFile(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Saving player gold. (int32)
                    writer.Write(Game.PlayerStats.Gold);
                    
                    // Saving player health. (int32)
                    Game.PlayerStats.CurrentHealth = Game.ScreenManager.Player.CurrentHealth;
                    writer.Write(Game.PlayerStats.CurrentHealth);
                    // Saving player mana. (int32)
                    Game.PlayerStats.CurrentMana = (int)Game.ScreenManager.Player.CurrentMana;
                    writer.Write(Game.PlayerStats.CurrentMana);

                    // Saving player Age. (byte)
                    writer.Write(Game.PlayerStats.Age);
                    // Saving player's Child Age (byte)
                    writer.Write(Game.PlayerStats.ChildAge);
                    // Saving player spell (byte)
                    writer.Write(Game.PlayerStats.Spell);
                    // Saving player class (byte)
                    writer.Write(Game.PlayerStats.Class);
                    // Saving player special item (byte)
                    writer.Write(Game.PlayerStats.SpecialItem);
                    // Saving traits. Saved as bytes, but should be loaded into a Vector2.
                    writer.Write((byte)Game.PlayerStats.Traits.X);
                    writer.Write((byte)Game.PlayerStats.Traits.Y);
                    // Saving player name (string)
                    writer.Write(Game.PlayerStats.PlayerName);

                    // Saving player parts. All bytes.
                    writer.Write(Game.PlayerStats.HeadPiece);
                    writer.Write(Game.PlayerStats.ShoulderPiece);
                    writer.Write(Game.PlayerStats.ChestPiece);

                    // Saving diary entry (byte)
                    writer.Write(Game.PlayerStats.DiaryEntry);

                    // Saving bonus stats. All int32s.
                    writer.Write(Game.PlayerStats.BonusHealth);
                    writer.Write(Game.PlayerStats.BonusStrength);
                    writer.Write(Game.PlayerStats.BonusMana);
                    writer.Write(Game.PlayerStats.BonusDefense);
                    writer.Write(Game.PlayerStats.BonusWeight);
                    writer.Write(Game.PlayerStats.BonusMagic);

                    // Saving lich health and mana. Only needed for lich.
                    writer.Write(Game.PlayerStats.LichHealth); // int32
                    writer.Write(Game.PlayerStats.LichMana); // int32
                    writer.Write(Game.PlayerStats.LichHealthMod); // Single

                    // Saving boss progress. All bools.
                    writer.Write(Game.PlayerStats.NewBossBeaten);
                    writer.Write(Game.PlayerStats.EyeballBossBeaten);
                    writer.Write(Game.PlayerStats.FairyBossBeaten);
                    writer.Write(Game.PlayerStats.FireballBossBeaten);
                    writer.Write(Game.PlayerStats.BlobBossBeaten);
                    writer.Write(Game.PlayerStats.LastbossBeaten);

                    // Saving the number of times the castle was beaten (for new game plus) and number of enemies beaten (for rank). (int32)
                    writer.Write(Game.PlayerStats.TimesCastleBeaten);
                    writer.Write(Game.PlayerStats.NumEnemiesBeaten);

                    // Saving misc. flags.
                    // Saving castle lock state. Bool.
                    writer.Write(Game.PlayerStats.TutorialComplete);
                    writer.Write(Game.PlayerStats.CharacterFound);
                    writer.Write(Game.PlayerStats.LoadStartingRoom);

                    writer.Write(Game.PlayerStats.LockCastle);
                    writer.Write(Game.PlayerStats.SpokeToBlacksmith);
                    writer.Write(Game.PlayerStats.SpokeToEnchantress);
                    writer.Write(Game.PlayerStats.SpokeToArchitect);
                    writer.Write(Game.PlayerStats.SpokeToTollCollector);
                    writer.Write(Game.PlayerStats.IsDead);
                    writer.Write(Game.PlayerStats.FinalDoorOpened);
                    writer.Write(Game.PlayerStats.RerolledChildren);
                    writer.Write(Game.PlayerStats.IsFemale);
                    writer.Write(Game.PlayerStats.TimesDead); // int32;
                    writer.Write(Game.PlayerStats.HasArchitectFee);
                    writer.Write(Game.PlayerStats.ReadLastDiary);
                    writer.Write(Game.PlayerStats.SpokenToLastBoss);
                    writer.Write(Game.PlayerStats.HardcoreMode);

                    Game.PlayerStats.TotalHoursPlayed += Game.HoursPlayedSinceLastSave;
                    Game.HoursPlayedSinceLastSave = 0;
                    writer.Write(Game.PlayerStats.TotalHoursPlayed);
                    
                    // Saving the wizard spell list.
                    writer.Write((byte)Game.PlayerStats.WizardSpellList.X);
                    writer.Write((byte)Game.PlayerStats.WizardSpellList.Y);
                    writer.Write((byte)Game.PlayerStats.WizardSpellList.Z);

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("\nSaving Player Stats");
                        Console.WriteLine("Gold: " + Game.PlayerStats.Gold);
                        Console.WriteLine("Current Health: " + Game.PlayerStats.CurrentHealth);
                        Console.WriteLine("Current Mana: " + Game.PlayerStats.CurrentMana);
                        Console.WriteLine("Age: " + Game.PlayerStats.Age);
                        Console.WriteLine("Child Age: " + Game.PlayerStats.ChildAge);
                        Console.WriteLine("Spell: " + Game.PlayerStats.Spell);
                        Console.WriteLine("Class: " + Game.PlayerStats.Class);
                        Console.WriteLine("Special Item: " + Game.PlayerStats.SpecialItem);
                        Console.WriteLine("Traits: " + Game.PlayerStats.Traits.X + ", " + Game.PlayerStats.Traits.Y);
                        Console.WriteLine("Name: " + Game.PlayerStats.PlayerName);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Head Piece: " + Game.PlayerStats.HeadPiece);
                        Console.WriteLine("Shoulder Piece: " + Game.PlayerStats.ShoulderPiece);
                        Console.WriteLine("Chest Piece: " + Game.PlayerStats.ChestPiece);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Diary Entry: " + Game.PlayerStats.DiaryEntry);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Bonus Health: " + Game.PlayerStats.BonusHealth);
                        Console.WriteLine("Bonus Strength: " + Game.PlayerStats.BonusStrength);
                        Console.WriteLine("Bonus Mana: " + Game.PlayerStats.BonusMana);
                        Console.WriteLine("Bonus Armor: " + Game.PlayerStats.BonusDefense);
                        Console.WriteLine("Bonus Weight: " + Game.PlayerStats.BonusWeight);
                        Console.WriteLine("Bonus Magic: " + Game.PlayerStats.BonusMagic);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Lich Health: " + Game.PlayerStats.LichHealth);
                        Console.WriteLine("Lich Mana: " + Game.PlayerStats.LichMana);
                        Console.WriteLine("Lich Health Mod: " + Game.PlayerStats.LichHealthMod);
                        Console.WriteLine("---------------");
                        Console.WriteLine("New Boss Beaten: " + Game.PlayerStats.NewBossBeaten);
                        Console.WriteLine("Eyeball Boss Beaten: " + Game.PlayerStats.EyeballBossBeaten);
                        Console.WriteLine("Fairy Boss Beaten: " + Game.PlayerStats.FairyBossBeaten);
                        Console.WriteLine("Fireball Boss Beaten: " + Game.PlayerStats.FireballBossBeaten);
                        Console.WriteLine("Blob Boss Beaten: " + Game.PlayerStats.BlobBossBeaten);
                        Console.WriteLine("Last Boss Beaten: " + Game.PlayerStats.LastbossBeaten);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Times Castle Beaten: " + Game.PlayerStats.TimesCastleBeaten);
                        Console.WriteLine("Number of Enemies Beaten: " + Game.PlayerStats.NumEnemiesBeaten);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Tutorial Complete: " + Game.PlayerStats.TutorialComplete);
                        Console.WriteLine("Character Found: " + Game.PlayerStats.CharacterFound);
                        Console.WriteLine("Load Starting Room: " + Game.PlayerStats.LoadStartingRoom);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Spoke to Blacksmith: " + Game.PlayerStats.SpokeToBlacksmith);
                        Console.WriteLine("Spoke to Enchantress: " + Game.PlayerStats.SpokeToEnchantress);
                        Console.WriteLine("Spoke to Architect: " + Game.PlayerStats.SpokeToArchitect);
                        Console.WriteLine("Spoke to Toll Collector: " + Game.PlayerStats.SpokeToTollCollector);
                        Console.WriteLine("Player Is Dead: " + Game.PlayerStats.IsDead);
                        Console.WriteLine("Final Door Opened: " + Game.PlayerStats.FinalDoorOpened);
                        Console.WriteLine("Rerolled Children: " + Game.PlayerStats.RerolledChildren);
                        Console.WriteLine("Is Female: " + Game.PlayerStats.IsFemale);
                        Console.WriteLine("Times Dead: " + Game.PlayerStats.TimesDead);
                        Console.WriteLine("Has Architect Fee: " + Game.PlayerStats.HasArchitectFee);
                        Console.WriteLine("Player read last diary: " + Game.PlayerStats.ReadLastDiary);
                        Console.WriteLine("Player has spoken to last boss: " + Game.PlayerStats.SpokenToLastBoss);
                        Console.WriteLine("Is Hardcore mode: " + Game.PlayerStats.HardcoreMode);
                        Console.WriteLine("Total Hours Played " + Game.PlayerStats.TotalHoursPlayed);
                        Console.WriteLine("Wizard Spell 1: " + Game.PlayerStats.WizardSpellList.X);
                        Console.WriteLine("Wizard Spell 2: " + Game.PlayerStats.WizardSpellList.Y);
                        Console.WriteLine("Wizard Spell 3: " + Game.PlayerStats.WizardSpellList.Z);
                    }

                    Console.WriteLine("///// ENEMY LIST DATA - BEGIN SAVING /////");
                    // Saving the currently created branch.
                    List<Vector4> enemyList = Game.PlayerStats.EnemiesKilledList;
                    foreach (Vector4 enemy in enemyList)
                    {
                        writer.Write((byte)enemy.X);
                        writer.Write((byte)enemy.Y);
                        writer.Write((byte)enemy.Z);
                        writer.Write((byte)enemy.W);
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Saving Enemy List Data");
                        int counter = 0;
                        foreach (Vector4 enemy in enemyList)
                        {
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Basic, Killed: " + enemy.X);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Advanced, Killed: " + enemy.Y);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Expert, Killed: " + enemy.Z);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Miniboss, Killed: " + enemy.W);
                            counter++;
                        }
                    }

                    int numKilledEnemiesInRun = Game.PlayerStats.EnemiesKilledInRun.Count;
                    List<Vector2> enemiesKilledInRun = Game.PlayerStats.EnemiesKilledInRun;
                    writer.Write(numKilledEnemiesInRun);

                    foreach (Vector2 enemyData in enemiesKilledInRun)
                    {
                        writer.Write((int)enemyData.X); // Saving enemy's room location.
                        writer.Write((int)enemyData.Y); // Saving enemy's index in that specific room.
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Saving num enemies killed");
                        Console.WriteLine("Number of enemies killed in run: " + numKilledEnemiesInRun);
                        foreach (Vector2 enemy in enemiesKilledInRun)
                        {
                            Console.WriteLine("Enemy Room Index: " + enemy.X);
                            Console.WriteLine("Enemy Index in EnemyList: " + enemy.Y);
                        }
                    }

                    Console.WriteLine("///// ENEMY LIST DATA - SAVE COMPLETE /////");

                    Console.WriteLine("///// DLC DATA - BEGIN SAVING /////");
                    // Saving Challenge Room Data. All bools.
                    writer.Write(Game.PlayerStats.ChallengeEyeballUnlocked);
                    writer.Write(Game.PlayerStats.ChallengeSkullUnlocked);
                    writer.Write(Game.PlayerStats.ChallengeFireballUnlocked);
                    writer.Write(Game.PlayerStats.ChallengeBlobUnlocked);
                    writer.Write(Game.PlayerStats.ChallengeLastBossUnlocked);

                    writer.Write(Game.PlayerStats.ChallengeEyeballBeaten);
                    writer.Write(Game.PlayerStats.ChallengeSkullBeaten);
                    writer.Write(Game.PlayerStats.ChallengeFireballBeaten);
                    writer.Write(Game.PlayerStats.ChallengeBlobBeaten);
                    writer.Write(Game.PlayerStats.ChallengeLastBossBeaten);

                    writer.Write(Game.PlayerStats.ChallengeEyeballTimesUpgraded);
                    writer.Write(Game.PlayerStats.ChallengeSkullTimesUpgraded);
                    writer.Write(Game.PlayerStats.ChallengeFireballTimesUpgraded);
                    writer.Write(Game.PlayerStats.ChallengeBlobTimesUpgraded);
                    writer.Write(Game.PlayerStats.ChallengeLastBossTimesUpgraded);

                    writer.Write(Game.PlayerStats.RomanNumeral);
                    writer.Write(Game.PlayerStats.HasProsopagnosia);
                    writer.Write(LevelEV.SAVEFILE_REVISION_NUMBER);
                    writer.Write(Game.PlayerStats.ArchitectUsed);

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Eyeball Challenge Unlocked: " + Game.PlayerStats.ChallengeEyeballUnlocked);
                        Console.WriteLine("Skull Challenge Unlocked: " + Game.PlayerStats.ChallengeSkullUnlocked);
                        Console.WriteLine("Fireball Challenge Unlocked: " + Game.PlayerStats.ChallengeFireballUnlocked);
                        Console.WriteLine("Blob Challenge Unlocked: " + Game.PlayerStats.ChallengeBlobUnlocked);
                        Console.WriteLine("Last Boss Challenge Unlocked: " + Game.PlayerStats.ChallengeLastBossUnlocked);

                        Console.WriteLine("Eyeball Challenge Beaten: " + Game.PlayerStats.ChallengeEyeballBeaten);
                        Console.WriteLine("Skull Challenge Beaten: " + Game.PlayerStats.ChallengeSkullBeaten);
                        Console.WriteLine("Fireball Challenge Beaten: " + Game.PlayerStats.ChallengeFireballBeaten);
                        Console.WriteLine("Blob Challenge Beaten: " + Game.PlayerStats.ChallengeBlobBeaten);
                        Console.WriteLine("Last Boss Challenge Beaten: " + Game.PlayerStats.ChallengeLastBossBeaten);

                        Console.WriteLine("Eyeball Challenge Times Upgraded: " + Game.PlayerStats.ChallengeEyeballTimesUpgraded);
                        Console.WriteLine("Skull Challenge Times Upgraded: " + Game.PlayerStats.ChallengeSkullTimesUpgraded);
                        Console.WriteLine("Fireball Challenge Times Upgraded: " + Game.PlayerStats.ChallengeFireballTimesUpgraded);
                        Console.WriteLine("Blob Challenge Times Upgraded: " + Game.PlayerStats.ChallengeBlobTimesUpgraded);
                        Console.WriteLine("Last Boss Challenge Times Upgraded: " + Game.PlayerStats.ChallengeLastBossTimesUpgraded);

                        Console.WriteLine("Player Name Number: " + Game.PlayerStats.RomanNumeral);
                        Console.WriteLine("Player HasProsopagnosia: " + Game.PlayerStats.HasProsopagnosia);
                        Console.WriteLine("Save File Revision Number: " + LevelEV.SAVEFILE_REVISION_NUMBER);
                        Console.WriteLine("Architect used: " + Game.PlayerStats.ArchitectUsed);
                    }

                    Console.WriteLine("///// DLC DATA - SAVE COMPLETE /////");

                    if (saveBackup == true)
                    {
                        FileStream fileStream = stream as FileStream;
                        if (fileStream != null)
                            fileStream.Flush(true);
                    }

                    writer.Close();
                }

                stream.Close();
            }
        }

        private void SaveUpgradeData(bool saveBackup)
        {
            string fileName = m_fileNameUpgrades;
            if (saveBackup == true)
                fileName = fileName.Insert(0, "AutoSave_");
            fileName = fileName.Insert(0, "Profile" + Game.GameConfig.ProfileSlot + "/");

            using (Stream stream = m_storageContainer.CreateFile(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nSaving Equipment States");

                    // Saving the base equipment states.
                    List<byte[]> blueprintArray = Game.PlayerStats.GetBlueprintArray;
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("Standard Blueprints");
                    foreach (byte[] categoryType in blueprintArray)
                    {
                        foreach (byte equipmentState in categoryType)
                        {
                            writer.Write(equipmentState);
                            if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                Console.Write(" " + equipmentState);
                        }
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write("\n");
                    }

                    // Saving the ability equipment states.
                    List<byte[]> abilityBPArray = Game.PlayerStats.GetRuneArray;
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nRune Blueprints");
                    foreach (byte[] categoryType in abilityBPArray)
                    {
                        foreach (byte equipmentState in categoryType)
                        {
                            writer.Write(equipmentState);
                            if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                Console.Write(" " + equipmentState);
                        }
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write("\n");
                    }

                    // Saving equipped items
                    sbyte[] equippedArray = Game.PlayerStats.GetEquippedArray;
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nEquipped Standard Item");
                    foreach (sbyte equipmentState in equippedArray)
                    {
                        writer.Write(equipmentState);
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + equipmentState);
                    }

                    // Saving equipped abilities
                    sbyte[] equippedAbilityArray = Game.PlayerStats.GetEquippedRuneArray;
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nEquipped Abilities");
                    foreach (sbyte equipmentState in equippedAbilityArray)
                    {
                        writer.Write(equipmentState);
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + equipmentState);
                    }

                    // Saving skills data.
                    SkillObj[] skillArray = SkillSystem.GetSkillArray();
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nskills");
                    foreach (SkillObj skill in skillArray)
                    {
                        writer.Write(skill.CurrentLevel);
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + skill.CurrentLevel);
                    }

                    if (saveBackup == true)
                    {
                        FileStream fileStream = stream as FileStream;
                        if (fileStream != null)
                            fileStream.Flush(true);
                    }

                    writer.Close();
                }
                stream.Close();
            }
        }

        private void SaveMap(bool saveBackup)
        {
            string fileName = m_fileNameMap;
            if (saveBackup == true)
                fileName = fileName.Insert(0, "AutoSave_");
            fileName = fileName.Insert(0, "Profile" + Game.GameConfig.ProfileSlot + "/");

            using (Stream stream = m_storageContainer.CreateFile(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nSaving Map");
                    
                    int newlineCounter = 0;

                    ProceduralLevelScreen levelToSave = Game.ScreenManager.GetLevelScreen();
                    if (levelToSave != null)
                    {

                        //Console.WriteLine("START");
                        //foreach (RoomObj room in levelToSave.RoomList)
                        //{
                        //    if (room.Name != "Boss" && room.Name != "Compass" && room.Name != "Bonus")
                        //    {
                        //        int count = 0;
                        //        foreach (GameObj obj in room.GameObjList)
                        //        {
                        //            if (obj is ChestObj)
                        //                count++;
                        //        }
                        //        Console.WriteLine(count);
                        //    }
                        //}
                        //Console.WriteLine("END");

                        //Console.WriteLine("Enemy start");
                        //foreach (RoomObj room in levelToSave.RoomList)
                        //{
                        //    int count = 0;
                        //    foreach (EnemyObj enemy in room.EnemyList)
                        //    {
                        //        if (enemy.IsProcedural == true)
                        //            count++;
                        //    }
                        //    Console.WriteLine(count);
                        //}
                        //Console.WriteLine("Enemy End");

                        // Store the number of rooms in the level first.
                        if (LevelEV.RUN_DEMO_VERSION == true)
                            writer.Write(levelToSave.RoomList.Count - 4); // Subtracting only 3 rooms because the demo only has 2 bosses (castle + last boss + tutorial room + compass).  IF THIS EVER CHANGES, THIS NEEDS TO BE CHANGED TOO.
                        else
                            writer.Write(levelToSave.RoomList.Count - 12); // Subtracting the 5 boss rooms + the tutorial room + compass room + 5 challenge rooms.

                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.WriteLine("Map size: " + (levelToSave.RoomList.Count - 12)); // Subtracting the 5 boss rooms + the tutorial room + the compass room + 5 challenge rooms.

                        List<byte> enemyTypeList = new List<byte>();
                        List<byte> enemyDifficultyList = new List<byte>();

                        // Storing the actual map.
                        foreach (RoomObj room in levelToSave.RoomList)
                        {
                            if (room.Name != "Boss" && room.Name != "Tutorial" && room.Name != "Ending" && room.Name != "Compass" && room.Name != "ChallengeBoss")// && room.Name != "Bonus") // Do not store boss rooms because they will be added when loaded.
                            {
                                // Saving the room's pool index.
                                writer.Write((int)room.PoolIndex);
                                // Saving the level type of the room. (byte)
                                writer.Write((byte)room.LevelType);
                                // Saving the level's position. (int)
                                writer.Write((int)room.X);
                                writer.Write((int)room.Y);

                                // Saving the room's colour.
                                writer.Write((byte)room.TextureColor.R);
                                writer.Write((byte)room.TextureColor.G);
                                writer.Write((byte)room.TextureColor.B);

                                if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                    Console.Write("I:" + room.PoolIndex + " T:" + (int)room.LevelType + ", ");
                                newlineCounter++;
                                if (newlineCounter > 5)
                                {
                                    newlineCounter = 0;
                                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                        Console.Write("\n");
                                }

                                foreach (EnemyObj enemy in room.EnemyList)
                                {
                                    if (enemy.IsProcedural == true)
                                    {
                                        enemyTypeList.Add(enemy.Type);
                                        enemyDifficultyList.Add((byte)enemy.Difficulty);
                                    }
                                }
                            }
                        }

                        // Saving the enemies in the level
                        int numEnemies = enemyTypeList.Count;
                        writer.Write(numEnemies); // Saving the number of enemies in the game. (int32)
                        foreach (byte enemyType in enemyTypeList)
                            writer.Write(enemyType); //(byte)

                        // Saving the difficulty of each enemy.
                        foreach (byte enemyDifficulty in enemyDifficultyList)
                            writer.Write(enemyDifficulty);
                    }
                    else
                        Console.WriteLine("WARNING: Attempting to save LEVEL screen but it was null. Make sure it exists in the screen manager before saving it.");

                    if (saveBackup == true)
                    {
                        FileStream fileStream = stream as FileStream;
                        if (fileStream != null)
                            fileStream.Flush(true);
                    }

                    writer.Close();
                }
                stream.Close();
            }
        }

        private void SaveMapData(bool saveBackup)
        {
            string fileName = m_fileNameMapData;
            if (saveBackup == true)
                fileName = fileName.Insert(0, "AutoSave_");
            fileName = fileName.Insert(0, "Profile" + Game.GameConfig.ProfileSlot + "/");

            using (Stream stream = m_storageContainer.CreateFile(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    ProceduralLevelScreen levelToSave = Game.ScreenManager.GetLevelScreen();
                    if (levelToSave != null)
                    {
                        List<RoomObj> levelMapList = levelToSave.MapRoomsAdded;

                        List<bool> roomVisited = new List<bool>();
                        List<bool> bonusRoomCompleted = new List<bool>();
                        List<int> bonusRoomData = new List<int>();
                        List<bool> chestOpened = new List<bool>();
                        List<byte> chestTypes = new List<byte>();
                        List<bool> fairyChestFailed = new List<bool>();
                        List<bool> enemyDead = new List<bool>();
                        List<bool> breakablesOpened = new List<bool>();

                        foreach (RoomObj room in levelToSave.RoomList)
                        {
                            if (levelMapList.Contains(room))
                                roomVisited.Add(true);
                            else
                                roomVisited.Add(false);

                            BonusRoomObj bonusRoom = room as BonusRoomObj;
                            if (bonusRoom != null)
                            {
                                if (bonusRoom.RoomCompleted == true)
                                    bonusRoomCompleted.Add(true);
                                else
                                    bonusRoomCompleted.Add(false);

                                bonusRoomData.Add(bonusRoom.ID);
                            }

                            if (room.Name != "Boss" && room.Name != "ChallengeBoss")// && room.Name != "Bonus")
                            {
                                foreach (EnemyObj enemy in room.EnemyList)
                                {
                                    if (enemy.IsKilled == true)
                                        enemyDead.Add(true);
                                    else
                                        enemyDead.Add(false);
                                }
                            }

                            if (room.Name != "Bonus" && room.Name != "Boss" && room.Name != "Compass" && room.Name != "ChallengeBoss") // Don't save bonus room or boss room chests, or bonus room breakables (it's ok if they reset).
                            {
                                foreach (GameObj obj in room.GameObjList)
                                {
                                    // Saving breakables state.
                                    BreakableObj breakable = obj as BreakableObj;
                                    if (breakable != null)
                                    {
                                        if (breakable.Broken == true)
                                            breakablesOpened.Add(true);
                                        else
                                            breakablesOpened.Add(false);
                                    }

                                    // Saving chest states.
                                    ChestObj chest = obj as ChestObj;
                                    if (chest != null)
                                    {
                                        chestTypes.Add(chest.ChestType);

                                        if (chest.IsOpen == true)
                                            chestOpened.Add(true);
                                        else
                                            chestOpened.Add(false);

                                        FairyChestObj fairyChest = chest as FairyChestObj;
                                        if (fairyChest != null)
                                        {
                                            if (fairyChest.State == ChestConditionChecker.STATE_FAILED)
                                                fairyChestFailed.Add(true);
                                            else
                                                fairyChestFailed.Add(false);
                                        }
                                    }
                                }
                            }
                        }

                        // Saving rooms visited state.
                        writer.Write(roomVisited.Count); // int32
                        foreach (bool state in roomVisited)
                            writer.Write(state);

                        // Saving the state of the bonus rooms.
                        writer.Write(bonusRoomCompleted.Count); // int32
                        foreach (bool state in bonusRoomCompleted)
                            writer.Write(state);

                        // Saving the data of the bonus rooms.
                        // No need to save number of bonus rooms because that's saved above.
                        foreach (int data in bonusRoomData)
                            writer.Write(data);

                        // Saving the type of chests.
                        writer.Write(chestTypes.Count); // int32
                        foreach (byte type in chestTypes)
                            writer.Write(type);

                        // Saving the state of chests.
                        writer.Write(chestOpened.Count); // int32
                        foreach (bool state in chestOpened)
                            writer.Write(state);

                        // Saving the state of fairy chests.
                        writer.Write(fairyChestFailed.Count); // int32
                        foreach (bool state in fairyChestFailed)
                            writer.Write(state);

                        // Saving the state of enemies
                        writer.Write(enemyDead.Count); // int32
                        foreach (bool state in enemyDead)
                            writer.Write(state);

                        // Saving breakable object states
                        writer.Write(breakablesOpened.Count); // int32
                        foreach (bool state in breakablesOpened)
                            writer.Write(state);
                    }
                    else
                        Console.WriteLine("WARNING: Attempting to save level screen MAP data but level was null. Make sure it exists in the screen manager before saving it.");
                    
                    if (saveBackup == true)
                    {
                        FileStream fileStream = stream as FileStream;
                        if (fileStream != null)
                            fileStream.Flush(true);
                    }

                    writer.Close();
                }
                stream.Close();
            }
        }

        private void SaveLineageData(bool saveBackup)
        {
            string fileName = m_fileNameLineage;
            if (saveBackup == true)
                fileName = fileName.Insert(0, "AutoSave_");
            fileName = fileName.Insert(0, "Profile" + Game.GameConfig.ProfileSlot + "/");

            using (Stream stream = m_storageContainer.CreateFile(fileName))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Console.WriteLine("///// PLAYER LINEAGE DATA - BEGIN SAVING /////");
                    // Saving the currently created branch.
                    List<PlayerLineageData> currentBranches = Game.PlayerStats.CurrentBranches;
                    int numChildren = 0;

                    if (currentBranches != null)
                    {
                        numChildren = currentBranches.Count;
                        writer.Write(numChildren);

                        for (int i = 0; i < numChildren; i++)
                        {
                            // If you have an old version save file, update the name to match the new format before saving.
                            if (Game.PlayerStats.RevisionNumber <= 0)
                            {
                                string playerName = currentBranches[i].Name;
                                string romanNumeral = "";
                                Game.ConvertPlayerNameFormat(ref playerName, ref romanNumeral);
                                PlayerLineageData data = currentBranches[i];
                                data.Name = playerName;
                                data.RomanNumeral = romanNumeral;
                                currentBranches[i] = data;
                            }

                            writer.Write(currentBranches[i].Name); // string
                            writer.Write(currentBranches[i].Spell); // byte
                            writer.Write(currentBranches[i].Class); // byte
                            writer.Write(currentBranches[i].HeadPiece); // byte
                            writer.Write(currentBranches[i].ChestPiece); // byte
                            writer.Write(currentBranches[i].ShoulderPiece); // byte

                            writer.Write(currentBranches[i].Age); // byte
                            writer.Write(currentBranches[i].ChildAge); // byte

                            writer.Write((byte)currentBranches[i].Traits.X); // byte
                            writer.Write((byte)currentBranches[i].Traits.Y); // byte
                            writer.Write(currentBranches[i].IsFemale); // bool
                            writer.Write(currentBranches[i].RomanNumeral); // string
                        }
                    }
                    else
                        writer.Write(numChildren); // Writing zero children.

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Saving Current Branch Lineage Data");
                        for (int i = 0; i < numChildren; i++)
                        {
                            Console.WriteLine("Player Name: " + currentBranches[i].Name);
                            Console.WriteLine("Spell: " + currentBranches[i].Name);
                            Console.WriteLine("Class: " + currentBranches[i].Name);
                            Console.WriteLine("Head Piece: " + currentBranches[i].HeadPiece);
                            Console.WriteLine("Chest Piece: " + currentBranches[i].ChestPiece);
                            Console.WriteLine("Shoulder Piece: " + currentBranches[i].ShoulderPiece);
                            Console.WriteLine("Player Age: " + currentBranches[i].Age);
                            Console.WriteLine("Player Child Age: " + currentBranches[i].ChildAge);
                            Console.WriteLine("Traits: " + currentBranches[i].Traits.X + ", " + currentBranches[i].Traits.Y);
                            Console.WriteLine("Is Female: " + currentBranches[i].IsFemale);
                            Console.WriteLine("Roman Numeral: " + currentBranches[i].RomanNumeral);
                        }
                    }

                    ////////////////////////////////////////

                    // Saving family tree info
                    List<FamilyTreeNode> familyTreeArray = Game.PlayerStats.FamilyTreeArray;
                    int numBranches = 0;

                    if (familyTreeArray != null)
                    {
                        numBranches = familyTreeArray.Count;
                        writer.Write(numBranches);

                        for (int i = 0; i < numBranches; i++)
                        {
                            // If you have an old version save file, update the name to match the new format before saving.
                            if (Game.PlayerStats.RevisionNumber <= 0)
                            {
                                string playerName = familyTreeArray[i].Name;
                                string romanNumeral = "";
                                Game.ConvertPlayerNameFormat(ref playerName, ref romanNumeral);
                                FamilyTreeNode data = familyTreeArray[i];
                                data.Name = playerName;
                                data.RomanNumeral = romanNumeral;
                                familyTreeArray[i] = data;
                            }

                            writer.Write(familyTreeArray[i].Name); // string
                            writer.Write(familyTreeArray[i].Age); // byte
                            writer.Write(familyTreeArray[i].Class); // byte
                            writer.Write(familyTreeArray[i].HeadPiece); // byte
                            writer.Write(familyTreeArray[i].ChestPiece); // byte
                            writer.Write(familyTreeArray[i].ShoulderPiece); // byte
                            writer.Write(familyTreeArray[i].NumEnemiesBeaten); // int
                            writer.Write(familyTreeArray[i].BeatenABoss); // bool
                            writer.Write((byte)familyTreeArray[i].Traits.X); // byte
                            writer.Write((byte)familyTreeArray[i].Traits.Y); // byte
                            writer.Write(familyTreeArray[i].IsFemale); // bool
                            writer.Write(familyTreeArray[i].RomanNumeral); // string
                        }
                    }
                    else
                        writer.Write(numBranches); // Tree has no nodes.

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Saving Family Tree Data");
                        Console.WriteLine("Number of Branches: " + numBranches);
                        for (int i = 0; i < numBranches; i++)
                        {
                            Console.WriteLine("/// Saving branch");
                            Console.WriteLine("Name: " + familyTreeArray[i].Name);
                            Console.WriteLine("Age: " + familyTreeArray[i].Age);
                            Console.WriteLine("Class: " + familyTreeArray[i].Class);
                            Console.WriteLine("Head Piece: " + familyTreeArray[i].HeadPiece);
                            Console.WriteLine("Chest Piece: " + familyTreeArray[i].ChestPiece);
                            Console.WriteLine("Shoulder Piece: " + familyTreeArray[i].ShoulderPiece);
                            Console.WriteLine("Number of Enemies Beaten: " + familyTreeArray[i].NumEnemiesBeaten);
                            Console.WriteLine("Beaten a Boss: " + familyTreeArray[i].BeatenABoss);
                            Console.WriteLine("Traits: " + familyTreeArray[i].Traits.X + ", " + familyTreeArray[i].Traits.Y);
                            Console.WriteLine("Is Female: " + familyTreeArray[i].IsFemale);
                            Console.WriteLine("Roman Numeral: " + familyTreeArray[i].RomanNumeral);
                        }
                    }

                    Console.WriteLine("///// PLAYER LINEAGE DATA - SAVE COMPLETE /////");
                    if (saveBackup == true)
                    {
                        FileStream fileStream = stream as FileStream;
                        if (fileStream != null)
                            fileStream.Flush(true);
                    }

                    writer.Close();
                }
                stream.Close();
            }
        }

        private void LoadData(SaveType loadType, ProceduralLevelScreen level)
        {
            if (FileExists(loadType))
            {
                switch (loadType)
                {
                    case (SaveType.PlayerData):
                        //if (Game.ScreenManager.Player != null)
                            LoadPlayerData();
                        break;
                    case (SaveType.UpgradeData):
                        //if (Game.ScreenManager.Player != null)
                            LoadUpgradeData();
                        break;
                    case (SaveType.Map):
                        Console.WriteLine("Cannot load Map directly from LoadData. Call LoadMap() instead.");
                        //if (Game.PlayerStats.IsDead == false)
                        //{
                        //    ProceduralLevelScreen createdLevel = LoadMap();
                        //    createdLevel.LoadGameData = true;
                        //}
                        //else
                        //    Game.ScreenManager.DisplayScreen(ScreenType.Lineage, true, null);
                        break;
                    case (SaveType.MapData):
                        if (level != null)
                            LoadMapData(level);
                        else
                            Console.WriteLine("Could not load Map data. Level was null.");
                        break;
                    case (SaveType.Lineage):
                        LoadLineageData();
                        break;
                }

                Console.WriteLine("\nData of type " + loadType + " Loaded.");
            }
            else
                Console.WriteLine("Could not load data of type " + loadType + " because data did not exist.");
        }

        private void LoadPlayerData()
        {
            using (Stream stream = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    //Game.PlayerStats.PlayerPosition = new Vector2(reader.ReadInt32(), reader.ReadInt32());
                    Game.PlayerStats.Gold = reader.ReadInt32();
                    Game.PlayerStats.CurrentHealth = reader.ReadInt32();
                    Game.PlayerStats.CurrentMana = reader.ReadInt32();
                    Game.PlayerStats.Age = reader.ReadByte();
                    Game.PlayerStats.ChildAge = reader.ReadByte();
                    Game.PlayerStats.Spell = reader.ReadByte();
                    Game.PlayerStats.Class = reader.ReadByte();
                    Game.PlayerStats.SpecialItem = reader.ReadByte();
                    Game.PlayerStats.Traits = new Vector2(reader.ReadByte(), reader.ReadByte());
                    Game.PlayerStats.PlayerName = reader.ReadString();

                    // Reading player parts
                    Game.PlayerStats.HeadPiece = reader.ReadByte();
                    Game.PlayerStats.ShoulderPiece = reader.ReadByte();
                    Game.PlayerStats.ChestPiece = reader.ReadByte();

                    // Necessary to kick in the try catch block in CDGSplashScreen since reader.ReadByte() returns 0 instead of crashing when reading incorrect data.
                    //if (Game.PlayerStats.HeadPiece == 0 || Game.PlayerStats.ShoulderPiece == 0 || Game.PlayerStats.ChestPiece == 0)
                    if (Game.PlayerStats.HeadPiece == 0 && Game.PlayerStats.ShoulderPiece == 0 && Game.PlayerStats.ChestPiece == 0)
                       throw new Exception("Corrupted Save File: All equipment pieces are 0.");

                    // Reading Diary entry position
                    Game.PlayerStats.DiaryEntry = reader.ReadByte();

                    // Reading bonus stats.
                    Game.PlayerStats.BonusHealth = reader.ReadInt32();
                    Game.PlayerStats.BonusStrength = reader.ReadInt32();
                    Game.PlayerStats.BonusMana = reader.ReadInt32();
                    Game.PlayerStats.BonusDefense = reader.ReadInt32();
                    Game.PlayerStats.BonusWeight = reader.ReadInt32();
                    Game.PlayerStats.BonusMagic = reader.ReadInt32();

                    // Reading lich stats.
                    Game.PlayerStats.LichHealth = reader.ReadInt32();
                    Game.PlayerStats.LichMana = reader.ReadInt32();
                    Game.PlayerStats.LichHealthMod = reader.ReadSingle();

                    // Reading boss progress states
                    Game.PlayerStats.NewBossBeaten = reader.ReadBoolean();
                    Game.PlayerStats.EyeballBossBeaten = reader.ReadBoolean();
                    Game.PlayerStats.FairyBossBeaten = reader.ReadBoolean();
                    Game.PlayerStats.FireballBossBeaten = reader.ReadBoolean();
                    Game.PlayerStats.BlobBossBeaten = reader.ReadBoolean();
                    Game.PlayerStats.LastbossBeaten = reader.ReadBoolean();

                    // Reading new game plus progress
                    Game.PlayerStats.TimesCastleBeaten = reader.ReadInt32();
                    Game.PlayerStats.NumEnemiesBeaten = reader.ReadInt32();

                    // Loading misc flags
                    Game.PlayerStats.TutorialComplete = reader.ReadBoolean();
                    Game.PlayerStats.CharacterFound = reader.ReadBoolean();
                    Game.PlayerStats.LoadStartingRoom = reader.ReadBoolean();

                    Game.PlayerStats.LockCastle = reader.ReadBoolean();
                    Game.PlayerStats.SpokeToBlacksmith = reader.ReadBoolean();
                    Game.PlayerStats.SpokeToEnchantress = reader.ReadBoolean();
                    Game.PlayerStats.SpokeToArchitect = reader.ReadBoolean();
                    Game.PlayerStats.SpokeToTollCollector = reader.ReadBoolean();
                    Game.PlayerStats.IsDead = reader.ReadBoolean();
                    Game.PlayerStats.FinalDoorOpened = reader.ReadBoolean();
                    Game.PlayerStats.RerolledChildren = reader.ReadBoolean();
                    Game.PlayerStats.IsFemale = reader.ReadBoolean();
                    Game.PlayerStats.TimesDead = reader.ReadInt32();
                    Game.PlayerStats.HasArchitectFee = reader.ReadBoolean();
                    Game.PlayerStats.ReadLastDiary = reader.ReadBoolean();
                    Game.PlayerStats.SpokenToLastBoss = reader.ReadBoolean();
                    Game.PlayerStats.HardcoreMode = reader.ReadBoolean();
                    Game.PlayerStats.TotalHoursPlayed = reader.ReadSingle();

                    // Loading wizard spells
                    byte wizardSpell1 = reader.ReadByte();
                    byte wizardSpell2 = reader.ReadByte();
                    byte wizardSpell3 = reader.ReadByte();

                    Game.PlayerStats.WizardSpellList = new Vector3(wizardSpell1, wizardSpell2, wizardSpell3);

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("\nLoading Player Stats");
                        Console.WriteLine("Gold: " + Game.PlayerStats.Gold);
                        Console.WriteLine("Current Health: " + Game.PlayerStats.CurrentHealth);
                        Console.WriteLine("Current Mana: " + Game.PlayerStats.CurrentMana);
                        Console.WriteLine("Age: " + Game.PlayerStats.Age);
                        Console.WriteLine("Child Age: " + Game.PlayerStats.ChildAge);
                        Console.WriteLine("Spell: " + Game.PlayerStats.Spell);
                        Console.WriteLine("Class: " + Game.PlayerStats.Class);
                        Console.WriteLine("Special Item: " + Game.PlayerStats.SpecialItem);
                        Console.WriteLine("Traits: " + Game.PlayerStats.Traits.X + ", " + Game.PlayerStats.Traits.Y);
                        Console.WriteLine("Name: " + Game.PlayerStats.PlayerName);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Head Piece: " + Game.PlayerStats.HeadPiece);
                        Console.WriteLine("Shoulder Piece: " + Game.PlayerStats.ShoulderPiece);
                        Console.WriteLine("Chest Piece: " + Game.PlayerStats.ChestPiece);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Diary Entry: " + Game.PlayerStats.DiaryEntry);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Bonus Health: " + Game.PlayerStats.BonusHealth);
                        Console.WriteLine("Bonus Strength: " + Game.PlayerStats.BonusStrength);
                        Console.WriteLine("Bonus Mana: " + Game.PlayerStats.BonusMana);
                        Console.WriteLine("Bonus Armor: " + Game.PlayerStats.BonusDefense);
                        Console.WriteLine("Bonus Weight: " + Game.PlayerStats.BonusWeight);
                        Console.WriteLine("Bonus Magic: " + Game.PlayerStats.BonusMagic);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Lich Health: " + Game.PlayerStats.LichHealth);
                        Console.WriteLine("Lich Mana: " + Game.PlayerStats.LichMana);
                        Console.WriteLine("Lich Health Mod: " + Game.PlayerStats.LichHealthMod);
                        Console.WriteLine("---------------");
                        Console.WriteLine("New Boss Beaten: " + Game.PlayerStats.NewBossBeaten);
                        Console.WriteLine("Eyeball Boss Beaten: " + Game.PlayerStats.EyeballBossBeaten);
                        Console.WriteLine("Fairy Boss Beaten: " + Game.PlayerStats.FairyBossBeaten);
                        Console.WriteLine("Fireball Boss Beaten: " + Game.PlayerStats.FireballBossBeaten);
                        Console.WriteLine("Blob Boss Beaten: " + Game.PlayerStats.BlobBossBeaten);
                        Console.WriteLine("Last Boss Beaten: " + Game.PlayerStats.LastbossBeaten);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Times Castle Beaten: " + Game.PlayerStats.TimesCastleBeaten);
                        Console.WriteLine("Number of Enemies Beaten: " + Game.PlayerStats.NumEnemiesBeaten);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Tutorial Complete: " + Game.PlayerStats.TutorialComplete);
                        Console.WriteLine("Character Found: " + Game.PlayerStats.CharacterFound);
                        Console.WriteLine("Load Starting Room: " + Game.PlayerStats.LoadStartingRoom);
                        Console.WriteLine("---------------");
                        Console.WriteLine("Castle Locked: " + Game.PlayerStats.LockCastle);
                        Console.WriteLine("Spoke to Blacksmith: " + Game.PlayerStats.SpokeToBlacksmith);
                        Console.WriteLine("Spoke to Enchantress: " + Game.PlayerStats.SpokeToEnchantress);
                        Console.WriteLine("Spoke to Architect: " + Game.PlayerStats.SpokeToArchitect);
                        Console.WriteLine("Spoke to Toll Collector: " + Game.PlayerStats.SpokeToTollCollector);
                        Console.WriteLine("Player Is Dead: " + Game.PlayerStats.IsDead);
                        Console.WriteLine("Final Door Opened: " + Game.PlayerStats.FinalDoorOpened);
                        Console.WriteLine("Rerolled Children: " + Game.PlayerStats.RerolledChildren);
                        Console.WriteLine("Is Female: " + Game.PlayerStats.IsFemale);
                        Console.WriteLine("Times Dead: " + Game.PlayerStats.TimesDead);
                        Console.WriteLine("Has Architect Fee: " + Game.PlayerStats.HasArchitectFee);
                        Console.WriteLine("Player read last diary: " + Game.PlayerStats.ReadLastDiary);
                        Console.WriteLine("Player has spoken to last boss: " + Game.PlayerStats.SpokenToLastBoss);
                        Console.WriteLine("Is Hardcore mode: " + Game.PlayerStats.HardcoreMode);
                        Console.WriteLine("Total Hours Played " + Game.PlayerStats.TotalHoursPlayed);
                        Console.WriteLine("Wizard Spell 1: " + Game.PlayerStats.WizardSpellList.X);
                        Console.WriteLine("Wizard Spell 2: " + Game.PlayerStats.WizardSpellList.Y);
                        Console.WriteLine("Wizard Spell 3: " + Game.PlayerStats.WizardSpellList.Z);
                    }

                    Console.WriteLine("///// ENEMY LIST DATA - BEGIN LOADING /////");

                    for (int i = 0; i < EnemyType.Total; i++)
                    {
                        Vector4 enemyStats = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                        Game.PlayerStats.EnemiesKilledList[i] = enemyStats;
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Loading Enemy List Data");
                        int counter = 0;
                        foreach (Vector4 enemy in Game.PlayerStats.EnemiesKilledList)
                        {
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Basic, Killed: " + enemy.X);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Advanced, Killed: " + enemy.Y);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Expert, Killed: " + enemy.Z);
                            Console.WriteLine("Enemy Type: " + counter + ", Difficulty: Miniboss, Killed: " + enemy.W);

                            counter++;
                        }
                    }

                    int numKilledEnemiesInRun = reader.ReadInt32();

                    for (int i = 0; i < numKilledEnemiesInRun; i++)
                    {
                        Vector2 enemyKilled = new Vector2(reader.ReadInt32(), reader.ReadInt32());
                        Game.PlayerStats.EnemiesKilledInRun.Add(enemyKilled);
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("Loading num enemies killed");
                        Console.WriteLine("Number of enemies killed in run: " + numKilledEnemiesInRun);
                        foreach (Vector2 enemy in Game.PlayerStats.EnemiesKilledInRun)
                        {
                            Console.WriteLine("Enemy Room Index: " + enemy.X);
                            Console.WriteLine("Enemy Index in EnemyList: " + enemy.Y);
                        }
                    }

                    Console.WriteLine("///// ENEMY LIST DATA - LOAD COMPLETE /////");



                    if (reader.PeekChar() != -1) // If a character is found, then DLC is in the file.
                    {
                        Console.WriteLine("///// DLC DATA FOUND - BEGIN LOADING /////");
                        Game.PlayerStats.ChallengeEyeballUnlocked = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeSkullUnlocked = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeFireballUnlocked = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeBlobUnlocked = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeLastBossUnlocked = reader.ReadBoolean();

                        Game.PlayerStats.ChallengeEyeballBeaten = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeSkullBeaten = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeFireballBeaten = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeBlobBeaten = reader.ReadBoolean();
                        Game.PlayerStats.ChallengeLastBossBeaten = reader.ReadBoolean();

                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        {
                            Console.WriteLine("Eyeball Challenge Unlocked: " + Game.PlayerStats.ChallengeEyeballUnlocked);
                            Console.WriteLine("Skull Challenge Unlocked: " + Game.PlayerStats.ChallengeSkullUnlocked);
                            Console.WriteLine("Fireball Challenge Unlocked: " + Game.PlayerStats.ChallengeFireballUnlocked);
                            Console.WriteLine("Blob Challenge Unlocked: " + Game.PlayerStats.ChallengeBlobUnlocked);
                            Console.WriteLine("Last Boss Challenge Unlocked: " + Game.PlayerStats.ChallengeLastBossUnlocked);

                            Console.WriteLine("Eyeball Challenge Beaten: " + Game.PlayerStats.ChallengeEyeballBeaten);
                            Console.WriteLine("Skull Challenge Beaten: " + Game.PlayerStats.ChallengeSkullBeaten);
                            Console.WriteLine("Fireball Challenge Beaten: " + Game.PlayerStats.ChallengeFireballBeaten);
                            Console.WriteLine("Blob Challenge Beaten: " + Game.PlayerStats.ChallengeBlobBeaten);
                            Console.WriteLine("Last Boss Challenge Beaten: " + Game.PlayerStats.ChallengeLastBossBeaten);
                        }

                        if (reader.PeekChar() != -1) // Even more DLC added
                        {
                            Game.PlayerStats.ChallengeEyeballTimesUpgraded = reader.ReadSByte();
                            Game.PlayerStats.ChallengeSkullTimesUpgraded = reader.ReadSByte();
                            Game.PlayerStats.ChallengeFireballTimesUpgraded = reader.ReadSByte();
                            Game.PlayerStats.ChallengeBlobTimesUpgraded = reader.ReadSByte();
                            Game.PlayerStats.ChallengeLastBossTimesUpgraded = reader.ReadSByte();
                            Game.PlayerStats.RomanNumeral = reader.ReadString();
                            Game.PlayerStats.HasProsopagnosia = reader.ReadBoolean();
                            Game.PlayerStats.RevisionNumber = reader.ReadInt32();

                            if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            {
                                Console.WriteLine("Eyeball Challenge Times Upgraded: " + Game.PlayerStats.ChallengeEyeballTimesUpgraded);
                                Console.WriteLine("Skull Challenge Times Upgraded: " + Game.PlayerStats.ChallengeSkullTimesUpgraded);
                                Console.WriteLine("Fireball Challenge Times Upgraded: " + Game.PlayerStats.ChallengeFireballTimesUpgraded);
                                Console.WriteLine("Blob Challenge Times Upgraded: " + Game.PlayerStats.ChallengeBlobTimesUpgraded);
                                Console.WriteLine("Last Boss Challenge Times Upgraded: " + Game.PlayerStats.ChallengeLastBossTimesUpgraded);
                                Console.WriteLine("Player Name Number: " + Game.PlayerStats.RomanNumeral);
                                Console.WriteLine("Player has Prosopagnosia: " + Game.PlayerStats.HasProsopagnosia);
                            }

                            if (reader.PeekChar() != -1) // Even more...
                            {
                                Game.PlayerStats.ArchitectUsed = reader.ReadBoolean();

                                if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                {
                                    Console.WriteLine("Architect used: " + Game.PlayerStats.ArchitectUsed);
                                }
                            }
                        }

                        Console.WriteLine("///// DLC DATA - LOADING COMPLETE /////");
                    }
                    else
                        Console.WriteLine("///// NO DLC DATA FOUND - SKIPPED LOADING /////");

                    reader.Close();
                }
                stream.Close();
            }
        }

        private void LoadUpgradeData()
        {
            using (Stream stream = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                    {
                        Console.WriteLine("\nLoading Equipment States");
                        Console.WriteLine("\nLoading Standard Blueprints");
                    }

                    List<byte[]> blueprintArray = Game.PlayerStats.GetBlueprintArray;
                    for (int i = 0; i < EquipmentCategoryType.Total; i++)
                    {
                        for (int k = 0; k < EquipmentBaseType.Total; k++)
                        {
                            blueprintArray[i][k] = reader.ReadByte();
                            if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                Console.Write(" " + blueprintArray[i][k]);
                        }
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write("\n");
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nLoading Ability Blueprints");
                    List<byte[]> abilityBPArray = Game.PlayerStats.GetRuneArray;
                    for (int i = 0; i < EquipmentCategoryType.Total; i++)
                    {
                        for (int k = 0; k < EquipmentAbilityType.Total; k++)
                        {
                            abilityBPArray[i][k] = reader.ReadByte();
                            if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                                Console.Write(" " + abilityBPArray[i][k]);
                        }
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write("\n");
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nLoading Equipped Standard Items");
                    sbyte[] equippedArray = Game.PlayerStats.GetEquippedArray;
                    for (int i = 0; i < EquipmentCategoryType.Total; i++)
                    {
                        equippedArray[i] = reader.ReadSByte();
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + equippedArray[i]);
                    }

                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nLoading Equipped Abilities");
                    sbyte[] equippedAbilityArray = Game.PlayerStats.GetEquippedRuneArray;
                    for (int i = 0; i < EquipmentCategoryType.Total; i++)
                    {
                        equippedAbilityArray[i] = reader.ReadSByte();
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + equippedAbilityArray[i]);
                    }

                    SkillObj[] traitArray = SkillSystem.GetSkillArray();
                    if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        Console.WriteLine("\nLoading Traits");

                    SkillSystem.ResetAllTraits(); // Reset all traits first.
                    Game.PlayerStats.CurrentLevel = 0; // Reset player level.
                    for (int i = 0; i < (int)SkillType.DIVIDER - 2; i++)  //The starting 2 traits are null and filler.
                    {
                        int traitLevel = reader.ReadInt32();
                        for (int k = 0; k < traitLevel; k++)
                            SkillSystem.LevelUpTrait(traitArray[i], false);
                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                            Console.Write(" " + traitArray[i].CurrentLevel);
                    }
                    reader.Close();

                    Game.ScreenManager.Player.UpdateEquipmentColours();
                }
                stream.Close();

                // These checks probably aren't necessary anymore.
                // Your game was corrupted if you have double jump equipped but no enchantress unlocked.
                //if (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.DoubleJump) > 0 && SkillSystem.GetSkill(SkillType.Enchanter).CurrentLevel < 1 && LevelEV.CREATE_RETAIL_VERSION == true)
                //    throw new Exception("Corrupted Save file");

                // Another check for corruption on the skill tree
                //bool possibleCorruption = false;
                //List<FamilyTreeNode> familyTree = Game.PlayerStats.FamilyTreeArray;
                //foreach (FamilyTreeNode node in familyTree)
                //{
                //    if (node.Class > ClassType.Assassin)
                //    {
                //        possibleCorruption = true;
                //        break;
                //    }
                //}

                //// If you ever find a class that is normally locked in the family tree, and the smithy is not unlocked, then the file is corrupt.
                //if (possibleCorruption == true && SkillSystem.GetSkill(SkillType.Smithy).CurrentLevel < 1 && LevelEV.CREATE_RETAIL_VERSION == true)
                //    throw new Exception("Corrupted Save file");
            }
        }

        public ProceduralLevelScreen LoadMap()
        {
            GetStorageContainer();
            ProceduralLevelScreen loadedLevel = null;
            using (Stream stream = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int roomSize = reader.ReadInt32();
                    Vector4[] roomList = new Vector4[roomSize];
                    Vector3[] roomColorList = new Vector3[roomSize];

                    for (int i = 0; i < roomSize; i++)
                    {
                        roomList[i].W = reader.ReadInt32(); // Reading the pool index.
                        roomList[i].X = reader.ReadByte(); // Reading the level type.
                        roomList[i].Y = reader.ReadInt32(); // Reading the level's X pos.
                        roomList[i].Z = reader.ReadInt32(); // Reading the level's Y pos.

                        roomColorList[i].X = reader.ReadByte(); // Reading RGB
                        roomColorList[i].Y = reader.ReadByte();
                        roomColorList[i].Z = reader.ReadByte();
                    }

                    loadedLevel = LevelBuilder2.CreateLevel(roomList, roomColorList);

                    int numEnemies = reader.ReadInt32(); // Reading the number of enemies in the game.
                    List<byte> enemyList = new List<byte>();
                    for (int i = 0; i < numEnemies; i++)
                        enemyList.Add(reader.ReadByte()); // Reading the enemy type and storing it into the array.

                    List<byte> enemyDifficultyList = new List<byte>();
                    for (int i = 0; i < numEnemies; i++) // Reading the enemy difficulty and storing it in an array.
                        enemyDifficultyList.Add(reader.ReadByte());

                    LevelBuilder2.OverrideProceduralEnemies(loadedLevel, enemyList.ToArray(), enemyDifficultyList.ToArray());

                    reader.Close();
                }
                stream.Close();
            }
            m_storageContainer.Dispose();

            return loadedLevel;
        }

        private void LoadMapData(ProceduralLevelScreen createdLevel)
        {
            //createdLevel.InitializeChests(true); // Can't remember why this was put here.  It was disabled because it was shifting chests twice, screwing up their positions.

            using (Stream stream = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // Loading room visited state.
                    int numRooms = reader.ReadInt32();
                    List<bool> roomsVisited = new List<bool>();
                    for (int i = 0; i < numRooms; i++)
                        roomsVisited.Add(reader.ReadBoolean());

                    // Loading bonus room states.
                    int numBonusRooms = reader.ReadInt32();
                    List<bool> bonusRoomStates = new List<bool>();
                    for (int i = 0; i < numBonusRooms; i++)
                        bonusRoomStates.Add(reader.ReadBoolean());

                    // Loading bonus room data.
                    List<int> bonusRoomData = new List<int>();
                    for (int i = 0; i < numBonusRooms; i++)
                        bonusRoomData.Add(reader.ReadInt32());

                    // Loading chest types
                    int numChests = reader.ReadInt32();
                    List<byte> chestTypes = new List<byte>();
                    for (int i = 0; i < numChests; i++)
                        chestTypes.Add(reader.ReadByte());

                    // Loading chest states
                    numChests = reader.ReadInt32();
                    List<bool> chestStates = new List<bool>();
                    for (int i = 0; i < numChests; i++)
                        chestStates.Add(reader.ReadBoolean());

                    // Loading fairy chest states
                    numChests = reader.ReadInt32();
                    List<bool> fairyChestStates = new List<bool>();
                    for (int i = 0; i < numChests; i++)
                        fairyChestStates.Add(reader.ReadBoolean());

                    // Loading enemy states
                    int numEnemies = reader.ReadInt32();
                    List<bool> enemyStates = new List<bool>();
                    for (int i = 0; i < numEnemies; i++)
                        enemyStates.Add(reader.ReadBoolean());

                    // Loading breakable object states
                    int numBreakables = reader.ReadInt32();
                    List<bool> breakableStates = new List<bool>();
                    for (int i = 0; i < numBreakables; i++)
                        breakableStates.Add(reader.ReadBoolean());

                    //int roomCounter = 0;
                    int bonusRoomCounter = 0;
                    int chestTypeCounter = 0;
                    int chestCounter = 0;
                    int fairyChestCounter = 0;
                    int enemyCounter = 0;
                    int breakablesCounter = 0;

                    //foreach (RoomObj room in createdLevel.RoomList)
                    //{
                    //    if (room.Name != "Boss")
                    //    {
                    //        int counter = 0;
                    //        foreach (GameObj obj in room.GameObjList)
                    //        {
                    //            if (obj is ChestObj)
                    //                counter++;
                    //        }
                    //        Console.WriteLine(counter);
                    //    }
                    //}

                    foreach (RoomObj room in createdLevel.RoomList)
                    {
                         //DO NOT set the state of rooms visited yet. This must be done AFTER chest states, otherwise the map won't update properly.

                        // Setting the state of bonus rooms.
                        if (numBonusRooms > 0)
                        {
                            BonusRoomObj bonusRoom = room as BonusRoomObj;
                            if (bonusRoom != null)
                            {
                                bool bonusRoomState = bonusRoomStates[bonusRoomCounter];
                                int roomData = bonusRoomData[bonusRoomCounter];

                                bonusRoomCounter++;

                                if (bonusRoomState == true)
                                    bonusRoom.RoomCompleted = true;

                                bonusRoom.ID = roomData;
                            }
                        }

                        // Setting the state of enemies.
                        // Only bring enemies back to life if you are locking the castle (i.e. not reloading a file but starting a new lineage).
                        if (numEnemies > 0)
                        {
                            if (Game.PlayerStats.LockCastle == false)
                            {
                                if (room.Name != "Boss" && room.Name != "ChallengeBoss")// && room.Name != "Bonus")
                                {
                                    foreach (EnemyObj enemy in room.EnemyList)
                                    {
                                        bool enemyState = enemyStates[enemyCounter];
                                        enemyCounter++;

                                        if (enemyState == true)
                                            enemy.KillSilently();
                                    }
                                }
                            }
                        }

                        // Setting the states of chests.
                        if (room.Name != "Bonus" && room.Name != "Boss" && room.Name != "Compass" && room.Name != "ChallengeBoss") // Don't save bonus room chests or breakables.
                        {
                            foreach (GameObj obj in room.GameObjList)
                            {
                                // Only save breakable states if the castle is not locked.
                                if (Game.PlayerStats.LockCastle == false)
                                {
                                    if (numBreakables > 0)
                                    {
                                        BreakableObj breakable = obj as BreakableObj;
                                        if (breakable != null)
                                        {
                                            bool breakableState = breakableStates[breakablesCounter];
                                            breakablesCounter++;
                                            if (breakableState == true)
                                                breakable.ForceBreak();
                                        }
                                    }
                                }

                                //if (numChests > 0)
                                //{
                                ChestObj chest = obj as ChestObj;
                                if (chest != null)
                                {
                                    chest.IsProcedural = false;
                                    byte chestType = chestTypes[chestTypeCounter];
                                    chestTypeCounter++;
                                    chest.ChestType = chestType;

                                    bool chestState = chestStates[chestCounter];
                                    chestCounter++;

                                    if (chestState == true)
                                        chest.ForceOpen();

                                    // Only reset fairy chests if you are locking the castle (i.e. not reloading a file but starting a new lineage).
                                    if (Game.PlayerStats.LockCastle == false)
                                    {
                                        FairyChestObj fairyChest = chest as FairyChestObj;
                                        if (fairyChest != null)
                                        {
                                            bool fairyChestState = fairyChestStates[fairyChestCounter];
                                            fairyChestCounter++;

                                            if (fairyChestState == true)
                                                fairyChest.SetChestFailed(true);
                                        }
                                    }
                                }
                                //}
                            }
                        }
                    }

                    if (numRooms > 0)
                    {
                        List<RoomObj> roomsVisitedList = new List<RoomObj>();
                        // Setting rooms visited states after the chests have been set.
                        //foreach (RoomObj room in createdLevel.RoomList)
                        //{
                        //    bool roomState = roomsVisited[roomCounter];
                        //    roomCounter++;

                        //    if (roomState == true)
                        //        roomsVisitedList.Add(room);
                        //}

                        int roomsVisitedCount = roomsVisited.Count;
                        for (int i = 0; i < roomsVisitedCount; i++)
                        {
                            if (roomsVisited[i] == true)
                                roomsVisitedList.Add(createdLevel.RoomList[i]);
                        }
                        
                        createdLevel.MapRoomsUnveiled = roomsVisitedList;
                    }

                    reader.Close();
                }
                stream.Close();
            }
        }

        private void LoadLineageData()
        {
            using (Stream stream = m_storageContainer.OpenFile("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Console.WriteLine("///// PLAYER LINEAGE DATA - BEGIN LOADING /////");

                    // Loading the currently created branch.
                    List<PlayerLineageData> loadedBranches = new List<PlayerLineageData>();
                    int numChildren = reader.ReadInt32();

                    for (int i = 0; i < numChildren; i++)
                    {
                        PlayerLineageData data = new PlayerLineageData();

                        data.Name = reader.ReadString();
                        data.Spell = reader.ReadByte();
                        data.Class = reader.ReadByte();
                        data.HeadPiece = reader.ReadByte();
                        data.ChestPiece = reader.ReadByte();
                        data.ShoulderPiece = reader.ReadByte();
                        data.Age = reader.ReadByte();
                        data.ChildAge = reader.ReadByte();
                        data.Traits = new Vector2(reader.ReadByte(), reader.ReadByte());
                        data.IsFemale = reader.ReadBoolean();

                        if (Game.PlayerStats.RevisionNumber > 0)
                            data.RomanNumeral = reader.ReadString();

                        loadedBranches.Add(data);
                    }

                    if (loadedBranches.Count > 0)
                    {
                        // Loading the CurrentBranches into Game.PlayerStats.
                        Game.PlayerStats.CurrentBranches = loadedBranches;

                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        {
                            Console.WriteLine("Loading Current Branch Lineage Data");
                            List<PlayerLineageData> currentBranches = Game.PlayerStats.CurrentBranches;
                            for (int i = 0; i < numChildren; i++)
                            {
                                Console.WriteLine("Player Name: " + currentBranches[i].Name);
                                Console.WriteLine("Spell: " + currentBranches[i].Name);
                                Console.WriteLine("Class: " + currentBranches[i].Name);
                                Console.WriteLine("Head Piece: " + currentBranches[i].HeadPiece);
                                Console.WriteLine("Chest Piece: " + currentBranches[i].ChestPiece);
                                Console.WriteLine("Shoulder Piece: " + currentBranches[i].ShoulderPiece);
                                Console.WriteLine("Player Age: " + currentBranches[i].Age);
                                Console.WriteLine("Player Child Age: " + currentBranches[i].ChildAge);
                                Console.WriteLine("Traits: " + currentBranches[i].Traits.X + ", " + currentBranches[i].Traits.Y);
                                Console.WriteLine("Is Female: " + currentBranches[i].IsFemale);
                                if (Game.PlayerStats.RevisionNumber > 0)
                                    Console.WriteLine("Roman Number:" + currentBranches[i].RomanNumeral);
                            }
                            currentBranches = null;
                        }
                    }

                    loadedBranches = null;

                    ////////////////////////////////////////

                    // Loading family tree info

                    List<FamilyTreeNode> familyTree = new List<FamilyTreeNode>();
                    int numBranches = reader.ReadInt32();

                    for (int i = 0; i < numBranches; i++)
                    {
                        FamilyTreeNode data = new FamilyTreeNode();
                        data.Name = reader.ReadString();
                        data.Age = reader.ReadByte();
                        data.Class = reader.ReadByte();
                        data.HeadPiece = reader.ReadByte();
                        data.ChestPiece = reader.ReadByte();
                        data.ShoulderPiece = reader.ReadByte();
                        data.NumEnemiesBeaten = reader.ReadInt32();
                        data.BeatenABoss = reader.ReadBoolean();
                        data.Traits.X = reader.ReadByte();
                        data.Traits.Y = reader.ReadByte();
                        data.IsFemale = reader.ReadBoolean();
                        if (Game.PlayerStats.RevisionNumber > 0)
                            data.RomanNumeral = reader.ReadString();
                        familyTree.Add(data);
                    }

                    if (familyTree.Count > 0)
                    {
                        // Loading the created Family Tree list into Game.PlayerStats.
                        Game.PlayerStats.FamilyTreeArray = familyTree;

                        if (LevelEV.SHOW_SAVELOAD_DEBUG_TEXT == true)
                        {
                            List<FamilyTreeNode> familyTreeArray = Game.PlayerStats.FamilyTreeArray;
                            Console.WriteLine("Loading Family Tree Data");
                            Console.WriteLine("Number of Branches: " + numBranches);
                            for (int i = 0; i < numBranches; i++)
                            {
                                Console.WriteLine("/// Saving branch");
                                Console.WriteLine("Name: " + familyTreeArray[i].Name);
                                Console.WriteLine("Age: " + familyTreeArray[i].Age);
                                Console.WriteLine("Class: " + familyTreeArray[i].Class);
                                Console.WriteLine("Head Piece: " + familyTreeArray[i].HeadPiece);
                                Console.WriteLine("Chest Piece: " + familyTreeArray[i].ChestPiece);
                                Console.WriteLine("Shoulder Piece: " + familyTreeArray[i].ShoulderPiece);
                                Console.WriteLine("Number of Enemies Beaten: " + familyTreeArray[i].NumEnemiesBeaten);
                                Console.WriteLine("Beaten a Boss: " + familyTreeArray[i].BeatenABoss);
                                Console.WriteLine("Traits: " + familyTreeArray[i].Traits.X + ", " + familyTreeArray[i].Traits.Y);
                                Console.WriteLine("Is Female: " + familyTreeArray[i].IsFemale);
                                if (Game.PlayerStats.RevisionNumber > 0)
                                    Console.WriteLine("Roman Numeral: " + familyTreeArray[i].RomanNumeral);
                            }
                            familyTreeArray = null;
                        }
                    }

                    familyTree = null;

                    ///////////////////////////////////////////
                    Console.WriteLine("///// PLAYER LINEAGE DATA - LOAD COMPLETE /////");

                    reader.Close();
                }
                stream.Close();
            }
        }

        public bool FileExists(SaveType saveType)
        {
            bool disposeStorage = true;
            if (m_storageContainer != null && m_storageContainer.IsDisposed == false)
                disposeStorage = false; // Don't dispose the storage container because FileExists() was called from LoadData().

            GetStorageContainer();

            bool fileExists = false;
            switch (saveType)
            {
                case (SaveType.PlayerData):
                    fileExists = m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNamePlayer);
                    break;
                case (SaveType.UpgradeData):
                    fileExists = m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameUpgrades);
                    break;
                case (SaveType.Map):
                    fileExists = m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMap);
                    break;
                case (SaveType.MapData):
                    fileExists = m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameMapData);
                    break;
                case (SaveType.Lineage):
                    fileExists = m_storageContainer.FileExists("Profile" + Game.GameConfig.ProfileSlot + "/" + m_fileNameLineage);
                    break;
            }

            if (disposeStorage == true)
            {
                m_storageContainer.Dispose();
                m_storageContainer = null;
            }

            return fileExists;
        }

        public StorageContainer GetContainer()
        {
            return m_storageContainer;
        }

        public void GetSaveHeader(byte profile, out byte playerClass, out string playerName, out int playerLevel, out bool playerIsDead, out int castlesBeaten, out bool isFemale)
        {
            playerName = null;
            playerClass = 0;
            playerLevel = 0;
            playerIsDead = false;
            castlesBeaten = 0;
            isFemale = false;

            GetStorageContainer();

            if (m_storageContainer.FileExists("Profile" + profile + "/" + m_fileNamePlayer))
            {
                using (Stream stream = m_storageContainer.OpenFile("Profile" + profile + "/" + m_fileNamePlayer, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        reader.ReadInt32(); // Gold
                        reader.ReadInt32(); // Health
                        reader.ReadInt32(); // Mana
                        reader.ReadByte(); // Age
                        reader.ReadByte(); // Child Age
                        reader.ReadByte(); // Spell
                        playerClass = reader.ReadByte();
                        reader.ReadByte(); // Special Item
                        reader.ReadByte(); // TraitX
                        reader.ReadByte(); // TraitY
                        playerName = reader.ReadString();

                        reader.ReadByte(); // Head Piece
                        reader.ReadByte(); // Shoulder Piece
                        reader.ReadByte(); // Chest Piece
                        reader.ReadByte(); // Diary Entry
                        reader.ReadInt32(); // Bonus Health
                        reader.ReadInt32(); // Bonus Strength
                        reader.ReadInt32(); // Bonus Mana
                        reader.ReadInt32(); // Bonus Defense
                        reader.ReadInt32(); // Bonus Weight
                        reader.ReadInt32(); // Bonus Magic

                        // Reading lich stats.
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadSingle();

                        // Reading boss progress states
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();

                        // Reading new game plus progress
                        castlesBeaten = reader.ReadInt32();
                        reader.ReadInt32();

                        // Loading misc flags
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();

                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        playerIsDead = reader.ReadBoolean();
                        reader.ReadBoolean();
                        reader.ReadBoolean();
                        isFemale = reader.ReadBoolean();

                        reader.Close();
                    }
                    stream.Close();
                }
            }

            if (m_storageContainer.FileExists("Profile" + profile + "/" + m_fileNameUpgrades))
            {
                using (Stream stream = m_storageContainer.OpenFile("Profile" + profile + "/" + m_fileNameUpgrades, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        for (int i = 0; i < EquipmentCategoryType.Total; i++)
                        {
                            for (int k = 0; k < EquipmentBaseType.Total; k++)
                                reader.ReadByte();
                        }

                        for (int i = 0; i < EquipmentCategoryType.Total; i++)
                        {
                            for (int k = 0; k < EquipmentAbilityType.Total; k++)
                                reader.ReadByte();
                        }

                        for (int i = 0; i < EquipmentCategoryType.Total; i++)
                            reader.ReadSByte();

                        for (int i = 0; i < EquipmentCategoryType.Total; i++)
                            reader.ReadSByte();

                        int levelCounter = 0;
                        for (int i = 0; i < (int)SkillType.DIVIDER - 2; i++)  //The starting 2 traits are null and filler.
                        {
                            int traitLevel = reader.ReadInt32();
                            for (int k = 0; k < traitLevel; k++)
                                levelCounter++;
                        }

                        playerLevel = levelCounter;
                        reader.Close();
                    }
                    stream.Close();
                }
            }

            m_storageContainer.Dispose();
            m_storageContainer = null;
        }
    }

    public enum SavingState
    {
        NotSaving,
        ReadyToSelectStorageDevice,
        SelectingStorageDevice,

        ReadyToOpenStorageContainer,    // once we have a storage device start here
        OpeningStorageContainer,
        ReadyToSave,
    }

    public enum SaveType
    {
        None,
        PlayerData,
        UpgradeData,
        Map,
        MapData,
        Lineage,
    }
}

