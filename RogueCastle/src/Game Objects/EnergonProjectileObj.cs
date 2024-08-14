using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnergonProjectileObj : ProjectileObj
    {
        private const byte TYPE_SWORD = 0;
        private const byte TYPE_SHIELD = 1;
        private const byte TYPE_DOWNSWORD = 2;
        private byte m_currentAttackType = TYPE_SWORD;

        private bool m_canHitEnemy = false;
        private EnemyObj_Energon m_parent;

        public EnergonProjectileObj(string spriteName, EnemyObj_Energon parent)
            : base(spriteName)
        {
            this.TurnSpeed = 1;
            this.IsWeighted = false;
            m_parent = parent;
            this.ChaseTarget = true;
        }

        public void SetType(byte type)
        {
            m_currentAttackType = type;
            switch (type)
            {
                case (TYPE_SWORD):
                    this.ChangeSprite("EnergonSwordProjectile_Sprite");
                    break;
                case (TYPE_SHIELD):
                    this.ChangeSprite("EnergonShieldProjectile_Sprite");
                    break;
                case (TYPE_DOWNSWORD):
                    this.ChangeSprite("EnergonDownSwordProjectile_Sprite");
                    break;
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;
            if (player != null && m_canHitEnemy == false)
            {
                if ((this.AttackType == TYPE_SWORD && otherBox.Type == Consts.WEAPON_HITBOX && player.IsAirAttacking == false) ||
                    (this.AttackType == TYPE_SHIELD && otherBox.Type == Consts.BODY_HITBOX && player.State == PlayerObj.STATE_BLOCKING) ||
                    (this.AttackType == TYPE_DOWNSWORD && otherBox.Type == Consts.WEAPON_HITBOX && player.IsAirAttacking == true))
                {
                    Target = m_parent;
                    this.CollisionTypeTag = GameTypes.CollisionType_PLAYER;
                    this.CurrentSpeed *= 2;
                    player.AttachedLevel.ImpactEffectPool.DisplayEnemyImpactEffect(this.Position);
                }
                else if (otherBox.Type == Consts.BODY_HITBOX)
                    m_parent.DestroyProjectile(this);
            }
            else
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_parent = null;
                base.Dispose();
            }
        }

        private void TurnToFace(Vector2 facePosition, float turnSpeed)
        {
            float x = facePosition.X - this.Position.X;
            float y = facePosition.Y - this.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = MathHelper.WrapAngle(desiredAngle - this.Orientation);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            this.Orientation = MathHelper.WrapAngle(this.Orientation + difference);
        }

        public byte AttackType
        {
            get { return m_currentAttackType; }
        }
    }
}
