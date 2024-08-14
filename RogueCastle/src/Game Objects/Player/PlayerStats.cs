using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class PlayerStats : IDisposableObj
    {
        public int CurrentLevel { get; set; }
        private int m_gold = 0;
        public int Gold
        {
            get { return m_gold; }
            set
            {
                m_gold = value;
                if (m_gold < 0) m_gold = 0;
            }
        }
        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }
        public byte Age { get; set; }
        public byte ChildAge { get; set; }
        public byte Spell { get; set; }
        public byte Class { get; set; }
        public byte SpecialItem { get; set; }
        public Vector2 Traits { get; set; }
        public string PlayerName { get; set; }

        private string m_romanNumeral = "";
        public string RomanNumeral
        {
            get { return m_romanNumeral; }
            set { m_romanNumeral = value; }
        }

        public byte HeadPiece { get; set; }
        public byte ShoulderPiece { get; set; }
        public byte ChestPiece { get; set; }

        public byte DiaryEntry { get; set; }

        public int BonusHealth { get; set; }
        public int BonusStrength { get; set; }
        public int BonusMana { get; set; }
        public int BonusDefense { get; set; }
        public int BonusWeight { get; set; }
        public int BonusMagic { get; set; }

        public int LichHealth { get; set; }
        public int LichMana { get; set; }
        public float LichHealthMod { get; set; }

        public bool NewBossBeaten { get; set; } // A flag to keep track of this particular hero beat a boss.
        public bool EyeballBossBeaten { get; set; }
        public bool FairyBossBeaten { get; set; }
        public bool FireballBossBeaten { get; set; }
        public bool BlobBossBeaten { get; set; }
        public bool LastbossBeaten { get; set; }

        public int TimesCastleBeaten { get; set; } // Times you've beaten the game.
        public int NumEnemiesBeaten { get; set; } // Number of enemies you've beaten in a single run.

        public bool TutorialComplete { get; set; }
        public bool CharacterFound { get; set; }
        public bool LoadStartingRoom { get; set; }

        public bool LockCastle { get; set; }
        public bool SpokeToBlacksmith { get; set; }
        public bool SpokeToEnchantress{ get; set; }
        public bool SpokeToArchitect { get; set; }
        public bool SpokeToTollCollector { get; set; }
        public bool IsDead { get; set; }
        public bool FinalDoorOpened { get; set; }
        public bool RerolledChildren { get; set; }

        public byte ForceLanguageGender = 0;
        public bool GodMode = false;
        private bool m_isFemale = false;
        public bool IsFemale
        {
            get
            {
                if (ForceLanguageGender == 0)
                    return m_isFemale;
                else if (ForceLanguageGender == 1)
                    return false;
                else
                    return true;
            }
            set { m_isFemale = value; }
        }

        public int TimesDead { get; set; }
        public bool HasArchitectFee { get; set; }
        public bool ReadLastDiary { get; set; }
        public bool SpokenToLastBoss { get; set; }
        public bool HardcoreMode { get; set; }
        public float TotalHoursPlayed { get; set; }

        /// Adding Prosopagnosia trait.
        public bool HasProsopagnosia { get; set; }

        // Adding Save file revision numbers.
        public int RevisionNumber { get; set; }

        public Vector3 WizardSpellList { get; set; }

        // These arrays hold the blueprint unlock state of each item.
        private List<byte[]> m_blueprintArray;
        private List<byte[]> m_runeArray;

        // These arrays hold which item you currently have equipped. -1 means no item.
        private sbyte[] m_equippedArray;
        private sbyte[] m_equippedRuneArray;

        public List<PlayerLineageData> CurrentBranches;
        public List<FamilyTreeNode> FamilyTreeArray;

        public List<Vector4> EnemiesKilledList; // The total list of enemies killed.
        public List<Vector2> EnemiesKilledInRun; // The enemies killed in a single run.

        // Adding challenge icons.
        public bool ChallengeEyeballUnlocked { get; set; }
        public bool ChallengeFireballUnlocked { get; set; }
        public bool ChallengeBlobUnlocked { get; set; }
        public bool ChallengeSkullUnlocked { get; set; }
        public bool ChallengeLastBossUnlocked { get; set; }

        public bool ChallengeEyeballBeaten { get; set; }
        public bool ChallengeFireballBeaten { get; set; }
        public bool ChallengeBlobBeaten { get; set; }
        public bool ChallengeSkullBeaten { get; set; }
        public bool ChallengeLastBossBeaten { get; set; }

        public sbyte ChallengeEyeballTimesUpgraded { get; set; }
        public sbyte ChallengeFireballTimesUpgraded { get; set; }
        public sbyte ChallengeBlobTimesUpgraded { get; set; }
        public sbyte ChallengeSkullTimesUpgraded { get; set; }
        public sbyte ChallengeLastBossTimesUpgraded { get; set; }

        public bool ArchitectUsed { get; set; }
        /// 

        public PlayerStats()
        {
            if (LevelEV.RUN_TUTORIAL == false && this.TutorialComplete == false && LevelEV.RUN_TESTROOM == true) // This is debug that needs to be removed once tutorial is properly implemented.
                this.TutorialComplete = true;

            PlayerName = "Lee";//"Sir Johannes";
            SpecialItem = SpecialItemType.None;
            Class = ClassType.Knight;
            Spell = SpellType.Dagger;
            Age = 30;
            ChildAge = 5;
            LichHealthMod = 1; // Default is 1
            IsFemale = false;
            TimesCastleBeaten = 0;
            RomanNumeral = "";
            //LastbossBeaten = true;

            EnemiesKilledList = new List<Vector4>();
            for (int i = 0; i < EnemyType.Total; i++)
                EnemiesKilledList.Add(new Vector4());

            WizardSpellList = new Vector3(SpellType.Dagger, SpellType.Axe, SpellType.Boomerang);

            Traits = new Vector2(TraitType.None, TraitType.None);
            Gold = 0;
            CurrentLevel = 0;

            HeadPiece = 1;
            ShoulderPiece = 1;
            ChestPiece = 1;
            LoadStartingRoom = true; // Default is true.

            m_blueprintArray = new List<byte[]>();
            m_runeArray = new List<byte[]>();
            m_equippedArray = new sbyte[EquipmentCategoryType.Total];
            m_equippedRuneArray = new sbyte[EquipmentCategoryType.Total];

            FamilyTreeArray = new List<FamilyTreeNode>();
            InitializeFirstChild();
            EnemiesKilledInRun = new List<Vector2>();
            CurrentBranches = null;

            for (int i = 0; i < EquipmentCategoryType.Total; i++)
            {
                m_blueprintArray.Add(new byte[EquipmentBaseType.Total]);
                m_runeArray.Add(new byte[EquipmentAbilityType.Total]);
                m_equippedArray[i] = -1;
                m_equippedRuneArray[i] = -1;
            }

            //TEDDY - KENNY GAVE TO RANDOMIZE THE PLAYERS EQUIPMENT AND STUFF
            
            HeadPiece = (byte)CDGMath.RandomInt(1, 5);
            ShoulderPiece = (byte)CDGMath.RandomInt(1, 5);
            ChestPiece = (byte)CDGMath.RandomInt(1, 5);
            sbyte rand = (sbyte)CDGMath.RandomInt(0, 14);
            /*
            m_equippedArray[EquipmentCategoryType.Chest] = (sbyte)CDGMath.RandomInt(0, 14);
            m_equippedArray[EquipmentCategoryType.Helm] = (sbyte)CDGMath.RandomInt(0, 14);
            m_equippedArray[EquipmentCategoryType.Limbs] = (sbyte)CDGMath.RandomInt(0, 14);
            m_equippedArray[EquipmentCategoryType.Cape] = (sbyte)CDGMath.RandomInt(0, 14);
            m_equippedArray[EquipmentCategoryType.Sword] = rand;
            */
            
            // Debug stuff to test items.
            m_blueprintArray[EquipmentCategoryType.Helm][EquipmentBaseType.Bronze] = EquipmentState.FoundButNotSeen;
            m_blueprintArray[EquipmentCategoryType.Limbs][EquipmentBaseType.Bronze] = EquipmentState.FoundButNotSeen;
            m_blueprintArray[EquipmentCategoryType.Sword][EquipmentBaseType.Bronze] = EquipmentState.FoundButNotSeen;

            m_runeArray[EquipmentCategoryType.Helm][EquipmentAbilityType.DoubleJump] = EquipmentState.FoundButNotSeen;
            m_runeArray[EquipmentCategoryType.Sword][EquipmentAbilityType.Dash] = EquipmentState.FoundButNotSeen;
            //m_blueprintArray[0][0] = EquipmentState.Purchased;
            //m_blueprintArray[0][1] = EquipmentState.NotPurchased;
            //m_blueprintArray[0][2] = EquipmentState.NotPurchased;
            //m_blueprintArray[0][3] = EquipmentState.NotPurchased;
            //m_blueprintArray[0][4] = EquipmentState.NotPurchased;
            //m_blueprintArray[EquipmentCategoryType.Helm][4] = EquipmentState.NotPurchased;
            //m_blueprintArray[EquipmentCategoryType.Cape][0] = EquipmentState.Purchased;
            //m_blueprintArray[EquipmentCategoryType.Cape][7] = EquipmentState.Purchased;
            //m_blueprintArray[EquipmentCategoryType.Cape][14] = EquipmentState.NotPurchased;
            //m_blueprintArray[EquipmentCategoryType.Cape][5] = EquipmentState.Purchased;

            //m_runeArray[0][0] = EquipmentState.Purchased;
            //m_runeArray[0][1] = EquipmentState.NotPurchased;
            //m_equippedRuneArray[0] = 0;
            //m_equippedRuneArray[0] = EquipmentAbilityType.Flight;

            //EyeballBossBeaten = true;
            //ChallengeEyeballBeaten = true;
            //ChallengeEyeballUnlocked = true;

            //FairyBossBeaten = true;
            //ChallengeSkullBeaten = false;
            //ChallengeSkullUnlocked = true;

            //FireballBossBeaten = true;
            //ChallengeFireballBeaten = false;
            //ChallengeFireballUnlocked = true;

            //BlobBossBeaten = true;
            //ChallengeBlobBeaten = false;
            //ChallengeBlobUnlocked = true;

            //ChallengeLastBossUnlocked = true;
            //ChallengeLastBossBeaten = true;

            //HeadPiece = 7;
        }

        private void InitializeFirstChild()
        {
            FamilyTreeNode firstNode = new FamilyTreeNode()
            {
                Name = "Johannes",
                Age = 30,
                ChildAge = 20,
                Class = ClassType.Knight,
                HeadPiece = 8,
                ChestPiece = 1,
                ShoulderPiece = 1,
                NumEnemiesBeaten = 0,
                BeatenABoss = true,
                IsFemale = false,
                Traits = Vector2.Zero,
            };
            FamilyTreeArray.Add(firstNode);
        }


        private bool m_isDisposed = false;
        public void Dispose()
        {
            if (IsDisposed == false)
            {
                m_blueprintArray.Clear();
                m_blueprintArray = null;
                m_runeArray.Clear();
                m_runeArray = null;
                m_isDisposed = true;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        public List<byte[]> GetBlueprintArray
        {
            get { return m_blueprintArray; }
        }
        
        public sbyte[] GetEquippedArray
        {
            get { return m_equippedArray; }
        }

        public byte TotalBlueprintsPurchased
        {
            get
            {
                byte total = 0;
                foreach (byte[] categoryType in GetBlueprintArray)
                {
                    foreach (byte purchaseState in categoryType)
                    {
                        if (purchaseState >= EquipmentState.Purchased)
                            total++;
                    }
                }
                return total;
            }
        }

        public byte TotalRunesPurchased
        {
            get
            {
                byte total = 0;
                foreach (byte[] categoryType in GetRuneArray)
                {
                    foreach (byte purchaseState in categoryType)
                    {
                        if (purchaseState >= EquipmentState.Purchased)
                            total++;
                    }
                }
                return total;
            }
        }

        public byte TotalBlueprintsFound
        {
            get
            {
                byte total = 0;
                foreach (byte[] categoryType in GetBlueprintArray)
                {
                    foreach (byte purchaseState in categoryType)
                    {
                        if (purchaseState >= EquipmentState.FoundButNotSeen)
                            total++;
                    }
                }
                return total;
            }
        }

        public byte TotalRunesFound
        {
            get
            {
                byte total = 0;
                foreach (byte[] categoryType in GetRuneArray)
                {
                    foreach (byte purchaseState in categoryType)
                    {
                        if (purchaseState >= EquipmentState.FoundButNotSeen)
                            total++;
                    }
                }
                return total;
            }
        }

        public List<byte[]> GetRuneArray
        {
            get { return m_runeArray; }
        }

        public sbyte[] GetEquippedRuneArray
        {
            get { return m_equippedRuneArray; }
        }

        public byte GetNumberOfEquippedRunes(int equipmentAbilityType)
        {
            byte numEquippedAbilities = 0;
            if (LevelEV.UNLOCK_ALL_ABILITIES == true)
                return EquipmentCategoryType.Total;
            foreach (sbyte equippedAbility in m_equippedRuneArray)
            {
                if (equippedAbility == equipmentAbilityType)
                    numEquippedAbilities++;
            }
            return numEquippedAbilities;
        }
    }
}
