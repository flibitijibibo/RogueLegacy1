using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class SkillSystem
    {
        #region MANOR PIECE CONSTS
        private const int Trait_ObservatoryTelescope = 0;
        private const int Trait_ObservatoryBase = 1;
        private const int Trait_RightHighTower = 2;
        private const int Trait_RightHighUpper = 3;
        private const int Trait_RightHighBase = 4;
        private const int Trait_RightExtension = 5;
        private const int Trait_RightBigRoof = 6;
        private const int Trait_RightBigUpper = 7;
        private const int Trait_RightWingRoof = 8;
        private const int Trait_RightBigBase = 9;
        private const int Trait_RightWingBase = 10;
        private const int Trait_RightWingWindow = 11;
        private const int Trait_LeftExtensionBase = 12;
        private const int Trait_LeftFarRoof = 13;
        private const int Trait_LeftFarBase = 14;
        private const int Trait_LeftBigRoof = 15;
        private const int Trait_LeftBigUpper2 = 16;
        private const int Trait_LeftBigWindows = 17;
        private const int Trait_LeftBigUpper = 18;
        private const int Trait_LeftBigBase = 19;
        private const int Trait_LeftWingRoof = 20;
        private const int Trait_LeftWingBase = 21;
        private const int Trait_LeftWingWindow = 22;
        private const int Trait_GroundRoad = 23;
        private const int Trait_MainRoof = 24;
        private const int Trait_MainBase = 25;
        private const int Trait_FrontWindowTop = 26;
        private const int Trait_FrontWindowBottom = 27;
        private const int Trait_LeftTree1 = 28;
        private const int Trait_LeftTree2 = 29;
        private const int Trait_RightTree1 = 30;
        private const int TOTAL = 31;
        #endregion

        private static SkillType StartingTrait = SkillType.Smithy; //SkillType.Health_Up;//SkillType.Smithy;
        private static SkillObj m_blankTrait; // A blank trait used for dead traits that need to be removed later on.

        //Remember: m_skillTypeArray is compiled as follows int[rows,cols].
        private static SkillType[,] m_skillTypeArray =
        {
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Randomize_Children,	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.SuperSecret,	SkillType.Mana_Cost_Down,	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Invuln_Time_Up,	SkillType.SpellSword_Up,	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Down_Strike_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Spellsword_Unlock,	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Ninja_Up,	SkillType.Armor_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Potion_Up,	SkillType.Null, 	},
            {	SkillType.Lich_Up,	SkillType.Lich_Unlock,	SkillType.Prices_Down,	SkillType.Null, 	SkillType.Attack_Up,	SkillType.Null, 	SkillType.Magic_Damage_Up,	SkillType.Null, 	SkillType.Assassin_Up,	SkillType.Null, 	},
            {	SkillType.Death_Dodge,	SkillType.Null, 	SkillType.Ninja_Unlock,	SkillType.Barbarian_Up,	SkillType.Architect,	SkillType.Equip_Up,	SkillType.Enchanter,	SkillType.Mage_Up,	SkillType.Banker_Unlock,	SkillType.Banker_Up,	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Crit_Chance_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Knight_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Gold_Gain_Up,	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Crit_Damage_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Health_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	},
            {	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	SkillType.Smithy,	SkillType.Mana_Up,	SkillType.Null, 	SkillType.Null, 	SkillType.Null, 	},
        };

        private static Vector2[,] m_skillPositionArray = 
        {
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(860, 125),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (655,-100),	new Vector2(735, 95),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(655, 50),	new Vector2(655, 125),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(365, 150),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(655, 200),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(185, 250),	new Vector2(365, 250),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(735, 200),	new Vector2 (0,0),	},
            {	new Vector2(110, 360),	new Vector2(110, 460),	new Vector2(185, 360),	new Vector2 (0,0),	new Vector2(275, 555),	new Vector2 (0,0),	new Vector2(735, 555),	new Vector2 (0,0),	new Vector2(735, 280),	new Vector2 (0,0),	},
            {	new Vector2(40, 410),	new Vector2 (0,0),	new Vector2(185, 555),	new Vector2(275, 360),	new Vector2(275, 460),	new Vector2(505, 315),	new Vector2(735, 460),	new Vector2(735, 360),	new Vector2(860, 460),	new Vector2(938, 415),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(185, 680),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(505, 410),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(860, 680),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(275, 680),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(505, 490),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	},
            {	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2(505, 590),	new Vector2(505, 680),	new Vector2 (0,0),	new Vector2 (0,0),	new Vector2 (0,0),	}
        };

        private static int[,] m_manorPieceArray =
        {
            {	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	0,	    -1, 	},
            {	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	2,   	1,	    -1, 	},
            {	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	-1, 	2,	    3,	    -1, 	},
            {	-1, 	-1, 	15,	    -1, 	-1, 	-1, 	-1, 	-1, 	4,	    -1, 	},
            {	-1, 	-1, 	16,	    17,	    -1, 	-1, 	-1, 	-1, 	6,	    -1, 	},
            {	13,	    14,	    18,	    -1, 	22, 	-1, 	11, 	-1, 	7,	    -1, 	},
            {	12,	    -1, 	19,	    20,	    21, 	25, 	10, 	9,	    8,	    5,	    },
            {	-1, 	-1, 	29,	    -1, 	-1, 	27,	    -1, 	-1, 	31, 	-1, 	},
            {	-1, 	-1, 	30,	    -1, 	-1, 	28,	    -1, 	-1, 	-1, 	-1, 	},
            {	-1, 	-1, 	-1, 	-1, 	-1, 	26,	    24,	    -1, 	-1, 	-1, 	}

       };

        private static SkillLinker[,] m_skillLinkerArray;

        private static List<SkillObj> m_skillArray;

        private static bool m_iconsVisible = true;

        public static void Initialize()
        {
            m_blankTrait = new SkillObj("Icon_Sword_Sprite");

            if (m_skillTypeArray.Length != m_skillPositionArray.Length)
                throw new Exception("Cannot create Trait System. The type array is not the same length as the position array.");

            // Loading all traits into the trait array.
            m_skillArray = new List<SkillObj>();

            for (int i = 2; i < (int)SkillType.DIVIDER; i++) // The starting 2 traits are null and filler.
            {
                SkillObj skill = SkillBuilder.BuildSkill((SkillType)i);
                skill.Position = GetSkillPosition(skill);
                m_skillArray.Add(skill);
                //Console.WriteLine(trait.Position);
            }

            GetSkill(StartingTrait).Visible = true; // Set the starting trait to visible.

            m_skillLinkerArray = new SkillLinker[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    m_skillLinkerArray[i, k] = SkillBuilder.GetSkillLinker(i, k);
                }
            }
        }

        public static void LevelUpTrait(SkillObj trait, bool giveGoldBonus)
        {
            Game.PlayerStats.CurrentLevel++;
            trait.CurrentLevel++;
            UpdateTraitSprite(trait);

            if (trait.TraitType == SkillType.Gold_Flat_Bonus && giveGoldBonus == true)
                Game.PlayerStats.Gold += (int)trait.ModifierAmount;

            bool unlockAchievement = true;
            foreach (SkillObj skill in SkillArray)
            {
                if (skill.CurrentLevel < 1)
                {
                    unlockAchievement = false;
                    break;
                }
            }

            if (unlockAchievement == true)
                GameUtil.UnlockAchievement("FEAR_OF_DECISIONS");

            if (Game.PlayerStats.CurrentLevel >= 50)
                GameUtil.UnlockAchievement("FEAR_OF_WEALTH");
        }

        public static void ResetAllTraits()
        {
            foreach (SkillObj skill in m_skillArray)
            {
                skill.CurrentLevel = 0;
                skill.Visible = false;
            }
            GetSkill(StartingTrait).Visible = true; // Set the starting trait to visible.
            Game.PlayerStats.CurrentLevel = 0;
        }

        public static void UpdateAllTraitSprites()
        {
            foreach (SkillObj trait in m_skillArray)
            {
                UpdateTraitSprite(trait);
            }
        }

        public static void UpdateTraitSprite(SkillObj trait)
        {
            //if (trait.Visible == true)
            {
                string traitName = trait.IconName;
                if (trait.CurrentLevel > 0 && trait.CurrentLevel < trait.MaxLevel)
                    traitName = traitName.Replace("Locked", "");
                else if (trait.CurrentLevel > 0 && trait.CurrentLevel >= trait.MaxLevel)
                    traitName = traitName.Replace("Locked", "Max");

                trait.ChangeSprite(traitName);
            }
        }

        public static List<SkillObj> GetAllConnectingTraits(SkillObj trait)
        {
            int cols = GetTypeArrayColumns();
            int rows = GetTypeArrayRows();

            Vector2 traitIndex = GetTraitTypeIndex(trait);
            SkillObj[] traitArray = new SkillObj[4]; // 0 = right, 1 = left, 2 = top, 3 = bottom.

            if (traitIndex.X + 1 < cols) // Right Trait
                traitArray[0] = GetSkill((int)traitIndex.X + 1, (int)traitIndex.Y);
            if (traitIndex.X - 1 >= 0) // Left Trait
                traitArray[1] = GetSkill((int)traitIndex.X - 1, (int)traitIndex.Y);
            if (traitIndex.Y - 1 >= 0) // Top Trait
                traitArray[2] = GetSkill((int)traitIndex.X, (int)traitIndex.Y - 1);
            if (traitIndex.Y + 1 < rows) // Bottom Trait
                traitArray[3] = GetSkill((int)traitIndex.X, (int)traitIndex.Y + 1);

            List<SkillObj> traitList = new List<SkillObj>();
            foreach (SkillObj traitObj in traitArray)
            {
                if (traitObj != null) // && traitObj.traitType != TraitType.Filler)
                    traitList.Add(traitObj);
            }

            return traitList;
        }

        public static SkillObj GetSkill(SkillType skillType)
        {
            foreach (SkillObj traitToReturn in m_skillArray)
            {
                if (traitToReturn.TraitType == skillType)
                    return traitToReturn;
            }
            return m_blankTrait;
        }

        public static SkillObj GetSkill(int indexX, int indexY)
        {
            return GetSkill(m_skillTypeArray[indexY, indexX]);
        }

        // Returns the array index of a trait in the trait type array.
        public static Vector2 GetTraitTypeIndex(SkillObj trait)
        {
            Vector2 traitIndex = new Vector2(-1, -1);
            SkillType traitType = trait.TraitType;
            for (int i = 0; i < m_skillTypeArray.GetLength(1); i++)
            {
                for (int k = 0; k < m_skillTypeArray.GetLength(0); k++)
                {
                    if (m_skillTypeArray[k, i] == traitType)
                        traitIndex = new Vector2(i, k);
                }
            }

            return traitIndex;
        }

        // Returns the position of a trait in the trait position array.
        public static Vector2 GetSkillPosition(SkillObj skill)
        {
            Vector2 skillTypeIndex = GetTraitTypeIndex(skill);
            return m_skillPositionArray[(int)skillTypeIndex.Y, (int)skillTypeIndex.X];
        }

        public static int GetTypeArrayRows()
        {
            return m_skillTypeArray.GetLength(0);
        }

        public static int GetTypeArrayColumns()
        {
            return m_skillTypeArray.GetLength(1);
        }


        public static SkillObj[] GetSkillArray()
        {
            return m_skillArray.ToArray();
        }

        public static List<SkillObj> SkillArray
        {
            get { return m_skillArray; }
        }

        public static int GetManorPiece(SkillObj trait)
        {
            Vector2 traitPosition = GetTraitTypeIndex(trait);
            return m_manorPieceArray[(int)traitPosition.Y, (int)traitPosition.X];
        }

        public static SkillLinker GetSkillLink(int x, int y)
        {
            return m_skillLinkerArray[x, y];
        }

        public static void HideAllIcons()
        {
            foreach (SkillObj skill in m_skillArray)
                skill.Opacity = 0;

            m_iconsVisible = false;
        }

        public static void ShowAllIcons()
        {
            foreach (SkillObj skill in m_skillArray)
                skill.Opacity = 1;

            m_iconsVisible = true;
        }

        public static bool IconsVisible
        {
            get { return m_iconsVisible; }
        }

        public static void RefreshTextObjs()
        {
            for (int i = 2; i < (int)SkillType.DIVIDER; i++) // The starting 2 traits are null and filler.
            {
                SkillObj skill = m_skillArray[i - 2];
                SkillBuilder.BuildSkill((SkillType)i, skill);
            }
        }
    }

    public struct SkillLinker
    {
        public Vector2 TopLink;
        public Vector2 BottomLink;
        public Vector2 LeftLink;
        public Vector2 RightLink;
    }
}