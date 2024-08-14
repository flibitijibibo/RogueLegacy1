using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class KingObj : PhysicsObj//PhysicsObjContainer
    {
        private bool m_wasHit = false;

        public KingObj(string spriteName)
            : base(spriteName)
        {
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            IPhysicsObj otherBoxParent = otherBox.AbsParent as IPhysicsObj;

            if (collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT && otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_PLAYER && m_wasHit == false)
            {
                SoundManager.Play3DSound(this, Game.ScreenManager.Player,"EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
                //SoundManager.PlaySound("Player_Male_Injury_01", "Player_Male_Injury_02", "Player_Male_Injury_03", "Player_Male_Injury_04", "Player_Male_Injury_05",
                //    "Player_Male_Injury_06", "Player_Male_Injury_07", "Player_Male_Injury_08", "Player_Male_Injury_09", "Player_Male_Injury_10");
                SoundManager.PlaySound("Boss_Title_Exit");
                SoundManager.PlaySound("Player_Death_Grunt");
                Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                if (thisBox.AbsRotation != 0 || otherBox.AbsRotation != 0)
                    intersectPt = Rectangle.Intersect(thisBox.AbsParent.Bounds, otherBox.AbsParent.Bounds).Center;
                Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);
                (otherBox.AbsParent as PlayerObj).AttachedLevel.ImpactEffectPool.DisplayEnemyImpactEffect(impactPosition);

                m_wasHit = true;
            }

            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new KingObj(this.SpriteName);
        }

        public bool WasHit
        {
            get { return m_wasHit; }
        }
    }
}
