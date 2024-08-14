using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastle
{
    public class WordBuilder
    {
        private static string[] intro = { "Sir Skunky the Knight\n" }; //{ "So it continues... ", "Once more, ", "As constant as the passage of time, ", "The story grows stale, and " };
        private static string[] middle = { "Gigantism - You are larger\n"  };//{ "an old hero's journey draws to a close, ", "another hero arises ", "the fated journey of another hero begins ", "another fool appears " };
        private static string[] end = { "Dextrocardia - HP/MP pools are swapped\n" };//{ "as the consuming darkness creeps ever closer.", "as the allure of defeating the castle grows.", "and the evil that plagues the world remains.", "because this game is too damn hard." };

        public static string BuildDungeonNameLocID(GameTypes.LevelType levelType)
        {
            switch (levelType)
            {
                case GameTypes.LevelType.CASTLE:
                    return "LOC_ID_DUNGEON_NAME_1"; //"Castle Hamson"; //"The Keep"; //TEDDY - CHANGING NAME
                case GameTypes.LevelType.DUNGEON:
                    return "LOC_ID_DUNGEON_NAME_2"; //"The Land of Darkness"; //"The Dungeon"; //TEDDY - CHANGING NAME
                case GameTypes.LevelType.GARDEN:
                    return "LOC_ID_DUNGEON_NAME_3"; //"Forest Abkhazia";//"The Dungeon"; //TEDDY - CHANGING NAME
                case GameTypes.LevelType.TOWER:
                    return "LOC_ID_DUNGEON_NAME_4"; //"The Maya";//"The Tower"; //TEDDY - CHANGING NAME
            }

            return "";
        }

        // This doesn't seem to be used so no need for localization  --Dave
        public static void RandomIntro(TextObj textObj, int wordWrap = 0)
        {
            textObj.Text = intro[CDGMath.RandomInt(0, intro.Length - 1)] + middle[CDGMath.RandomInt(0, middle.Length - 1)] + end[CDGMath.RandomInt(0, end.Length - 1)];
            if (wordWrap > 0)
                textObj.WordWrap(wordWrap);
        }
    }
}
