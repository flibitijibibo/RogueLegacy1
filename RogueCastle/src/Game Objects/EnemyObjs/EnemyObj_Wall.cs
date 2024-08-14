using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Wall : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        protected override void InitializeEV()
        {
            XPValue = 5000;
            MaxHealth = 10000;
            Damage = 900;
            ProjectileDamage = 50;

            this.IsWeighted = false;
            this.Scale = new Vector2(1f, 3.0f);
            ProjectileScale = new Vector2(3.0f, 3.0f);
            Speed = 1.75f;
            EngageRadius = 1900;
            ProjectileRadius = 1600;
            MeleeRadius = 650;
            CooldownTime = 2.0f;
            KnockBack = new Vector2(5, 6);
            JumpHeight = 20.5f;
            AlwaysFaceTarget = true;
            CanFallOffLedges = false;
            CanBeKnockedBack = false;

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
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            //walkTowardsLS.AddAction(new DelayLogicAction(0.5f, 1.25f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.5f, 1.25f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f, 1.25f));

            LogicSet throwBigBoneLS = new LogicSet(this);
            throwBigBoneLS.AddAction(new MoveLogicAction(m_target, true));
            throwBigBoneLS.AddAction(new RunFunctionLogicAction(this, "ResizeProjectile", true));
            //throwBigBoneLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "SpearProjectile", new Vector2(90, 0), 0, 6.75f, false, 1.75f, ProjectileDamage));
            throwBigBoneLS.AddAction(new RunFunctionLogicAction(this, "ResizeProjectile", false));
            throwBigBoneLS.AddAction(new DelayLogicAction(3.0f));

            LogicSet throwRandomProjectileLS = new LogicSet(this);
            throwRandomProjectileLS.AddAction(new MoveLogicAction(m_target, true));
            throwRandomProjectileLS.AddAction(new RunFunctionLogicAction(this, "FireRandomProjectile"));
            throwRandomProjectileLS.AddAction(new RunFunctionLogicAction(this, "FireRandomProjectile"));
            throwRandomProjectileLS.AddAction(new RunFunctionLogicAction(this, "FireRandomProjectile"));
            throwRandomProjectileLS.AddAction(new DelayLogicAction(1.3f));

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, throwBigBoneLS, throwRandomProjectileLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 40, 40, 20); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        public void FireRandomProjectile()
        {
            //ProjectileObj obj = m_levelScreen.ProjectileManager.FireProjectile("BoneProjectile", this, new Vector2(-25.0f, CDGMath.RandomFloat(-300.0f, 300.0f)), 0, 5.5f, false, 5.0f, ProjectileDamage);
            //obj.Scale = ProjectileScale;
        }

        public void ResizeProjectile(Boolean resize)
        {
            if (resize == true)
                ProjectileScale = new Vector2(2.5f, 2.5f);
            else
                ProjectileScale = new Vector2(2.5f, 2.5f);
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    RunLogicBlock(false, m_generalBasicLB, 0, 0, 0, 20, 80); //walkTowardsLS, walkAwayLS, walkStopLS, throwBigBoneLS, throwRandomProjectileLS
                    break;
                default:
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

        public override void Update(GameTime gameTime)
        {
            HeadingY = 0;
            if (HeadingX > 0) this.HeadingX = 1; // A hack to maintain a horizontal speed.
            else if (HeadingX < 0) this.HeadingX = -1;
            base.Update(gameTime);
            this.Y -= this.HeadingY; // Hack to make sure the wall only goes horizontal since collisions are turned off.
        }

        public EnemyObj_Wall(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyGhostIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            CollisionTypeTag = GameTypes.CollisionType_ENEMYWALL;
            this.Type = EnemyType.Wall;
        }
    }
}
