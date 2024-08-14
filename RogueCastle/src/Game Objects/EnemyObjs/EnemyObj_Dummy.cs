using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;

namespace RogueCastle
{
    public class EnemyObj_Dummy: EnemyObj
    {
        protected override void InitializeEV()
        {
            Scale = new Vector2(2.2f, 2.2f);
            AnimationDelay = 1 / 60f;
            Speed = 200.0f;
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
            this.Name = "Training Dummy";
            this.LocStringID = "LOC_ID_TRAINING_DUMMY_1";

            this.IsWeighted = false;
            this.LockFlip = true;
            
            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                case (GameTypes.EnemyDifficulty.EXPERT):
                case (GameTypes.EnemyDifficulty.ADVANCED):
                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            if (m_target != null && m_target.CurrentHealth > 0)
            {
                this.ChangeSprite("DummySad_Character");
                this.PlayAnimation(false);

                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
                Blink(Color.Red, 0.1f);
                this.m_levelScreen.ImpactEffectPool.DisplayEnemyImpactEffect(collisionPt);
                this.m_levelScreen.ImpactEffectPool.WoodChipEffect(new Vector2(this.X, this.Bounds.Center.Y));

                if (isPlayer == true) // Only perform these checks if the object hitting the enemy is the player.
                {
                    m_target.NumSequentialAttacks++;

                    if (m_target.IsAirAttacking == true)
                    {
                        m_target.IsAirAttacking = false; // Only allow one object to perform upwards air knockback on the player.
                        m_target.AccelerationY = -m_target.AirAttackKnockBack;
                        m_target.NumAirBounces++;
                    }
                }

                m_levelScreen.TextManager.DisplayNumberText(damage, Color.Red, new Vector2(this.X, this.Bounds.Top));
                m_levelScreen.SetLastEnemyHit(this);
                RandomizeName();
            }
        }

        private void RandomizeName()
        {
            string[] nameIDs = new string[]
            {
                "LOC_ID_TRAINING_DUMMY_2",
                "LOC_ID_TRAINING_DUMMY_3",
                "LOC_ID_TRAINING_DUMMY_4",
                "LOC_ID_TRAINING_DUMMY_5",
                "LOC_ID_TRAINING_DUMMY_1",
                "LOC_ID_TRAINING_DUMMY_6",
                "LOC_ID_TRAINING_DUMMY_7",
                "LOC_ID_TRAINING_DUMMY_8",
                "LOC_ID_TRAINING_DUMMY_9",
                "LOC_ID_TRAINING_DUMMY_10",
                "LOC_ID_TRAINING_DUMMY_11",
                "LOC_ID_TRAINING_DUMMY_12",
                "LOC_ID_TRAINING_DUMMY_13",
                "LOC_ID_TRAINING_DUMMY_14",
                "LOC_ID_TRAINING_DUMMY_15",
                "LOC_ID_TRAINING_DUMMY_16",
                "LOC_ID_TRAINING_DUMMY_17",
                "LOC_ID_TRAINING_DUMMY_18",
                "LOC_ID_TRAINING_DUMMY_19",
                "LOC_ID_TRAINING_DUMMY_20",
                "LOC_ID_TRAINING_DUMMY_21",
                "LOC_ID_TRAINING_DUMMY_22"
            };
            this.Name = "";// names[CDGMath.RandomInt(0, names.Length - 1)];
            this.LocStringID = nameIDs[CDGMath.RandomInt(0, nameIDs.Length - 1)];
        }

        public EnemyObj_Dummy(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("Dummy_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Dummy;
            //NonKillable = true;
        }
    }
}
