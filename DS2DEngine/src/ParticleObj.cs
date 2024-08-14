using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class ParticleObj : GameObj
    {
        // Position, Velocity, and Acceleration represent exactly what their names
        // indicate. They are public fields rather than properties so that users
        // can directly access their .X and .Y properties.
        public Vector2 Velocity;
        public Vector2 Acceleration;

        // how long this particle will "live"
        private float lifetime;
        public float Lifetime
        {
            get { return lifetime; }
            set { lifetime = value; }
        }

        // how long it has been since initialize was called
        private float timeSinceStart;
        public float TimeSinceStart
        {
            get { return timeSinceStart; }
            set { timeSinceStart = value; }
        }

        // how fast does it rotate? In radians.
        private float rotationSpeed;
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        private Texture2D sprite;
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        private float startDelay;
        public float StartDelay
        {
            get { return startDelay; }
            set { startDelay = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(SpriteSourceRect.Width / 2, SpriteSourceRect.Height / 2); }
        }

        public Rectangle SpriteSourceRect = new Rectangle(0, 0, 0, 0);

        // is this particle still alive? once TimeSinceStart becomes greater than
        // Lifetime, the particle should no longer be drawn or updated.
        public bool Active
        {
            get { return TimeSinceStart < Lifetime; }
        }


        // initialize is called by ParticleSystem to set up the particle, and prepares
        // the particle for use.
        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration,
            float lifetime, Vector2 scale, float rotationSpeed, float startingRotation)
        {
            // set the values to the requested values
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
            this.Lifetime = lifetime;
            this.Scale = scale;
            this.RotationSpeed = rotationSpeed;

            // reset TimeSinceStart - we have to do this because particles will be
            // reused.
            this.TimeSinceStart = 0.0f;

            // set rotation to some random value between 0 and 360 degrees.
            this.Rotation = startingRotation;
        }

        // update is called by the ParticleSystem on every frame. This is where the
        // particle's position and that kind of thing get updated.
        public void Update(float dt)
        {
            if (StartDelay <= 0)
            {
                Velocity += Acceleration * dt;
                Position += Velocity * dt;

                Rotation += RotationSpeed * dt;

                TimeSinceStart += dt;
            }
            else
                StartDelay -= dt;
        }

        public void Stop()
        {
            TimeSinceStart = Lifetime;
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ParticleObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            ParticleObj clone = obj as ParticleObj;
            clone.Lifetime = this.Lifetime;
            clone.Acceleration = this.Acceleration;
            clone.Velocity = this.Velocity;
            clone.TimeSinceStart = this.TimeSinceStart;
            clone.StartDelay = this.StartDelay;
            clone.RotationSpeed = this.RotationSpeed;
            clone.Sprite = this.Sprite;
        }
    }
}
