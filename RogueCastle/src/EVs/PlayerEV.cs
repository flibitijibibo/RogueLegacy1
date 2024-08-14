using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastle
{
    class PlayerEV
    {
        public const float KNIGHT_DAMAGE_MOD = 1.0f;

        public const float ASSASSIN_CRITDAMAGE_MOD = 1.25f;
        public const float ASSASSIN_CRITCHANCE_MOD = 0.15f;
        public const float ASSASSIN_DAMAGE_MOD = 0.75f;
        public const float ASSASSIN_HEALTH_MOD = 0.75f;
        public const float ASSASSIN_MANA_MOD = 0.65f;//0.75f;

        public const float BARBARIAN_DAMAGE_MOD = 0.75f;
        public const float BARBARIAN_HEALTH_MOD = 1.5f;
        public const float BARBARIAN_MANA_MOD = 0.5f;

        public const float MAGE_DAMAGE_MOD = 0.5f;
        public const float MAGE_MAGICDAMAGE_MOD = 1.25f;//1.3f;//1.35f;//1.5f; //1.4f;
        public const float MAGE_HEALTH_MOD = 0.5f;
        public const float MAGE_MANA_MOD = 1.5f;

        public const float BANKER_DAMAGE_MOD = 0.75f;
        public const float BANKER_HEALTH_MOD = 0.5f;
        public const float BANKER_GOLDGAIN_MOD = 0.3f;
        public const float BANKER_MANA_MOD = 0.5f;

        public const float NINJA_DAMAGE_MOD = 1.75f;
        public const float NINJA_MOVESPEED_MOD = 0.3f;
        public const float NINJA_HEALTH_MOD = 0.6f;
        public const float NINJA_MANA_MOD = 0.4f;

        public const float LICH_DAMAGE_MOD = 0.75f;//0.6f;
        public const float LICH_MAGICDAMAGE_MOD = 1.5f;
        public const float LICH_HEALTH_MOD = 0.35f;
        public const float LICH_MANA_MOD = 0.5f;//0.7f;//0.25f;

        public const float SPELLSWORD_DAMAGE_MOD = 0.75f;
        public const float SPELLSWORD_MANA_MOD = 0.4f;
        public const float SPELLSWORD_HEALTH_MOD = 0.75f;

        public const float DRAGON_HEALTH_MOD = 0.40f;
        public const float DRAGON_MANA_MOD = 0.25f;
        public const float DRAGON_MOVESPEED_MOD = 0f;

        public const float TRAITOR_HEALTH_MOD = 0.70f;
        public const float TRAITOR_MANA_MOD = 0.70f;
        public const float TRAITOR_CLOSE_MANACOST = 30;
        public const float TRAITOR_CLOSE_DAMAGEMOD = 1f;//0.5f;
        public const float TRAITOR_AXE_MANACOST = 30;
        public const float TRAITOR_AXE_DAMAGEMOD = 1f;//0.75f;

    }
}
