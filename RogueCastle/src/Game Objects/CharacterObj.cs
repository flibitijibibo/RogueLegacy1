using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public abstract class CharacterObj : PhysicsObjContainer, IStateObj, IKillableObj
    {
        //TODO: Clone().

        public string LocStringID { get; set; }

        //EV Values
        public float JumpHeight { get; set; }
        public virtual int MaxHealth { get; internal set; }
        public Vector2 KnockBack { get; internal set; }
        protected float CurrentAirSpeed = 0;
        public float DoubleJumpHeight { get; internal set; }
        public bool CanBeKnockedBack { get; set; }

        public float SlopeClimbRotation = 45;

        protected int StepUp; // The number of pixels that the player can hit before he "steps up" a block.

        public int State { get; set; }

        protected bool m_isTouchingGround = false;

        protected bool m_isKilled = false;
        protected ProceduralLevelScreen m_levelScreen;


        // Internal flags used to remember the state of the enemy when ResetState() is called.
        public SpriteEffects InternalFlip = SpriteEffects.None;
        protected bool m_internalLockFlip = false;
        protected bool m_internalIsWeighted = true; // An internal flag that tells what state to set the enemy's IsWeighted flag to.
        protected float m_internalRotation = 0;
        protected float m_internalAnimationDelay = 1 / 10f;
        protected Vector2 m_internalScale = new Vector2(1, 1);

        private Color m_blinkColour = Color.White;
        protected float m_blinkTimer = 0;

        private int m_currentHealth = 0;
        public int CurrentHealth 
        { 
            get { return m_currentHealth;}
            set
            {
                m_currentHealth = value;
                if (m_currentHealth > MaxHealth)
                    m_currentHealth = MaxHealth;
                if (m_currentHealth < 0)
                    m_currentHealth = 0;
            }
        }

        public CharacterObj(string spriteName, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo)
            : base(spriteName, physicsManager)
        {
            m_levelScreen = levelToAttachTo;
            CanBeKnockedBack = true;
        }

        protected abstract void InitializeEV();

        protected abstract void InitializeLogic();

        public virtual void HandleInput() { }

        public virtual void Update(GameTime gameTime)
        {
            if (m_blinkTimer > 0)
            {
                m_blinkTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                this.TextureColor = m_blinkColour;
            }
            else if (this.TextureColor == m_blinkColour)
                this.TextureColor = Color.White;
        }

        public void Blink(Color blinkColour, float duration)
        {
            m_blinkColour = blinkColour;
            m_blinkTimer = duration;
        }

        public virtual void Kill(bool giveXP = true)
        {
            AccelerationX = 0;
            AccelerationY = 0;
            this.Opacity = 1;
            this.CurrentSpeed = 0;
            this.StopAnimation();
            this.Visible = false;
            m_isKilled = true;
            IsCollidable = false;
            IsWeighted = false;
            m_blinkTimer = 0;
        }

        public virtual void Reset() 
        {
            this.AccelerationX = 0;
            this.AccelerationY = 0;
            this.CurrentSpeed = 0;
            this.CurrentHealth = this.MaxHealth;
            this.Opacity = 1;

            IsCollidable = true;
            IsWeighted = m_internalIsWeighted;
            LockFlip = m_internalLockFlip;
            this.Rotation = m_internalRotation;
            this.AnimationDelay = m_internalAnimationDelay;
            this.Scale = m_internalScale;
            Flip = InternalFlip;
            m_isKilled = false;
            this.Visible = true;
            IsTriggered = false;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_levelScreen = null;
                base.Dispose();
            }
        }

        public bool IsKilled { get { return m_isKilled; } }
        public bool IsTouchingGround { get { return m_isTouchingGround; } }
        public Vector2 InternalScale { get { return m_internalScale; } }
    }
}
