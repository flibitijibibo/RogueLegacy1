using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Platform : EnemyObj
    {
        private bool m_isExtended = false;
        private float m_retractCounter = 0;
        private bool m_blinkedWarning = false;

        private float RetractDelay = 0f;

        protected override void InitializeEV()
        {
            Scale = new Vector2(2.0f, 2.0f);//(1.0f, 1.0f);
            AnimationDelay = 1 / 30f;
            //ProjectileScale = new Vector2(3.0f, 3.0f);
            Speed = 0.0f;
            MaxHealth = 999;
            EngageRadius = 30;
            ProjectileRadius = 20;
            MeleeRadius = 10;
            CooldownTime = 2.0f;
            KnockBack = new Vector2(1, 2);
            Damage = 25;
            JumpHeight = 20.5f;
            AlwaysFaceTarget = false;
            CanFallOffLedges = false;
            XPValue = 2;
            CanBeKnockedBack = false;
            this.LockFlip = true;
            this.IsWeighted = false;
            RetractDelay = 3.0f;//5f;
            Name = "Platform";

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                case (GameTypes.EnemyDifficulty.EXPERT):
                case (GameTypes.EnemyDifficulty.ADVANCED):
                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            if (Game.PlayerStats.Traits.X == TraitType.PlatformsOpen || Game.PlayerStats.Traits.Y == TraitType.PlatformsOpen)
            {
                m_isExtended = true;
                this.PlayAnimation("EndRetract", "EndRetract");
            }
        }

        public override void Update(GameTime gameTime)
        {
            bool forceExtract = false;
            if (Game.PlayerStats.Traits.X == TraitType.PlatformsOpen || Game.PlayerStats.Traits.Y == TraitType.PlatformsOpen)
                forceExtract = true;

            if (forceExtract == false)
            {
                if (m_retractCounter > 0)
                {
                    m_retractCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_retractCounter <= 1.5f && m_blinkedWarning == false)
                    {
                        m_blinkedWarning = true;

                        float delay = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            Tweener.Tween.RunFunction(delay, this, "Blink", Color.Red, 0.05f);
                            delay += 0.1f;
                        }
                    }

                    if (m_retractCounter <= 0)
                    {
                        m_isExtended = false;
                        PlayAnimation("StartExtract", "EndExtract"); // Got the naming flipped in the spritesheet.
                        SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Platform_Activate_03", "Platform_Activate_04");
                    }
                }
            }
            else
            {
                if (m_isExtended == false)
                {
                    m_isExtended = true;
                    this.PlayAnimation("EndRetract", "EndRetract");
                }
            }

            base.Update(gameTime);
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer = true)
        {
            if (m_target.IsAirAttacking == true)
            {
                if (m_isExtended == false && (this.CurrentFrame == 1 || this.CurrentFrame == TotalFrames))
                {
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player,"EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
                    Blink(Color.Red, 0.1f);
                    this.m_levelScreen.ImpactEffectPool.DisplayEnemyImpactEffect(position);

                    m_isExtended = true;
                    m_blinkedWarning = false;
                    PlayAnimation("StartRetract", "EndRetract");
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Platform_Activate_01", "Platform_Activate_02");
                    m_retractCounter = RetractDelay;

                    if (m_target.IsAirAttacking == true)
                    {
                        m_target.IsAirAttacking = false; // Only allow one object to perform upwards air knockback on the player.
                        m_target.AccelerationY = -m_target.AirAttackKnockBack;
                        m_target.NumAirBounces++;
                    }
                }
            }
        }

        public override void ResetState()
        {
            if (Game.PlayerStats.Traits.X == TraitType.PlatformsOpen || Game.PlayerStats.Traits.Y == TraitType.PlatformsOpen)
                return;
            this.PlayAnimation(1, 1);
            m_isExtended = false;
            m_blinkedWarning = false;
            m_retractCounter = 0;
            base.ResetState();
        }

        public override void Reset()
        {
            if (Game.PlayerStats.Traits.X == TraitType.PlatformsOpen || Game.PlayerStats.Traits.Y == TraitType.PlatformsOpen)
                return;
            this.PlayAnimation(1, 1);
            m_isExtended = false;
            m_blinkedWarning = false;
            m_retractCounter = 0;
            base.Reset();
            this.StopAnimation();
        }

        public EnemyObj_Platform(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyPlatform_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.CollisionTypeTag = GameTypes.CollisionType_WALL;
            this.Type = EnemyType.Platform;
            this.CollidesBottom = false;
            this.CollidesLeft = false;
            this.CollidesRight = false;
            this.StopAnimation();
            this.PlayAnimationOnRestart = false;
            NonKillable = true;
            this.DisableCollisionBoxRotations = false;
        }
    }
}
