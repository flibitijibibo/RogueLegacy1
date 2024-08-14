using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    class SpellEV
    {
        public const string AXE_Name = "Axe"; public const int AXE_Cost = 15; public const float AXE_Damage = 1f; public const float AXE_XVal = 0f; public const float AXE_YVal = 0f; public const int AXE_Rarity = 1;
        public const string DAGGER_Name = "Dagger"; public const int DAGGER_Cost = 10; public const float DAGGER_Damage = 1f; public const float DAGGER_XVal = 0f; public const float DAGGER_YVal = 0f; public const int DAGGER_Rarity = 1;
        public const string TIMEBOMB_Name = "Runic Trigger"; public const int TIMEBOMB_Cost = 15; public const float TIMEBOMB_Damage = 1.5f; public const float TIMEBOMB_XVal = 1f; public const float TIMEBOMB_YVal = 0f; public const int TIMEBOMB_Rarity = 1;
        public const string TIMESTOP_Name = "Stop Watch"; public const int TIMESTOP_Cost = 15; public const float TIMESTOP_Damage = 0f; public const float TIMESTOP_XVal = 3f; public const float TIMESTOP_YVal = 0f; public const int TIMESTOP_Rarity = 2;
        public const string NUKE_Name = "Nuke"; public const int NUKE_Cost = 40; public const float NUKE_Damage = 0.75f; public const float NUKE_XVal = 0f; public const float NUKE_YVal = 0f; public const int NUKE_Rarity = 3;
        public const string TRANSLOCATER_Name = "Quantum Translocater"; public const int TRANSLOCATER_Cost = 5; public const float TRANSLOCATER_Damage = 0f; public const float TRANSLOCATER_XVal = 0f; public const float TRANSLOCATER_YVal = 0f; public const int TRANSLOCATER_Rarity = 3;
        public const string DISPLACER_Name = "Displacer"; public const int DISPLACER_Cost = 10; public const float DISPLACER_Damage = 0f; public const float DISPLACER_XVal = 0f; public const float DISPLACER_YVal = 0f; public const int DISPLACER_Rarity = 0;
        public const string BOOMERANG_Name = "Cross"; public const int BOOMERANG_Cost = 15; public const float BOOMERANG_Damage = 1f; public const float BOOMERANG_XVal = 18f; public const float BOOMERANG_YVal = 0f; public const int BOOMERANG_Rarity = 2;
        public const string DUAL_BLADES_Name = "Spark"; public const int DUAL_BLADES_Cost = 15; public const float DUAL_BLADES_Damage = 1f; public const float DUAL_BLADES_XVal = 0f; public const float DUAL_BLADES_YVal = 0f; public const int DUAL_BLADES_Rarity = 1;
        public const string CLOSE_Name = "Katana"; public const int CLOSE_Cost = 15; public const float CLOSE_Damage = 0.5f; public const float CLOSE_XVal = 2.1f; public const float CLOSE_YVal = 0f; public const int CLOSE_Rarity = 2;
        public const string DAMAGE_SHIELD_Name = "Leaf"; public const int DAMAGE_SHIELD_Cost = 15; public const float DAMAGE_SHIELD_Damage = 1f; public const float DAMAGE_SHIELD_XVal = 9999f; public const float DAMAGE_SHIELD_YVal = 5f; public const int DAMAGE_SHIELD_Rarity = 2;
        public const string BOUNCE_Name = "Chaos"; public const int BOUNCE_Cost = 30; public const float BOUNCE_Damage = 0.4f; public const float BOUNCE_XVal = 3.5f; public const float BOUNCE_YVal = 0f; public const int BOUNCE_Rarity = 3;
        public const string LASER_Name = "Laser"; public const int LASER_Cost = 15; public const float LASER_Damage = 1f; public const float LASER_XVal = 5f; public const float LASER_YVal = 0f; public const int LASER_Rarity = 3;
        public const string DRAGONFIRE_Name = "Dragon Fire"; public const int DRAGONFIRE_Cost = 15; public const float DRAGONFIRE_Damage = 1f; public const float DRAGONFIRE_XVal = 0.35f; public const float DRAGONFIRE_YVal = 0f; public const int DRAGONFIRE_Rarity = 3;
        public const string RAPIDDAGGER_Name = "Rapid Dagger"; public const int RAPIDDAGGER_Cost = 30; public const float RAPIDDAGGER_Damage = 0.75f; public const float RAPIDDAGGER_XVal = 0f; public const float RAPIDDAGGER_YVal = 0f; public const int RAPIDDAGGER_Rarity = 1;
        public const string DRAGONFIRENEO_Name = "Dragon Fire Neo"; public const int DRAGONFIRENEO_Cost = 0; public const float DRAGONFIRENEO_Damage = 1f; public const float DRAGONFIRENEO_XVal = 0.75f; public const float DRAGONFIRENEO_YVal = 0f; public const int DRAGONFIRENEO_Rarity = 3;		


        public static ProjectileData GetProjData(byte spellType, PlayerObj player)
        {
            ProjectileData projData = new ProjectileData(player)
            {
                SpriteName = "BoneProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = 0,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = Vector2.One,
                ShowIcon = false,
            };

            switch (spellType)
            {
                case (SpellType.Axe):
                    projData.SpriteName = "SpellAxe_Sprite";
                    projData.Angle = new Vector2(-74, -74);
                    projData.Speed = new Vector2(1050, 1050);//(1000, 1000);
                    projData.SourceAnchor = new Vector2(50, -50);
                    projData.IsWeighted = true;
                    projData.RotationSpeed = 10;//15;
                    projData.CollidesWithTerrain = false;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.Scale = new Vector2(3,3);//(2, 2);
                    break;
                case (SpellType.Dagger):
                    projData.SpriteName = "SpellDagger_Sprite";
                    projData.Angle = Vector2.Zero;
                    projData.SourceAnchor = new Vector2(50, 0);
                    projData.Speed = new Vector2(1750, 1750);
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;//35;
                    projData.CollidesWithTerrain = true;
                    projData.DestroysWithTerrain = true;
                    projData.Scale = new Vector2(2.5f,2.5f);//(2, 2);
                    break;
                case (SpellType.DragonFire):
                    projData.SpriteName = "TurretProjectile_Sprite";
                    projData.Angle = Vector2.Zero;
                    projData.SourceAnchor = new Vector2(50, 0);
                    projData.Speed = new Vector2(1100,1100);//(1450, 1450);
                    projData.Lifespan = DRAGONFIRE_XVal;
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;//35;
                    projData.CollidesWithTerrain = true;
                    projData.DestroysWithTerrain = true;
                    projData.Scale = new Vector2(2.5f,2.5f);//(2, 2);
                    break;
                case (SpellType.DragonFireNeo):
                    projData.SpriteName = "TurretProjectile_Sprite";
                    projData.Angle = Vector2.Zero;
                    projData.SourceAnchor = new Vector2(50, 0);
                    projData.Speed = new Vector2(1750,1750);//(1450, 1450);
                    projData.Lifespan = DRAGONFIRENEO_XVal;
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;//35;
                    projData.CollidesWithTerrain = true;
                    projData.DestroysWithTerrain = true;
                    projData.Scale = new Vector2(2.75f,2.75f);//(2, 2);
                    break;
                case (SpellType.TimeBomb):
                    projData.SpriteName = "SpellTimeBomb_Sprite";
                    projData.Angle = new Vector2(-35,-35);//(-65, -65);
                    projData.Speed = new Vector2(500,500); //(1000, 1000);
                    projData.SourceAnchor = new Vector2 (50, -50);//(0, -100);//(50, -50);
                    projData.IsWeighted = true;
                    projData.RotationSpeed = 0;
                    projData.StartingRotation = 0;
                    projData.CollidesWithTerrain = true;
                    projData.DestroysWithTerrain = false;
                    projData.CollidesWith1Ways = true;
                    projData.Scale = new Vector2(3,3);//(2, 2);
                    break;
                case (SpellType.TimeStop):
                    break;
                case (SpellType.Nuke):
                    projData.SpriteName = "SpellNuke_Sprite";
                    projData.Angle = new Vector2(-65, -65);
                    projData.Speed = new Vector2(500, 500);
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;
                    projData.CollidesWithTerrain = false;
                    projData.DestroysWithTerrain = false;
                    projData.ChaseTarget = false; // This needs to be set to false because I'm doing something special for the nuke spell.
                    projData.DestroysWithEnemy = true;
                    projData.Scale = new Vector2(2, 2);
                    break;
                case (SpellType.Translocator):
                    break;
                case (SpellType.Displacer):
                    projData.SourceAnchor = new Vector2(0,0);//(300, 0);
                    projData.SpriteName = "SpellDisplacer_Sprite";
                    projData.Angle = new Vector2(0,0);//(90,90);//(0,0);//(-65, -65);
                    projData.Speed = Vector2.Zero; //new Vector2(8000,8000);//(1000, 1000);
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;//45;//0;
                    projData.CollidesWithTerrain = true;
                    projData.DestroysWithTerrain = false;
                    projData.CollidesWith1Ways = true; //SETTING TO TRUE TO FIX BUGS WITH LARGE GUYS GETTING INTO TINY HOLES
                    projData.Scale = new Vector2(2,2);//(2, 2);
                    break;
                case (SpellType.Boomerang):
                    projData.SpriteName = "SpellBoomerang_Sprite";
                    projData.Angle = new Vector2(0, 0);
                    projData.SourceAnchor = new Vector2(50, -10);
                    projData.Speed = new Vector2(790, 790);//Vector2(730, 730);//(1000, 1000);
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 25;
                    projData.CollidesWithTerrain = false;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.Scale = new Vector2(3,3);
                    break;
                case (SpellType.DualBlades):
                    projData.SpriteName = "SpellDualBlades_Sprite";
                    projData.Angle = new Vector2(-55, -55);
                    projData.SourceAnchor = new Vector2(50, 30);
                    projData.Speed = new Vector2(1000, 1000);
                    projData.IsWeighted = false;
                    //projData.StartingRotation = 45;
                    projData.RotationSpeed = 30;//20;
                    projData.CollidesWithTerrain = false;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.Scale = new Vector2(2, 2);
                    break;
                case (SpellType.Close):
                    projData.SpriteName = "SpellClose_Sprite";
                    //projData.Angle = new Vector2(90, 90);
                    projData.SourceAnchor = new Vector2(120, -60);//(75,-200);//(50, 0);
                    projData.Speed = new Vector2(0,0);//(450,450);//(1000, 1000);
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0f;
                    projData.DestroysWithEnemy = false;
                    projData.DestroysWithTerrain = false;
                    projData.CollidesWithTerrain = false;
                    projData.Scale = new Vector2(2.5f, 2.5f);
                    projData.LockPosition = true;
                    break;
                case (SpellType.DamageShield):
                    projData.SpriteName = "SpellDamageShield_Sprite";
                    projData.Angle = new Vector2(-65, -65);
                    projData.Speed = new Vector2(3.25f, 3.25f);//(2.45f, 2.45f);//(2.0f, 2.0f);
                    projData.Target = player;
                    projData.IsWeighted = false;
                    projData.RotationSpeed = 0;
                    projData.CollidesWithTerrain = false;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.Scale = new Vector2(3.0f, 3.0f);
                    projData.DestroyOnRoomTransition = false;
                    break;
                case (SpellType.Bounce):
                    projData.SpriteName = "SpellBounce_Sprite";
                    projData.Angle = new Vector2(-135, -135);
                    projData.Speed = new Vector2(785,785);//(825, 825);
                    projData.IsWeighted = false;
                    projData.StartingRotation = -135;
                    projData.FollowArc = false;
                    projData.RotationSpeed = 20;
                    projData.SourceAnchor = new Vector2(-10, -10);
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;//true;
                    projData.CollidesWithTerrain = true;
                    projData.Scale = new Vector2(3.25f, 3.25f);//(2.5f, 2.5f);
                    break;
                case (SpellType.Laser):
                    projData.SpriteName = "LaserSpell_Sprite";
                    projData.Angle = new Vector2(0, 0);
                    projData.Speed = new Vector2(0, 0);
                    projData.IsWeighted = false;
                    projData.IsCollidable = false;
                    projData.StartingRotation = 0;
                    projData.FollowArc = false;
                    projData.RotationSpeed = 0;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.CollidesWithTerrain = false;
                    projData.LockPosition = true;
                    break;
                case (SpellType.RapidDagger):
                    projData.SpriteName = "LaserSpell_Sprite";
                    projData.Angle = new Vector2(0, 0);
                    projData.Speed = new Vector2(0, 0);
                    projData.IsWeighted = false;
                    projData.IsCollidable = false;
                    projData.StartingRotation = 0;
                    projData.FollowArc = false;
                    projData.RotationSpeed = 0;
                    projData.DestroysWithTerrain = false;
                    projData.DestroysWithEnemy = false;
                    projData.CollidesWithTerrain = false;
                    projData.LockPosition = true;
                    break;
            }

            return projData;
        }

        public static int GetManaCost(byte spellType)
        {
            switch (spellType)
            {
                case (SpellType.Dagger):
                    return SpellEV.DAGGER_Cost; 
                case (SpellType.Axe):
                    return SpellEV.AXE_Cost;
                case (SpellType.TimeBomb):
                    return SpellEV.TIMEBOMB_Cost;
                case (SpellType.TimeStop):
                    return SpellEV.TIMESTOP_Cost;
                case (SpellType.Nuke):
                    return SpellEV.NUKE_Cost;
                case (SpellType.Translocator):
                    return SpellEV.TRANSLOCATER_Cost;
                case (SpellType.Displacer):
                    return SpellEV.DISPLACER_Cost;
                case (SpellType.Boomerang):
                    return SpellEV.BOOMERANG_Cost;
                case (SpellType.DualBlades):
                    return SpellEV.DUAL_BLADES_Cost;
                case (SpellType.Close):
                    return SpellEV.CLOSE_Cost;
                case (SpellType.DamageShield):
                    return SpellEV.DAMAGE_SHIELD_Cost;
                case (SpellType.Bounce):
                    return SpellEV.BOUNCE_Cost;
                case (SpellType.Laser):
                    return SpellEV.LASER_Cost;
                case (SpellType.DragonFire):
                    return SpellEV.DRAGONFIRE_Cost;
                case (SpellType.DragonFireNeo):
                    return SpellEV.DRAGONFIRENEO_Cost;
                case (SpellType.RapidDagger):
                    return SpellEV.RAPIDDAGGER_Cost;
            }

            return 0;
        }

        public static int GetRarity(byte spellType)
        {
            switch (spellType)
            {
                case (SpellType.Dagger):
                    return SpellEV.DAGGER_Rarity;
                case (SpellType.Axe):
                    return SpellEV.AXE_Rarity;
                case (SpellType.TimeBomb):
                    return SpellEV.TIMEBOMB_Rarity;
                case (SpellType.TimeStop):
                    return SpellEV.TIMESTOP_Rarity;
                case (SpellType.Nuke):
                    return SpellEV.NUKE_Rarity;
                case (SpellType.Translocator):
                    return SpellEV.TRANSLOCATER_Rarity;
                case (SpellType.Displacer):
                    return SpellEV.DISPLACER_Rarity;
                case (SpellType.Boomerang):
                    return SpellEV.BOOMERANG_Rarity;
                case (SpellType.DualBlades):
                    return SpellEV.DUAL_BLADES_Rarity;
                case (SpellType.Close):
                    return SpellEV.CLOSE_Rarity;
                case (SpellType.DamageShield):
                    return SpellEV.DAMAGE_SHIELD_Rarity;
                case (SpellType.Bounce):
                    return SpellEV.BOUNCE_Rarity;
                case (SpellType.Laser):
                    return SpellEV.LASER_Rarity;
                case (SpellType.DragonFire):
                    return SpellEV.DRAGONFIRE_Rarity;
                case (SpellType.DragonFireNeo):
                    return SpellEV.DRAGONFIRENEO_Rarity;
                case (SpellType.RapidDagger):
                    return SpellEV.RAPIDDAGGER_Rarity;
            }

            return 0;
        }

        public static float GetDamageMultiplier(byte spellType)
        {
            switch (spellType)
            {
                case (SpellType.Dagger):
                    return SpellEV.DAGGER_Damage;
                case (SpellType.Axe):
                    return SpellEV.AXE_Damage;
                case (SpellType.TimeBomb):
                    return SpellEV.TIMEBOMB_Damage;
                case (SpellType.TimeStop):
                    return SpellEV.TIMESTOP_Damage;
                case (SpellType.Nuke):
                    return SpellEV.NUKE_Damage;
                case (SpellType.Translocator):
                    return SpellEV.TRANSLOCATER_Damage;
                case (SpellType.Displacer):
                    return SpellEV.DISPLACER_Damage;
                case (SpellType.Boomerang):
                    return SpellEV.BOOMERANG_Damage;
                case (SpellType.DualBlades):
                    return SpellEV.DUAL_BLADES_Damage;
                case (SpellType.Close):
                    return SpellEV.CLOSE_Damage;
                case (SpellType.DamageShield):
                    return SpellEV.DAMAGE_SHIELD_Damage;
                case (SpellType.Bounce):
                    return SpellEV.BOUNCE_Damage;
                case (SpellType.Laser):
                    return SpellEV.LASER_Damage;
                case (SpellType.DragonFire):
                    return SpellEV.DRAGONFIRE_Damage;
                case (SpellType.DragonFireNeo):
                    return SpellEV.DRAGONFIRENEO_Damage;
                case (SpellType.RapidDagger):
                    return SpellEV.RAPIDDAGGER_Damage;
            }

            return 0;
        }

        public static float GetXValue(byte spellType)
        {
            switch (spellType)
            {
                case (SpellType.Dagger):
                    return SpellEV.DAGGER_XVal;
                case (SpellType.Axe):
                    return SpellEV.AXE_XVal;
                case (SpellType.TimeBomb):
                    return SpellEV.TIMEBOMB_XVal;
                case (SpellType.TimeStop):
                    return SpellEV.TIMESTOP_XVal;
                case (SpellType.Nuke):
                    return SpellEV.NUKE_XVal;
                case (SpellType.Translocator):
                    return SpellEV.TRANSLOCATER_XVal;
                case (SpellType.Displacer):
                    return SpellEV.DISPLACER_XVal;
                case (SpellType.Boomerang):
                    return SpellEV.BOOMERANG_XVal;
                case (SpellType.DualBlades):
                    return SpellEV.DUAL_BLADES_XVal;
                case (SpellType.Close):
                    return SpellEV.CLOSE_XVal;
                case (SpellType.DamageShield):
                    return SpellEV.DAMAGE_SHIELD_XVal;
                case (SpellType.Bounce):
                    return SpellEV.BOUNCE_XVal;
                case (SpellType.Laser):
                    return SpellEV.LASER_XVal;
                case (SpellType.DragonFire):
                    return SpellEV.DRAGONFIRE_XVal;
                case (SpellType.DragonFireNeo):
                    return SpellEV.DRAGONFIRENEO_XVal;
            }

            return 0;
        }

        public static float GetYValue(byte spellType)
        {
            switch (spellType)
            {
                case (SpellType.Dagger):
                    return SpellEV.DAGGER_YVal;
                case (SpellType.Axe):
                    return SpellEV.AXE_YVal;
                case (SpellType.TimeBomb):
                    return SpellEV.TIMEBOMB_YVal;
                case (SpellType.TimeStop):
                    return SpellEV.TIMESTOP_YVal;
                case (SpellType.Nuke):
                    return SpellEV.NUKE_YVal;
                case (SpellType.Translocator):
                    return SpellEV.TRANSLOCATER_YVal;
                case (SpellType.Displacer):
                    return SpellEV.DISPLACER_YVal;
                case (SpellType.Boomerang):
                    return SpellEV.BOOMERANG_YVal;
                case (SpellType.DualBlades):
                    return SpellEV.DUAL_BLADES_YVal;
                case (SpellType.Close):
                    return SpellEV.CLOSE_YVal;
                case (SpellType.DamageShield):
                    return SpellEV.DAMAGE_SHIELD_YVal;
                case (SpellType.Bounce):
                    return SpellEV.BOUNCE_YVal;
                case (SpellType.Laser):
                    return SpellEV.LASER_YVal;
                case (SpellType.DragonFire):
                    return SpellEV.DRAGONFIRE_YVal;
                case (SpellType.DragonFireNeo):
                    return SpellEV.DRAGONFIRENEO_YVal;
            }

            return 0;
        }
    }
}
