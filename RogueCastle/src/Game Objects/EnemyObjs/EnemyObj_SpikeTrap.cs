using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_SpikeTrap : EnemyObj
    {
        private LogicSet m_extractLS;
        private float ExtractDelay = 0.0f; //0.5f;

        private Rectangle DetectionRect;

        protected override void InitializeEV()
        {
            Scale = new Vector2(2.0f, 2.0f);//(1.0f, 1.0f);
            AnimationDelay = 1 / 10f;
            //ProjectileScale = new Vector2(3.0f, 3.0f);
            Speed = 0.0f;
            MaxHealth = 10;
            EngageRadius = 2100;
            ProjectileRadius = 2200;
            MeleeRadius = 650;
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
            
            ExtractDelay = 0.1f;
            DetectionRect = new Rectangle(0, 0, 120, 30); //(0,0, 200,50);
            Name = "Spike Trap";
            LocStringID = EnemyEV.SpikeTrap_Basic_Name_locID;

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                case (GameTypes.EnemyDifficulty.EXPERT):
                case (GameTypes.EnemyDifficulty.ADVANCED):
                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            this.IsCollidable = false;
        }

        protected override void InitializeLogic()
        {
            m_extractLS = new LogicSet(this);
            m_extractLS.AddAction(new PlayAnimationLogicAction(1, 2));
            m_extractLS.AddAction(new DelayLogicAction(ExtractDelay));
            m_extractLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"TrapSpike_01", "TrapSpike_02", "TrapSpike_03"));
            m_extractLS.AddAction(new PlayAnimationLogicAction(2, 4));

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
            if (IsPaused == false)
            {
                if (Game.PlayerStats.Traits.X != TraitType.PAD && Game.PlayerStats.Traits.Y != TraitType.PAD)
                {
                    if (CollisionMath.Intersects(AbsDetectionRect, m_target.Bounds) == true)
                    {
                        if (this.CurrentFrame == 1 || this.CurrentFrame == this.TotalFrames)
                        {
                            this.IsCollidable = true;
                            m_extractLS.Execute();
                            //this.PlayAnimation("StartExtract", "ExtractComplete", false);
                        }
                    }
                    else
                    {
                        if (this.CurrentFrame == 5 && m_extractLS.IsActive == false) // "ExtractComplete"
                        {
                            this.IsCollidable = false;
                            this.PlayAnimation("StartRetract", "RetractComplete", false);
                        }
                    }
                }

                if (m_extractLS.IsActive == true)
                    m_extractLS.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public EnemyObj_SpikeTrap(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemySpikeTrap_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.SpikeTrap;
            this.StopAnimation();
            this.PlayAnimationOnRestart = false;
            NonKillable = true;
        }

        public override void Reset()
        {
            this.PlayAnimation(1, 1);
            base.Reset();
        }

        public override void ResetState()
        {
            this.PlayAnimation(1, 1);
            base.ResetState();
        }

        private Rectangle AbsDetectionRect
        {
            get { return new Rectangle((int)(this.X - DetectionRect.Width / 2f), (int)(this.Y - DetectionRect.Height), DetectionRect.Width, DetectionRect.Height); }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_extractLS.Dispose();
                m_extractLS = null;
                base.Dispose();
            }
        }
    }
}
