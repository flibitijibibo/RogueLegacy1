using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Turret : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();

        protected override void InitializeEV()
        {
            Scale = new Vector2(2.0f, 2.0f);//(1.0f, 1.0f);
            AnimationDelay = 1 / 10f;
            //ProjectileScale = new Vector2(3.0f, 3.0f);
            Speed = 0.0f;
            MaxHealth = 10;
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

            LockFlip = true;
            this.IsWeighted = false;
            IsCollidable = false;
            this.Name = "Wall Turret";
            this.LocStringID = "LOC_ID_ENEMY_NAME_117";

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

        protected override void InitializeLogic()
        {
            float angle = this.Rotation;
            float delay = this.ParseTagToFloat("delay");
            float speed = this.ParseTagToFloat("speed");
            if (delay == 0)
            {
                Console.WriteLine("ERROR: Turret set with delay of 0. Shoots too fast.");
                delay = 10000;
            }


            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "TurretProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(speed, speed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(angle, angle),
                CollidesWithTerrain = true,
                Scale = ProjectileScale,
            };

            LogicSet fireProjectileLS = new LogicSet(this);
            fireProjectileLS.AddAction(new PlayAnimationLogicAction(false), Types.Sequence.Parallel);
            fireProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            //fireProjectileLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileLS.AddAction(new DelayLogicAction(delay));

            m_generalBasicLB.AddLogicSet(fireProjectileLS);
            m_generalAdvancedLB.AddLogicSet(fireProjectileLS);
            m_generalExpertLB.AddLogicSet(fireProjectileLS);
            m_generalMiniBossLB.AddLogicSet(fireProjectileLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);

            projData.Dispose();

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                    RunLogicBlock(false, m_generalBasicLB, 100);
                    break;
            }
        }

        protected override void RunAdvancedLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        protected override void RunExpertLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        protected override void RunMinibossLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                    break;
            }
        }

        public EnemyObj_Turret(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyTurretFire_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.IsCollidable = false;
            ForceDraw = true;
            this.Type = EnemyType.Turret;
            this.StopAnimation();
            this.PlayAnimationOnRestart = false;
            NonKillable = true;
        }
    }
}
