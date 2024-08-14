using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyBuilder
    {
        public static EnemyObj BuildEnemy(int enemyType, PlayerObj player, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty, bool doNotInitialize = false)
        {
            EnemyObj objToReturn = null;
            switch (enemyType)
            {
                case (EnemyType.Skeleton):
                    objToReturn = new EnemyObj_Skeleton(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Knight):
                    objToReturn = new EnemyObj_Knight(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Fireball):
                    objToReturn = new EnemyObj_Fireball(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Fairy):
                    objToReturn = new EnemyObj_Fairy(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Turret):
                    objToReturn = new EnemyObj_Turret(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Wall):
                    objToReturn = new EnemyObj_Wall(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Horse):
                    objToReturn = new EnemyObj_Horse(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Zombie):
                    objToReturn = new EnemyObj_Zombie(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Wolf):
                    objToReturn = new EnemyObj_Wolf(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.BallAndChain):
                    objToReturn = new EnemyObj_BallAndChain(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Eyeball):
                    objToReturn = new EnemyObj_Eyeball(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Ninja):
                    objToReturn = new EnemyObj_Ninja(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Blob):
                    objToReturn = new EnemyObj_Blob(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.SwordKnight):
                    objToReturn = new EnemyObj_SwordKnight(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Eagle):
                    objToReturn = new EnemyObj_Eagle(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.ShieldKnight):
                    objToReturn = new EnemyObj_ShieldKnight(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.FireWizard):
                    objToReturn = new EnemyObj_FireWizard(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.IceWizard):
                    objToReturn = new EnemyObj_IceWizard(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.EarthWizard):
                    objToReturn = new EnemyObj_EarthWizard(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.BouncySpike):
                    objToReturn = new EnemyObj_BouncySpike(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.SpikeTrap):
                    objToReturn = new EnemyObj_SpikeTrap(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Plant):
                    objToReturn = new EnemyObj_Plant(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Energon):
                    objToReturn = new EnemyObj_Energon(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Spark):
                    objToReturn = new EnemyObj_Spark(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.SkeletonArcher):
                    objToReturn = new EnemyObj_SkeletonArcher(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Chicken):
                    objToReturn = new EnemyObj_Chicken(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Platform):
                    objToReturn = new EnemyObj_Platform(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.HomingTurret):
                    objToReturn = new EnemyObj_HomingTurret(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.LastBoss):
                    objToReturn = new EnemyObj_LastBoss(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Dummy):
                    objToReturn = new EnemyObj_Dummy(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Starburst):
                    objToReturn = new EnemyObj_Starburst(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Portrait):
                    objToReturn = new EnemyObj_Portrait(player, physicsManager, levelToAttachTo, difficulty);
                    break;
                case (EnemyType.Mimic):
                    objToReturn = new EnemyObj_Mimic(player, physicsManager, levelToAttachTo, difficulty);
                    break;
            }

            if (player == null && doNotInitialize == false)
                objToReturn.Initialize();
            return objToReturn;
        }
    }
}
