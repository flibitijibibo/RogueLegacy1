using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using DS2DEngine;

namespace RogueCastle
{
    public class DialogueManager : IDisposable
    {
        private static bool m_isDisposed = false;

        private static Dictionary<string, Dictionary<string, DialogueObj>> m_languageArray;
        private static Dictionary<string, DialogueObj> m_dialogDict;
        private static string m_currentLanguage;

        public static void Initialize()
        {
            m_languageArray = new Dictionary<string, Dictionary<string, DialogueObj>>();
            m_dialogDict = new Dictionary<string, DialogueObj>();

            // Text entries
            m_dialogDict.Add("Meet Blacksmith",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_1", "LOC_ID_TEXT_1", "LOC_ID_TEXT_1", "LOC_ID_TEXT_1", "LOC_ID_TEXT_1", },
                    new string[] { "LOC_ID_TEXT_2", "LOC_ID_TEXT_3", "LOC_ID_TEXT_4", "LOC_ID_TEXT_5", "LOC_ID_TEXT_6", }
                ));
            m_dialogDict.Add("Meet Enchantress",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_7", "LOC_ID_TEXT_7", "LOC_ID_TEXT_7", "LOC_ID_TEXT_7", "LOC_ID_TEXT_7", },
                    new string[] { "LOC_ID_TEXT_8", "LOC_ID_TEXT_9", "LOC_ID_TEXT_10", "LOC_ID_TEXT_11", "LOC_ID_TEXT_12", }
                ));
            m_dialogDict.Add("Meet Architect",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_13", "LOC_ID_TEXT_21", },
                    new string[] { "LOC_ID_TEXT_14", "LOC_ID_TEXT_15", "LOC_ID_TEXT_16", "LOC_ID_TEXT_17", "LOC_ID_TEXT_18", "LOC_ID_TEXT_19", "LOC_ID_TEXT_20", "LOC_ID_TEXT_22", }
                ));
            m_dialogDict.Add("Meet Architect 2",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_23", },
                    new string[] { "LOC_ID_TEXT_24", }
                ));
            m_dialogDict.Add("No Castle Architect",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_25", },
                    new string[] { "LOC_ID_TEXT_26", }
                ));
            m_dialogDict.Add("Castle Already Locked Architect",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_27", },
                    new string[] { "LOC_ID_TEXT_28", }
                ));
            m_dialogDict.Add("Castle Lock Complete Architect",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_29", },
                    new string[] { "LOC_ID_TEXT_30", }
                ));
            m_dialogDict.Add("Meet Toll Collector 1",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_31", "LOC_ID_TEXT_33", },
                    new string[] { "LOC_ID_TEXT_32", "LOC_ID_TEXT_34", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip0",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_35", },
                    new string[] { "LOC_ID_TEXT_36", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip10",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_37", },
                    new string[] { "LOC_ID_TEXT_38", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip20",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_39", },
                    new string[] { "LOC_ID_TEXT_40", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip30",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_41", },
                    new string[] { "LOC_ID_TEXT_42", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip40",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_43", },
                    new string[] { "LOC_ID_TEXT_44", }
                ));
            m_dialogDict.Add("Meet Toll Collector Skip50",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_45", },
                    new string[] { "LOC_ID_TEXT_46", }
                ));
            m_dialogDict.Add("Toll Collector Obol",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_47", },
                    new string[] { "LOC_ID_TEXT_48", }
                ));
            m_dialogDict.Add("Challenge Icon Eyeball",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_49", "LOC_ID_TEXT_49", "LOC_ID_TEXT_49", },
                    new string[] { "LOC_ID_TEXT_50", "LOC_ID_TEXT_51", "LOC_ID_TEXT_52", }
                ));
            m_dialogDict.Add("Challenge Icon Skull",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_53", "LOC_ID_TEXT_53", "LOC_ID_TEXT_53", },
                    new string[] { "LOC_ID_TEXT_54", "LOC_ID_TEXT_55", "LOC_ID_TEXT_56", }
                ));
            m_dialogDict.Add("Challenge Icon Fireball",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_57", "LOC_ID_TEXT_57", "LOC_ID_TEXT_57", },
                    new string[] { "LOC_ID_TEXT_58", "LOC_ID_TEXT_59", "LOC_ID_TEXT_60", }
                ));
            m_dialogDict.Add("Challenge Icon Blob",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_61", "LOC_ID_TEXT_61", "LOC_ID_TEXT_61", },
                    new string[] { "LOC_ID_TEXT_62", "LOC_ID_TEXT_63", "LOC_ID_TEXT_64", }
                ));
            m_dialogDict.Add("Challenge Icon Last Boss",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_65", "LOC_ID_TEXT_65", "LOC_ID_TEXT_65", },
                    new string[] { "LOC_ID_TEXT_66", "LOC_ID_TEXT_67", "LOC_ID_TEXT_68", }
                ));
            m_dialogDict.Add("Special Item Prayer",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_69", },
                    new string[] { "LOC_ID_TEXT_70", }
                ));
            m_dialogDict.Add("Meet Last Boss",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_71", },
                    new string[] { "LOC_ID_TEXT_72", }
                ));
            m_dialogDict.Add("Chest_Locked 0",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_73", },
                    new string[] { "LOC_ID_TEXT_74", }
                ));
            m_dialogDict.Add("Chest_Locked 1",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_75", },
                    new string[] { "LOC_ID_TEXT_76", }
                ));
            m_dialogDict.Add("Chest_Locked 2",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_77", },
                    new string[] { "LOC_ID_TEXT_78", }
                ));
            m_dialogDict.Add("Chest_Locked 3",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_79", },
                    new string[] { "LOC_ID_TEXT_80", }
                ));
            m_dialogDict.Add("Chest_Locked 4",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_81", },
                    new string[] { "LOC_ID_TEXT_82", }
                ));
            m_dialogDict.Add("Chest_Locked 5",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_83", },
                    new string[] { "LOC_ID_TEXT_84", }
                ));
            m_dialogDict.Add("Chest_Locked 6",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_85", },
                    new string[] { "LOC_ID_TEXT_86", }
                ));
            m_dialogDict.Add("Chest_Locked 7",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_87", },
                    new string[] { "LOC_ID_TEXT_88", }
                ));
            m_dialogDict.Add("Chest_Locked 8",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_89", },
                    new string[] { "LOC_ID_TEXT_90", }
                ));
            m_dialogDict.Add("Chest_Locked 9",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_91", },
                    new string[] { "LOC_ID_TEXT_92", }
                ));
            m_dialogDict.Add("Chest_Locked 10",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_93", },
                    new string[] { "LOC_ID_TEXT_94", }
                ));
            m_dialogDict.Add("Chest_Failed 3",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_95", },
                    new string[] { "LOC_ID_TEXT_96", }
                ));
            m_dialogDict.Add("Chest_Failed 4",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_97", },
                    new string[] { "LOC_ID_TEXT_98", }
                ));
            m_dialogDict.Add("Chest_Failed 5",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_99", },
                    new string[] { "LOC_ID_TEXT_100", }
                ));
            m_dialogDict.Add("Chest_Failed 6",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_101", },
                    new string[] { "LOC_ID_TEXT_102", }
                ));
            m_dialogDict.Add("CarnivalRoom1-Start",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_103", "LOC_ID_TEXT_103", },
                    new string[] { "LOC_ID_TEXT_104", "LOC_ID_TEXT_106", }
                ));
            m_dialogDict.Add("CarnivalRoom1-End",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_107", },
                    new string[] { "LOC_ID_TEXT_108", }
                ));
            m_dialogDict.Add("CarnivalRoom1-Reward",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_109", },
                    new string[] { "LOC_ID_TEXT_110", }
                ));
            m_dialogDict.Add("CarnivalRoom1-Fail",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_111", },
                    new string[] { "LOC_ID_TEXT_112", }
                ));
            m_dialogDict.Add("CarnivalRoom2-Start",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_114", "LOC_ID_TEXT_114", },
                    new string[] { "LOC_ID_TEXT_115", "LOC_ID_TEXT_117", }
                ));
            m_dialogDict.Add("CarnivalRoom2-Reward",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_118", },
                    new string[] { "LOC_ID_TEXT_119", }
                ));
            m_dialogDict.Add("CarnivalRoom2-Fail",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_120", },
                    new string[] { "LOC_ID_TEXT_121", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-Start",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_123", "LOC_ID_TEXT_125", "LOC_ID_TEXT_125", "LOC_ID_TEXT_128", },
                    new string[] { "LOC_ID_TEXT_124", "LOC_ID_TEXT_126", "LOC_ID_TEXT_127", "LOC_ID_TEXT_129", }
                ));
            m_dialogDict.Add("ChestBonusRoom2-Start",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_130", "LOC_ID_TEXT_132", "LOC_ID_TEXT_132", "LOC_ID_TEXT_135", },
                    new string[] { "LOC_ID_TEXT_131", "LOC_ID_TEXT_133", "LOC_ID_TEXT_134", "LOC_ID_TEXT_136", }
                ));
            m_dialogDict.Add("ChestBonusRoom3-Start",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_137", "LOC_ID_TEXT_139", "LOC_ID_TEXT_139", "LOC_ID_TEXT_142", },
                    new string[] { "LOC_ID_TEXT_138", "LOC_ID_TEXT_140", "LOC_ID_TEXT_141", "LOC_ID_TEXT_143", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-NoMoney",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_144", },
                    new string[] { "LOC_ID_TEXT_145", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-Lost",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_146", "LOC_ID_TEXT_146", "LOC_ID_TEXT_146", },
                    new string[] { "LOC_ID_TEXT_147", "LOC_ID_TEXT_148", "LOC_ID_TEXT_149", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-Won",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_150", },
                    new string[] { "LOC_ID_TEXT_151", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-Choose",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_152", },
                    new string[] { "LOC_ID_TEXT_153", }
                ));
            m_dialogDict.Add("ChestBonusRoom1-End",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_154", },
                    new string[] { "LOC_ID_TEXT_155", }
                ));
            m_dialogDict.Add("PortraitRoomText0",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", "LOC_ID_TEXT_156", },
                    new string[] { "LOC_ID_TEXT_157", "LOC_ID_TEXT_160", "LOC_ID_TEXT_161", "LOC_ID_TEXT_162", "LOC_ID_TEXT_163", "LOC_ID_TEXT_164", "LOC_ID_TEXT_165", }
                ));
            m_dialogDict.Add("PortraitRoomText1",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", "LOC_ID_TEXT_166", },
                    new string[] { "LOC_ID_TEXT_167", "LOC_ID_TEXT_170", "LOC_ID_TEXT_171", "LOC_ID_TEXT_172", "LOC_ID_TEXT_173", "LOC_ID_TEXT_174", "LOC_ID_TEXT_175", "LOC_ID_TEXT_176", }
                ));
            m_dialogDict.Add("PortraitRoomText2",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", "LOC_ID_TEXT_177", },
                    new string[] { "LOC_ID_TEXT_178", "LOC_ID_TEXT_181", "LOC_ID_TEXT_182", "LOC_ID_TEXT_183", "LOC_ID_TEXT_184", "LOC_ID_TEXT_185", "LOC_ID_TEXT_186", }
                ));
            m_dialogDict.Add("PortraitRoomText3",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_187", "LOC_ID_TEXT_187", "LOC_ID_TEXT_187", "LOC_ID_TEXT_187", "LOC_ID_TEXT_187", },
                    new string[] { "LOC_ID_TEXT_188", "LOC_ID_TEXT_191", "LOC_ID_TEXT_192", "LOC_ID_TEXT_193", "LOC_ID_TEXT_194", }
                ));
            m_dialogDict.Add("PortraitRoomText4",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", "LOC_ID_TEXT_195", },
                    new string[] { "LOC_ID_TEXT_196", "LOC_ID_TEXT_199", "LOC_ID_TEXT_200", "LOC_ID_TEXT_201", "LOC_ID_TEXT_202", "LOC_ID_TEXT_203", "LOC_ID_TEXT_204", "LOC_ID_TEXT_205", }
                ));
            m_dialogDict.Add("PortraitRoomText5",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_206", "LOC_ID_TEXT_206", "LOC_ID_TEXT_206", "LOC_ID_TEXT_206", "LOC_ID_TEXT_206", "LOC_ID_TEXT_206", },
                    new string[] { "LOC_ID_TEXT_207", "LOC_ID_TEXT_210", "LOC_ID_TEXT_211", "LOC_ID_TEXT_212", "LOC_ID_TEXT_213", "LOC_ID_TEXT_214", }
                ));
            m_dialogDict.Add("PortraitRoomText6",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_215", "LOC_ID_TEXT_215", "LOC_ID_TEXT_215", "LOC_ID_TEXT_215", "LOC_ID_TEXT_215", "LOC_ID_TEXT_215", },
                    new string[] { "LOC_ID_TEXT_216", "LOC_ID_TEXT_219", "LOC_ID_TEXT_220", "LOC_ID_TEXT_221", "LOC_ID_TEXT_222", "LOC_ID_TEXT_223", }
                ));
            m_dialogDict.Add("PortraitRoomText7",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", },
                    new string[] { "LOC_ID_TEXT_225", "LOC_ID_TEXT_228", "LOC_ID_TEXT_229", "LOC_ID_TEXT_230", "LOC_ID_TEXT_231", "LOC_ID_TEXT_232", "LOC_ID_TEXT_233", "LOC_ID_TEXT_234", "LOC_ID_TEXT_235", "LOC_ID_TEXT_236", }
                ));
            m_dialogDict.Add("PortraitRoomText8",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224", "LOC_ID_TEXT_224" },
                    new string[] { "LOC_ID_PORTRAIT_TEXT_NEW_1", "LOC_ID_PORTRAIT_TEXT_NEW_2", "LOC_ID_PORTRAIT_TEXT_NEW_3", "LOC_ID_PORTRAIT_TEXT_NEW_4", "LOC_ID_PORTRAIT_TEXT_NEW_5", "LOC_ID_PORTRAIT_TEXT_NEW_6"}
                ));
            m_dialogDict.Add("ConfirmTest1",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_237", },
                    new string[] { "LOC_ID_TEXT_238", }
                ));
            m_dialogDict.Add("DeleteFileWarning",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_239", },
                    new string[] { "LOC_ID_TEXT_240", }
                ));
            m_dialogDict.Add("RestoreDefaultControlsWarning",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_241", },
                    new string[] { "LOC_ID_TEXT_242", }
                ));
            m_dialogDict.Add("LineageChoiceWarning",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_243", },
                    new string[] { "LOC_ID_TEXT_244", }
                ));
            m_dialogDict.Add("Resolution Changed",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_245", },
                    new string[] { "LOC_ID_TEXT_246", }
                ));
            m_dialogDict.Add("Delete Save",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_248", },
                    new string[] { "LOC_ID_TEXT_249", }
                ));
            m_dialogDict.Add("Delete Save2",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_251", },
                    new string[] { "LOC_ID_TEXT_252", }
                ));
            m_dialogDict.Add("Back to Menu",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_254", },
                    new string[] { "LOC_ID_TEXT_255", }
                ));
            m_dialogDict.Add("Quit Rogue Legacy",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_256", },
                    new string[] { "LOC_ID_TEXT_257", }
                ));
            m_dialogDict.Add("Save File Error",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_258", "LOC_ID_TEXT_258", },
                    new string[] { "LOC_ID_TEXT_259", "LOC_ID_TEXT_260", }
                ));
            m_dialogDict.Add("Save File Error 2",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_261", },
                    new string[] { "LOC_ID_TEXT_262", }
                ));
            m_dialogDict.Add("Save File Error Antivirus",
                new DialogueObj(
                    new string[] { "LOC_ID_TEXT_263", "LOC_ID_TEXT_263", },
                    new string[] { "LOC_ID_TEXT_264", "LOC_ID_TEXT_265", }
                ));

            // Diary entries
            m_dialogDict.Add("DiaryEntry0",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", },
                    new string[] { "LOC_ID_DIARY_2", "LOC_ID_DIARY_3", "LOC_ID_DIARY_4", "LOC_ID_DIARY_5", "LOC_ID_DIARY_6", "LOC_ID_DIARY_7", }
                ));
            m_dialogDict.Add("DiaryEntry1",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", "LOC_ID_DIARY_8", },
                    new string[] { "LOC_ID_DIARY_9", "LOC_ID_DIARY_10", "LOC_ID_DIARY_11", "LOC_ID_DIARY_12", "LOC_ID_DIARY_13", "LOC_ID_DIARY_14", "LOC_ID_DIARY_15", }
                ));
            m_dialogDict.Add("DiaryEntry2",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_16", "LOC_ID_DIARY_16", "LOC_ID_DIARY_16", "LOC_ID_DIARY_16", "LOC_ID_DIARY_16", "LOC_ID_DIARY_16", },
                    new string[] { "LOC_ID_DIARY_17", "LOC_ID_DIARY_18", "LOC_ID_DIARY_19", "LOC_ID_DIARY_20", "LOC_ID_DIARY_21", "LOC_ID_DIARY_22", }
                ));
            m_dialogDict.Add("DiaryEntry3",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_23", "LOC_ID_DIARY_23", "LOC_ID_DIARY_23", "LOC_ID_DIARY_23", "LOC_ID_DIARY_23", "LOC_ID_DIARY_23", },
                    new string[] { "LOC_ID_DIARY_24", "LOC_ID_DIARY_25", "LOC_ID_DIARY_26", "LOC_ID_DIARY_27", "LOC_ID_DIARY_28", "LOC_ID_DIARY_29", }
                ));
            m_dialogDict.Add("DiaryEntry4",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_30", "LOC_ID_DIARY_30", "LOC_ID_DIARY_30", "LOC_ID_DIARY_30", "LOC_ID_DIARY_30", },
                    new string[] { "LOC_ID_DIARY_31", "LOC_ID_DIARY_32", "LOC_ID_DIARY_33", "LOC_ID_DIARY_34", "LOC_ID_DIARY_35", }
                ));
            m_dialogDict.Add("DiaryEntry5",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_36", "LOC_ID_DIARY_36", "LOC_ID_DIARY_36", "LOC_ID_DIARY_36", "LOC_ID_DIARY_36", },
                    new string[] { "LOC_ID_DIARY_37", "LOC_ID_DIARY_38", "LOC_ID_DIARY_39", "LOC_ID_DIARY_40", "LOC_ID_DIARY_41", }
                ));
            m_dialogDict.Add("DiaryEntry6",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_42", "LOC_ID_DIARY_42", "LOC_ID_DIARY_42", "LOC_ID_DIARY_42", "LOC_ID_DIARY_42", "LOC_ID_DIARY_42", },
                    new string[] { "LOC_ID_DIARY_43", "LOC_ID_DIARY_44", "LOC_ID_DIARY_45", "LOC_ID_DIARY_46", "LOC_ID_DIARY_47", "LOC_ID_DIARY_48", }
                ));
            m_dialogDict.Add("DiaryEntry7",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_49", "LOC_ID_DIARY_49", "LOC_ID_DIARY_49", "LOC_ID_DIARY_49", "LOC_ID_DIARY_49", },
                    new string[] { "LOC_ID_DIARY_50", "LOC_ID_DIARY_51", "LOC_ID_DIARY_52", "LOC_ID_DIARY_53", "LOC_ID_DIARY_54", }
                ));
            m_dialogDict.Add("DiaryEntry8",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_55", "LOC_ID_DIARY_55", "LOC_ID_DIARY_55", "LOC_ID_DIARY_55", },
                    new string[] { "LOC_ID_DIARY_56", "LOC_ID_DIARY_57", "LOC_ID_DIARY_58", "LOC_ID_DIARY_59", }
                ));
            m_dialogDict.Add("DiaryEntry9",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_60", "LOC_ID_DIARY_60", "LOC_ID_DIARY_60", "LOC_ID_DIARY_60", "LOC_ID_DIARY_60", "LOC_ID_DIARY_60", },
                    new string[] { "LOC_ID_DIARY_61", "LOC_ID_DIARY_62", "LOC_ID_DIARY_63", "LOC_ID_DIARY_64", "LOC_ID_DIARY_65", "LOC_ID_DIARY_66", }
                ));
            m_dialogDict.Add("DiaryEntry10",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_67", "LOC_ID_DIARY_67", "LOC_ID_DIARY_67", "LOC_ID_DIARY_67", "LOC_ID_DIARY_67", },
                    new string[] { "LOC_ID_DIARY_68", "LOC_ID_DIARY_69", "LOC_ID_DIARY_70", "LOC_ID_DIARY_71", "LOC_ID_DIARY_72", }
                ));
            m_dialogDict.Add("DiaryEntry11",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_73", "LOC_ID_DIARY_73", "LOC_ID_DIARY_73", "LOC_ID_DIARY_73", "LOC_ID_DIARY_73", },
                    new string[] { "LOC_ID_DIARY_74", "LOC_ID_DIARY_75", "LOC_ID_DIARY_76", "LOC_ID_DIARY_77", "LOC_ID_DIARY_78", }
                ));
            m_dialogDict.Add("DiaryEntry12",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_79", "LOC_ID_DIARY_79", "LOC_ID_DIARY_79", "LOC_ID_DIARY_79", },
                    new string[] { "LOC_ID_DIARY_80", "LOC_ID_DIARY_81", "LOC_ID_DIARY_82", "LOC_ID_DIARY_83", }
                ));
            m_dialogDict.Add("DiaryEntry13",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_84", "LOC_ID_DIARY_84", "LOC_ID_DIARY_84", "LOC_ID_DIARY_84", },
                    new string[] { "LOC_ID_DIARY_85", "LOC_ID_DIARY_86", "LOC_ID_DIARY_87", "LOC_ID_DIARY_88", }
                ));
            m_dialogDict.Add("DiaryEntry14",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_89", "LOC_ID_DIARY_89", "LOC_ID_DIARY_89", "LOC_ID_DIARY_89", "LOC_ID_DIARY_89", },
                    new string[] { "LOC_ID_DIARY_90", "LOC_ID_DIARY_91", "LOC_ID_DIARY_92", "LOC_ID_DIARY_93", "LOC_ID_DIARY_94", }
                ));
            m_dialogDict.Add("DiaryEntry15",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_95", "LOC_ID_DIARY_95", "LOC_ID_DIARY_95", "LOC_ID_DIARY_95", "LOC_ID_DIARY_95", },
                    new string[] { "LOC_ID_DIARY_96", "LOC_ID_DIARY_97", "LOC_ID_DIARY_98", "LOC_ID_DIARY_99", "LOC_ID_DIARY_100", }
                ));
            m_dialogDict.Add("DiaryEntry16",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_101", "LOC_ID_DIARY_101", "LOC_ID_DIARY_101", "LOC_ID_DIARY_101", "LOC_ID_DIARY_101", },
                    new string[] { "LOC_ID_DIARY_102", "LOC_ID_DIARY_103", "LOC_ID_DIARY_104", "LOC_ID_DIARY_105", "LOC_ID_DIARY_106", }
                ));
            m_dialogDict.Add("DiaryEntry17",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_107", "LOC_ID_DIARY_107", "LOC_ID_DIARY_107", "LOC_ID_DIARY_107", },
                    new string[] { "LOC_ID_DIARY_108", "LOC_ID_DIARY_109", "LOC_ID_DIARY_110", "LOC_ID_DIARY_111", }
                ));
            m_dialogDict.Add("DiaryEntry18",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_112", "LOC_ID_DIARY_112", "LOC_ID_DIARY_112", "LOC_ID_DIARY_112", },
                    new string[] { "LOC_ID_DIARY_113", "LOC_ID_DIARY_114", "LOC_ID_DIARY_115", "LOC_ID_DIARY_116", }
                ));
            m_dialogDict.Add("DiaryEntry19",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_117", "LOC_ID_DIARY_117", "LOC_ID_DIARY_117", "LOC_ID_DIARY_117", "LOC_ID_DIARY_117", },
                    new string[] { "LOC_ID_DIARY_118", "LOC_ID_DIARY_119", "LOC_ID_DIARY_120", "LOC_ID_DIARY_121", "LOC_ID_DIARY_122", }
                ));
            m_dialogDict.Add("DiaryEntry20",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_123", "LOC_ID_DIARY_123", "LOC_ID_DIARY_123", "LOC_ID_DIARY_123", },
                    new string[] { "LOC_ID_DIARY_124", "LOC_ID_DIARY_125", "LOC_ID_DIARY_126", "LOC_ID_DIARY_127", }
                ));
            m_dialogDict.Add("DiaryEntry21",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_128", "LOC_ID_DIARY_128", "LOC_ID_DIARY_128", "LOC_ID_DIARY_128", "LOC_ID_DIARY_128", "LOC_ID_DIARY_128", },
                    new string[] { "LOC_ID_DIARY_129", "LOC_ID_DIARY_130", "LOC_ID_DIARY_131", "LOC_ID_DIARY_132", "LOC_ID_DIARY_133", "LOC_ID_DIARY_134", }
                ));
            m_dialogDict.Add("DiaryEntry22",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", "LOC_ID_DIARY_135", },
                    new string[] { "LOC_ID_DIARY_136", "LOC_ID_DIARY_137", "LOC_ID_DIARY_138", "LOC_ID_DIARY_139", "LOC_ID_DIARY_140", "LOC_ID_DIARY_141", "LOC_ID_DIARY_142", "LOC_ID_DIARY_143", }
                ));
            m_dialogDict.Add("DiaryEntry23",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_144", "LOC_ID_DIARY_144", "LOC_ID_DIARY_144", },
                    new string[] { "LOC_ID_DIARY_145", "LOC_ID_DIARY_146", "LOC_ID_DIARY_147", }
                ));
            m_dialogDict.Add("DiaryEntry24",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", "LOC_ID_DIARY_148", },
                    new string[] { "LOC_ID_DIARY_149", "LOC_ID_DIARY_150", "LOC_ID_DIARY_151", "LOC_ID_DIARY_152", "LOC_ID_DIARY_153", "LOC_ID_DIARY_154", "LOC_ID_DIARY_155", "LOC_ID_DIARY_156", "LOC_ID_DIARY_157", "LOC_ID_DIARY_158", "LOC_ID_DIARY_159", }
                ));
            m_dialogDict.Add("FinalBossTalk01_Special",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_160", "LOC_ID_DIARY_160", "LOC_ID_DIARY_160", },
                    new string[] { "LOC_ID_DIARY_161", "LOC_ID_DIARY_162", "LOC_ID_DIARY_163", }
                ));
            m_dialogDict.Add("FinalBossTalk01",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_164", "LOC_ID_DIARY_164", "LOC_ID_DIARY_164", "LOC_ID_DIARY_164", "LOC_ID_DIARY_164", "LOC_ID_DIARY_164", },
                    new string[] { "LOC_ID_DIARY_165", "LOC_ID_DIARY_166", "LOC_ID_DIARY_167", "LOC_ID_DIARY_168", "LOC_ID_DIARY_169", "LOC_ID_DIARY_170", }
                ));
            m_dialogDict.Add("FinalBossTalk02",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_171", "LOC_ID_DIARY_171", "LOC_ID_DIARY_171", "LOC_ID_DIARY_171", "LOC_ID_DIARY_171", },
                    new string[] { "LOC_ID_DIARY_172", "LOC_ID_DIARY_173", "LOC_ID_DIARY_174", "LOC_ID_DIARY_175", "LOC_ID_DIARY_176", }
                ));
            m_dialogDict.Add("FinalBossTalk03",
                new DialogueObj(
                    new string[] { "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", "LOC_ID_DIARY_177", },
                    new string[] { "LOC_ID_DIARY_178", "LOC_ID_DIARY_179", "LOC_ID_DIARY_180", "LOC_ID_DIARY_181", "LOC_ID_DIARY_182", "LOC_ID_DIARY_183", "LOC_ID_DIARY_184", "LOC_ID_DIARY_185", "LOC_ID_DIARY_186", }
                ));
            m_dialogDict.Add("DonationBoxTalk01",
                new DialogueObj(
                    new string[] { "LOC_ID_DONATIONBOX_TITLE_1" },
                    new string[] { "LOC_ID_DONATIONBOX_TEXT_1" }
                ));
            m_dialogDict.Add("DonationBoxTalkUpgraded",
                new DialogueObj(
                    new string[] { "" },
                    new string[] { "LOC_ID_DONATIONBOX_TEXT_4" }
                ));
            m_dialogDict.Add("DonationBoxTalkMaxxed",
                new DialogueObj(
                    new string[] { "LOC_ID_DONATIONBOX_TITLE_1" },
                    new string[] { "LOC_ID_DONATIONBOX_TEXT_2" }
                ));
            m_dialogDict.Add("DonationBoxTalkPoor",
                new DialogueObj(
                    new string[] { "LOC_ID_DONATIONBOX_TITLE_1" },
                    new string[] { "LOC_ID_DONATIONBOX_TEXT_3" }
                ));
        }

        public static void LoadLanguageDocument(ContentManager content, string fileName)
        {
            LoadLanguageDocument(Path.Combine(content.RootDirectory, fileName + ".txt"));
        }

        public static void LoadLanguageDocument(string fullFilePath)
        {
            FileInfo fileInfo = new FileInfo(fullFilePath);

            using (StreamReader reader = fileInfo.OpenText())
            {
                ParseDocument(reader);
            }
        }

        private static void ParseDocument(StreamReader reader)
        {
            string line = "";
            int lineCount = 0;
            string currentLabel = "";
            string currentSpeaker = "";
            string currentDialogue = null;
            List<string> currentSpeakerArray = new List<string>();
            List<string> currentTextArray = new List<string>();
            bool newTextBlock = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (line != "" && line.IndexOf("//") != 0) // ignore blank lines.
                {
                    string data = line.Substring(line.IndexOf(" ") + 1);
                    if (lineCount == 0 && !line.Contains("@language"))
                        throw new Exception("Cannot create text dictionary from file. Unspecified language type.");

                    if (line.Contains("@language"))
                        SetLanguage(data);
                    else if (line.Contains("@label"))
                    {
                        if (newTextBlock == false)
                        {
                            if (currentDialogue != null)
                            {
                                currentTextArray.Add(currentDialogue);
                                currentDialogue = null;
                            }
                            AddText(currentLabel, currentSpeakerArray.ToArray(), currentTextArray.ToArray());
                            newTextBlock = true;
                        }

                        if (newTextBlock == true)
                        {
                            // Resets everything to a new text block.
                            newTextBlock = false;
                            currentLabel = data;
                            currentSpeakerArray.Clear();
                            currentTextArray.Clear();
                            currentDialogue = null;
                            currentSpeaker = "";
                        }
                    }
                    else if (line.Contains("@title"))
                        currentSpeaker = data;
                    else if (line.Contains("@text"))
                    {
                        currentSpeakerArray.Add(currentSpeaker);
                        if (currentDialogue != null)
                        {
                            currentTextArray.Add(currentDialogue);
                            currentDialogue = null;
                        }
                        currentDialogue = data;
                    }
                    else
                        currentDialogue += "\n" + line;
                }
                lineCount++;
            }

            if (currentDialogue != null)
            {
                currentTextArray.Add(currentDialogue);
                currentDialogue = null;
            }
            AddText(currentLabel, currentSpeakerArray.ToArray(), currentTextArray.ToArray());

            //Console.WriteLine("m_languageArray[\"English\"]: " + m_languageArray["English"]);
        }

        public static void LoadLanguageBinFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            //using (StreamReader fileStream = new StreamReader(filePath))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    string line = "";
                    int lineCount = 0;
                    string currentLabel = "";
                    string currentSpeaker = "";
                    string currentDialogue = null;
                    List<string> currentSpeakerArray = new List<string>();
                    List<string> currentTextArray = new List<string>();
                    bool newTextBlock = true;

                    do
                    {
                        line = reader.ReadString();

                        if (line != "" && line.IndexOf("//") != 0) // ignore blank lines.
                        {
                            string data = line.Substring(line.IndexOf(" ") + 1);
                            if (lineCount == 0 && !line.Contains("@language"))
                                throw new Exception("Cannot create text dictionary from file. Unspecified language type.");

                            if (line.Contains("@language"))
                                SetLanguage(data);
                            else if (line.Contains("@label"))
                            {
                                if (newTextBlock == false)
                                {
                                    if (currentDialogue != null)
                                    {
                                        currentTextArray.Add(currentDialogue);
                                        currentDialogue = null;
                                    }

                                    AddText(currentLabel, currentSpeakerArray.ToArray(), currentTextArray.ToArray());
                                    newTextBlock = true;
                                }

                                if (newTextBlock == true)
                                {
                                    // Resets everything to a new text block.
                                    newTextBlock = false;
                                    currentLabel = data;
                                    currentSpeakerArray.Clear();
                                    currentTextArray.Clear();
                                    currentDialogue = null;
                                    currentSpeaker = "";
                                }
                            }
                            else if (line.Contains("@title"))
                                currentSpeaker = data;
                            else if (line.Contains("@text"))
                            {
                                currentSpeakerArray.Add(currentSpeaker);
                                if (currentDialogue != null)
                                {
                                    currentTextArray.Add(currentDialogue);
                                    currentDialogue = null;
                                }
                                currentDialogue = data;
                            }
                            else if (line != "eof")
                                currentDialogue += "\n" + line;
                        }
                        lineCount++;

                    } while (line != "eof"); // end of file check.

                    if (newTextBlock == false)
                    {
                        if (currentDialogue != null)
                        {
                            currentTextArray.Add(currentDialogue);
                            currentDialogue = null;
                        }

                        AddText(currentLabel, currentSpeakerArray.ToArray(), currentTextArray.ToArray());
                    }
                }
            }
        }

        public static void SetLanguage(string language)
        {
            m_currentLanguage = language;
            if (m_languageArray.ContainsKey(m_currentLanguage) == false)
            {
                Console.WriteLine("Adding language dictionary for language: " + language);
                m_languageArray.Add(language, new Dictionary<string, DialogueObj>());
            }
        }

        public static void AddText(string key, string[] speakers, string[] text)
        {
            if (m_currentLanguage != null)
            {
                if (m_languageArray[m_currentLanguage].ContainsKey(key))
                    Console.WriteLine("Cannot add text. Text with title already specified.");
                else
                {
                    DialogueObj dialogueObj = new DialogueObj(speakers, text);
                    m_languageArray[m_currentLanguage].Add(key, dialogueObj);

#if false
                    // Generate this:
                    /*
                    m_dialogDict.Add("DiaryEntry0",
                        new DialogueObj(
                            new string[] { "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1", "LOC_ID_DIARY_1" },
                            new string[] { "LOC_ID_DIARY_2", "LOC_ID_DIARY_3", "LOC_ID_DIARY_4", "LOC_ID_DIARY_5", "LOC_ID_DIARY_6", "LOC_ID_DIARY_7" }
                        ));
                     */

                    Console.WriteLine("m_dialogDict.Add(\"" + key + "\",");
                    Console.WriteLine("    new DialogueObj(");
                    Console.Write("        new string[] { ");
                    foreach (var item in speakers)
                    {
                        Console.Write("\"" + item.ToString() + "\", ");
                    }
                    Console.WriteLine("},");
                    Console.Write("        new string[] { ");
                    foreach (var item in text)
                    {
                        Console.Write("\"" + item.ToString() + "\", ");
                    }
                    Console.WriteLine("}");
                    Console.WriteLine("    ));");
#endif
                }
            }
            else
                Console.WriteLine("Call SetLanguage() before attempting to add text to a specified language.");
        }

        public static DialogueObj GetText(string key)
        {
            //return m_languageArray[m_currentLanguage][key];
            return m_dialogDict[key];
        }

        public static string GetCurrentLanguage()
        {
            return m_currentLanguage;
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Dialogue Manager");

                foreach (KeyValuePair<string, Dictionary<string, DialogueObj>> languageDict in m_languageArray)
                {
                    foreach (KeyValuePair<string, DialogueObj> text in languageDict.Value)
                        text.Value.Dispose();
                    languageDict.Value.Clear();
                }
                m_languageArray.Clear();
                m_languageArray = null;
                m_isDisposed = true;
            }
        }

        public static bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
