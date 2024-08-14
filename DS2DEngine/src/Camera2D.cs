using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DS2DEngine
{
    public class Camera2D : SpriteBatch
    {
        private float m_zoom;
        private Matrix m_transform;
        private float m_rotation;
        private Rectangle m_bounds;
        private int m_width;
        private int m_height;

        public Vector2 Position;

        private Texture2D DEBUG_texture;
        private Color DEBUG_color = new Color(100, 100, 100);

        public GameTime GameTime { get; set; }
        public float ElapsedTotalSeconds { get; set; } // Used to reduce performance load since this number is called a lot.

        public Camera2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice)
        {
            m_zoom = 1.0f;
            m_rotation = 0.0f;
            Position = Vector2.Zero;

            m_width = width; // EngineEV.ScreenWidth; //graphicsDevice.Viewport.Width;
            m_height = height; // EngineEV.ScreenHeight; // graphicsDevice.Viewport.Height;


            m_bounds = new Rectangle((int)(-Width * 0.5f), (int)(-Height * 0.5f), Width, Height);
            
            DEBUG_texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Int32[] pixel = { 0xFFFFFF }; // White. 0xFF is Red, 0xFF0000 is Blue
            DEBUG_texture.SetData<Int32>(pixel, 0, 1);
        }

        public Matrix GetTransformation()
        {
            m_transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                         Matrix.CreateRotationZ(m_rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Width * 0.5f,
                                                                              Height * 0.5f, 0));
            return m_transform;
        }

        public void DrawLine(Texture2D texture, float width, Color color, Vector2 pt1, Vector2 pt2)
        {
            float angle = (float)Math.Atan2(pt2.Y - pt1.Y, pt2.X - pt1.X);
            float length = Vector2.Distance(pt1, pt2);

            this.Draw(texture, pt1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public void Draw_CameraBox()
        {
            this.Draw(DEBUG_texture, Bounds, DEBUG_color);
        }

        public float Zoom
        {
            get { return m_zoom; }
            set 
            { 
                m_zoom = value;
                if (m_zoom < 0.025f) m_zoom = 0.025f;
                //if (m_zoom > 2) m_zoom = 2;
            }
        }

        public float Rotation
        {
            get { return MathHelper.ToDegrees(m_rotation); }
            set { m_rotation = MathHelper.ToRadians(value); }
        }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public int Width
        {
            get { return m_width; }
        }

        public int Height
        {
            get { return m_height; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)(Position.X - (Width * 0.5f * 1/Zoom)), 
                                       (int)(Position.Y - (Height * 0.5f * 1/Zoom)), 
                                       (int)(m_bounds.Width * 1/Zoom), (int)(m_bounds.Height * 1/Zoom)); }
        }

        // A bounding box larger than the actual bounds of the camera that determines whether physics and logic should kick in.
        public Rectangle LogicBounds
        {
            get { return new Rectangle(Bounds.X - 200, Bounds.Y - 200, Bounds.Width + 400, Bounds.Height + 400); }
        }

        public Vector2 TopLeftCorner
        {
            get { return new Vector2(Position.X - (this.Width * 0.5f * 1/Zoom), Position.Y - (this.Height * 0.5f * 1/Zoom)); }
        }

        public bool CenteredZoom
        {
            get { return (Zoom < 1.05f && Zoom > 0.95f); }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed == false)
            {
                DEBUG_texture.Dispose();
                DEBUG_texture = null;
                base.Dispose(disposing);
            }
        }
    }
}
